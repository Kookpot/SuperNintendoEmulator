using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Newtonsoft.Json;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.CPU
{
    public partial class C65816 : ICPU
    {
        public int Cycles { get; set; }
        public bool IRQPending { get; set; }

        private const int OneCycleSlow = 8;
        private const int OneCycleXSlow = 12;
        private const int OneCycle = 6;
        private const int TwoCycles = OneCycle * 2;
        private const int ThreeCycles = OneCycle * 3;

        public byte[] WRAM { get; private set; } = new byte[131072];

        [JsonIgnore]
        private byte[,] _sRAM = new byte[8, 32768];

        [JsonIgnore]
        private Timer _sRAMTimer;

        private int _wRAMAddress;
        private int _dataBus;
        private bool _nmiPending;

        private enum Flags
        {
            Carry = 0x1,
            Zero = 0x2,
            IRQ = 0x4,
            BCD = 0x8,
            X = 0x10,
            M = 0x20,
            Overflow = 0x40,
            Negative = 0x80
        }

        private int _a;
        private int _x;
        private int _y;
        private int _s;
        private int _db;
        private int _dp;
        private int _pb;
        private int _p;
        private int _pc;
        private bool _m6502;
        private bool _waiState;
        private bool _stpState;

        [JsonIgnore]
        private ISNESSystem _system;

        public void Reset()
        {
            _a = 0;
            _x = 0;
            _y = 0;
            _s = 0x1FF;
            _db = 0;
            _dp = 0;
            _pb = 0;
            _p = 0x34; //cpucyclesleft
            _pc = Read16(0xFFFC);
            _m6502 = true;
            _waiState = false;
            _stpState = false;
            Cycles = 0;
        }

        public int Read8(int address, bool incCycles = true)
        {
            int bank = address >> 16;
            address &= 0xFFFF;
            var returnVal = 0;
            if (bank < 0x7E | bank > 0x7F)
            {
                if (_system.ROM.IsHiRom())
                {
                    if ((bank & 0x7F) < 0x40)
                    {
                        switch (address)
                        {
                            case object _ when 0 <= address && address <= 0x1FFF:
                                returnVal = WRAM[address];
                                break;
                            case object _ when 0x2100 <= address && address <= 0x213F:
                                returnVal = _system.PPU.Read8(address);
                                break;
                            case object _ when 0x2140 <= address && address <= 0x217F:
                                returnVal = _system.APU.Read8IO(address);
                                break;
                            case 0x2180:
                                returnVal = WRAM[_wRAMAddress];
                                _wRAMAddress = (_wRAMAddress + 1) & 0x1FFFF;
                                break;
                            case 0x2181:
                                break;
                            case 0x2182:
                                break;
                            case 0x2183:
                                break;
                            case object _ when 0x2184 <= address && address <= 0x3FFF:
                                returnVal = _dataBus;
                                break;
                            case object _ when 0x4000 <= address && address <= 0x42FF:
                                returnVal = _system.IO.Read8(address);
                                break;
                            case object _ when 0x4300 <= address && address <= 0x43FF:
                                returnVal = _system.DMA.Read8(address);
                                break;
                            case object _ when 0x6000 <= address && address <= 0x7FFF:
                                if ((bank & 0x7F) > 0x1F)
                                {
                                    returnVal = _sRAM[0, address & 0x1FFF];
                                }
                                break;
                            case object _ when 0x8000 <= address && address <= 0xFFFF:
                                returnVal = _system.ROM.IsExHiRom() ?
                                    _system.ROM.ROMData[(bank & 0x3F) | 0x40, address] :
                                    _system.ROM.ROMData[bank & 0x7F, address];
                                break;
                        }
                    }
                    else if (_system.ROM.IsExHiRom() && bank < 0xC0)
                    {
                        returnVal = _system.ROM.ROMData[(bank & 0x3F) | 0x40, address];
                    }
                    else
                    {
                        returnVal = _system.ROM.ROMData[bank & 0x3F, address];
                    }
                }
                else if (address < 0x8000 && bank > 0x6F && bank < 0x78)
                {
                    if (_system.ROM.GetSRAMSize() > 0)
                    {
                        returnVal = _sRAM[bank & 7, address & 0x1FFF];
                    }
                }
                else
                {
                    switch (address)
                    {
                        case object _ when 0 <= address && address <= 0x1FFF:
                            returnVal = WRAM[address];
                            break;
                        case object _ when 0x2100 <= address && address <= 0x213F:
                            returnVal = _system.PPU.Read8(address);
                            break;
                        case object _ when 0x2140 <= address && address <= 0x217F:
                            returnVal = _system.APU.Read8IO(address);
                            break;
                        case 0x2180:
                            returnVal = WRAM[_wRAMAddress];
                            _wRAMAddress = (_wRAMAddress + 1) & 0x1FFFF;
                            break;
                        case 0x2181:
                            break;
                        case 0x2182:
                            break;
                        case 0x2183:
                            break;
                        case object _ when 0x2184 <= address && address <= 0x3FFF:
                            returnVal = _dataBus;
                            break;
                        case object _ when 0x4000 <= address && address <= 0x42FF:
                            returnVal = _system.IO.Read8(address);
                            break;
                        case object _ when 0x4300 <= address && address <= 0x43FF:
                            returnVal = _system.DMA.Read8(address);
                            break;
                        case object _ when 0x8000 <= address && address <= 0xFFFF:
                            returnVal = _system.ROM.GetBanks() < 0x40 ?
                                _system.ROM.ROMData[bank & 0x3F, address & 0x7FFF] :
                                _system.ROM.ROMData[bank & 0x7F, address & 0x7FFF];
                            break;
                    }
                }
            }
            if (bank == 0x7E)
            {
                if (address == 57856)
                {
                    return 96;
                }
                returnVal = WRAM[address];
            }
            if (bank == 0x7F)
            {
                if (address == 57856)
                {
                    return 96;
                }
                returnVal = WRAM[address | 0x10000];
            }
            if (incCycles)
            {
                AccessCycles(bank, address);
            }
            _dataBus = returnVal;
            return returnVal;
        }

        public int Read16(int address, bool incCycles = true)
        {
            int returnVal = Read8(address, incCycles);
            returnVal |= Read8(AddWB(address, 1), incCycles) << 8;
            return returnVal;
        }

        public void Write8(int address, int value, bool incCycles = true, [CallerFilePath] string filePath = "",[CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            int bank = address >> 16;
            address &= 0xFFFF;
            if (bank < 0x7E || bank > 0x7F)
            {
                if (_system.ROM.IsHiRom())
                {
                    if ((bank & 0x7F) < 0x40)
                    {
                        switch (address)
                        {
                            case object _ when 0 <= address && address <= 0x1FFF:
                                WRAM[address] = (byte)value;
                                break;
                            case object _ when 0x2100 <= address && address <= 0x213F:
                                _system.PPU.Write8(address, value);
                                break;
                            case object _ when 0x2140 <= address && address <= 0x217F:
                                _system.APU.Write8IO(address, value);
                                break;
                            case 0x2180:
                                WRAM[_wRAMAddress] = (byte)value;
                                _wRAMAddress = (_wRAMAddress + 1) & 0x1FFFF;
                                break;
                            case 0x2181:
                                _wRAMAddress = value | (_wRAMAddress & 0x1FF00);
                                break;
                            case 0x2182:
                                _wRAMAddress = (value << 8) | (_wRAMAddress & 0x100FF);
                                break;
                            case 0x2183:
                                _wRAMAddress = ((value & 1) << 16) | (_wRAMAddress & 0xFFFF);
                                break;
                            case object _ when 0x4000 <= address && address <= 0x42FF:
                                _system.IO.Write8(address, value);
                                break;
                            case object _ when 0x4300 <= address && address <= 0x43FF:
                                _system.DMA.Write8(address, value);
                                break;
                            case object _ when 0x6000 <= address && address <= 0x7FFF:
                                if ((bank & 0x7F) > 0x1F)
                                {
                                    _sRAM[0, address & 0x1FFF] = (byte)value;
                                    if (_sRAMTimer == null)
                                    {
                                        _sRAMTimer = new Timer(SaveSRAM, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                                    }
                                }
                                break;
                        }
                    }
                }
                else if (address < 0x8000 && bank > 0x6F && bank < 0x78)
                {
                    if (_system.ROM.GetSRAMSize() > 0)
                    {
                        _sRAM[bank & 7, address & 0x1FFF] = (byte)value;
                        if (_sRAMTimer == null)
                        {
                            _sRAMTimer = new Timer(SaveSRAM, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(6));
                        }
                    }
                }
                else
                {
                    switch (address)
                    {
                        case object _ when 0 <= address && address <= 0x1FFF:
                            WRAM[address] = (byte)value;
                            break;
                        case object _ when 0x2100 <= address && address <= 0x213F:
                            _system.PPU.Write8(address, value);
                            break;
                        case object _ when 0x2140 <= address && address <= 0x217F:
                            _system.APU.Write8IO(address, value);
                            break;
                        case 0x2180:
                            WRAM[_wRAMAddress] = (byte)value;
                            _wRAMAddress = (_wRAMAddress + 1) & 0x1FFFF;
                            break;
                        case 0x2181:
                            _wRAMAddress = value | (_wRAMAddress & 0x1FF00);
                            break;
                        case 0x2182:
                            _wRAMAddress = (value << 8) | (_wRAMAddress & 0x100FF);
                            break;
                        case 0x2183:
                            _wRAMAddress = ((value & 1) << 16) | (_wRAMAddress & 0xFFFF);
                            break;
                        case object _ when 0x4000 <= address && address <= 0x42FF:
                            _system.IO.Write8(address, value);
                            break;
                        case object _ when 0x4300 <= address && address <= 0x43FF:
                            _system.DMA.Write8(address, value);
                            break;
                    }
                }
            }
            if (bank == 0x7E)
            {
                WRAM[address] = (byte)value;
            }
            if (bank == 0x7F)
            {
                WRAM[address | 0x10000] = (byte) value;
            }
            if (incCycles)
            {
                AccessCycles(bank, address);
            }
            _dataBus = value;
        }

        public void ExecuteStep()
        {
            if (_nmiPending)
            {
                NMI();
            }
            if (IRQPending)
            {
                IRQ();
            }
            if (_waiState || _stpState)
            {
                Cycles += 4;
                return;
            }
            int opCode = Read8PC();
            switch (opCode)
            {
                case 0x61:
                    ADC(DINDX());
                    break;
                case 0x63:
                    ADC(SREL());
                    break;
                case 0x65:
                    ADC(D());
                    break;
                case 0x67:
                    ADC(DINDL());
                    break;
                case 0x69:
                    ADCIMM();
                    break;
                case 0x6D:
                    ADC(ABS());
                    break;
                case 0x6F:
                    ADC(ABSL());
                    break;
                case 0x71:
                    ADC(DINDY(false));
                    break;
                case 0x72:
                    ADC(DIND());
                    break;
                case 0x73:
                    ADC(SRELY());
                    break;
                case 0x75:
                    ADC(DX());
                    break;
                case 0x77:
                    ADC(DINDLY());
                    break;
                case 0x79:
                    ADC(ABSY(false));
                    break;
                case 0x7D:
                    ADC(ABSX(false));
                    break;
                case 0x7F:
                    ADC(ABSLX());
                    break;
                case 0x21:
                    _AND(DINDX());
                    break;
                case 0x23:
                    _AND(SREL());
                    break;
                case 0x25:
                    _AND(D());
                    break;
                case 0x27:
                    _AND(DINDL());
                    break;
                case 0x29:
                    ANDIMM();
                    break;
                case 0x2D:
                    _AND(ABS());
                    break;
                case 0x2F:
                    _AND(ABSL());
                    break;
                case 0x31:
                    _AND(DINDY(false));
                    break;
                case 0x32:
                    _AND(DIND());
                    break;
                case 0x33:
                    _AND(SRELY());
                    break;
                case 0x35:
                    _AND(DX());
                    break;
                case 0x37:
                    _AND(DINDLY());
                    break;
                case 0x39:
                    _AND(ABSY(false));
                    break;
                case 0x3D:
                    _AND(ABSX(false));
                    break;
                case 0x3F:
                    _AND(ABSLX());
                    break;
                case 0x6:
                    ASL(D());
                    break;
                case 0xA:
                    ASLA();
                    break;
                case 0xE:
                    ASL(ABS());
                    break;
                case 0x16:
                    ASL(DX());
                    break;
                case 0x1E:
                    ASL(ABSX());
                    break;
                case 0x90:
                    BCC(REL());
                    break;
                case 0xB0:
                    BCS(REL());
                    break;
                case 0xF0:
                    BEQ(REL());
                    break;
                case 0x24:
                    BIT(D());
                    break;
                case 0x2C:
                    BIT(ABS());
                    break;
                case 0x34:
                    BIT(DX());
                    break;
                case 0x3C:
                    BIT(ABSX(false));
                    break;
                case 0x89:
                    BITIMM();
                    break;
                case 0x30:
                    BMI(REL());
                    break;
                case 0xD0:
                    BNE(REL());
                    break;
                case 0x10:
                    BPL(REL());
                    break;
                case 0x80:
                    BRA(REL());
                    break;
                case 0x0:
                    BRK();
                    break;
                case 0x82:
                    BRA(RELL());
                    break;
                case 0x50:
                    BVC(REL());
                    break;
                case 0x70:
                    BVS(REL());
                    break;
                case 0x18:
                    CLC();
                    break;
                case 0xD8:
                    CLD();
                    break;
                case 0x58:
                    CLI();
                    break;
                case 0xB8:
                    CLV();
                    break;
                case 0xC1:
                    CMP(DINDX());
                    break;
                case 0xC3:
                    CMP(SREL());
                    break;
                case 0xC5:
                    CMP(D());
                    break;
                case 0xC7:
                    CMP(DINDL());
                    break;
                case 0xC9:
                    CMPIMM();
                    break;
                case 0xCD:
                    CMP(ABS());
                    break;
                case 0xCF:
                    CMP(ABSL());
                    break;
                case 0xD1:
                    CMP(DINDY(false));
                    break;
                case 0xD2:
                    CMP(DIND());
                    break;
                case 0xD3:
                    CMP(SRELY());
                    break;
                case 0xD5:
                    CMP(DX());
                    break;
                case 0xD7:
                    CMP(DINDLY());
                    break;
                case 0xD9:
                    CMP(ABSY(false));
                    break;
                case 0xDD:
                    CMP(ABSX(false));
                    break;
                case 0xDF:
                    CMP(ABSLX());
                    break;
                case 0x2:
                    COP();
                    break;
                case 0xE0:
                    CPXIMM();
                    break;
                case 0xE4:
                    CPX(D());
                    break;
                case 0xEC:
                    CPX(ABS());
                    break;
                case 0xC0:
                    CPYIMM();
                    break;
                case 0xC4:
                    CPY(D());
                    break;
                case 0xCC:
                    CPY(ABS());
                    break;
                case 0xC6:
                    DEC(D());
                    break;
                case 0x3A:
                    DECA();
                    break;
                case 0xCE:
                    DEC(ABS());
                    break;
                case 0xD6:
                    DEC(DX());
                    break;
                case 0xDE:
                    DEC(ABSX());
                    break;
                case 0xCA:
                    DEX();
                    break;
                case 0x88:
                    DEY();
                    break;
                case 0x41:
                    EOR(DINDX());
                    break;
                case 0x43:
                    EOR(SREL());
                    break;
                case 0x45:
                    EOR(D());
                    break;
                case 0x47:
                    EOR(DINDL());
                    break;
                case 0x49:
                    EORIMM();
                    break;
                case 0x4D:
                    EOR(ABS());
                    break;
                case 0x4F:
                    EOR(ABSL());
                    break;
                case 0x51:
                    EOR(DINDY(false));
                    break;
                case 0x52:
                    EOR(DIND());
                    break;
                case 0x53:
                    EOR(SRELY());
                    break;
                case 0x55:
                    EOR(DX());
                    break;
                case 0x57:
                    EOR(DINDLY());
                    break;
                case 0x59:
                    EOR(ABSY(false));
                    break;
                case 0x5D:
                    EOR(ABSX(false));
                    break;
                case 0x5F:
                    EOR(ABSLX());
                    break;
                case 0xE6:
                    INC(D());
                    break;
                case 0x1A:
                    INCA();
                    break;
                case 0xEE:
                    INC(ABS());
                    break;
                case 0xF6:
                    INC(DX());
                    break;
                case 0xFE:
                    INC(ABSX());
                    break;
                case 0xE8:
                    INX();
                    break;
                case 0xC8:
                    INY();
                    break;
                case 0x4C:
                    JMP(ABS());
                    break;
                case 0x5C:
                    JML(ABSL());
                    break;
                case 0x6C:
                    JMP(ABSIND());
                    break;
                case 0x7C:
                    JMPABSINDX();
                    break;
                case 0xDC:
                    JML(ABSINDL());
                    break;
                case 0x20:
                    JSR(ABS());
                    break;
                case 0x22:
                    JSL(ABSL());
                    break;
                case 0xFC:
                    JSR(ABSINDX());
                    break;
                case 0xA1:
                    LDA(DINDX());
                    break;
                case 0xA3:
                    LDA(SREL());
                    break;
                case 0xA5:
                    LDA(D());
                    break;
                case 0xA7:
                    LDA(DINDL());
                    break;
                case 0xA9:
                    LDAIMM();
                    break;
                case 0xAD:
                    LDA(ABS());
                    break;
                case 0xAF:
                    LDA(ABSL());
                    break;
                case 0xB1:
                    LDA(DINDY(false));
                    break;
                case 0xB2:
                    LDA(DIND());
                    break;
                case 0xB3:
                    LDA(SRELY());
                    break;
                case 0xB5:
                    LDA(DX());
                    break;
                case 0xB7:
                    LDA(DINDLY());
                    break;
                case 0xB9:
                    LDA(ABSY(false));
                    break;
                case 0xBD:
                    LDA(ABSX(false));
                    break;
                case 0xBF:
                    LDA(ABSLX());
                    break;
                case 0xA2:
                    LDXIMM();
                    break;
                case 0xA6:
                    LDX(D());
                    break;
                case 0xAE:
                    LDX(ABS());
                    break;
                case 0xB6:
                    LDX(DY());
                    break;
                case 0xBE:
                    LDX(ABSY(false));
                    break;
                case 0xA0:
                    LDYIMM();
                    break;
                case 0xA4:
                    LDY(D());
                    break;
                case 0xAC:
                    LDY(ABS());
                    break;
                case 0xB4:
                    LDY(DX());
                    break;
                case 0xBC:
                    LDY(ABSX(false));
                    break;
                case 0x46:
                    LSR(D());
                    break;
                case 0x4A:
                    LSRA();
                    break;
                case 0x4E:
                    LSR(ABS());
                    break;
                case 0x56:
                    LSR(DX());
                    break;
                case 0x5E:
                    LSR(ABSX());
                    break;
                case 0x54:
                    MVN();
                    break;
                case 0x44:
                    MVP();
                    break;
                case 0xEA:
                    NOP();
                    break;
                case 0x1:
                    ORA(DINDX());
                    break;
                case 0x3:
                    ORA(SREL());
                    break;
                case 0x5:
                    ORA(D());
                    break;
                case 0x7:
                    ORA(DINDL());
                    break;
                case 0x9:
                    ORAIMM();
                    break;
                case 0xD:
                    ORA(ABS());
                    break;
                case 0xF:
                    ORA(ABSL());
                    break;
                case 0x11:
                    ORA(DINDY(false));
                    break;
                case 0x12:
                    ORA(DIND());
                    break;
                case 0x13:
                    ORA(SRELY());
                    break;
                case 0x15:
                    ORA(DX());
                    break;
                case 0x17:
                    ORA(DINDLY());
                    break;
                case 0x19:
                    ORA(ABSY(false));
                    break;
                case 0x1D:
                    ORA(ABSX(false));
                    break;
                case 0x1F:
                    ORA(ABSLX());
                    break;
                case 0xF4:
                    PEA(ABS());
                    break;
                case 0xD4:
                    PEA(DIND());
                    break;
                case 0x62:
                    PER();
                    break;
                case 0x48:
                    PHA();
                    break;
                case 0x8B:
                    PHB();
                    break;
                case 0xB:
                    PHD();
                    break;
                case 0x4B:
                    PHK();
                    break;
                case 0x8:
                    PHP();
                    break;
                case 0xDA:
                    PHX();
                    break;
                case 0x5A:
                    PHY();
                    break;
                case 0x68:
                    PLA();
                    break;
                case 0xAB:
                    PLB();
                    break;
                case 0x2B:
                    PLD();
                    break;
                case 0x28:
                    PLP();
                    break;
                case 0xFA:
                    PLX();
                    break;
                case 0x7A:
                    PLY();
                    break;
                case 0xC2:
                    REP(IMM8());
                    break;
                case 0x26:
                    ROL(D());
                    break;
                case 0x2A:
                    ROLA();
                    break;
                case 0x2E:
                    ROL(ABS());
                    break;
                case 0x36:
                    ROL(DX());
                    break;
                case 0x3E:
                    ROL(ABSX());
                    break;
                case 0x66:
                    ROR(D());
                    break;
                case 0x6A:
                    RORA();
                    break;
                case 0x6E:
                    ROR(ABS());
                    break;
                case 0x76:
                    ROR(DX());
                    break;
                case 0x7E:
                    ROR(ABSX());
                    break;
                case 0x40:
                    RTI();
                    break;
                case 0x6B:
                    RTL();
                    break;
                case 0x60:
                    RTS();
                    break;
                case 0xE1:
                    SBC(DINDX());
                    break;
                case 0xE3:
                    SBC(SREL());
                    break;
                case 0xE5:
                    SBC(D());
                    break;
                case 0xE7:
                    SBC(DINDL());
                    break;
                case 0xE9:
                    SBCIMM();
                    break;
                case 0xED:
                    SBC(ABS());
                    break;
                case 0xEF:
                    SBC(ABSL());
                    break;
                case 0xF1:
                    SBC(DINDY(false));
                    break;
                case 0xF2:
                    SBC(DIND());
                    break;
                case 0xF3:
                    SBC(SRELY());
                    break;
                case 0xF5:
                    SBC(DX());
                    break;
                case 0xF7:
                    SBC(DINDLY());
                    break;
                case 0xF9:
                    SBC(ABSY(false));
                    break;
                case 0xFD:
                    SBC(ABSX(false));
                    break;
                case 0xFF:
                    SBC(ABSLX());
                    break;
                case 0x38:
                    SEC();
                    break;
                case 0xF8:
                    SED();
                    break;
                case 0x78:
                    SEI();
                    break;
                case 0xE2:
                    SEP(IMM8());
                    break;
                case 0x81:
                    STA(DINDX());
                    break;
                case 0x83:
                    STA(SREL());
                    break;
                case 0x85:
                    STA(D());
                    break;
                case 0x87:
                    STA(DINDL());
                    break;
                case 0x8D:
                    STA(ABS());
                    break;
                case 0x8F:
                    STA(ABSL());
                    break;
                case 0x91:
                    STA(DINDY());
                    break;
                case 0x92:
                    STA(DIND());
                    break;
                case 0x93:
                    STA(SRELY());
                    break;
                case 0x95:
                    STA(DX());
                    break;
                case 0x97:
                    STA(DINDLY());
                    break;
                case 0x99:
                    STA(ABSY());
                    break;
                case 0x9D:
                    STA(ABSX());
                    break;
                case 0x9F:
                    STA(ABSLX());
                    break;
                case 0xDB:
                    STP();
                    break;
                case 0x86:
                    STX(D());
                    break;
                case 0x8E:
                    STX(ABS());
                    break;
                case 0x96:
                    STX(DY());
                    break;
                case 0x84:
                    STY(D());
                    break;
                case 0x8C:
                    STY(ABS());
                    break;
                case 0x94:
                    STY(DX());
                    break;
                case 0x64:
                    STZ(D());
                    break;
                case 0x74:
                    STZ(DX());
                    break;
                case 0x9C:
                    STZ(ABS());
                    break;
                case 0x9E:
                    STZ(ABSX());
                    break;
                case 0xAA:
                    TAX();
                    break;
                case 0xA8:
                    TAY();
                    break;
                case 0x5B:
                    TCD();
                    break;
                case 0x1B:
                    TCS();
                    break;
                case 0x7B:
                    TDC();
                    break;
                case 0x14:
                    TRB(D());
                    break;
                case 0x1C:
                    TRB(ABS());
                    break;
                case 0x4:
                    TSB(D());
                    break;
                case 0xC:
                    TSB(ABS());
                    break;
                case 0x3B:
                    TSC();
                    break;
                case 0xBA:
                    TSX();
                    break;
                case 0x8A:
                    TXA();
                    break;
                case 0x9A:
                    TXS();
                    break;
                case 0x9B:
                    TXY();
                    break;
                case 0x98:
                    TYA();
                    break;
                case 0xBB:
                    TYX();
                    break;
                case 0xCB:
                    WAI();
                    break;
                case 0x42:
                    WDM();
                    break;
                case 0xEB:
                    XBA();
                    break;
                case 0xFB:
                    XCE();
                    break;
            }
        }

        public void SetSystem(ISNESSystem system)
        {
            _system = system;
        }

        public void DoNMI()
        {
            _nmiPending = true;
        }

        public void LoadSRAM()
        {
            var fileName = _system.FileName.Replace(".smc", ".srm").Replace(".sfc", ".srm");
            if (new FileInfo(fileName).Exists)
            {
                using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bformatter = new BinaryFormatter();
                    _sRAM = (byte[,])bformatter.Deserialize(stream);
                }
            }
        }

        private int Read24(int address, bool incCycles = true)
        {
            int returnVal = Read8(address, incCycles);
            returnVal |= Read8(AddWB(address, 1), incCycles) << 8;
            returnVal |= Read8(AddWB(address, 2), incCycles) << 16;
            return returnVal;
        }

        private void SaveSRAM(object state)
        {
            var fileName = _system.FileName.Replace(".smc", ".srm").Replace(".sfc", ".srm");
            using (Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, _sRAM);
            }
            _sRAMTimer.Dispose();
            _sRAMTimer = null;
        }

        private void Write16(int address, int value, bool incCycles = true)
        {
            Write8(address, value & 0xFF, incCycles);
            Write8(AddWB(address, 1), (value >> 8) & 0xFF, incCycles);
        }

        private void NMI()
        {
            _nmiPending = false;
            ExecuteStep();
            CheckWAI();
            if (_m6502)
            {
                Push16(_pc);
                Push8(_p & ~0x10);

                _pc = Read16(0xFFFA);
            }
            else
            {
                Push8(_pb);
                Push16(_pc);
                Push8(_p);
                _pc = Read16(0xFFEA);
            }
            _pb = 0;
            ClearFlag(Flags.BCD);
            SetFlag(Flags.IRQ);

            Cycles += TwoCycles;
        }

        private void IRQ()
        {
            CheckWAI();
            if ((_p & (int) Flags.IRQ) == 0)
            {
                if (_m6502)
                {
                    Push16(_pc);
                    Push8(_p & ~0x10);
                    _pc = Read16(0xFFFE);
                }
                else
                {
                    Push8(_pb);
                    Push16(_pc);
                    Push8(_p);
                    _pc = Read16(0xFFEE);
                }
                _pb = 0;
                ClearFlag(Flags.BCD);
                SetFlag(Flags.IRQ);
                Cycles += TwoCycles;
            }
        }

        private void CheckWAI()
        {
            if (_waiState)
            {
                _pc = (_pc + 1) & 0xFFFF;
                _waiState = false;
            }
        }

        private void SetFlag(Flags flag)
        {
            _p |= (int) flag;
        }

        private void ClearFlag(Flags flag)
        {
            _p &= (int) ~flag;
        }

        private void SetFlag(bool condition, Flags flag)
        {
            if (condition)
            {
                SetFlag(flag);
            }
            else
            {
                ClearFlag(flag);
            }
        }

        private void SetZNFlags8(byte value)
        {
            SetFlag(value == 0, Flags.Zero);
            SetFlag((value & 0x80) != 0, Flags.Negative);
        }
        private void SetZNFlags16(int value)
        {
            SetFlag(value == 0, Flags.Zero);
            SetFlag((value & 0x8000) != 0, Flags.Negative);
        }

        private void ClearIndex8()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                _x &= 0xFF;
                _y &= 0xFF;
            }
        }

        private void Push8(int value)
        {
            Write8(_s, value);
            if (_m6502)
            {
                _s = ((_s - 1) & 0xFF) | 0x100;
            }
            else
            {
                _s = (_s - 1) & 0xFFFF;
            }
        }

        private void Push16(int value)
        {
            Push8((value >> 8) & 0xFF);
            Push8(value & 0xFF);
        }

        private int Pull8()
        {
            if (_m6502)
            {
                _s = ((_s + 1) & 0xFF) | 0x100;
            }
            else
            {
                _s = (_s + 1) & 0xFFFF;
            }
            return Read8(_s);
        }

        private int Pull16()
        {
            int returnValue = Pull8();
            returnValue |= Pull8() << 8;
            return returnValue;
        }

        private int Read8PC()
        {
            int returnVal = Read8(_pc | (_pb << 16));
            _pc = (_pc + 1) & 0xFFFF;
            return returnVal;
        }

        private int Read16PC()
        {
            int returnVal = Read16(_pc | (_pb << 16));
            _pc = (_pc + 2) & 0xFFFF;
            return returnVal;
        }

        private int Read24PC()
        {
            int returnVal = Read24(_pc | (_pb << 16));
            _pc = (_pc + 3) & 0xFFFF;
            return returnVal;
        }

        private int Read16E(int address)
        {
            int returnVal = Read8(address);
            if (_m6502)
            {
                returnVal |= Read8(AddWP(address, 1)) << 8;
            }
            else
            {
                returnVal |= Read8(AddWB(address, 1)) << 8;
            }
            return returnVal;
        }

        private void AccessCycles(int bank, int address)
        {
            switch (bank)
            {
                case object _ when 0x0 <= bank && bank <= 0x3F:
                    switch (address)
                    {
                        case object _ when 0x0 <= address && address <= 0x1FFF:
                            Cycles += OneCycleSlow;
                            break;
                        case object _ when 0x2000 <= address && address <= 0x3FFF:
                            Cycles += OneCycle;
                            break;
                        case object _ when 0x4000 <= address && address <= 0x41FF:
                            Cycles += OneCycleXSlow;
                            break;
                        case object _ when 0x4200 <= address && address <= 0x5FFF:
                            Cycles += OneCycle;
                            break;
                        default:
                            Cycles += OneCycleSlow;
                            break;
                    }
                    break;

                case object _ when 0x40 <= bank && bank <= 0x7F:
                    Cycles += OneCycleSlow;
                    break;
                case object _ when 0x80 <= bank && bank <= 0xBF:
                    switch (address)
                    {
                        case object _ when 0x0 <= address && address <= 0x1FFF:
                            Cycles += OneCycleSlow;
                            break;
                        case object _ when 0x2000 <= address && address <= 0x3FFF:
                            Cycles += OneCycle;
                            break;
                        case object _ when 0x4000 <= address && address <= 0x41FF:
                            Cycles += OneCycleXSlow;
                            break;
                        case object _ when 0x4200 <= address && address <= 0x5FFF:
                            Cycles += OneCycle;
                            break;
                        case object _ when 0x6000 <= address && address <= 0x7FFF:
                            Cycles += OneCycleSlow;
                            break;
                        case object _ when 0x8000 <= address && address <= 0xFFFF:
                            if ((_system.IO.MemorySelection & 1) != 0)
                            {
                                Cycles += OneCycle;
                            }
                            else
                            {
                                Cycles += OneCycleSlow;
                            }
                            break;
                    }
                    break;
                default:
                    if ((_system.IO.MemorySelection & 1) != 0)
                    {
                        Cycles += OneCycle;
                    }
                    else
                    {
                        Cycles += OneCycleSlow;
                    }
                    break;
            }
        }

        private static int AddWP(int address, int amount)
        {
            return ((address + amount) & 0xFF) | (address & 0xFFFF00);
        }

        private static int AddWB(int address, int amount)
        {
            return ((address + amount) & 0xFFFF) | (address & 0xFF0000);
        }

        private int ABS()
        {
            return Read16PC() | (_db << 16);
        }

        private int ABSL()
        {
            return Read24PC();
        }

        private int ABSLX()
        {
            return ABSL() + _x;
        }

        private int ABSX(bool write = true)
        {
            int address = ABS();
            int returnValue = (address + _x) & 0xFFFFFF;
            bool pageCrossed = (returnValue & 0xFF00) != (address & 0xFF00);
            bool iOpCycle = (_p & (int) Flags.X) == 0 || pageCrossed || write;
            if (iOpCycle)
            {
                Cycles += OneCycle;
            }
            return returnValue;
        }

        private int ABSY(bool write = true)
        {
            int address = ABS();
            int returnValue = (address + _y) & 0xFFFFFF;
            bool pageCrossed = (returnValue & 0xFF00) != (address & 0xFF00);
            bool iOpCycle = (_p & (int) Flags.X) == 0 || pageCrossed || write;
            if (iOpCycle)
            {
                Cycles += OneCycle;
            }
            return returnValue;
        }

        private int ABSIND()
        {
            return Read16(Read16PC());
        }

        private int ABSINDL()
        {
            return Read24(Read16PC());
        }

        private int ABSINDX()
        {
            return Read16(((Read16PC() + _x) & 0xFFFF) | (_pb << 16));
        }

        private int D()
        {
            int address = Read8PC() + _dp;
            if ((_dp & 0xFF) != 0)
            {
                Cycles += OneCycle;
            }
            return address;
        }

        private int DX()
        {
            int addressBase = D();
            int addressHigh = addressBase & 0xFF00;
            int address = addressBase + _x;
            int returnValue;
            if (_m6502)
            {
                returnValue = addressHigh | (address & 0xFF);
            }
            else
            {
                returnValue = address & 0xFFFF;
            }
            Cycles += OneCycle;
            return returnValue;
        }

        private int DY()
        {
            int addressBase = D();
            int addressHigh = addressBase & 0xFF00;
            int address = addressBase + _y;
            int returnValue;
            if (_m6502)
            {
                returnValue = addressHigh | (address & 0xFF);
            }
            else
            {
                returnValue = address & 0xFFFF;
            }
            Cycles += OneCycle;
            return returnValue;
        }

        private int DIND()
        {
            return Read16E(D()) | (_db << 16);
        }

        private int DINDX()
        {
            return Read16E(DX()) | (_db << 16);
        }

        private int DINDY(bool write = true)
        {
            int address = DIND();
            int returnValue = (address + _y) & 0xFFFFFF;
            bool PageCrossed = (returnValue & 0xFF00) != (address & 0xFF00);
            bool iOpCycle = (_p & (int) Flags.X) == 0 || PageCrossed || write;
            if (iOpCycle)
            {
                Cycles += OneCycle;
            }
            return returnValue;
        }

        private int DINDL()
        {
            return Read24(D());
        }

        private int DINDLY()
        {
            return (DINDL() + _y) & 0xFFFFFF;
        }

        private int IMM8()
        {
            int returnValue = _pc | (_pb << 16);
            _pc = (_pc + 1) & 0xFFFF;
            return returnValue;
        }

        private int IMM16()
        {
            int returnValue = _pc | (_pb << 16);
            _pc = (_pc + 2) & 0xFFFF;
            return returnValue;
        }

        private int REL()
        {
            int displacement = Read8PC();
            if ((displacement & 0x80) != 0)
            {
                displacement = (int) (displacement | 0xFFFFFF00);
            }
            return (_pc + displacement) & 0xFFFF;
        }

        private int RELL()
        {
            int displacement = Read16PC();
            if ((displacement & 0x8000) != 0)
            {
                displacement = (int) (displacement | 0xFFFF0000);
            }
            return (_pc + displacement) & 0xFFFF;
        }

        private int SREL()
        {
            int returnValue = (Read8PC() + _s) & 0xFFFF;
            Cycles += OneCycle;
            return returnValue;
        }

        private int SRELY()
        {
            int returnValue = (Read16(SREL()) + _y + (_db << 16)) & 0xFFFFFF;
            Cycles += OneCycle;
            return returnValue;
        }
    }
}