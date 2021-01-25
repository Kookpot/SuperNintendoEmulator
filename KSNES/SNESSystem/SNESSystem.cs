using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using KSNES.AudioProcessing;
using KSNES.CPU;
using KSNES.PictureProcessing;
using KSNES.Rendering;
using KSNES.ROM;

namespace KSNES.SNESSystem
{
    public class SNESSystem : ISNESSystem
    {
        public ICPU CPU { get; private set; }
        public IPPU PPU { get; private set; }
        public IAPU APU { get; private set; }

        [JsonIgnore]
        public IROM ROM { get; set; }

        private byte[] _ram;

        [JsonIgnore]
        private readonly int[] _dmaOffs = 
        {
            0, 0, 0, 0,
            0, 1, 0, 1,
            0, 0, 0, 0,
            0, 0, 1, 1,
            0, 1, 2, 3,
            0, 1, 0, 1,
            0, 0, 0, 0,
            0, 0, 1, 1
        };
       
        [JsonIgnore]
        private readonly int[] _dmaOffLengths = {1, 2, 2, 4, 4, 4, 2, 4};

        private const double _apuCyclesPerMaster = 32040 * 32.0 / (1364.0 * 262 * 60);

        private byte[] _dmaBadr;
        private ushort[] _dmaAadr;
        private byte[] _dmaAadrBank;
        private ushort[] _dmaSize;
        private byte[] _hdmaIndBank;
        private ushort[] _hdmaTableAdr;
        private byte[] _hdmaRepCount;
        private byte[] _dmaUnusedByte;

        public int XPos { get; private set; }
        public int YPos { get; private set; }

        private int _cpuCyclesLeft;
        private int _cpuMemOps;
        private double _apuCatchCycles;

        private int _ramAdr;

        private bool _hIrqEnabled;
        private bool _vIrqEnabled;
        private bool _nmiEnabled;
        private int _hTimer;
        private int _vTimer;
        private bool _inNmi;
        private bool _inIrq;
        private bool _inHblank;
        private bool _inVblank;

        private bool _autoJoyRead;
        private bool _autoJoyBusy;
        private int _autoJoyTimer;
        public bool PPULatch { get; private set; }

        private int _joypad1Val;
        private int _joypad2Val;
        private int _joypad1AutoRead;
        private int _joypad2AutoRead;
        private bool _joypadStrobe;
        private int _joypad1State;
        private int _joypad2State;

        private int _multiplyA;
        private int _divA;
        private int _divResult;
        private int _mulResult;

        private bool _fastMem;

        private int _dmaTimer;
        private int _hdmaTimer;
        private bool _dmaBusy;
        private bool[] _dmaActive;
        private bool[] _hdmaActive;

        private int[] _dmaMode;
        private bool[] _dmaFixed;
        private bool[] _dmaDec;
        private bool[] _hdmaInd;
        private bool[] _dmaFromB;
        private bool[] _dmaUnusedBit;

        private bool[] _hdmaDoTransfer;
        private bool[] _hdmaTerminated;
        private int _dmaOffIndex;
        public int OpenBus { get; private set; }
        public string FileName { get; set; }

        public event EventHandler FrameRendered;

        [JsonIgnore]
        public IRenderer Renderer { get; set; }

        [JsonIgnore]
        public IAudioHandler AudioHandler { get; set; }

        [JsonIgnore]
        public string GameName { get; set; }

        [JsonIgnore]
        private bool _isExecuting;

        public SNESSystem(ICPU cpu, IRenderer renderer, IROM rom, IPPU ppu, IAPU apu, IAudioHandler audioHandler)
        {
            CPU = cpu;
            Renderer = renderer;
            AudioHandler = audioHandler;
            ROM = rom;
            rom?.SetSystem(this);
            PPU = ppu;
            PPU?.SetSystem(this);
            APU = apu;
            CPU?.SetSystem(this);
        }

        public ISNESSystem Merge(ISNESSystem system)
        {
            system.AudioHandler = AudioHandler;
            system.Renderer = Renderer;
            system.ROM = ROM;
            ROM.SetSystem(system);
            system.APU.Attach();
            return system;
        }

        public void LoadROM(string fileName)
        {
            FileName = fileName;
            byte[] data = File.ReadAllBytes(FileName);
            LoadRom(data);
            GameName = ROM.Header.Name;
            Reset1();
            CPU.Reset();
            PPU.Reset();
            APU.Reset();
            Reset2();
            Run();
        }

        public void StopEmulation()
        {
            _isExecuting = false;
            AudioHandler.Pauze();
        }

        public bool IsRunning()
        {
            return _isExecuting;
        }

        public void ResumeEmulation()
        {
            AudioHandler.Resume();
            if (!string.IsNullOrEmpty(FileName))
            {
                if (ROM?.Header == null)
                {
                    byte[] data = File.ReadAllBytes(FileName);
                    LoadRom(data);
                }
                ROM.LoadSRAM();
                _isExecuting = true;
                while (_isExecuting)
                {
                    RunFrame(false);
                    Renderer.RenderBuffer(PPU.GetPixels());
                    APU.SetSamples(AudioHandler.SampleBufferL, AudioHandler.SampleBufferR);
                    AudioHandler.NextBuffer();
                    FrameRendered?.Invoke(this, null);
                }
            }
        }

        public void Run()
        {
            ResumeEmulation();
        }

        public int Read(int adr, bool dma = false)
        {
            if (!dma)
            {
                _cpuMemOps++;
                _cpuCyclesLeft += GetAccessTime(adr);
            }
            int val = Rread(adr);
            OpenBus = val;
            return val;
        }

        public void Write(int adr, int value, bool dma = false)
        {
            if (!dma)
            {
                _cpuMemOps++;
                _cpuCyclesLeft += GetAccessTime(adr);
            }
            OpenBus = value;
            adr &= 0xffffff;
            int bank = adr >> 16;
            adr &= 0xffff;
            if (bank == 0x7e || bank == 0x7f)
            {
                _ram[((bank & 0x1) << 16) | adr] = (byte) value;
            }
            if (adr < 0x8000 && (bank < 0x40 || bank >= 0x80 && bank < 0xc0))
            {
                if (adr < 0x2000)
                {
                    _ram[adr & 0x1fff] = (byte) value;
                }
                if (adr >= 0x2100 && adr < 0x2200)
                {
                    WriteBBus(adr & 0xff, value);
                }
                if (adr == 0x4016)
                {
                    _joypadStrobe = (value & 0x1) > 0;
                }
                if (adr >= 0x4200 && adr < 0x4380)
                {
                    WriteReg(adr, value);
                }
            }
            ROM.Write(bank, adr, (byte) value);
        }

        private void Reset1()
        {
            _ram = new byte[0x20000];
            _dmaBadr = new byte[8];
            _dmaAadr = new ushort[8];
            _dmaAadrBank = new byte[8];
            _dmaSize = new ushort[8];
            _hdmaIndBank = new byte[8];
            _hdmaTableAdr = new ushort[8];
            _hdmaRepCount = new byte[8];
            _dmaUnusedByte = new byte[8];
        }

        private void Reset2()
        {
            XPos = 0;
            YPos = 0;
            _cpuCyclesLeft = 5 * 8 + 12;
            _cpuMemOps = 0;
            _apuCatchCycles = 0;
            _ramAdr = 0;
            _hIrqEnabled = false;
            _vIrqEnabled = false;
            _nmiEnabled = false;
            _hTimer = 0x1ff;
            _vTimer = 0x1ff;
            _inNmi = false;
            _inIrq = false;
            _inHblank = false;
            _inVblank = false;
            _autoJoyRead = false;
            _autoJoyBusy = false;
            _autoJoyTimer = 0;
            PPULatch = true;
            _joypad1Val = 0;
            _joypad2Val = 0;
            _joypad1AutoRead = 0;
            _joypad2AutoRead = 0;
            _joypadStrobe = false;
            _joypad1State = 0;
            _joypad2State = 0;
            _multiplyA = 0xff;
            _divA = 0xffff;
            _divResult = 0x101;
            _mulResult = 0xfe01;
            _fastMem = false;
            _dmaTimer = 0;
            _hdmaTimer = 0;
            _dmaBusy = false;
            _dmaActive = new bool[8];
            _hdmaActive = new bool[8];
            _dmaMode = new int[8];
            _dmaFixed = new bool[8];
            _dmaDec = new bool[8];
            _hdmaInd = new bool[8];
            _dmaFromB = new bool[8];
            _dmaUnusedBit = new bool[8];
            _hdmaDoTransfer = new bool[8];
            _hdmaTerminated = new bool[8];
            _dmaOffIndex = 0;
            OpenBus = 0;
        }

        private void LoadRom(byte[] rom)
        {
            Header header;
            if (rom.Length % 0x8000 == 0)
            {
                header = ParseHeader(rom);
            }
            else if ((rom.Length - 512) % 0x8000 == 0)
            {
                var newData = new byte[rom.Length - 0x200];
                Array.Copy(rom, 0x200, newData, 0, newData.Length);
                rom = newData;
                header = ParseHeader(rom);
            }
            else
            {
                return;
            }
            GameName = header.Name;
            if (header.Type != 0)
            {
                return;
            }
            if (rom.Length < header.RomSize)
            {
                int extraData = rom.Length - header.RomSize / 2;
                var nRom = new byte[header.RomSize];
                for (var i = 0; i < nRom.Length; i++)
                {
                    if (i < header.RomSize / 2)
                    {
                        nRom[i] = rom[i];
                    }
                    else
                    {
                        nRom[i] = rom[header.RomSize / 2 + i % extraData];
                    }
                }
                rom = nRom;
            }
            ROM.LoadROM(rom, header);
        }

        private void Cycle(bool noPpu) 
        {
            _apuCatchCycles += _apuCyclesPerMaster * 2;
            if (_joypadStrobe)
            {
                _joypad1Val = _joypad1State;
                _joypad2Val = _joypad2State;
            }
            if (_hdmaTimer > 0)
            {
                _hdmaTimer -= 2;
            }
            else if (_dmaBusy)
            {
                HandleDma();
            }
            else if (XPos < 536 || XPos >= 576)
            {
                CpuCycle();
            }
            if (YPos == _vTimer && _vIrqEnabled)
            {
                if (!_hIrqEnabled)
                {
                    if (XPos == 0)
                    {
                        _inIrq = true;
                        CPU.IrqWanted = true;
                    }
                }
                else
                {
                    if (XPos == _hTimer * 4)
                    {
                        _inIrq = true;
                        CPU.IrqWanted = true;
                    }
                }
            }
            else if (XPos == _hTimer * 4 && _hIrqEnabled && !_vIrqEnabled)
            {
                _inIrq = true;
                CPU.IrqWanted = true;
            }
            if (XPos == 1024)
            {
                _inHblank = true;
                if (!_inVblank)
                {
                    HandleHdma();
                }
            }
            else if (XPos == 0)
            {
                _inHblank = false;
                PPU.CheckOverscan(YPos);
            }
            else if (XPos == 512 && !noPpu)
            {
                PPU.RenderLine(YPos);
            }
            if (YPos == (PPU.FrameOverscan ? 240 : 225) && XPos == 0)
            {
                _inNmi = true;
                _inVblank = true;
                if (_autoJoyRead)
                {
                    _autoJoyBusy = true;
                    _autoJoyTimer = 4224;
                    DoAutoJoyRead();
                }
                if (_nmiEnabled)
                {
                    CPU.NmiWanted = true;
                }
            }
            else if (YPos == 0 && XPos == 0)
            {
                _inNmi = false;
                _inVblank = false;
                InitHdma();
            }
            if (_autoJoyBusy)
            {
                _autoJoyTimer -= 2;
                if (_autoJoyTimer == 0)
                {
                    _autoJoyBusy = false;
                }
            }
            XPos += 2;
            if (XPos == 1364)
            {
                XPos = 0;
                YPos++;
                if (YPos == 262)
                {
                    CatchUpApu();
                    YPos = 0;
                }
            }
        }

        private void CpuCycle()
        {
            if (_cpuCyclesLeft == 0)
            {
                CPU.CyclesLeft = 0;
                _cpuMemOps = 0;
                CPU.Cycle();
                _cpuCyclesLeft += (CPU.CyclesLeft + 1 - _cpuMemOps) * 6;
            }
            _cpuCyclesLeft -= 2;
        }

        private void CatchUpApu() 
        {
            long catchUpCycles = (long) _apuCatchCycles & 0xffffffff;
            for (var i = 0; i < catchUpCycles; i++)
            {
                APU.Cycle();
            }
            _apuCatchCycles -= catchUpCycles;
        }

        private void RunFrame(bool noPpu)
        {
            do
            {
                Cycle(noPpu);
            } while (!(XPos == 0 && YPos == 0));
        }

        private void DoAutoJoyRead()
        {
            _joypad1AutoRead = 0;
            _joypad2AutoRead = 0;
            _joypad1Val = _joypad1State;
            _joypad2Val = _joypad2State;
            for (var i = 0; i < 16; i++)
            {
                int bit = _joypad1Val & 0x1;
                _joypad1Val >>= 1;
                _joypad1Val |= 0x8000;
                _joypad1AutoRead |= bit << (15 - i);
                bit = _joypad2Val & 0x1;
                _joypad2Val >>= 1;
                _joypad2Val |= 0x8000;
                _joypad2AutoRead |= bit << (15 - i);
            }
        }

        private void HandleDma() 
        {
            if (_dmaTimer > 0)
            {
                _dmaTimer -= 2;
                return;
            }
            int i;
            for (i = 0; i < 8; i++)
            {
                if (_dmaActive[i])
                {
                    break;
                }
            }
            if (i == 8)
            {
                _dmaBusy = false;
                _dmaOffIndex = 0;
                return;
            }
            int tableOff = _dmaMode[i] * 4 + _dmaOffIndex++;
            _dmaOffIndex &= 0x3;
            if (_dmaFromB[i])
            {
                Write((_dmaAadrBank[i] << 16) | _dmaAadr[i], ReadBBus((_dmaBadr[i] + _dmaOffs[tableOff]) & 0xff), true);
            }
            else
            {
                WriteBBus((_dmaBadr[i] + _dmaOffs[tableOff]) & 0xff, Read((_dmaAadrBank[i] << 16) | _dmaAadr[i], true));
            }
            _dmaTimer += 6;
            if (!_dmaFixed[i])
            {
                if (_dmaDec[i])
                {
                    _dmaAadr[i]--;
                }
                else
                {
                    _dmaAadr[i]++;
                }
            }
            _dmaSize[i]--;
            if (_dmaSize[i] == 0)
            {
                _dmaOffIndex = 0;
                _dmaActive[i] = false;
                _dmaTimer += 8;
            }
        }

        private void InitHdma() 
        {
            _hdmaTimer = 18;
            for (var i = 0; i < 8; i++)
            {
                int dmaBank = _dmaAadrBank[i] << 16;
                if (_hdmaActive[i])
                {
                    _dmaActive[i] = false;
                    _hdmaTableAdr[i] = _dmaAadr[i];
                    _hdmaRepCount[i] = (byte) Read(dmaBank | _hdmaTableAdr[i]++, true);
                    _hdmaTimer += 8;
                    if (_hdmaInd[i])
                    {
                        _dmaSize[i] = (ushort) Read(dmaBank | _hdmaTableAdr[i]++, true);
                        _dmaSize[i] |= (ushort) (Read(dmaBank | _hdmaTableAdr[i]++, true) << 8);
                        _hdmaTimer += 16;
                    }
                    _hdmaDoTransfer[i] = true;
                }
                else
                {
                    _hdmaDoTransfer[i] = false;
                }
                _hdmaTerminated[i] = false;
            }
        }

        private void HandleHdma()
        {
            _hdmaTimer = 18;
            for (var i = 0; i < 8; i++)
            {
                if (_hdmaActive[i] && !_hdmaTerminated[i])
                {
                    _dmaActive[i] = false;
                    _hdmaTimer += 8;
                    if (_hdmaDoTransfer[i])
                    {
                        for (var j = 0; j < _dmaOffLengths[_dmaMode[i]]; j++)
                        {
                            int tableOff = _dmaMode[i] * 4 + j;
                            _hdmaTimer += 8;
                            if (_hdmaInd[i])
                            {
                                if (_dmaFromB[i])
                                {
                                    Write((_hdmaIndBank[i] << 16) | _dmaSize[i], ReadBBus((_dmaBadr[i] + _dmaOffs[tableOff]) & 0xff), true);
                                }
                                else
                                {
                                    WriteBBus((_dmaBadr[i] + _dmaOffs[tableOff]) & 0xff, Read((_hdmaIndBank[i] << 16) | _dmaSize[i], true));
                                }
                                _dmaSize[i]++;
                            }
                            else
                            {
                                if (_dmaFromB[i])
                                {
                                    Write((_dmaAadrBank[i] << 16) | _hdmaTableAdr[i], ReadBBus((_dmaBadr[i] + _dmaOffs[tableOff]) & 0xff), true);
                                }
                                else
                                {
                                    WriteBBus((_dmaBadr[i] + _dmaOffs[tableOff]) & 0xff, Read((_dmaAadrBank[i] << 16) | _hdmaTableAdr[i], true));
                                }
                                _hdmaTableAdr[i]++;
                            }
                        }
                    }
                    _hdmaRepCount[i]--;
                    _hdmaDoTransfer[i] = (_hdmaRepCount[i] & 0x80) > 0;
                    int dmaBank = _dmaAadrBank[i] << 16;
                    if ((_hdmaRepCount[i] & 0x7f) == 0)
                    {
                        _hdmaRepCount[i] = (byte) Read(dmaBank | _hdmaTableAdr[i]++, true);
                        if (_hdmaInd[i])
                        {
                            _dmaSize[i] = (ushort) Read(dmaBank | _hdmaTableAdr[i]++, true);
                            _dmaSize[i] |= (ushort) (Read(dmaBank | _hdmaTableAdr[i]++, true) << 8);
                            _hdmaTimer += 16;
                        }
                        if (_hdmaRepCount[i] == 0)
                        {
                            _hdmaTerminated[i] = true;
                        }
                        _hdmaDoTransfer[i] = true;
                    }
                }
            }
        }

        private int ReadReg(int adr) 
        {
            switch (adr)
            {
                case 0x4210:
                    int val = 0x1;
                    val |= _inNmi ? 0x80 : 0;
                    val |= OpenBus & 0x70;
                    _inNmi = false;
                    return val;
                case 0x4211:
                    int val2 = _inIrq ? 0x80 : 0;
                    val2 |= OpenBus & 0x7f;
                    _inIrq = false;
                    CPU.IrqWanted = false;
                    return val2;
                case 0x4212:
                    int val3 = _autoJoyBusy ? 0x1 : 0;
                    val3 |= _inHblank ? 0x40 : 0;
                    val3 |= _inVblank ? 0x80 : 0;
                    val3 |= OpenBus & 0x3e;
                    return val3;
                case 0x4213:
                    return PPULatch ? 0x80 : 0;
                case 0x4214:
                    return _divResult & 0xff;
                case 0x4215:
                    return (_divResult & 0xff00) >> 8;
                case 0x4216:
                    return _mulResult & 0xff;
                case 0x4217:
                    return (_mulResult & 0xff00) >> 8;
                case 0x4218:
                    return _joypad1AutoRead & 0xff;
                case 0x4219:
                    return (_joypad1AutoRead & 0xff00) >> 8;
                case 0x421a:
                    return _joypad2AutoRead & 0xff;
                case 0x421b:
                    return (_joypad2AutoRead & 0xff00) >> 8;
                case 0x421c:
                case 0x421d:
                case 0x421e:
                case 0x421f:
                    return 0;
            }
            if (adr >= 0x4300 && adr < 0x4380)
            {
                int channel = (adr & 0xf0) >> 4;
                switch (adr & 0xff0f)
                {
                    case 0x4300:
                        int val = _dmaMode[channel];
                        val |= _dmaFixed[channel] ? 0x8 : 0;
                        val |= _dmaDec[channel] ? 0x10 : 0;
                        val |= _dmaUnusedBit[channel] ? 0x20 : 0;
                        val |= _hdmaInd[channel] ? 0x40 : 0;
                        val |= _dmaFromB[channel] ? 0x80 : 0;
                        return val;
                    case 0x4301:
                        return _dmaBadr[channel];
                    case 0x4302:
                        return _dmaAadr[channel] & 0xff;
                    case 0x4303:
                        return (_dmaAadr[channel] & 0xff00) >> 8;
                    case 0x4304:
                        return _dmaAadrBank[channel];
                    case 0x4305:
                        return _dmaSize[channel] & 0xff;
                    case 0x4306:
                        return (_dmaSize[channel] & 0xff00) >> 8;
                    case 0x4307:
                        return _hdmaIndBank[channel];
                    case 0x4308:
                        return _hdmaTableAdr[channel] & 0xff;
                    case 0x4309:
                        return (_hdmaTableAdr[channel] & 0xff00) >> 8;
                    case 0x430a:
                        return _hdmaRepCount[channel];
                    case 0x430b:
                    case 0x430f:
                        return _dmaUnusedByte[channel];
                }
            }

            return OpenBus;
        }

        private void WriteReg(int adr, int value) 
        {
            switch (adr)
            {
                case 0x4200:
                    _autoJoyRead = (value & 0x1) > 0;
                    _hIrqEnabled = (value & 0x10) > 0;
                    _vIrqEnabled = (value & 0x20) > 0;
                    _nmiEnabled = (value & 0x80) > 0;
                    return;
                case 0x4201:
                    if (PPULatch && (value & 0x80) == 0)
                    {
                        PPU.LatchedHpos = XPos >> 2;
                        PPU.LatchedVpos = YPos;
                        PPU.CountersLatched = true;
                    }
                    PPULatch = (value & 0x80) > 0;
                    return;
                case 0x4202:
                    _multiplyA = value;
                    return;
                case 0x4203:
                    _mulResult = _multiplyA * value;
                    return;
                case 0x4204:
                    _divA = (_divA & 0xff00) | value;
                    return;
                case 0x4205:
                    _divA = (_divA & 0xff) | (value << 8);
                    return;
                case 0x4206:
                    _divResult = 0xffff;
                    _mulResult = _divA;
                    if (value != 0)
                    {
                        _divResult = (_divA / value) & 0xffff;
                        _mulResult = _divA % value;
                    }
                    return;
                case 0x4207:
                    _hTimer = (_hTimer & 0x100) | value;
                    return;
                case 0x4208:
                    _hTimer = (_hTimer & 0xff) | ((value & 0x1) << 8);
                    return;
                case 0x4209:
                    _vTimer = (_vTimer & 0x100) | value;
                    return;
                case 0x420a:
                    _vTimer = (_vTimer & 0xff) | ((value & 0x1) << 8);
                    return;
                case 0x420b:
                    _dmaActive[0] = (value & 0x1) > 0;
                    _dmaActive[1] = (value & 0x2) > 0;
                    _dmaActive[2] = (value & 0x4) > 0;
                    _dmaActive[3] = (value & 0x8) > 0;
                    _dmaActive[4] = (value & 0x10) > 0;
                    _dmaActive[5] = (value & 0x20) > 0;
                    _dmaActive[6] = (value & 0x40) > 0;
                    _dmaActive[7] = (value & 0x80) > 0;
                    _dmaBusy = value > 0;
                    _dmaTimer += _dmaBusy ? 8 : 0;
                    return;
                case 0x420c:
                    _hdmaActive[0] = (value & 0x1) > 0;
                    _hdmaActive[1] = (value & 0x2) > 0;
                    _hdmaActive[2] = (value & 0x4) > 0;
                    _hdmaActive[3] = (value & 0x8) > 0;
                    _hdmaActive[4] = (value & 0x10) > 0;
                    _hdmaActive[5] = (value & 0x20) > 0;
                    _hdmaActive[6] = (value & 0x40) > 0;
                    _hdmaActive[7] = (value & 0x80) > 0;
                    return;
                case 0x420d:
                    _fastMem = (value & 0x1) > 0;
                    return;
            }

            if (adr >= 0x4300 && adr < 0x4380)
            {
                int channel = (adr & 0xf0) >> 4;
                switch (adr & 0xff0f)
                {
                    case 0x4300:
                        _dmaMode[channel] = value & 0x7;
                        _dmaFixed[channel] = (value & 0x08) > 0;
                        _dmaDec[channel] = (value & 0x10) > 0;
                        _dmaUnusedBit[channel] = (value & 0x20) > 0;
                        _hdmaInd[channel] = (value & 0x40) > 0;
                        _dmaFromB[channel] = (value & 0x80) > 0;
                        return;
                    case 0x4301:
                        _dmaBadr[channel] = (byte) value;
                        return;
                    case 0x4302:
                        _dmaAadr[channel] = (ushort) ((_dmaAadr[channel] & 0xff00) | value);
                        return;
                    case 0x4303:
                        _dmaAadr[channel] = (ushort) ((_dmaAadr[channel] & 0xff) | (value << 8));
                        return;
                    case 0x4304:
                        _dmaAadrBank[channel] = (byte) value;
                        return;
                    case 0x4305:
                        _dmaSize[channel] = (ushort) ((_dmaSize[channel] & 0xff00) | value);
                        return;
                    case 0x4306:
                        _dmaSize[channel] = (ushort) ((_dmaSize[channel] & 0xff) | (value << 8));
                        return;
                    case 0x4307:
                        _hdmaIndBank[channel] = (byte) value;
                        return;
                    case 0x4308:
                        _hdmaTableAdr[channel] = (ushort) ((_hdmaTableAdr[channel] & 0xff00) | value);
                        return;
                    case 0x4309:
                        _hdmaTableAdr[channel] = (ushort) ((_hdmaTableAdr[channel] & 0xff) | (value << 8));
                        return;
                    case 0x430a:
                        _hdmaRepCount[channel] = (byte) value;
                        return;
                    case 0x430b:
                    case 0x430f:
                        _dmaUnusedByte[channel] = (byte) value;
                        return;
                }
            }
        }

        private int ReadBBus(int adr) 
        {
            if (adr > 0x33 && adr < 0x40)
            {
                return PPU.Read(adr);
            }
            if (adr >= 0x40 && adr < 0x80)
            {
                CatchUpApu();
                return APU.SpcWritePorts[adr & 0x3];
            }
            if (adr == 0x80)
            {
                int val = _ram[_ramAdr++];
                _ramAdr &= 0x1ffff;
                return val;
            }
            return OpenBus;
        }

        private void WriteBBus(int adr, int value)
        {
            if (adr < 0x34)
            {
                PPU.Write(adr, value);
                return;
            }
            if (adr >= 0x40 && adr < 0x80)
            {
                CatchUpApu();
                APU.SpcReadPorts[adr & 0x3] = (byte) value;
                return;
            }
            switch (adr)
            {
                case 0x80:
                    _ram[_ramAdr++] = (byte) value;
                    _ramAdr &= 0x1ffff;
                    return;
                case 0x81:
                    _ramAdr = (_ramAdr & 0x1ff00) | value;
                    return;
                case 0x82:
                    _ramAdr = (_ramAdr & 0x100ff) | (value << 8);
                    return;
                case 0x83:
                    _ramAdr = (_ramAdr & 0x0ffff) | ((value & 1) << 16);
                    return;
            }
        }

        private int Rread(int adr) 
        {
            adr &= 0xffffff;
            int bank = adr >> 16;
            adr &= 0xffff;
            if (bank == 0x7e || bank == 0x7f)
            {
                return _ram[((bank & 0x1) << 16) | adr];
            }
            if (adr < 0x8000 && (bank < 0x40 || bank >= 0x80 && bank < 0xc0))
            {
                if (adr < 0x2000)
                {
                    return _ram[adr & 0x1fff];
                }
                if (adr >= 0x2100 && adr < 0x2200)
                {
                    return ReadBBus(adr & 0xff);
                }
                if (adr == 0x4016)
                {
                    int val = _joypad1Val & 0x1;
                    _joypad1Val >>= 1;
                    _joypad1Val |= 0x8000;
                    return val;
                }
                if (adr == 0x4017)
                {
                    int val = _joypad2Val & 0x1;
                    _joypad2Val >>= 1;
                    _joypad2Val |= 0x8000;
                    return val;
                }
                if (adr >= 0x4200 && adr < 0x4380)
                {
                    return ReadReg(adr);
                }
            }
            return ROM.Read(bank, adr);
        }

        private int GetAccessTime(int adr)
        {
            adr &= 0xffffff;
            int bank = adr >> 16;
            adr &= 0xffff;
            if (bank >= 0x40 && bank < 0x80)
            {
                return 8;
            }
            if (bank >= 0xc0)
            {
                return _fastMem ? 6 : 8;
            }
            if (adr < 0x2000)
            {
                return 8;
            }
            if (adr < 0x4000)
            {
                return 6;
            }
            if (adr < 0x4200)
            {
                return 12;
            }
            if (adr < 0x6000)
            {
                return 6;
            }
            if (adr < 0x8000)
            {
                return 8;
            }
            return _fastMem && bank >= 0x80 ? 6 : 8;
        }

        public void SetKeyDown(SNESButton button)
        {
            _joypad1State |= 1 << (int) button;
        }

        public void SetKeyUp(SNESButton button)
        {
            _joypad1State &= ~(1 << (int) button) & 0xfff;
        }

        private static Header ParseHeader(byte[] rom)
        {
            string str = Encoding.ASCII.GetString(rom, 0x7FC0, 21);
            var header = new Header {
                  Name = str,
                  Type = rom[0x7fd5] & 0xf,
                  Speed = rom[0x7fd5] >> 4,
                  Chips = rom[0x7fd6],
                  RomSize = 0x400 << rom[0x7fd7],
                  RamSize = 0x400 << rom[0x7fd8]
            };
            if (header.RomSize < rom.Length)
            {
                double bankCount = Math.Pow(2, Math.Ceiling(Math.Log(rom.Length / 0x8000, 2)));
                header.RomSize = (int) bankCount * 0x8000;
            }
            return header;
        }
    }
}