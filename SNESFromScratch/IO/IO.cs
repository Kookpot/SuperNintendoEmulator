using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.IO
{
    public class IO : IIO
    {
        public int NMITimeEnabled { get; set; }
        public int HorizontalTime { get; set; }
        public int VerticalTime { get; set; }
        public int MDMAEnabled { get; set; }
        public int HDMAEnabled { get; set; }
        public int MemorySelection { get; set; }

        public int NMIRead { get; set; }
        public int TimeUp { get; set; }
        public int HVBJoy { get; set; }
        public int Joy1 { get; set; }
        public int HVIRQ { get; private set; }

        private int _joy2;
        private int _joy3;
        private int _joy4;
        private int _ioRead;
        private int _divisionRead;
        private int _multiplyRead;
        private int _joyABit;
        private int _joyBBit;
        private int _wRAMMultiply;
        private int _wRAMDivision;

        private ISNESSystem _system;

        public void SetSystem(ISNESSystem system)
        {
            _system = system;
        }

        public void Reset()
        {
            _ioRead = 0xFF;
            NMIRead = 2;
            _wRAMMultiply = 0xFFFF;
            _wRAMDivision = 0xFFFF;
            HorizontalTime = 0x1FF;
            VerticalTime = 0x1FF;
        }

        public int Read8(int address)
        {
            var returnVal = 0;
            switch (address)
            {
                case 0x4016:
                    if ((NMITimeEnabled & 1) == 0)
                    {
                        if (_joyABit == 0)
                        {
                            _joyABit = 0x8000;
                        }
                        if ((Joy1 & _joyABit) != 0)
                        {
                            returnVal = 1;
                        }
                        _joyABit >>= 1;
                    }
                    else
                    {
                        returnVal = 1;
                    }
                    break;
                case 0x4017:
                    if ((NMITimeEnabled & 1) == 0)
                    {
                        if (_joyBBit == 0)
                        {
                            _joyBBit = 0x8000;
                        }
                        if ((_joy2 & _joyBBit) != 0)
                        {
                            returnVal = 1;
                        }
                        _joyBBit >>= 1;
                    }
                    else
                    {
                        returnVal = 0;
                    }
                    break;
                case 0x4210:
                    returnVal = NMIRead;
                    NMIRead &= ~0x80;
                    break;
                case 0x4211:
                    returnVal = TimeUp;
                    TimeUp &= ~0x80;
                    break;
                case 0x4212:
                    returnVal = HVBJoy;
                    break;
                case 0x4213:
                    returnVal = _ioRead;
                    break;
                case 0x4214:
                    returnVal = _divisionRead & 0xFF;
                    break;
                case 0x4215:
                    returnVal = _divisionRead >> 8;
                    break;
                case 0x4216:
                    returnVal = _multiplyRead & 0xFF;
                    break;
                case 0x4217:
                    returnVal = _multiplyRead >> 8;
                    break;
                case 0x4218:
                    returnVal = Joy1 & 0xFF;
                    break;
                case 0x4219:
                    returnVal = Joy1 >> 8;
                    break;
                case 0x421A:
                    returnVal = _joy2 & 0xFF;
                    break;
                case 0x421B:
                    returnVal = _joy2 >> 8;
                    break;
                case 0x421C:
                    returnVal = _joy3 & 0xFF;
                    break;
                case 0x421D:
                    returnVal = _joy3 >> 8;
                    break;
                case 0x421E:
                    returnVal = _joy4 & 0xFF;
                    break;
                case 0x421F:
                    returnVal = _joy4 >> 8;
                    break;
            }
            return returnVal;
        }

        public void Write8(int address, int value)
        {
            switch (address)
            {
                case 0x4200:
                    if ((NMITimeEnabled & 0x80) == 0 && (value & 0x80) != 0 && (NMIRead & 0x80) != 0 && _system.ScanLine > 224)
                    {
                        _system.CPU.DoNMI();
                    }
                    NMITimeEnabled = value;
                    HVIRQ = (NMITimeEnabled >> 4) & 3;
                    if (HVIRQ == 0)
                    {
                        TimeUp &= ~0x80;
                    }
                    break;
                    case 0x4201:
                    if ((value & 0x80) == 0 && (_ioRead & 0x80) != 0)
                    {
                        _system.PPU.OpHorizontalCounter = _system.PPUDot;
                        _system.PPU.OpVerticalCounter = _system.ScanLine;
                        _system.PPU.Stat78 |= 0x40;
                    }
                    _ioRead = value;
                    break;
                case 0x4202:
                    _wRAMMultiply = value;
                    break;
                case 0x4203:
                    _multiplyRead = _wRAMMultiply * value;
                    _divisionRead = value;
                    break;
                case 0x4204:
                    _wRAMDivision = value | (_wRAMDivision & 0xFF00);
                    break;
                case 0x4205:
                    _wRAMDivision = (value << 8) | (_wRAMDivision & 0xFF);
                    break;
                case 0x4206:
                    if (value == 0)
                    {
                        _divisionRead = 0xFFFF;
                        _multiplyRead = _wRAMDivision;
                    }
                    else
                    {
                        _divisionRead = _wRAMDivision / value;
                        _multiplyRead = _wRAMDivision % value;
                    }
                    break;
                case 0x4207:
                    HorizontalTime = value | (HorizontalTime & 0x100);
                    break;
                case 0x4208:
                    HorizontalTime = ((value & 1) << 8) | (HorizontalTime & 0xFF);
                    break;
                case 0x4209:
                    VerticalTime = value | (VerticalTime & 0x100);
                    break;
                case 0x420A:
                    VerticalTime = ((value & 1) << 8) | (VerticalTime & 0xFF);
                    break;
                case 0x420B:
                    MDMAEnabled = value;
                    break;
                case 0x420C:
                    HDMAEnabled = value;
                    for (int channel = 0; channel <= 7; channel++)
                    {
                        if ((HDMAEnabled & (1 << channel)) != 0)
                        {
                            _system.DMA.SetChannelEnabled(channel, true);
                        }
                    }
                    break;
                case 0x420D:
                    MemorySelection = value;
                    break;
            }
        }
    }
}