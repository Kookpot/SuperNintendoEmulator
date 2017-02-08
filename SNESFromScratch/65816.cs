using System;
using System.IO;

namespace SNESFromScratch
{
    public class C65816
    {
        #region Constants

        public static byte NegativeFlag = 0x80;
        public static byte OverflowFlag = 0x40;
        public static byte Accumulator8BitsFlag = 0x20;
        public static byte Index8BitsFlag = 0x10;
        public static byte DecimalFlag = 0x8;
        public static byte InterruptFlag = 0x4;
        public static byte ZeroFlag = 0x2;
        public static byte CarryFlag = 0x1;

        public static double CyclesPerScanline = 0.0000635/(1/3580000.0);
        public static double HBlankCycles = (CyclesPerScanline/340)*84;
        private readonly string _startup;

        #endregion

        #region private members

        private readonly ROM _rom;
        private readonly PPU _ppu;
        private readonly SPU _spu;
        private readonly IOPort _ioPort;
        private readonly FPS _fps;
        private StreamWriter _writer;

        public struct CpuRegs
        {
            public int A; //Accumulator (16 bits)
            public int X, Y; //Index X/Y (16 bits)
            public int StackPointer;
            public byte DataBank;
            public int DirectPage;
            public byte ProgramBank;
            public byte P; //Flags
            public int ProgramCounter;
        }

        public CpuRegs Registers;
        private bool _emulate6502 = true;

        private int _effectiveAddress;
        private bool _pageCrossed;

        public double Cycles;

        public bool SnesOn;
        public bool STPDisable;
        private bool _waiDisable;

        public byte[] Memory = new byte[0x20000]; //WRAM 128kb
        public byte[][] SaveRAM = new[] { new byte[0x8000], new byte[0x8000], new byte[0x8000], new byte[0x8000], new byte[0x8000], new byte[0x8000], new byte[0x8000], new byte[0x8000] };

        private int _wramAddress;

        public bool IRQOcurred, VBlank, HBlank;
        public int CurrentLine;

        public bool Debug;

        private readonly Form1 _form;

        #endregion

        public C65816(ROM rom, PPU ppu, SPU spu, IOPort ioPort, FPS fps, String startUp, Form1 form)
        {
            _rom = rom;
            _ppu = ppu;
            _spu = spu;
            _ioPort = ioPort;
            _fps = fps;
            _startup = startUp;
            _form = form;
        }

        #region Flag Handling Functions

        private void SetFlag(byte value)
        {
            Registers.P = (byte) (Registers.P | value);
        }

        private void ClearFlag(byte value)
        {
            Registers.P = (byte) (Registers.P & ~value);
        }

        private void SetZeroNegativeFlag(byte value)
        {
            if (value != 0)
            {
                ClearFlag(ZeroFlag);
            }
            else
            {
                SetFlag(ZeroFlag);
            }
            if ((value & 0x80) != 0)
            {
                SetFlag(NegativeFlag);
            }
            else
            {
                ClearFlag(NegativeFlag);
            }
        }

        private void SetZeroNegativeFlag16(int value)
        {
            if (value != 0)
            {
                ClearFlag(ZeroFlag);
            }
            else
            {
                SetFlag(ZeroFlag);
            }
            if ((value & 0x8000) != 0)
            {
                SetFlag(NegativeFlag);
            }
            else
            {
                ClearFlag(NegativeFlag);
            }
        }

        private void TestFlag(bool condition, byte value)
        {
            if (condition)
            {
                SetFlag(value);
            }
            else
            {
                ClearFlag(value);
            }
        }

        #endregion

        #region Stack Push/Pull

        private void Push(byte value)
        {
            WriteMemory(0, Registers.StackPointer, value);
            Registers.StackPointer -= 1;
        }

        private byte Pull()
        {
            Registers.StackPointer += 1;
            return ReadMemory(0, Registers.StackPointer);
        }

        private void Push16(int value)
        {
            Push((byte) ((value & 0xFF00)/0x100));
            Push((byte) (value & 0xFF));
        }

        private int Pull16()
        {
            return Pull() + Pull()*0x100;
        }

        #endregion

        #region Unsigned/Signed converter, Update_Mode

        private static SByte SignedByte(byte byteToConvert)
        {
            if (byteToConvert < 0x80) { return (SByte) byteToConvert; }
            return (SByte) (byteToConvert - 0x100);
        }

        private static int SignedInteger(int integerToConvert)
        {
            if (integerToConvert < 0x8000) { return integerToConvert; }
            return integerToConvert - 0x10000;
        }

        private void UpdateMode()
        {
            if ((Registers.P & Index8BitsFlag) <= 0 && !_emulate6502) return;
            Registers.X = Registers.X & 0xFF;
            Registers.Y = Registers.Y & 0xFF;
        }

        #endregion

        #region Read/Write

        public byte ReadMemory(byte bank, int address)
        {
            if (address > 0xFFFF) { bank += 1; }
            address = address & 0xFFFF;
            if (_rom.Header.HiROM)
            {
                if ((bank & 0x7F) < 0x40)
                {
                    if (address <= 0x1FFF) { return Memory[address]; }
                    if (address <= 0x213F) { return _ppu.ReadPPU(address); }
                    if (address <= 0x217F) { return _spu.ReadSPU(address); }
                    if (address == 0x2180)
                    {
                        var value = Memory[_wramAddress];
                        _wramAddress = (_wramAddress + 1) & 0x1FFFF;
                        return value;
                    }
                    if (address < 0x4000)
                    {
                        var x = 10;
                    }
                    if (address <= 0x41FF) { return _ioPort.ReadIO(address); }
                    if (address <= 0x43FF) { return _ioPort.ReadIO(address); }
                    if (address < 0x6000)
                    {
                        var x = 10;
                    }
                    if (address <= 0x7FFF) { return SaveRAM[0][address & 0x1FFF]; }
                    if (address <= 0xFFFF) { return _rom.ROMData[((bank & 0x3F)*2) + 1][address & 0x7FFF]; }
                }

                if ((bank & 0x7F) < 0x7E)
                {
                    return (address & 0x8000) != 0 ? _rom.ROMData[((bank & 0x3F) * 2) + 1][address & 0x7FFF] : _rom.ROMData[(bank & 0x3F) * 2][address & 0x7FFF];
                }

                if (bank == 0xFE)
                {
                    return _rom.ROMData[(address & 0x8000) != 0 ? 0x7D : 0x7C][address & 0x7FFF];
                }

                if (bank == 0xFF)
                {
                    return _rom.ROMData[(address & 0x8000) != 0 ? 0x7F : 0x7E][address & 0x7FFF];
                }
            }
            else
            {
                bank = (byte) (bank & 0x7F);
                if (bank < 0x70)
                {
                    if (address < 0x1FFF) { return Memory[address]; }
                    if (address < 0x213F) { return _ppu.ReadPPU(address); }
                    if (address < 0x217F) { return _spu.ReadSPU(address); }
                    if (address == 0x2180)
                    {
                        var value = Memory[_wramAddress];
                        _wramAddress = (_wramAddress + 1) & 0x1FFFF;
                        return value;
                    }
                    if (address < 0x4000)
                    {
                        var x = 10;
                    }
                    if (address <= 0x41FF) { return _ioPort.ReadIO(address); }
                    if (address <= 0x43FF) { return _ioPort.ReadIO(address); }
                    if (address < 0x8000)
                    {
                        var x = 10;
                    }
                    if (address >= 0x8000 && address <= 0xFFFF)
                    {
                        if (_rom.Header.Banks <= 0x10)
                        {
                            return _rom.ROMData[bank & 0xF][address & 0x7FFF];
                        }
                        if (_rom.Header.Banks <= 0x20)
                        {
                            return _rom.ROMData[bank & 0x1F][address & 0x7FFF];
                        }
                        return bank < 0x40 ? _rom.ROMData[bank][address & 0x7FFF] : _rom.ROMData[bank & 0x3F][address & 0x7FFF];
                    }
                }

                if (bank >= 0x70 && bank <= 0x77) { return SaveRAM[bank & 0x7][address & 0x1FFF]; }
            }
            if (bank == 0x7E) { return Memory[address]; }
            return bank == 0x7F ? Memory[address + 0x10000] : (byte) 0;
        }

        public int ReadMemory16(byte bank, int address)
        {
            return ReadMemory(bank, address) + (ReadMemory(bank, address + 1)*0x100);
        }

        public int ReadMemory24(byte bank, int address)
        {
            return ReadMemory(bank, address) + (ReadMemory(bank, address + 1)*0x100) + (ReadMemory(bank, address + 2)*0x10000);
        }

        public void WriteMemory(int bank, int address, byte value)
        {
            if (address > 0xFFFF) { bank += 1; }
            address = address & 0xFFFF;
            bank = bank & 0x7F;
            if (bank < 0x70)
            {
                if (address <= 0x1FFF) { Memory[address] = value; }
                else if (address <= 0x213F) { _ppu.WritePPU(address, value); }
                else if (address <= 0x213F) { _spu.WriteSPU(address, value); }
                else if (address == 0x2180)
                {
                    Memory[_wramAddress] = value;
                    _wramAddress = (_wramAddress + 1) & 0x1FFFF;
                }
                else if (address == 0x2181) { _wramAddress = value + (_wramAddress & 0x1FF00); }
                else if (address == 0x2182) { _wramAddress = (value*0x100) + (_wramAddress & 0x100FF); }
                else if (address == 0x2183)
                {
                    if (value == 1)
                    {
                        _wramAddress = _wramAddress | 0x10000;
                    }
                    else
                    {
                        _wramAddress = _wramAddress & ~0x10000;
                    }
                }
                else if (address >= 0x4000 && address <= 0x41FF) { _ioPort.WriteIO(address, value); }
                else if (address >= 0x4200 && address <= 0x43FF) { _ioPort.WriteIO(address, value); }
                else if (address >= 0x6000 && address <= 0x7FFF)
                {
                    if (_rom.Header.HiROM && (bank > 0x2F && bank < 0x40))
                    {
                        SaveRAM[0][address & 0x1FFF] = value;
                    }
                }
            }
            if (bank >= 0x70 && bank <= 0x77) { SaveRAM[bank & 7][address & 0x1FFF] = value; }
            if (bank == 0x7E) { Memory[address] = value; }
            if (bank == 0x7F) { Memory[address + 0x10000] = value; }
        }

        public void WriteMemory16(int bank, int address, int value)
        {
            WriteMemory(bank, address, (byte) (value & 0xFF));
            WriteMemory(bank, address + 1, (byte) ((value & 0xFF00)/0x100));
        }

        public void WriteMemory24(int bank, int address, int value)
        {
            WriteMemory(bank, address, (byte) (value & 0xFF));
            WriteMemory(bank, address + 1, (byte) ((value & 0xFF00)/0x100));
            WriteMemory(bank, address + 2, (byte) ((value & 0xFF0000)/0x10000));
        }

        #endregion

        #region CPU Reset/Execute

        public void Reset65816()
        {
            Registers.A = 0;
            Registers.X = 0;
            Registers.Y = 0;
            Registers.StackPointer = 0x1FF;
            Registers.DataBank = 0;
            Registers.DirectPage = 0;
            Registers.ProgramBank = 0;

            Registers.P = 0;
            SetFlag(Accumulator8BitsFlag);
            SetFlag(Index8BitsFlag);

            _emulate6502 = true;
            STPDisable = false;
            _waiDisable = false;

            Cycles = 0;
            Registers.ProgramCounter = ReadMemory16(0, 0xFFFC);
        }

        private static string Hex(byte text)
        {
            return string.Format("{0:X}", text);
        }

        private static string Hex(int text)
        {
            return string.Format("{0:X}", text);
        }

        public void Execute65816(double targetCycles)
        {
            while (Cycles < targetCycles)
            {
                var opCode = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter);
                if (Debug)
                {
                    if (_writer == null)
                    {
                        _writer = new StreamWriter(File.Open(_startup + @"\SNES.Net Debug.txt", FileMode.Create));
                    }
                    _writer.WriteLine("PC: " + Hex(Registers.ProgramBank) + ":" +
                                  Hex(Registers.ProgramCounter) + " DBR: " +
                                  Hex(Registers.DataBank) + " D: " + Hex(Registers.DirectPage) + " SP: " +
                                  Hex(Registers.StackPointer) + " P: " + Hex(Registers.P) + " A: " + Hex(Registers.A) +
                                  " X: " + Hex(Registers.X) + " Y: " + Hex(Registers.Y) + " EA OLD: " +
                                  Hex(_effectiveAddress) + " -- OP: " + Hex(opCode) +  " Cycles:" + Cycles);
                }
                Registers.ProgramCounter += 1;
                _pageCrossed = false;
                switch (opCode)
                {
                     #region Cases

                    case 0x61: //ADC (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                            Cycles += 1;
                        }
                        if ((byte) (Registers.DirectPage & 0xFF) != 0) { Cycles += 1; }
                        Cycles += 6;
                        break;
                    case 0x63: //ADC sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0x65: //ADC dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 3;
                        break;
                    case 0x67: //ADC dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 6;
                        break;
                    case 0x69: //ADC #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            AddWithCarry();
                        }
                        else
                        {
                            Immediate16();
                            AddWithCarry16();
                        }
                        Cycles += 2;
                        break;
                    case 0x6D: //ADC addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0x6F: //ADC long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0x71: //ADC ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        if (_pageCrossed) { Cycles += 1; }
                        Cycles += 5;
                        break;
                    case 0x72: //ADC (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0x73: //ADC (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 7;
                        break;
                    case 0x75: //ADC dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0x77: //ADC dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 6;
                        break;
                    case 0x79: //ADC addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0x7D: //ADC addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0x7F: //ADC long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AddWithCarry();
                        }
                        else
                        {
                            AddWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0x21: //AND (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x23: //AND sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x25: //AND dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 3;
                        break;
                    case 0x27: //AND dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x29: //AND #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            AndWithAccumulator();
                        }
                        else
                        {
                            Immediate16();
                            AndWithAccumulator16();
                        }
                        Cycles += 2;
                        break;
                    case 0x2D: //AND addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x2F: //AND long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x31: //AND ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x32: //AND (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x33: //AND (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 7;
                        break;
                    case 0x35: //AND dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x37: //AND dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x39: //AND addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x3D: //AND addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x3F: //AND long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            AndWithAccumulator();
                        }
                        else
                        {
                            AndWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x6: //ASL dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ArithmeticShiftLeft();
                        }
                        else
                        {
                            ArithmeticShiftLeft16();
                            Cycles += 2;
                        }
                        Cycles += 5;
                        break;
                    case 0xA: //ASL A
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ArithmeticShiftLeftA();
                        }
                        else
                        {
                            ArithmeticShiftLeftA16();
                        }
                        Cycles += 2;
                        break;
                    case 0xE: //ASL addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ArithmeticShiftLeft();
                        }
                        else
                        {
                            ArithmeticShiftLeft16();
                        }
                        Cycles += 6;
                        break;
                    case 0x16: //ASL dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ArithmeticShiftLeft();
                        }
                        else
                        {
                            ArithmeticShiftLeft16();
                        }
                        Cycles += 6;
                        break;
                    case 0x1E: //ASL addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ArithmeticShiftLeft();
                        }
                        else
                        {
                            ArithmeticShiftLeft16();
                        }
                        Cycles += 7;
                        break;
                    case 0x90:
                        BranchOnCarryClear();
                        Cycles += 2; //BCC nearlabel
                        break;
                    case 0xB0:
                        BranchOnCarrySet();
                        Cycles += 2; //BCS nearlabel
                        break;
                    case 0xF0:
                        BranchOnEqual();
                        Cycles += 2; //BEQ nearlabel
                        break;
                    case 0x24: //BIT dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestBits();
                        }
                        else
                        {
                            TestBits16();
                        }
                        Cycles += 3;
                        break;
                    case 0x2C: //BIT addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestBits();
                        }
                        else
                        {
                            TestBits16();
                        }
                        Cycles += 4;
                        break;
                    case 0x34: //BIT dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestBits();
                        }
                        else
                        {
                            TestBits16();
                        }
                        Cycles += 4;
                        break;
                    case 0x3C: //BIT addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestBits();
                        }
                        else
                        {
                            TestBits16();
                        }
                        Cycles += 4;
                        break;
                    case 0x89: //BIT #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            TestBits();
                        }
                        else
                        {
                            Immediate16();
                            TestBits16();
                        }
                        Cycles += 2;
                        break;
                    case 0x30:
                        BranchOnMinus();
                        Cycles += 2; //BMI nearlabel
                        break;
                    case 0xD0:
                        BranchOnNotEqual();
                        Cycles += 2; //BNE nearlabel
                        break;
                    case 0x10:
                        BranchOnPlus();
                        Cycles += 2; //BPL nearlabel
                        break;
                    case 0x80:
                        BranchAlways();
                        Cycles += 3; //BRA nearlabel
                        break;
                    case 0x0:
                        Break();
                        if (_emulate6502)
                        {
                            Cycles += 7;
                        }
                        else
                        {
                            Cycles += 8; //BRK
                        }
                        break;
                    case 0x82:
                        BranchLongAlways();
                        Cycles += 4; //BRL label
                        break;
                    case 0x50:
                        BranchOnOverflowClear();
                        Cycles += 2; //BVC nearlabel
                        break;
                    case 0x70:
                        BranchOnOverflowSet();
                        Cycles += 2; //BVS nearlabel
                        break;
                    case 0x18:
                        ClearCarry();
                        Cycles += 2; //CLC
                        break;
                    case 0xD8:
                        ClearDecimal();
                        Cycles += 2; //CLD
                        break;
                    case 0x58:
                        ClearInterruptDisable();
                        Cycles += 2; //CLI
                        break;
                    case 0xB8:
                        ClearOverflow();
                        Cycles += 2; //CLV
                        break;
                    case 0xC1: //CMP (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 6;
                        break;
                    case 0xC3: //CMP sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 4;
                        break;
                    case 0xC5: //CMP dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 3;
                        break;
                    case 0xC7: //CMP dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 6;
                        break;
                    case 0xC9: //CMP #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            Compare();
                        }
                        else
                        {
                            Immediate16();
                            Compare16();
                        }
                        Cycles += 2;
                        break;
                    case 0xCD: //CMP addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 4;
                        break;
                    case 0xCF: //CMP long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 5;
                        break;
                    case 0xD1: //CMP ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 5;
                        break;
                    case 0xD2: //CMP (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 5;
                        break;
                    case 0xD3: //CMP (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 7;
                        break;
                    case 0xD5: //CMP dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 4;
                        break;
                    case 0xD7: //CMP dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 6;
                        break;
                    case 0xD9: //CMP addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 4;
                        break;
                    case 0xDD: //CMP addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 4;
                        break;
                    case 0xDF: //CMP long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Compare();
                        }
                        else
                        {
                            Compare16();
                        }
                        Cycles += 5;
                        break;
                    case 0x2:
                        CoPEnable();
                        Cycles += 7; //COP const
                        break;
                    case 0xE0: //CPX #const
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            Immediate();
                            CompareWithX();
                        }
                        else
                        {
                            Immediate16();
                            CompareWithX16();
                        }
                        Cycles += 2;
                        break;
                    case 0xE4: //CPX dp
                        ZeroPage();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            CompareWithX();
                        }
                        else
                        {
                            CompareWithX16();
                        }
                        Cycles += 3;
                        break;
                    case 0xEC: //CPX addr
                        Absolute();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            CompareWithX();
                        }
                        else
                        {
                            CompareWithX16();
                        }
                        Cycles += 4;
                        break;
                    case 0xC0: //CPY #const
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            Immediate();
                            CompareWithY();
                        }
                        else
                        {
                            Immediate16();
                            CompareWithY16();
                        }
                        Cycles += 2;
                        break;
                    case 0xC4: //CPY dp
                        ZeroPage();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            CompareWithY();
                        }
                        else
                        {
                            CompareWithY16();
                        }
                        Cycles += 3;
                        break;
                    case 0xCC: //CPY addr
                        Absolute();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            CompareWithY();
                        }
                        else
                        {
                            CompareWithY16();
                        }
                        Cycles += 4;
                        break;
                    case 0x3A: //DEC A 
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            DecrementA();
                        }
                        else
                        {
                            DecrementA16();
                        }
                        Cycles += 2;
                        break;
                    case 0xC6: //DEC dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Decrement();
                        }
                        else
                        {
                            Decrement16();
                        }
                        Cycles += 5;
                        break;
                    case 0xCE: //DEC addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Decrement();
                        }
                        else
                        {
                            Decrement16();
                        }
                        Cycles += 6;
                        break;
                    case 0xD6: //DEC dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Decrement();
                        }
                        else
                        {
                            Decrement16();
                        }
                        Cycles += 6;
                        break;
                    case 0xDE: //DEC addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Decrement();
                        }
                        else
                        {
                            Decrement16();
                        }
                        Cycles += 7;
                        break;
                    case 0xCA: //DEX
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            DecrementX();

                        }
                        else
                        {
                            DecrementX16();
                        }
                        Cycles += 2;
                        break;
                    case 0x88: //DEY
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            DecrementY();
                        }
                        else
                        {
                            DecrementY16();
                        }
                        Cycles += 2;
                        break;
                    case 0x41: //EOR (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 6;
                        break;
                    case 0x43: //EOR sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 4;
                        break;
                    case 0x45: //EOR dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 3;
                        break;
                    case 0x47: //EOR dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 6;
                        break;
                    case 0x49: //EOR #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            ExclusiveOr();
                        }
                        else
                        {
                            Immediate16();
                            ExclusiveOr16();
                        }
                        Cycles += 2;
                        break;
                    case 0x4D: //EOR addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 4;
                        break;
                    case 0x4F: //EOR long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 5;
                        break;
                    case 0x51: //EOR ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 5;
                        break;
                    case 0x52: //EOR (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 5;
                        break;
                    case 0x53: //EOR (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 7;
                        break;
                    case 0x55: //EOR dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 4;
                        break;
                    case 0x57: //EOR dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 6;
                        break;
                    case 0x59: //EOR addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 4;
                        break;
                    case 0x5D: //EOR addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 4;
                        break;
                    case 0x5F: //EOR long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            ExclusiveOr();
                        }
                        else
                        {
                            ExclusiveOr16();
                        }
                        Cycles += 5;
                        break;
                    case 0x1A: //INC A
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            IncrementA();
                        }
                        else
                        {
                            IncrementA16();
                        }
                        Cycles += 2;
                        break;
                    case 0xE6: //INC dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Increment();
                        }
                        else
                        {
                            Increment16();
                        }
                        Cycles += 5;
                        break;
                    case 0xEE: //INC addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Increment();
                        }
                        else
                        {
                            Increment16();
                        }
                        Cycles += 6;
                        break;
                    case 0xF6: //INC dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Increment();
                        }
                        else
                        {
                            Increment16();
                        }
                        Cycles += 6;
                        break;
                    case 0xFE: //INC addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Increment();
                        }
                        else
                        {
                            Increment16();
                        }
                        Cycles += 7;
                        break;

                    case 0xE8: //INX
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            IncrementX();
                        }
                        else
                        {
                            IncrementX16();
                        }
                        Cycles += 2;
                        break;

                    case 0xC8: //INY
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            IncrementY();
                        }
                        else
                        {
                            IncrementY16();
                        }
                        Cycles += 2;
                        break;

                    case 0x4C:
                        Absolute();
                        Jump();
                        Cycles += 3; //JMP addr
                        break;
                    case 0x5C:
                        AbsoluteLong();
                        Jump();
                        Registers.ProgramBank = (byte) ((_effectiveAddress & 0xFF0000)/0x10000);
                        Cycles += 4; //JMP long
                        break;
                    case 0x6C:
                        Indirect();
                        Jump();
                        Cycles += 5; //JMP (_addr_)
                        break;
                    case 0x7C:
                        IndirectX();
                        Jump();
                        Cycles += 6; //JMP (_addr,X_)
                        break;
                    case 0xDC:
                        IndirectLongJump();
                        Jump();
                        Registers.ProgramBank = (byte) ((_effectiveAddress & 0xFF0000)/0x10000);
                        Cycles += 6; //JMP addr
                        break;
                    case 0x20:
                        Absolute();
                        JumpToSubroutine();
                        Cycles += 6; //JSR addr
                        break;
                    case 0x22:
                        AbsoluteLong();
                        JumpToSubroutine(true);
                        Cycles += 8; //JSR long
                        break;
                    case 0xFC:
                        IndirectX();
                        JumpToSubroutine();
                        Cycles += 8; //JSR (addr,X)
                        break;
                    case 0xA1: //LDA (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0xA3: //LDA sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xA5: //LDA dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 3;
                        break;
                    case 0xA7: //LDA dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0xA9: //LDA #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            LoadAccumulator();
                        }
                        else
                        {
                            Immediate16();
                            LoadAccumulator16();
                        }
                        Cycles += 2;
                        break;
                    case 0xAD: //LDA addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xAF: //LDA long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0xB1: //LDA ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0xB2: //LDA (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0xB3: //LDA (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 7;
                        break;
                    case 0xB5: //LDA dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xB7: //LDA dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0xB9: //LDA addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xBD: //LDA addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xBF: //LDA long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LoadAccumulator();
                        }
                        else
                        {
                            LoadAccumulator16();
                        }
                        Cycles += 5;
                        break;

                    case 0xA2: //LDX #const
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            Immediate();
                            LoadX();
                        }
                        else
                        {
                            Immediate16();
                            LoadX16();
                        }
                        Cycles += 2;
                        break;
                    case 0xA6: //LDX dp
                        ZeroPage();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadX();
                        }
                        else
                        {
                            LoadX16();
                        }
                        Cycles += 3;
                        break;
                    case 0xAE: //LDX addr
                        Absolute();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadX();
                        }
                        else
                        {
                            LoadX16();
                        }
                        Cycles += 4;
                        break;
                    case 0xB6: //LDX dp,Y
                        ZeroPageY();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadX();
                        }
                        else
                        {
                            LoadX16();
                        }
                        Cycles += 4;
                        break;
                    case 0xBE: //LDX addr,Y
                        AbsoluteY();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadX();
                        }
                        else
                        {
                            LoadX16();
                        }
                        Cycles += 4;
                        break;
                    case 0xA0: //LDY #const
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            Immediate();
                            LoadY();
                        }
                        else
                        {
                            Immediate16();
                            LoadY16();
                        }
                        Cycles += 2;
                        break;
                    case 0xA4: //LDY dp
                        ZeroPage();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadY();
                        }
                        else
                        {
                            LoadY16();
                        }
                        Cycles += 3;
                        break;
                    case 0xAC: //LDY addr
                        Absolute();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadY();
                        }
                        else
                        {
                            LoadY16();
                        }
                        Cycles += 4;
                        break;
                    case 0xB4: //LDY dp,X
                        ZeroPageX();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadY();
                        }
                        else
                        {
                            LoadY16();
                        }
                        Cycles += 4;
                        break;
                    case 0xBC: //LDY addr,X
                        AbsoluteX();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            LoadY();
                        }
                        else
                        {
                            LoadY16();
                        }
                        Cycles += 4;
                        break;

                    case 0x46: //LSR dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LogicalShiftRight();
                        }
                        else
                        {
                            LogicalShiftRight16();
                        }
                        Cycles += 5;
                        break;
                    case 0x4A: //LSR A
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LogicalShiftRightA();
                        }
                        else
                        {
                            LogicalShiftRightA16();
                        }
                        Cycles += 2;
                        break;
                    case 0x4E: //LSR addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LogicalShiftRight();
                        }
                        else
                        {
                            LogicalShiftRight16();
                        }
                        Cycles += 6;
                        break;
                    case 0x56: //LSR dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LogicalShiftRight();
                        }
                        else
                        {
                            LogicalShiftRight16();
                        }
                        Cycles += 6;
                        break;
                    case 0x5E: //LSR addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            LogicalShiftRight();
                        }
                        else
                        {
                            LogicalShiftRight16();
                        }
                        Cycles += 7;
                        break;
                    case 0x54:
                        BlockMoveNegative();
                        Cycles += 1; //MVN srcbk,destbk
                        break;
                    case 0x44:
                        BlockMovePositive();
                        Cycles += 1; //MVP srcbk,destbk
                        break;
                    case 0xEA:
                        Cycles += 2; //NOP
                        break;
                    case 0x1: //ORA (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x3: //ORA sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x5: //ORA dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 3;
                        break;
                    case 0x7: //ORA dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x9: //ORA #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            OrWithAccumulator();
                        }
                        else
                        {
                            Immediate16();
                            OrWithAccumulator16();
                        }
                        Cycles += 2;
                        break;
                    case 0xD: //ORA addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xF: //ORA long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x11: //ORA ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x12: //ORA (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x13: //ORA (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 7;
                        break;
                    case 0x15: //ORA dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x17: //ORA dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x19: //ORA addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x1D: //ORA addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x1F: //ORA long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            OrWithAccumulator();
                        }
                        else
                        {
                            OrWithAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0xF4:
                        Absolute();
                        PushEffectiveAddress();
                        Cycles += 5; //PEA addr
                        break;
                    case 0xD4:
                        DPIndirect();
                        PushEffectiveAddress();
                        Cycles += 6; //PEI (dp)
                        break;
                    case 0x62: //PER label
                        _effectiveAddress = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter);
                        Registers.ProgramCounter += 2;
                        _effectiveAddress += Registers.ProgramCounter;
                        PushEffectiveAddress();
                        Cycles += 6;
                        break;
                    case 0x48: //PHA
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            PushAccumulator();
                        }
                        else
                        {
                            PushAccumulator16();
                        }
                        Cycles += 3;
                        break;
                    case 0x8B:
                        PushDataBank();
                        Cycles += 3; //PHB
                        break;
                    case 0xB:
                        PushDirectPage();
                        Cycles += 4; //PHD
                        break;
                    case 0x4B:
                        PushProgramBank();
                        Cycles += 3; //PHK
                        break;
                    case 0x8:
                        PushProcessorStatus();
                        Cycles += 3; //PHP
                        break;
                    case 0xDA: //PHX
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            PushX();
                        }
                        else
                        {
                            PushX16();
                        }
                        Cycles += 3;
                        break;
                    case 0x5A:
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            PushY();
                        }
                        else
                        {
                            PushY16();
                        }
                        Cycles += 3; //PHY
                        break;
                    case 0x68: //PLA
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            PullAccumulator();
                        }
                        else
                        {
                            PullAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0xAB:
                        PullDataBank();
                        Cycles += 4; //PLB
                        break;
                    case 0x2B:
                        PullDirectPage();
                        Cycles += 5; //PLD
                        break;
                    case 0x28:
                        PullProcessorStatus();
                        Cycles += 4; //PLP
                        break;
                    case 0xFA: //PLX
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            PullX();
                        }
                        else
                        {
                            PullX16();
                        }
                        Cycles += 4;
                        break;
                    case 0x7A: //PLY
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            PullY();
                        }
                        else
                        {
                            PullY16();

                        }
                        Cycles += 4;
                        break;
                    case 0xC2:
                        Immediate();
                        ResetStatus();
                        Cycles += 3; //REP #const
                        break;

                    case 0x26: //ROL dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateLeft();
                        }
                        else
                        {
                            RotateLeft16();
                        }
                        Cycles += 5;
                        break;
                    case 0x2A: //ROL A
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateLeftA();
                        }
                        else
                        {
                            RotateLeftA16();
                        }
                        Cycles += 2;
                        break;
                    case 0x2E: //ROL addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateLeft();
                        }
                        else
                        {
                            RotateLeft16();
                        }
                        Cycles += 6;
                        break;
                    case 0x36: //ROL dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateLeft();
                        }
                        else
                        {
                            RotateLeft16();
                        }
                        Cycles += 6;
                        break;
                    case 0x3E: //ROL addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateLeft();
                        }
                        else
                        {
                            RotateLeft16();
                        }
                        Cycles += 7;
                        break;
                    case 0x66: //ROR dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateRight();
                        }
                        else
                        {
                            RotateRight16();
                        }
                        Cycles += 5;
                        break;
                    case 0x6A: //ROR A
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateRightA();
                        }
                        else
                        {
                            RotateRightA16();
                        }
                        Cycles += 2;
                        break;
                    case 0x6E: //ROR addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateRight();
                        }
                        else
                        {
                            RotateRight16();
                        }
                        Cycles += 6;
                        break;
                    case 0x76: //ROR dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateRight();
                        }
                        else
                        {
                            RotateRight16();
                        }
                        Cycles += 6;
                        break;
                    case 0x7E: //ROR addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            RotateRight();
                        }
                        else
                        {
                            RotateRight16();
                        }
                        Cycles += 7;
                        break;
                    case 0x40:
                        ReturnFromInterrupt();
                        Cycles += 6; //RTI
                        break;
                    case 0x6B:
                        ReturnFromSubroutineLong();
                        Cycles += 6; //RTL
                        break;
                    case 0x60:
                        ReturnFromSubroutine();
                        Cycles += 6; //RTS
                        break;
                    case 0xE1: //SBC (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 6;
                        break;
                    case 0xE3: //SBC sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0xE5: //SBC dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 3;
                        break;
                    case 0xE7: //SBC dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 6;
                        break;
                    case 0xE9: //SBC #const
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            Immediate();
                            SubtractWithCarry();
                        }
                        else
                        {
                            Immediate16();
                            SubtractWithCarry16();
                        }
                        Cycles += 2;
                        break;
                    case 0xED: //SBC addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0xEF: //SBC long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0xF1: //SBC ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0xF2: //SBC (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0xF3: //SBC (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 7;
                        break;
                    case 0xF5: //SBC dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0xF7: //SBC dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 6;
                        break;
                    case 0xF9: //SBC addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0xFD: //SBC addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 4;
                        break;
                    case 0xFF: //SBC long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            SubtractWithCarry();
                        }
                        else
                        {
                            SubtractWithCarry16();
                        }
                        Cycles += 5;
                        break;
                    case 0x38:
                        SetCarry();
                        Cycles += 2; //SEC
                        break;
                    case 0xF8:
                        SetDecimal();
                        Cycles += 2; //SED
                        break;
                    case 0x78:
                        SetInterruptDisable();
                        Cycles += 2; //SEI
                        break;
                    case 0xE2:
                        Immediate();
                        SetStatus();
                        Cycles += 3; //SEP
                        break;
                    case 0x81: //STA (_dp_,X)
                        DPIndirectX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x83: //STA sr,S
                        StackRelative();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x85: //STA dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 3;
                        break;
                    case 0x87: //STA dp
                        IndirectLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x8D: //STA addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x8F: //STA long
                        AbsoluteLong();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x91: //STA ( dp),Y
                        IndirectY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x92: //STA (_dp_)
                        DPIndirect();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0x93: //STA (_sr_,S),Y
                        IndirectStackY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 7;
                        break;
                    case 0x95: //STA dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x97: //STA dp,Y
                        IndirectLongY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 6;
                        break;
                    case 0x99: //STA addr,Y
                        AbsoluteY();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x9D: //STA addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 4;
                        break;
                    case 0x9F: //STA long,X
                        AbsoluteLongX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreAccumulator();
                        }
                        else
                        {
                            StoreAccumulator16();
                        }
                        Cycles += 5;
                        break;
                    case 0xDB:
                        StopProcessor();
                        Cycles += 3; //STP 
                        break;
                    case 0x86: //STX dp
                        ZeroPage();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            StoreX();
                        }
                        else
                        {
                            StoreX16();
                        }
                        Cycles += 3;
                        break;
                    case 0x8E: //STX addr
                        Absolute();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            StoreX();
                        }
                        else
                        {
                            StoreX16();
                        }
                        Cycles += 4;
                        break;
                    case 0x96: //STX dp,Y
                        ZeroPageY();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            StoreX();
                        }
                        else
                        {
                            StoreX16();
                        }
                        Cycles += 4;
                        break;
                    case 0x84: //STY dp
                        ZeroPage();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            StoreY();
                        }
                        else
                        {
                            StoreY16();
                        }
                        Cycles += 3;
                        break;
                    case 0x8C: //STY addr
                        Absolute();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            StoreY();
                        }
                        else
                        {
                            StoreY16();
                        }
                        Cycles += 4;
                        break;
                    case 0x94: //STY dp,X
                        ZeroPageX();
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            StoreY();
                        }
                        else
                        {
                            StoreY16();
                        }
                        Cycles += 4;
                        break;
                    case 0x64: //STZ dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreZero();
                        }
                        else
                        {
                            StoreZero16();
                        }
                        Cycles += 3;
                        break;
                    case 0x74: //STZ dp,X
                        ZeroPageX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreZero();
                        }
                        else
                        {
                            StoreZero16();
                        }
                        Cycles += 4;
                        break;
                    case 0x9C: //STZ addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreZero();
                        }
                        else
                        {
                            StoreZero16();
                        }
                        Cycles += 4;
                        break;
                    case 0x9E: //STZ addr,X
                        AbsoluteX();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            StoreZero();
                        }
                        else
                        {
                            StoreZero16();
                        }
                        Cycles += 5;
                        break;

                    case 0xAA: //TAX
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            TransferAccumulatorToX();
                        }
                        else
                        {
                            TransferAccumulatorToX16();
                        }
                        Cycles += 2;
                        break;
                    case 0xA8: //TAY
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            TransferAccumulatorToY();
                        }
                        else
                        {
                            TransferAccumulatorToY16();
                        }
                        Cycles += 2;
                        break;
                    case 0x5B:
                        TransferAccumulatorToDP();
                        Cycles += 2; //TCD
                        break;
                    case 0x1B:
                        TransferAccumulatorToSP();
                        Cycles += 2; //TCS
                        break;
                    case 0x7B:
                        TransferDPToAccumulator();
                        Cycles += 2; //TDC
                        break;

                    case 0x14: //TRB dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestAndResetBit();
                        }
                        else
                        {
                            TestAndResetBit16();
                        }
                        Cycles += 5;
                        break;
                    case 0x1C: //TRB addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestAndResetBit();
                        }
                        else
                        {
                            TestAndResetBit16();
                        }
                        Cycles += 6;
                        break;
                    case 0x4: //TSB dp
                        ZeroPage();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestAndSetBit();
                        }
                        else
                        {
                            TestAndSetBit16();
                        }
                        Cycles += 5;
                        break;
                    case 0xC: //TSB addr
                        Absolute();
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TestAndSetBit();
                        }
                        else
                        {
                            TestAndSetBit16();
                        }
                        Cycles += 6;
                        break;

                    case 0x3B:
                        TransferSPtoAccumulator();
                        Cycles += 2; //TSC
                        break;
                    case 0xBA:
                        TransferSPtoX();
                        Cycles += 2; //TSX
                        break;
                    case 0x8A: //TXA
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TransferXtoAccumulator();
                        }
                        else
                        {
                            TransferXtoAccumulator16();
                        }
                        Cycles += 2;
                        break;
                    case 0x9A:
                        TransferXtoSP();
                        Cycles += 2; //TXS
                        break;
                    case 0x9B: //TXY
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            TransferXtoY();
                        }
                        else
                        {
                            TransferXtoY16();
                        }
                        Cycles += 2;
                        break;
                    case 0x98: //TYA
                        if ((Registers.P & Accumulator8BitsFlag) != 0)
                        {
                            TransferYtoAccumulator();
                        }
                        else
                        {
                            TransferYToAccumulator16();
                        }
                        Cycles += 2;
                        break;
                    case 0xBB: //TYX
                        if ((Registers.P & Index8BitsFlag) != 0)
                        {
                            TransferYtoX();
                        }
                        else
                        {
                            TransferYtoX16();
                        }
                        Cycles += 2;
                        break;
                    case 0xCB:
                        WaitForInterrupt();
                        Cycles += 3; //WAI
                        break;
                    case 0x42:
                        break;
                    case 0xEB:
                        ExchangeAccumulator();
                        Cycles += 3; //XBA 
                        break;
                    case 0xFB:
                        ExchangeCarryAndEmulation();
                        Cycles += 2; //XCE
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show("Opcode not implemented : " + String.Format("{0:X}", Registers.ProgramCounter) + " -> " + String.Format("{0:X}", opCode));
                        Cycles += 1;
                        break;

                        #endregion
                }
            }
            Cycles -= targetCycles;
        }

        #endregion

        #region Interrupts

        public void IRQ()
        {
            if ((Registers.P & InterruptFlag) != 0)
            {
                if (_waiDisable)
                {
                    Registers.ProgramCounter += 1;
                    _waiDisable = false;
                }
            }
            IRQOcurred = true;
            if (_emulate6502)
            {
                Push16(Registers.ProgramCounter);
                Push((byte) (Registers.P | 0x30));
                Registers.ProgramBank = 0;
                Registers.ProgramCounter = ReadMemory16(0, 0xFFFE);
                SetFlag(InterruptFlag);
                Cycles += 7;
            }
            else
            {
                Push(Registers.ProgramBank);
                Push16(Registers.ProgramCounter);
                Push(Registers.P);
                Registers.ProgramBank = 0;
                Registers.ProgramCounter = ReadMemory16(0, 0xFFEE);
                SetFlag(InterruptFlag);
                Cycles += 8;
            }
        }

        public void NMI()
        {
            if ((Registers.P & InterruptFlag) != 0)
            {
                if (_waiDisable)
                {
                    Registers.ProgramCounter += 1;
                    _waiDisable = false;
                }
            }
            if (_emulate6502)
            {
                Push16(Registers.ProgramCounter);
                Push((byte) (Registers.P | 0x30));
                Registers.ProgramBank = 0;
                Registers.ProgramCounter = ReadMemory16(0, 0xFFFA);
                SetFlag(InterruptFlag);
                Cycles += 7;
            }
            else
            {
                Push(Registers.ProgramBank);
                Push16(Registers.ProgramCounter);
                Push(Registers.P);
                Registers.ProgramBank = 0;
                Registers.ProgramCounter = ReadMemory16(0, 0xFFEA);
                SetFlag(InterruptFlag);
                Cycles += 8;
            }
        }

        #endregion

        #region Addressing Modes

        private void Immediate() //8 bits
        {
            _effectiveAddress = Registers.ProgramCounter + (Registers.ProgramBank*0x10000);
            Registers.ProgramCounter += 1;
        }

        private void Immediate16() //16 bits
        {
            _effectiveAddress = Registers.ProgramCounter + (Registers.ProgramBank*0x10000);
            Registers.ProgramCounter += 2;
        }

        private void ZeroPage()
        {
            _effectiveAddress = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage;
            Registers.ProgramCounter += 1;
        }

        private void ZeroPageX()
        {
            _effectiveAddress = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage + Registers.X;
            Registers.ProgramCounter += 1;
        }

        private void ZeroPageY()
        {
            _effectiveAddress = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage + Registers.Y;
            Registers.ProgramCounter += 1;
        }

        private void StackRelative()
        {
            _effectiveAddress = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.StackPointer;
            Registers.ProgramCounter += 1;
        }

        private void Absolute()
        {
            _effectiveAddress = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter) + (Registers.DataBank*0x10000);
            Registers.ProgramCounter += 2;
        }

        private void AbsoluteX()
        {
            _effectiveAddress = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter) + (Registers.DataBank*0x10000) + Registers.X;
            Registers.ProgramCounter += 2;
        }

        private void AbsoluteY()
        {
            _effectiveAddress = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter) + (Registers.DataBank*0x10000) + Registers.Y;
            Registers.ProgramCounter += 2;
        }

        private void AbsoluteLong()
        {
            _effectiveAddress = ReadMemory24(Registers.ProgramBank, Registers.ProgramCounter);
            Registers.ProgramCounter += 3;
        }

        private void AbsoluteLongX()
        {
            _effectiveAddress = ReadMemory24(Registers.ProgramBank, Registers.ProgramCounter) + Registers.X;
            Registers.ProgramCounter += 3;
        }

        private void Indirect()
        {
            var addr = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter);
            _effectiveAddress = ReadMemory16(Registers.ProgramBank, addr);
            Registers.ProgramCounter += 2;
        }

        private void DPIndirect()
        {
            var addr = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage;
            _effectiveAddress = ReadMemory16(0, addr) + (Registers.DataBank*0x10000);
            Registers.ProgramCounter += 1;
        }

        private void IndirectY()
        {
            var addr = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage;
            _effectiveAddress = ReadMemory16(0, addr) + (Registers.DataBank*0x10000);
            if ((_effectiveAddress & 0xFF00) != ((_effectiveAddress + Registers.Y) & 0xFF00))
            {
                _pageCrossed = true;
            }
            _effectiveAddress += Registers.Y;
            Registers.ProgramCounter += 1;
        }

        private void IndirectStackY()
        {
            var addr = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.StackPointer;
            _effectiveAddress = ReadMemory16(0, addr) + (Registers.DataBank*0x10000) + Registers.Y;
            Registers.ProgramCounter += 1;
        }

        private void IndirectLong()
        {
            var addr = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage;
            _effectiveAddress = ReadMemory24(0, addr);
            Registers.ProgramCounter += 1;
        }

        private void IndirectLongJump()
        {
            var addr = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter);
            _effectiveAddress = ReadMemory24(0, addr);
            Registers.ProgramCounter += 2;
        }

        private void IndirectLongY()
        {
            var addr = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage;
            _effectiveAddress = ReadMemory24(0, addr) + Registers.Y;
            Registers.ProgramCounter += 1;
        }

        private void IndirectX()
        {
            var addr = ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter) + Registers.X;
            _effectiveAddress = ReadMemory16(Registers.ProgramBank, addr);
            Registers.ProgramCounter += 2;
        }

        private void DPIndirectX()
        {
            var addr = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter) + Registers.DirectPage + Registers.X;
            _effectiveAddress = ReadMemory16(0, addr) + (Registers.DataBank*0x10000);
            Registers.ProgramCounter += 1;
        }

        #endregion

        #region Instructions

        private void AddWithCarry() //ADC (8 bits)
        {
            if ((Registers.P & DecimalFlag) == 0)
            {
                var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
                var result = (Registers.A & 0xFF) + value + (Registers.P & CarryFlag);
                TestFlag(result > 0xFF, CarryFlag);
                TestFlag(((~((Registers.A & 0xFF) ^ value)) & ((Registers.A & 0xFF) ^ result) & 0x80) != 0, OverflowFlag);
                Registers.A = (result & 0xFF) + (Registers.A & 0xFF00);
                SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
            }
            else
            {
                AddWithCarryBCD();
            }
        }

        private void AddWithCarryBCD() //ADC (BCD) (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = (Registers.A & 0xF) + (value & 0xF) + (Registers.P & CarryFlag);
            if (result > 9) { result += 6; }
            TestFlag(result > 0xF, CarryFlag);
            result = (Registers.A & 0xF0) + (value & 0xF0) + (result & 0xF) + ((Registers.P & CarryFlag)*0x10);
            TestFlag(((~((Registers.A & 0xFF) ^ value)) & ((Registers.A & 0xFF) ^ result) & 0x80) != 0, OverflowFlag);
            if (result > 0x9F) { result += 0x60; }
            TestFlag(result > 0xFF, CarryFlag);
            Registers.A = (result & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void AddWithCarry16() //ADC (16 bits)
        {
            if ((Registers.P & DecimalFlag) == 0)
            {
                var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
                var result = Registers.A + value + (Registers.P & CarryFlag);
                TestFlag(result > 0xFFFF, CarryFlag);
                TestFlag(((~(Registers.A ^ value)) & (Registers.A ^ result) & 0x8000) != 0, OverflowFlag);
                Registers.A = result & 0xFFFF;
                SetZeroNegativeFlag16(Registers.A);
            }
            else
            {
                AddWithCarryBCD16();
            }
        }

        private void AddWithCarryBCD16() //ADC (BCD) (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = (Registers.A & 0xF) + (value & 0xF) + (Registers.P & CarryFlag);
            if (result > 9) { result += 6; }
            TestFlag(result > 0xF, CarryFlag);
            result = (Registers.A & 0xF0) + (value & 0xF0) + (result & 0xF) + ((Registers.P & CarryFlag)*0x10);
            if (result > 0x9F) { result += 0x60; }
            TestFlag(result > 0xFF, CarryFlag);
            result = (Registers.A & 0xF00) + (value & 0xF00) + (result & 0xFF) + ((Registers.P & CarryFlag)*0x100);
            if (result > 0x9FF) { result += 0x600; }
            TestFlag(result > 0xFFF, CarryFlag);
            result = (Registers.A & 0xF000) + (value & 0xF000) + (result & 0xFFF) + ((Registers.P & CarryFlag)*0x1000);
            TestFlag(((~(Registers.A ^ value)) & (Registers.A ^ result) & 0x8000) != 0, OverflowFlag);
            if (result > 0x9FFF) { result += 0x6000; }
            TestFlag(result > 0xFFFF, CarryFlag);
            Registers.A = result & 0xFFFF;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void AndWithAccumulator() //AND (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            Registers.A = (value & (Registers.A & 0xFF)) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void AndWithAccumulator16() //AND (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            Registers.A = Registers.A & value;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void ArithmeticShiftLeft() //ASL (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((value & 0x80) != 0, CarryFlag);
            value = (byte) ((value << 1) & 0xFF);
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag(value);
        }

        private void ArithmeticShiftLeftA() //ASL_A (8 bits)
        {
            TestFlag(((Registers.A & 0xFF) & 0x80) > 0, CarryFlag);
            Registers.A = (((Registers.A & 0xFF) << 1) & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void ArithmeticShiftLeft16() //ASL (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((value & 0x8000) != 0, CarryFlag);
            value = (value << 1) & 0xFFFF;
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag16(value);
        }

        private void ArithmeticShiftLeftA16() //ASL_A(16 bits)
        {
            TestFlag((Registers.A & 0x8000) != 0, CarryFlag);
            Registers.A = (Registers.A << 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void BranchOnCarryClear() //BCC
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & CarryFlag) != 0) return;
            Registers.ProgramCounter += offset;
            Cycles += 1;
        }

        private void BranchOnCarrySet() //BCS
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & CarryFlag) != 0) { Registers.ProgramCounter += offset; }
        }

        private void BranchOnEqual() //BEQ
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & ZeroFlag) != 0) { Registers.ProgramCounter += offset; }
        }

        private void TestBits() //BIT (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((value & (Registers.A & 0xFF)) == 0, ZeroFlag);
            TestFlag((value & 0x80) != 0, NegativeFlag);
            TestFlag((value & 0x40) != 0, OverflowFlag);
        }

        private void TestBits16() //BIT (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((value & Registers.A) == 0, ZeroFlag);
            TestFlag((value & 0x8000) != 0, NegativeFlag);
            TestFlag((value & 0x4000) != 0, OverflowFlag);
        }

        private void BranchOnMinus() //BMI
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & NegativeFlag) != 0) { Registers.ProgramCounter += offset; }
        }

        private void BranchOnNotEqual() //BNE
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & ZeroFlag) == 0) { Registers.ProgramCounter += offset; }
        }

        private void BranchOnPlus() //BPL
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & NegativeFlag) == 0) { Registers.ProgramCounter += offset; }
        }

        private void BranchAlways() //BRA
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            Registers.ProgramCounter += offset;
        }

        private void Break() //BRK
        {
            if (_emulate6502)
            {
                Push16(Registers.ProgramCounter);
                Push((byte) (Registers.P | 0x30));
                Registers.ProgramBank = 0;
                Registers.ProgramCounter = ReadMemory16(0, 0xFFFE);
            }
            else
            {
                Push(Registers.ProgramBank);
                Push16(Registers.ProgramCounter);
                Push(Registers.P);
                Registers.ProgramBank = 0;
                Registers.ProgramCounter = ReadMemory16(0, 0xFFE6);
            }
        }

        private void BranchLongAlways() //BRL
        {
            var offset = SignedInteger(ReadMemory16(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 2;
            Registers.ProgramCounter += offset;
        }

        private void BranchOnOverflowClear() //BVC
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & OverflowFlag) == 0) { Registers.ProgramCounter += offset; }
        }

        private void BranchOnOverflowSet() //BVS
        {
            var offset = SignedByte(ReadMemory(Registers.ProgramBank, Registers.ProgramCounter));
            Registers.ProgramCounter += 1;
            if ((Registers.P & OverflowFlag) > 0) { Registers.ProgramCounter += offset; }
        }

        private void ClearCarry() //CLC
        {
            ClearFlag(CarryFlag);
        }

        private void ClearDecimal() //CLD
        {
            ClearFlag(DecimalFlag);
        }

        private void ClearInterruptDisable() //CLI
        {
            ClearFlag(InterruptFlag);
        }

        private void ClearOverflow() //CLV
        {
            ClearFlag(OverflowFlag);
        }

        private void Compare() //CMP (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = (Registers.A & 0xFF) - value;
            TestFlag((Registers.A & 0xFF) >= value, CarryFlag);
            SetZeroNegativeFlag((byte) (result & 0xFF));
        }

        private void Compare16() //CMP (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = Registers.A - value;
            TestFlag(Registers.A >= value, CarryFlag);
            SetZeroNegativeFlag16(result);
        }

        private void CoPEnable()
        {
            Push(Registers.ProgramBank);
            Push16(Registers.ProgramCounter);
            Push(Registers.P);
            Registers.ProgramBank = 0;
            Registers.ProgramCounter = ReadMemory16(0, 0xFFE4);
            SetFlag(InterruptFlag);
        }

        private void CompareWithX() //CPX (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = (Registers.X & 0xFF) - value;
            TestFlag((Registers.X & 0xFF) >= value, CarryFlag);
            SetZeroNegativeFlag((byte) (result & 0xFF));
        }

        private void CompareWithX16() //CPX (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = Registers.X - value;
            TestFlag(Registers.X >= value, CarryFlag);
            SetZeroNegativeFlag16(result);
        }

        private void CompareWithY() //CPY (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = (Registers.Y & 0xFF) - value;
            TestFlag((Registers.Y & 0xFF) >= value, CarryFlag);
            SetZeroNegativeFlag((byte) (result & 0xFF));
        }

        private void CompareWithY16() //CPY (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            var result = Registers.Y - value;
            TestFlag(Registers.Y >= value, CarryFlag);
            SetZeroNegativeFlag16(result);
        }

        private void Decrement() //DEC (8 bits)
        {
            var value = (byte) ((ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) - 1) & 0xFF);
            SetZeroNegativeFlag(value);
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void DecrementA() //DEC (8 bits)
        {
            Registers.A = ((Registers.A - 1) & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void Decrement16() //DEC (16 bits)
        {
            var value = (ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) - 1) & 0xFFFF;
            SetZeroNegativeFlag16(value);
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void DecrementA16() //DEC (16 bits)
        {
            Registers.A = (Registers.A - 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void DecrementX() //DEX (8 bits)
        {
            Registers.X = ((Registers.X - 1) & 0xFF) + (Registers.X & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.X & 0xFF));
        }

        private void DecrementX16() //DEX (16 bits)
        {
            Registers.X = (Registers.X - 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.X);
        }

        private void DecrementY() //DEY (8 bits)
        {
            Registers.Y = ((Registers.Y - 1) & 0xFF) + (Registers.Y & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.Y & 0xFF));
        }

        private void DecrementY16() //DEY (16 bits)
        {
            Registers.Y = (Registers.Y - 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.Y);
        }

        private void ExclusiveOr() //EOR (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            Registers.A = ((Registers.A & 0xFF) ^ value) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void ExclusiveOr16() //EOR (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            Registers.A = Registers.A ^ value;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void Increment() //INC (8 bits)
        {
            var value = (byte) ((ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) + 1) & 0xFF);
            SetZeroNegativeFlag(value);
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void IncrementA() //INC (8 bits)
        {
            Registers.A = ((Registers.A + 1) & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void Increment16() //INC (16 bits)
        {
            var value = (ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) + 1) & 0xFFFF;
            SetZeroNegativeFlag16(value);
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void IncrementA16() //INC (16 bits)
        {
            Registers.A = (Registers.A + 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void IncrementX() //INX (8 bits)
        {
            Registers.X = ((Registers.X + 1) & 0xFF) + (Registers.X & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.X & 0xFF));
        }

        private void IncrementX16() //INX (16 bits)
        {
            Registers.X = (Registers.X + 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.X);
        }

        private void IncrementY() //INY (8 bits)
        {
            Registers.Y = ((Registers.Y + 1) & 0xFF) + (Registers.Y & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.Y & 0xFF));
        }

        private void IncrementY16() //INY (16 bits)
        {
            Registers.Y = (Registers.Y + 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.Y);
        }

        private void Jump() //JMP
        {
            Registers.ProgramCounter = _effectiveAddress & 0xFFFF;
        }

        private void JumpToSubroutine(bool dbr = false) //JSR
        {
            if (dbr)
            {
                Push(Registers.ProgramBank);
                Registers.ProgramBank = (byte) ((_effectiveAddress & 0xFF0000)/0x10000);
            }
            Push16((Registers.ProgramCounter - 1) & 0xFFFF);
            Registers.ProgramCounter = _effectiveAddress & 0xFFFF;
        }

        private void LoadAccumulator() //LDA (8 bits)
        {
            Registers.A = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void LoadAccumulator16() //LDA (16 bits)
        {
            Registers.A = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            SetZeroNegativeFlag16(Registers.A);
        }

        private void LoadX() //LDX (8 bits)
        {
            Registers.X = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) + (Registers.X & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.X & 0xFF));
        }

        private void LoadX16() //LDX (16 bits)
        {
            Registers.X = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            SetZeroNegativeFlag16(Registers.X);
        }

        private void LoadY() //LDY (8 bits)
        {
            Registers.Y = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) + (Registers.Y & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.Y & 0xFF));
        }

        private void LoadY16() //LDY (16 bits)
        {
            Registers.Y = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            SetZeroNegativeFlag16(Registers.Y);
        }

        private void LogicalShiftRight() //LSR (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((value & 0x1) != 0, CarryFlag);
            value = (byte) ((value >> 1) & 0xFF);
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag(value);
        }

        private void LogicalShiftRightA() //LSR_A (8 bits)
        {
            TestFlag(((Registers.A & 0xFF) & 0x1) != 0, CarryFlag);
            Registers.A = (((Registers.A & 0xFF) >> 1) & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void LogicalShiftRight16() //LSR (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((value & 0x1) != 0, CarryFlag);
            value = (value >> 1) & 0xFFFF;
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag16(value);
        }

        private void LogicalShiftRightA16() //LSR_A (16 bits)
        {
            TestFlag((Registers.A & 0x1) != 0, CarryFlag);
            Registers.A = (Registers.A >> 1) & 0xFFFF;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void BlockMoveNegative() //MVN
        {
            Registers.DataBank = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter);
            var bank = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter + 1);
            Registers.ProgramCounter += 2;
            var byteToTransfer = ReadMemory(bank, Registers.X);
            Registers.X = (Registers.X + 1) & 0xFFFF;
            WriteMemory(Registers.DataBank, Registers.Y, byteToTransfer);
            Registers.Y = (Registers.Y + 1) & 0xFFFF;
            Registers.A = (Registers.A - 1) & 0xFFFF;
            if (Registers.A != 0xFFFF) { Registers.ProgramCounter -= 3; }
        }

        private void BlockMovePositive() //MVP
        {
            Registers.DataBank = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter);
            var bank = ReadMemory(Registers.ProgramBank, Registers.ProgramCounter + 1);
            Registers.ProgramCounter += 2;
            var byteToTransfer = ReadMemory(bank, Registers.X);
            Registers.X = (Registers.X - 1) & 0xFFFF;
            WriteMemory(Registers.DataBank, Registers.Y, byteToTransfer);
            Registers.Y = (Registers.Y - 1) & 0xFFFF;
            Registers.A = (Registers.A - 1) & 0xFFFF;
            if (Registers.A != 0xFFFF) { Registers.ProgramCounter -= 3; }
        }

        private void OrWithAccumulator() //ORA (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            Registers.A = ((Registers.A & 0xFF) | value) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void OrWithAccumulator16() //ORA (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            Registers.A = Registers.A | value;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void PushEffectiveAddress() //PEA/PEI/PER
        {
            Push16(_effectiveAddress);
        }

        private void PushAccumulator() //PHA (8 bits)
        {
            Push((byte) (Registers.A & 0xFF));
        }

        private void PushAccumulator16() //PHA (16 bits)
        {
            Push16(Registers.A);
        }

        private void PushDataBank() //PHB
        {
            Push(Registers.DataBank);
        }

        private void PushDirectPage() //PHD
        {
            Push16(Registers.DirectPage);
        }

        private void PushProgramBank() //PHK
        {
            Push(Registers.ProgramBank);
        }

        private void PushProcessorStatus() //PHP
        {
            Push(Registers.P);
        }

        private void PushX() //PHX (8 bits)
        {
            Push((byte) (Registers.X & 0xFF));
        }

        private void PushX16() //PHX (16 bits)
        {
            Push16(Registers.X);
        }

        private void PushY() //PHY (8 bits)
        {
            Push((byte) (Registers.Y & 0xFF));
        }

        private void PushY16() //PHY (16 bits)
        {
            Push16(Registers.Y);
        }

        private void PullAccumulator() //PLA (8 bits)
        {
            Registers.A = Pull() + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void PullAccumulator16() //PLA (16 bits)
        {
            Registers.A = Pull16();
            SetZeroNegativeFlag16(Registers.A);
        }

        private void PullDataBank() //PLB
        {
            Registers.DataBank = Pull();
        }

        private void PullDirectPage() //PLD
        {
            Registers.DirectPage = Pull16();
        }

        private void PullProcessorStatus() //PLP
        {
            Registers.P = Pull();
            UpdateMode();
        }

        private void PullX() //PLX (8 bits)
        {
            Registers.X = Pull() + (Registers.X & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.X & 0xFF));
        }

        private void PullX16() //PLX (16 bits)
        {
            Registers.X = Pull16();
            SetZeroNegativeFlag16(Registers.X);
        }

        private void PullY() //PLY (8 bits)
        {
            Registers.Y = Pull() + (Registers.Y & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.Y & 0xFF));
        }

        private void PullY16() //PLY (16 bits)
        {
            Registers.Y = Pull16();
            SetZeroNegativeFlag16(Registers.Y);
        }

        private void ResetStatus() //REP
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            ClearFlag(value);
            UpdateMode();
        }

        private void RotateLeft() //ROL (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((value & 0x80) != 0, CarryFlag);
                value = (byte) (((value << 1) & 0xFF) | 0x1);
            }
            else
            {
                TestFlag((value & 0x80) != 0, CarryFlag);
                value = (byte) ((value << 1) & 0xFF);
            }
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag(value);
        }

        private void RotateLeftA() //ROL (8 bits)
        {
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((Registers.A & 0x80) != 0, CarryFlag);
                Registers.A = ((((Registers.A & 0xFF) << 1) & 0xFF) | 0x1) + (Registers.A & 0xFF00);
            }
            else
            {
                TestFlag((Registers.A & 0x80) != 0, CarryFlag);
                Registers.A = (((Registers.A & 0xFF) << 1) & 0xFF) + (Registers.A & 0xFF00);
            }
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void RotateLeft16() //ROL (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((value & 0x8000) != 0, CarryFlag);
                value = ((value << 1) & 0xFFFF) | 0x1;
            }
            else
            {
                TestFlag((value & 0x8000) != 0, CarryFlag);
                value = (value << 1) & 0xFFFF;
            }
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag16(value);
        }

        private void RotateLeftA16() //ROL (16 bits)
        {
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((Registers.A & 0x8000) != 0, CarryFlag);
                Registers.A = ((Registers.A << 1) & 0xFFFF) | 0x1;
            }
            else
            {
                TestFlag((Registers.A & 0x8000) != 0, CarryFlag);
                Registers.A = (Registers.A << 1) & 0xFFFF;
            }
            SetZeroNegativeFlag16(Registers.A);
        }

        private void RotateRight() //ROR (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((value & 0x1) != 0, CarryFlag);
                value = (byte) (((value >> 1) & 0xFF) | 0x80);
            }
            else
            {
                TestFlag((value & 0x1) != 0, CarryFlag);
                value = (byte) ((value >> 1) & 0xFF);
            }
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag(value);
        }

        private void RotateRightA() //ROR (8 bits)
        {
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((Registers.A & 0x1) != 0, CarryFlag);
                Registers.A = ((((Registers.A & 0xFF) >> 1) & 0xFF) | 0x80) + (Registers.A & 0xFF00);
            }
            else
            {
                TestFlag((Registers.A & 0x1) != 0, CarryFlag);
                Registers.A = (((Registers.A & 0xFF) >> 1) & 0xFF) + (Registers.A & 0xFF00);
            }
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void RotateRight16() //ROR (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((value & 0x1) != 0, CarryFlag);
                value = ((value >> 1) & 0xFFFF) | 0x8000;
            }
            else
            {
                TestFlag((value & 0x1) != 0, CarryFlag);
                value = (value >> 1) & 0xFFFF;
            }
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
            SetZeroNegativeFlag16(value);
        }

        private void RotateRightA16() //ROR (16 bits)
        {
            if ((Registers.P & CarryFlag) != 0)
            {
                TestFlag((Registers.A & 0x1) != 0, CarryFlag);
                Registers.A = ((Registers.A >> 1) & 0xFFFF) | 0x8000;
            }
            else
            {
                TestFlag((Registers.A & 0x1) != 0, CarryFlag);
                Registers.A = (Registers.A >> 1) & 0xFFFF;
            }
            SetZeroNegativeFlag16(Registers.A);
        }

        private void ReturnFromInterrupt() //RTI
        {
            Registers.P = Pull();
            Registers.ProgramCounter = Pull16();
            Registers.ProgramBank = Pull();
        }

        private void ReturnFromSubroutineLong() //RTL
        {
            Registers.ProgramCounter = Pull16();
            Registers.ProgramCounter += 1;
            Registers.ProgramBank = Pull();
        }

        private void ReturnFromSubroutine() //RTS
        {
            Registers.ProgramCounter = Pull16();
            Registers.ProgramCounter += 1;
        }

        private void SubtractWithCarry() //SBC (8 bits)
        {
            if ((Registers.P & DecimalFlag) == 0)
            {
                var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) ^ 0xFF;
                var result = (Registers.A & 0xFF) + value + (Registers.P & CarryFlag);
                TestFlag(result > 0xFF, CarryFlag);
                TestFlag(((~((Registers.A & 0xFF) ^ value)) & ((Registers.A & 0xFF) ^ result) & 0x80) != 0, OverflowFlag);
                Registers.A = (result & 0xFF) + (Registers.A & 0xFF00);
                SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
            }
            else
            {
                SubtractWithCarryBCD();
            }
        }

        private void SubtractWithCarryBCD() //SBC (BCD) (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) ^ 0xFF;
            var result = (Registers.A & 0xF) + (value & 0xF) + (Registers.P & CarryFlag);
            if (result < 0x10) { result -= 6; }
            TestFlag(result > 0xF, CarryFlag);
            result = (Registers.A & 0xF0) + (value & 0xF0) + (result & 0xF) + ((Registers.P & CarryFlag)*0x10);
            TestFlag(((~((Registers.A & 0xFF) ^ value)) & ((Registers.A & 0xFF) ^ result) & 0x80) != 0, OverflowFlag);
            if (result < 0x100) { result -= 0x60; }
            TestFlag(result > 0xFF, CarryFlag);
            Registers.A = (result & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void SubtractWithCarry16() //SBC (16 bits)
        {
            if ((Registers.P & DecimalFlag) == 0)
            {
                var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) ^ 0xFFFF;
                var result = Registers.A + value + (Registers.P & CarryFlag);
                TestFlag(result > 0xFFFF, CarryFlag);
                TestFlag(((~(Registers.A ^ value)) & (Registers.A ^ result) & 0x8000) != 0, OverflowFlag);
                Registers.A = result & 0xFFFF;
                SetZeroNegativeFlag16(Registers.A);
            } 
            else
            {
                SubtractWithCarryBCD16();
            }
        }

        private void SubtractWithCarryBCD16() //SBC (BCD) (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF) ^ 0xFFFF;
            var result = (Registers.A & 0xF) + (value & 0xF) + (Registers.P & CarryFlag);
            if (result < 0x10) { result -= 6; }
            TestFlag(result > 0xF, CarryFlag);
            result = (Registers.A & 0xF0) + (value & 0xF0) + (result & 0xF) + ((Registers.P & CarryFlag)*0x10);
            if (result < 0x100) { result -= 0x60; }
            TestFlag(result > 0xFF, CarryFlag);
            result = (Registers.A & 0xF00) + (value & 0xF00) + (result & 0xFF) + ((Registers.P & CarryFlag)*0x100);
            if (result < 0x1000) { result -= 0x600; }
            TestFlag(result > 0xFFF, CarryFlag);
            result = (Registers.A & 0xF000) + (value & 0xF000) + (result & 0xFFF) + ((Registers.P & CarryFlag)*0x1000);
            TestFlag(((~(Registers.A ^ value)) & (Registers.A ^ result) & 0x8000) != 0, OverflowFlag);
            if (result < 0x10000) { result -= 0x6000; }
            TestFlag(result > 0xFFFF, CarryFlag);
            Registers.A = result & 0xFFFF;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void SetCarry() //SEC
        {
            SetFlag(CarryFlag);
        }

        private void SetDecimal() //SED
        {
            SetFlag(DecimalFlag);
        }

        private void SetInterruptDisable() //SEI
        {
            SetFlag(InterruptFlag);
        }

        private void SetStatus() //SEP
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            SetFlag(value);
            UpdateMode();
        }

        private void StoreAccumulator() //STA (8 bits)
        {
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, (byte) (Registers.A & 0xFF));
        }

        private void StoreAccumulator16() //STA (16 bits)
        {
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, Registers.A);
        }

        private void StopProcessor()
        {
            STPDisable = true;
        }

        private void StoreX() //STX (8 bits)
        {
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, (byte) (Registers.X & 0xFF));
        }

        private void StoreX16() //STX (16 bits)
        {
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, Registers.X);
        }

        private void StoreY() //STY (8 bits)
        {
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, (byte) (Registers.Y & 0xFF));
        }

        private void StoreY16() //STY (16 bits)
        {
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, Registers.Y);
        }

        private void StoreZero() //STZ (8 bits)
        {
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, 0);
        }

        private void StoreZero16() //STZ (16 bits)
        {
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, 0);
        }

        private void TransferAccumulatorToX() //TAX (8 bits)
        {
            Registers.X = (Registers.A & 0xFF) + (Registers.X & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.X & 0xFF));
        }

        private void TransferAccumulatorToX16() //TAX (16 bits)
        {
            Registers.X = Registers.A;
            SetZeroNegativeFlag16(Registers.X);
        }

        private void TransferAccumulatorToY() //TAY (8 bits)
        {
            Registers.Y = (Registers.A & 0xFF) + (Registers.Y & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.Y & 0xFF));
        }

        private void TransferAccumulatorToY16() //TAY (16 bits)
        {
            Registers.Y = Registers.A;
            SetZeroNegativeFlag16(Registers.Y);
        }

        private void TransferAccumulatorToDP() //TCD
        {
            Registers.DirectPage = Registers.A;
            SetZeroNegativeFlag16(Registers.DirectPage);
        }

        private void TransferAccumulatorToSP() //TCS
        {
            Registers.StackPointer = Registers.A;
        }

        private void TransferDPToAccumulator() //TDC
        {
            Registers.A = Registers.DirectPage;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void TestAndResetBit() //TRB (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag(((Registers.A & 0xFF) & value) == 0, ZeroFlag);
            value = (byte) (value & ~ (Registers.A & 0xFF));
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void TestAndResetBit16() //TRB (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((Registers.A & value) == 0, ZeroFlag);
            value = value & ~Registers.A;
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void TestAndSetBit() //TSB (8 bits)
        {
            var value = ReadMemory((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag(((Registers.A & 0xFF) & value) == 0, ZeroFlag);
            value = (byte) (value | (Registers.A & 0xFF));
            WriteMemory((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void TestAndSetBit16() //TSB (16 bits)
        {
            var value = ReadMemory16((byte) ((_effectiveAddress & 0xFF0000)/0x10000), _effectiveAddress & 0xFFFF);
            TestFlag((Registers.A & value) == 0, ZeroFlag);
            value = value | Registers.A;
            WriteMemory16((_effectiveAddress & 0xFF0000)/0x10000, _effectiveAddress & 0xFFFF, value);
        }

        private void TransferSPtoAccumulator() //TSC
        {
            Registers.A = Registers.StackPointer;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void TransferSPtoX() //TSX
        {
            Registers.X = Registers.StackPointer;
            SetZeroNegativeFlag16(Registers.X);
        }

        private void TransferXtoAccumulator() //TXA (8 bits)
        {
            Registers.A = (Registers.X & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void TransferXtoAccumulator16() //TXA (16 bits)
        {
            Registers.A = Registers.X;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void TransferXtoSP() //TXS
        {
            Registers.StackPointer = Registers.X;
        }

        private void TransferXtoY() //TXY (8 bits)
        {
            Registers.Y = (Registers.X & 0xFF) + (Registers.Y & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.Y & 0xFF));
        }

        private void TransferXtoY16() //TXY (16 bits)
        {
            Registers.Y = Registers.X;
            SetZeroNegativeFlag16(Registers.Y);
        }

        private void TransferYtoAccumulator() //TYA (8 bits)
        {
            Registers.A = (Registers.Y & 0xFF) + (Registers.A & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.A & 0xFF));
        }

        private void TransferYToAccumulator16() //TYA (16 bits)
        {
            Registers.A = Registers.Y;
            SetZeroNegativeFlag16(Registers.A);
        }

        private void TransferYtoX() //TYX (8 bits)
        {
            Registers.X = (Registers.Y & 0xFF) + (Registers.X & 0xFF00);
            SetZeroNegativeFlag((byte) (Registers.X & 0xFF));
        }

        private void TransferYtoX16() //TYX (16 bits)
        {
            Registers.X = Registers.Y;
            SetZeroNegativeFlag16(Registers.X);
        }

        private void WaitForInterrupt() //WAI
        {
            _waiDisable = true;
        }

        private void ExchangeAccumulator() //XBA
        {
            var lowByte = (byte) (Registers.A & 0xFF);
            var highByte = (byte) ((Registers.A & 0xFF00)/0x100);
            Registers.A = highByte + (lowByte*0x100);
        }

        private void ExchangeCarryAndEmulation() //XCE
        {
            var carry = (Registers.P & CarryFlag) != 0;
            if (_emulate6502)
            {
                SetFlag(CarryFlag);
            }
            else
            {
                ClearFlag(CarryFlag);
            }
            _emulate6502 = carry;
        }

        #endregion

        #region Main Loop

        public void MainLoop()
        {
            while(SnesOn)
            {
                VBlank = false;
                for (var scanline = 0; scanline <= 261; scanline++)
                {
                    CurrentLine = scanline;
                    HBlank = false;
                    if (!_waiDisable && !STPDisable)
                    {
                        if (_ioPort.IRQEnable == 2 && scanline == _ioPort.VCount)
                        {
                            IRQ();
                        }
                        Execute65816(CyclesPerScanline - HBlankCycles);
                        HBlank = true;
                        _ioPort.HBlankDMA(scanline);
                        if ((_ioPort.IRQEnable == 3 && scanline == _ioPort.VCount) || (_ioPort.IRQEnable == 1))
                        {
                            IRQ();
                        }
                        Execute65816(HBlankCycles);
                    }
                    if (scanline < 224)
                    {
                        _ppu.RenderScanline(scanline);
                    }
                    else
                    {
                        switch (scanline)
                        {
                            case 224:
                                _ioPort.ControllerReady = true;
                                _ppu.ObjRAMAddress = _ppu.ObjRAMFirstAddress;
                                VBlank = true;
                                if (_ioPort.NMIEnable) { NMI(); }
                                break;
                            case 227:
                                _ioPort.ControllerReady = false;
                                break;
                        }
                    }
                }
                if (_ppu.TakeScreenshot) { _ppu.Screenshot(); }
                _ppu.Blit();
                if (_fps.LimitFPS) { _fps.LockFramerate(60); }
                _form.Text = _fps.GetFPS();
                System.Windows.Forms.Application.DoEvents();
            }
        }

        #endregion

    }
}