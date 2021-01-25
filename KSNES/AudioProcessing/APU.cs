using Newtonsoft.Json;

namespace KSNES.AudioProcessing
{
    public class APU : IAPU
    {
        private readonly ISPC700 _spc;
        private readonly IDSP _dsp;

        [JsonIgnore] private readonly byte[] _bootRom =
        {
            0xcd, 0xef, 0xbd, 0xe8, 0x00, 0xc6, 0x1d, 0xd0, 0xfc, 0x8f, 0xaa, 0xf4, 0x8f, 0xbb, 0xf5, 0x78,
            0xcc, 0xf4, 0xd0, 0xfb, 0x2f, 0x19, 0xeb, 0xf4, 0xd0, 0xfc, 0x7e, 0xf4, 0xd0, 0x0b, 0xe4, 0xf5,
            0xcb, 0xf4, 0xd7, 0x00, 0xfc, 0xd0, 0xf3, 0xab, 0x01, 0x10, 0xef, 0x7e, 0xf4, 0x10, 0xeb, 0xba,
            0xf6, 0xda, 0x00, 0xba, 0xf4, 0xc4, 0xf4, 0xdd, 0x5d, 0xd0, 0xdb, 0x1f, 0x00, 0x00, 0xc0, 0xff
        };

        public byte[] RAM { get; private set; } = new byte[0x10000];

        public byte[] SpcWritePorts { get; private set; } = new byte[4];
        public byte[] SpcReadPorts { get; set; } = new byte[6];
        private byte _dspAdr;
        private bool _dspRomReadable = true;

        private int _cycles;

        private int _timer1int;
        private int _timer1div;
        private int _timer1target;
        private byte _timer1counter;
        private bool _timer1enabled;
        private int _timer2int;
        private int _timer2div;
        private int _timer2target;
        private byte _timer2counter;
        private bool _timer2enabled;
        private int _timer3int;
        private int _timer3div;
        private int _timer3target;
        private byte _timer3counter;
        private bool _timer3enabled;

        public APU(ISPC700 spc, IDSP dsp)
        {
            _spc = spc;
            _dsp = dsp;
            Attach();
        }

        public void Attach()
        {
            _spc?.SetAPU(this);
            _dsp?.SetAPU(this);
        }

        public void Reset()
        {
            RAM = new byte[0x10000];
            SpcWritePorts = new byte[4];
            SpcReadPorts = new byte[6];
            _dspAdr = 0;
            _dspRomReadable = true;
            _spc.Reset();
            _dsp.Reset();
            _cycles = 0;
            _timer1int = 0;
            _timer1div = 0;
            _timer1target = 0;
            _timer1counter = 0;
            _timer1enabled = false;
            _timer2int = 0;
            _timer2div = 0;
            _timer2target = 0;
            _timer2counter = 0;
            _timer2enabled = false;
            _timer3int = 0;
            _timer3div = 0;
            _timer3target = 0;
            _timer3counter = 0;
            _timer3enabled = false;
        }

        public void Cycle()
        {
            _spc.Cycle();
            if ((_cycles & 0x1f) == 0)
            {
                _dsp.Cycle();
            }

            if (_timer1int == 0)
            {
                _timer1int = 128;
                if (_timer1enabled)
                {
                    _timer1div++;
                    _timer1div &= 0xff;
                    if (_timer1div == _timer1target)
                    {
                        _timer1div = 0;
                        _timer1counter++;
                        _timer1counter &= 0xf;
                    }
                }
            }
            _timer1int--;
            if (_timer2int == 0)
            {
                _timer2int = 128;
                if (_timer2enabled)
                {
                    _timer2div++;
                    _timer2div &= 0xff;
                    if (_timer2div == _timer2target)
                    {
                        _timer2div = 0;
                        _timer2counter++;
                        _timer2counter &= 0xf;
                    }
                }
            }
            _timer2int--;
            if (_timer3int == 0)
            {
                _timer3int = 16;
                if (_timer3enabled)
                {
                    _timer3div++;
                    _timer3div &= 0xff;
                    if (_timer3div == _timer3target)
                    {
                        _timer3div = 0;
                        _timer3counter++;
                        _timer3counter &= 0xf;
                    }
                }
            }
            _timer3int--;
            _cycles++;
        }

        public byte Read(int adr)
        {
            adr &= 0xffff;
            switch (adr)
            {
                case 0xf0:
                case 0xf1:
                case 0xfa:
                case 0xfb:
                case 0xfc:
                    return 0;
                case 0xf2:
                    return _dspAdr;
                case 0xf3:
                    return _dsp.Read(_dspAdr & 0x7f);
                case 0xf4:
                case 0xf5:
                case 0xf6:
                case 0xf7:
                case 0xf8:
                case 0xf9:
                    return SpcReadPorts[adr - 0xf4];
                case 0xfd:
                    byte val = _timer1counter;
                    _timer1counter = 0;
                    return val;
                case 0xfe:
                    byte val2 = _timer2counter;
                    _timer2counter = 0;
                    return val2;
                case 0xff:
                    byte val3 = _timer3counter;
                    _timer3counter = 0;
                    return val3;
            }
            if (adr >= 0xffc0 && _dspRomReadable)
            {
                return _bootRom[adr & 0x3f];
            }
            return RAM[adr];
        }

        public void Write(int adr, byte value)
        {
            adr &= 0xffff;
            switch (adr)
            {
                case 0xf0:
                    break;
                case 0xf1:
                    if (!_timer1enabled && (value & 0x01) > 0)
                    {
                        _timer1div = 0;
                        _timer1counter = 0;
                    }

                    if (!_timer2enabled && (value & 0x02) > 0)
                    {
                        _timer2div = 0;
                        _timer2counter = 0;
                    }

                    if (!_timer3enabled && (value & 0x04) > 0)
                    {
                        _timer3div = 0;
                        _timer3counter = 0;
                    }

                    _timer1enabled = (value & 0x01) > 0;
                    _timer2enabled = (value & 0x02) > 0;
                    _timer3enabled = (value & 0x04) > 0;
                    _dspRomReadable = (value & 0x80) > 0;
                    if ((value & 0x10) > 0)
                    {
                        SpcReadPorts[0] = 0;
                        SpcReadPorts[1] = 0;
                    }

                    if ((value & 0x20) > 0)
                    {
                        SpcReadPorts[2] = 0;
                        SpcReadPorts[3] = 0;
                    }
                    break;
                case 0xf2:
                    _dspAdr = value;
                    break;
                case 0xf3:
                    if (_dspAdr < 0x80)
                    {
                        _dsp.Write(_dspAdr, value);
                    }
                    break;
                case 0xf4:
                case 0xf5:
                case 0xf6:
                case 0xf7:
                    SpcWritePorts[adr - 0xf4] = value;
                    break;
                case 0xf8:
                case 0xf9:
                    SpcReadPorts[adr - 0xf4] = value;
                    _timer1target = value;
                    break;
                case 0xfa:
                    _timer1target = value;
                    break;
                case 0xfb:
                    _timer2target = value;
                    break;
                case 0xfc:
                    _timer3target = value;
                    break;
            }
            RAM[adr] = value;
        }

        public void SetSamples(float[] left, float[] right)
        {
            const double add = 534.0 / 735.0;
            double total = 0;
            for (var i = 0; i < 735; i++)
            {
                left[i] = _dsp.SamplesL[(int) total];
                right[i] = _dsp.SamplesR[(int) total];
                total += add;
            }
            _dsp.SampleOffset = 0;
        }
    }
}