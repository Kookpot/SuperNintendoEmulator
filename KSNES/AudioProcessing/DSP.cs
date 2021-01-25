using Newtonsoft.Json;

namespace KSNES.AudioProcessing
{
    public class DSP : IDSP
    {
        public float[] SamplesL { get; private set; }
        public float[] SamplesR { get; private set; }
        public int SampleOffset { get; set; }

        private IAPU _apu;
        private byte[] _ram;

        [JsonIgnore]
        private readonly int[] _rates = {
            0, 2048, 1536, 1280, 1024, 768, 640, 512,
            384, 320, 256, 192, 160, 128, 96, 80,
            64, 48, 40, 32, 24, 20, 16, 12,
            10, 8, 6, 5, 4, 3, 2, 1
        };

        [JsonIgnore]
        private readonly int[] _gaussVals = {
            0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
            0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x002, 0x002, 0x002, 0x002, 0x002,
            0x002, 0x002, 0x003, 0x003, 0x003, 0x003, 0x003, 0x004, 0x004, 0x004, 0x004, 0x004, 0x005, 0x005, 0x005, 0x005,
            0x006, 0x006, 0x006, 0x006, 0x007, 0x007, 0x007, 0x008, 0x008, 0x008, 0x009, 0x009, 0x009, 0x00A, 0x00A, 0x00A,
            0x00B, 0x00B, 0x00B, 0x00C, 0x00C, 0x00D, 0x00D, 0x00E, 0x00E, 0x00F, 0x00F, 0x00F, 0x010, 0x010, 0x011, 0x011,
            0x012, 0x013, 0x013, 0x014, 0x014, 0x015, 0x015, 0x016, 0x017, 0x017, 0x018, 0x018, 0x019, 0x01A, 0x01B, 0x01B,
            0x01C, 0x01D, 0x01D, 0x01E, 0x01F, 0x020, 0x020, 0x021, 0x022, 0x023, 0x024, 0x024, 0x025, 0x026, 0x027, 0x028,
            0x029, 0x02A, 0x02B, 0x02C, 0x02D, 0x02E, 0x02F, 0x030, 0x031, 0x032, 0x033, 0x034, 0x035, 0x036, 0x037, 0x038,
            0x03A, 0x03B, 0x03C, 0x03D, 0x03E, 0x040, 0x041, 0x042, 0x043, 0x045, 0x046, 0x047, 0x049, 0x04A, 0x04C, 0x04D,
            0x04E, 0x050, 0x051, 0x053, 0x054, 0x056, 0x057, 0x059, 0x05A, 0x05C, 0x05E, 0x05F, 0x061, 0x063, 0x064, 0x066,
            0x068, 0x06A, 0x06B, 0x06D, 0x06F, 0x071, 0x073, 0x075, 0x076, 0x078, 0x07A, 0x07C, 0x07E, 0x080, 0x082, 0x084,
            0x086, 0x089, 0x08B, 0x08D, 0x08F, 0x091, 0x093, 0x096, 0x098, 0x09A, 0x09C, 0x09F, 0x0A1, 0x0A3, 0x0A6, 0x0A8,
            0x0AB, 0x0AD, 0x0AF, 0x0B2, 0x0B4, 0x0B7, 0x0BA, 0x0BC, 0x0BF, 0x0C1, 0x0C4, 0x0C7, 0x0C9, 0x0CC, 0x0CF, 0x0D2,
            0x0D4, 0x0D7, 0x0DA, 0x0DD, 0x0E0, 0x0E3, 0x0E6, 0x0E9, 0x0EC, 0x0EF, 0x0F2, 0x0F5, 0x0F8, 0x0FB, 0x0FE, 0x101,
            0x104, 0x107, 0x10B, 0x10E, 0x111, 0x114, 0x118, 0x11B, 0x11E, 0x122, 0x125, 0x129, 0x12C, 0x130, 0x133, 0x137,
            0x13A, 0x13E, 0x141, 0x145, 0x148, 0x14C, 0x150, 0x153, 0x157, 0x15B, 0x15F, 0x162, 0x166, 0x16A, 0x16E, 0x172,
            0x176, 0x17A, 0x17D, 0x181, 0x185, 0x189, 0x18D, 0x191, 0x195, 0x19A, 0x19E, 0x1A2, 0x1A6, 0x1AA, 0x1AE, 0x1B2,
            0x1B7, 0x1BB, 0x1BF, 0x1C3, 0x1C8, 0x1CC, 0x1D0, 0x1D5, 0x1D9, 0x1DD, 0x1E2, 0x1E6, 0x1EB, 0x1EF, 0x1F3, 0x1F8,
            0x1FC, 0x201, 0x205, 0x20A, 0x20F, 0x213, 0x218, 0x21C, 0x221, 0x226, 0x22A, 0x22F, 0x233, 0x238, 0x23D, 0x241,
            0x246, 0x24B, 0x250, 0x254, 0x259, 0x25E, 0x263, 0x267, 0x26C, 0x271, 0x276, 0x27B, 0x280, 0x284, 0x289, 0x28E,
            0x293, 0x298, 0x29D, 0x2A2, 0x2A6, 0x2AB, 0x2B0, 0x2B5, 0x2BA, 0x2BF, 0x2C4, 0x2C9, 0x2CE, 0x2D3, 0x2D8, 0x2DC,
            0x2E1, 0x2E6, 0x2EB, 0x2F0, 0x2F5, 0x2FA, 0x2FF, 0x304, 0x309, 0x30E, 0x313, 0x318, 0x31D, 0x322, 0x326, 0x32B,
            0x330, 0x335, 0x33A, 0x33F, 0x344, 0x349, 0x34E, 0x353, 0x357, 0x35C, 0x361, 0x366, 0x36B, 0x370, 0x374, 0x379,
            0x37E, 0x383, 0x388, 0x38C, 0x391, 0x396, 0x39B, 0x39F, 0x3A4, 0x3A9, 0x3AD, 0x3B2, 0x3B7, 0x3BB, 0x3C0, 0x3C5,
            0x3C9, 0x3CE, 0x3D2, 0x3D7, 0x3DC, 0x3E0, 0x3E5, 0x3E9, 0x3ED, 0x3F2, 0x3F6, 0x3FB, 0x3FF, 0x403, 0x408, 0x40C,
            0x410, 0x415, 0x419, 0x41D, 0x421, 0x425, 0x42A, 0x42E, 0x432, 0x436, 0x43A, 0x43E, 0x442, 0x446, 0x44A, 0x44E,
            0x452, 0x455, 0x459, 0x45D, 0x461, 0x465, 0x468, 0x46C, 0x470, 0x473, 0x477, 0x47A, 0x47E, 0x481, 0x485, 0x488,
            0x48C, 0x48F, 0x492, 0x496, 0x499, 0x49C, 0x49F, 0x4A2, 0x4A6, 0x4A9, 0x4AC, 0x4AF, 0x4B2, 0x4B5, 0x4B7, 0x4BA,
            0x4BD, 0x4C0, 0x4C3, 0x4C5, 0x4C8, 0x4CB, 0x4CD, 0x4D0, 0x4D2, 0x4D5, 0x4D7, 0x4D9, 0x4DC, 0x4DE, 0x4E0, 0x4E3,
            0x4E5, 0x4E7, 0x4E9, 0x4EB, 0x4ED, 0x4EF, 0x4F1, 0x4F3, 0x4F5, 0x4F6, 0x4F8, 0x4FA, 0x4FB, 0x4FD, 0x4FF, 0x500,
            0x502, 0x503, 0x504, 0x506, 0x507, 0x508, 0x50A, 0x50B, 0x50C, 0x50D, 0x50E, 0x50F, 0x510, 0x511, 0x511, 0x512,
            0x513, 0x514, 0x514, 0x515, 0x516, 0x516, 0x517, 0x517, 0x517, 0x518, 0x518, 0x518, 0x518, 0x518, 0x519, 0x519
        };

        private short[] _decodeBuffer;
        private short[] _rateNums;

        private int[] _pitch;
        private int[] _counter;
        private bool[] _pitchMod;

        private int[] _srcn;
        private int[] _decodeOffset;
        private int[] _prevFlags;
        private int[] _old;
        private int[] _older;
        private bool[] _enableNoise;

        private int _noiseSample;
        private int _noiseRate;
        private int _noiseCounter;

        private int[] _rateCounter;
        private int[] _adsrState;
        private int[] _sustainLevel;
        private bool[] _useGain;
        private int[] _gainMode;
        private bool[] _directGain;
        private int[] _gainValue;
        private int[] _gain;
        private int[] _channelVolumeL;
        private int[] _channelVolumeR;

        private int _volumeL;
        private int _volumeR;
        private bool _mute;

        private bool _resetFlag;
        private bool[] _noteOff;

        private int[] _sampleOut;

        private int _dirPage;

        public void Reset()
        {
            _ram = new byte[0x80];
            _decodeBuffer = new short[19 * 8];
            _rateNums = new short[5 * 8];
            for (var i = 0; i < 8; i++)
            {
                _rateNums[i * 5 + 3] = 1;
            }
            SamplesL = new float[534];
            SamplesR = new float[534];
            SampleOffset = 0;
            
            _pitch = new int[8];
            _counter = new int[8];
            _pitchMod = new bool[8];
            _srcn = new int[8];
            _decodeOffset = new int[8];
            _prevFlags = new int[8];
            _old = new int[8];
            _older = new int[8];
            _enableNoise = new bool[8];
            _noiseSample = -0x4000;
            _noiseRate = 0;
            _noiseCounter = 0;
            _rateCounter = new int[8];
            _adsrState = new[] { 3, 3, 3, 3, 3, 3, 3, 3 };
            _sustainLevel = new int[8];
            _useGain = new bool[8];
            _gainMode = new int[8];
            _directGain = new bool[8];
            _gainValue = new int[8];
            _gain = new int[8];
            _channelVolumeL = new int[8];
            _channelVolumeR = new int[8];
            _volumeL = 0;
            _volumeR = 0;
            _mute = true;
            _resetFlag = true;
            _noteOff = new[] { true, true, true, true, true, true, true, true };
            _sampleOut = new int[8];
            _dirPage = 0;
        }

        public void SetAPU(IAPU apu)
        {
            _apu = apu;
        }

        public void Cycle() 
        {
            var totalL = 0;
            var totalR = 0;
            for (var i = 0; i < 8; i++)
            {
                CycleChannel(i);
                totalL += (_sampleOut[i] * _channelVolumeL[i]) >> 6;
                totalR += (_sampleOut[i] * _channelVolumeR[i]) >> 6;
                totalL = totalL < -0x8000 ? -0x8000 : totalL > 0x7fff ? 0x7fff : totalL;
                totalR = totalR < -0x8000 ? -0x8000 : totalR > 0x7fff ? 0x7fff : totalR;
            }
            totalL = (totalL * _volumeL) >> 7;
            totalR = (totalR * _volumeR) >> 7;
            totalL = totalL < -0x8000 ? -0x8000 : totalL > 0x7fff ? 0x7fff : totalL;
            totalR = totalR < -0x8000 ? -0x8000 : totalR > 0x7fff ? 0x7fff : totalR;
            if (_mute)
            {
                totalL = 0;
                totalR = 0;
            }
            HandleNoise();
            SamplesL[SampleOffset] = (float) totalL / 0x8000;
            SamplesR[SampleOffset] = (float) totalR / 0x8000;
            SampleOffset++;
            if (SampleOffset > 533)
            {
                SampleOffset = 533;
            }
        }

        public byte Read(int adr)
        {
            return _ram[adr & 0x7f];
        }

        public void Write(int adr, byte value)
        {
            int channel = (adr & 0x70) >> 4;
            switch (adr)
            {
                case 0x0:
                case 0x10:
                case 0x20:
                case 0x30:
                case 0x40:
                case 0x50:
                case 0x60:
                case 0x70:
                    _channelVolumeL[channel] = value > 0x7f ? value - 0x100 : value;
                    break;
                case 0x1:
                case 0x11:
                case 0x21:
                case 0x31:
                case 0x41:
                case 0x51:
                case 0x61:
                case 0x71:
                    _channelVolumeR[channel] = value > 0x7f ? value - 0x100 : value;
                    break;
                case 0x2:
                case 0x12:
                case 0x22:
                case 0x32:
                case 0x42:
                case 0x52:
                case 0x62:
                case 0x72:
                    _pitch[channel] &= 0x3f00;
                    _pitch[channel] |= value;
                    break;
                case 0x3:
                case 0x13:
                case 0x23:
                case 0x33:
                case 0x43:
                case 0x53:
                case 0x63:
                case 0x73:
                    _pitch[channel] &= 0xff;
                    _pitch[channel] |= (value << 8) & 0x3f00;
                    break;
                case 0x4:
                case 0x14:
                case 0x24:
                case 0x34:
                case 0x44:
                case 0x54:
                case 0x64:
                case 0x74:
                    _srcn[channel] = value;
                    break;
                case 0x5:
                case 0x15:
                case 0x25:
                case 0x35:
                case 0x45:
                case 0x55:
                case 0x65:
                case 0x75:
                    _rateNums[channel * 5 + 0] = (short) _rates[(value & 0xf) * 2 + 1];
                    _rateNums[channel * 5 + 1] = (short) _rates[((value & 0x70) >> 4) * 2 + 16];
                    _useGain[channel] = (value & 0x80) == 0;
                    break;
                case 0x6:
                case 0x16:
                case 0x26:
                case 0x36:
                case 0x46:
                case 0x56:
                case 0x66:
                case 0x76:
                    _rateNums[channel * 5 + 2] = (short) _rates[value & 0x1f];
                    _sustainLevel[channel] = (((value & 0xe0) >> 5) + 1) * 0x100;
                    break;
                case 0x7:
                case 0x17:
                case 0x27:
                case 0x37:
                case 0x47:
                case 0x57:
                case 0x67:
                case 0x77:
                    if ((value & 0x80) > 0)
                    {
                        _directGain[channel] = false;
                        _gainMode[channel] = (value & 0x60) >> 5;
                        _rateNums[channel * 5 + 4] = (short) _rates[value & 0x1f];
                    }
                    else
                    {
                        _directGain[channel] = true;
                        _gainValue[channel] = (value & 0x7f) * 16;
                    }
                    break;
                case 0x0c:
                    _volumeL = value > 0x7f ? value - 0x100 : value;
                    break;
                case 0x1c:
                    _volumeR = value > 0x7f ? value - 0x100 : value;
                    break;
                case 0x2c:
                    break;
                case 0x3c:
                    break;
                case 0x4c:
                    var test = 1;
                    for (var i = 0; i < 8; i++)
                    {
                        if ((value & test) > 0)
                        {
                            _prevFlags[i] = 0;
                            int sampleAdr = (_dirPage << 8) + _srcn[i] * 4;
                            int startAdr = _apu.RAM[sampleAdr & 0xffff];
                            startAdr |= _apu.RAM[(sampleAdr + 1) & 0xffff] << 8;
                            _decodeOffset[i] = startAdr;
                            _gain[i] = 0;
                            if (_useGain[i])
                            {
                                _adsrState[i] = 4;
                            }
                            else
                            {
                                _adsrState[i] = 0;
                            }
                            for (var j = 0; j < 19; j++)
                            {
                                _decodeBuffer[i * 19 + j] = 0;
                            }
                        }
                        test <<= 1;
                    }
                    break;
                case 0x5c:
                    var test2 = 1;
                    for (var i = 0; i < 8; i++)
                    {
                        _noteOff[i] = (value & test2) > 0;
                        test2 <<= 1;
                    }
                    break;
                case 0x6c:
                    _resetFlag = (value & 0x80) > 0;
                    _mute = (value & 0x40) > 0;
                    _noiseRate = _rates[value & 0x1f];
                    break;
                case 0x7c:
                    _ram[0x7c] = 0;
                    value = 0;
                    break;
                case 0x0d:
                    break;
                case 0x2d:
                    var test3 = 2;
                    for (var i = 1; i < 8; i++)
                    {
                        _pitchMod[i] = (value & test3) > 0;
                        test3 <<= 1;
                    }
                    break;
                case 0x3d:
                    var test4 = 1;
                    for (var i = 0; i < 8; i++)
                    {
                        _enableNoise[i] = (value & test4) > 0;
                        test4 <<= 1;
                    }
                    break;
                case 0x4d:
                    break;
                case 0x5d:
                    _dirPage = value;
                    break;
                case 0x6d:
                    break;
                case 0x7d:
                    break;
                case 0xf:
                case 0x1f:
                case 0x2f:
                case 0x3f:
                case 0x4f:
                case 0x5f:
                case 0x6f:
                case 0x7f:
                    break;
            }
            _ram[adr & 0x7f] = value;
        }

        private void DecodeBrr(int ch) 
        {
            _decodeBuffer[ch * 19] = _decodeBuffer[ch * 19 + 16];
            _decodeBuffer[ch * 19 + 1] = _decodeBuffer[ch * 19 + 17];
            _decodeBuffer[ch * 19 + 2] = _decodeBuffer[ch * 19 + 18];
            if (_prevFlags[ch] == 1 || _prevFlags[ch] == 3)
            {
                int sampleAdr = (_dirPage << 8) + _srcn[ch] * 4;
                int loopAdr = _apu.RAM[(sampleAdr + 2) & 0xffff];
                loopAdr |= _apu.RAM[(sampleAdr + 3) & 0xffff] << 8;
                _decodeOffset[ch] = loopAdr;
                if (_prevFlags[ch] == 1)
                {
                    _gain[ch] = 0;
                    _adsrState[ch] = 3;
                }
                _ram[0x7c] |= (byte) (1 << ch);
            }
            byte header = _apu.RAM[_decodeOffset[ch]++];
            _decodeOffset[ch] &= 0xffff;
            int shift = header >> 4;
            int filter = (header & 0xc) >> 2;
            _prevFlags[ch] = header & 0x3;
            int byt = 0;
            for (var i = 0; i < 16; i++)
            {
                int s = byt & 0xf;
                if ((i & 1) == 0)
                {
                    byt = _apu.RAM[_decodeOffset[ch]++];
                    _decodeOffset[ch] &= 0xffff;
                    s = byt >> 4;
                }
                s = s > 7 ? s - 16 : s;
                if (shift <= 0xc)
                {
                    s = (s << shift) >> 1;
                }
                else
                {
                    s = s < 0 ? -2048 : 2048;
                }
                int old = _old[ch];
                int older = _older[ch];
                switch (filter)
                {
                    case 1:
                        s = s + old * 1 + ((-old * 1) >> 4);
                        break;
                    case 2:
                        s = s + old * 2 + ((-old * 3) >> 5) - older + ((older * 1) >> 4);
                        break;
                    case 3:
                        s = s + old * 2 + ((-old * 13) >> 6) - older + ((older * 3) >> 4);
                        break;
                }
                s = s > 0x7fff ? 0x7fff : s;
                s = s < -0x8000 ? -0x8000 : s;
                s &= 0x7fff;
                s = s > 0x3fff ? s - 0x8000 : s;
                _older[ch] = _old[ch];
                _old[ch] = s;
                _decodeBuffer[ch * 19 + i + 3] = (short) s;
            }
        }

        private int Interpolate(int ch, int sampleNum, int offset) 
        {
            short news = _decodeBuffer[ch * 19 + sampleNum + 3];
            short old = _decodeBuffer[ch * 19 + sampleNum + 2];
            short older = _decodeBuffer[ch * 19 + sampleNum + 1];
            short oldest = _decodeBuffer[ch * 19 + sampleNum];
            int outR = (_gaussVals[0xff - offset] * oldest) >> 10;
            outR += (_gaussVals[0x1ff - offset] * older) >> 10;
            outR += (_gaussVals[0x100 + offset] * old) >> 10;
            outR &= 0xffff;
            outR = outR > 0x7fff ? outR - 0x10000 : outR;
            outR += (_gaussVals[offset] * news) >> 10;
            outR = outR > 0x7fff ? 0x7fff : outR < -0x8000 ? -0x8000 : outR;
            return outR >> 1;
        }

        private void HandleNoise() 
        {
            if (_noiseRate != 0)
            {
                _noiseCounter++;
            }
            if (_noiseRate != 0 && _noiseCounter >= _noiseRate)
            {
                _noiseCounter = 0;
                int bit0 = _noiseSample & 1;
                int bit1 = (_noiseSample >> 1) & 1;
                _noiseSample = ((_noiseSample >> 1) & 0x3fff) | ((bit0 ^ bit1) << 14);
                _noiseSample = _noiseSample > 0x3fff ? _noiseSample - 0x8000 : _noiseSample;
            }
        }

        private void CycleChannel(int ch)
        {
            int pitch = _pitch[ch];
            if (_pitchMod[ch])
            {
                int factor = (_sampleOut[ch - 1] >> 4) + 0x400;
                pitch = (pitch * factor) >> 10;
                pitch = pitch > 0x3fff ? 0x3fff : pitch;
            }
            _counter[ch] += pitch;
            if (_counter[ch] > 0xffff)
            {
                DecodeBrr(ch);
            }
            _counter[ch] &= 0xffff;
            int sample = _enableNoise[ch] ? _noiseSample : Interpolate(ch, _counter[ch] >> 12, (_counter[ch] >> 4) & 0xff);
            if (_noteOff[ch] || _resetFlag)
            {
                _adsrState[ch] = 3;
                if (_resetFlag)
                {
                    _gain[ch] = 0;
                }
            }
            short rate = _rateNums[ch * 5 + _adsrState[ch]];
            if (rate != 0)
            {
                _rateCounter[ch]++;
            }
            if (rate != 0 && _rateCounter[ch] >= rate)
            {
                _rateCounter[ch] = 0;
                if (!_directGain[ch] || !_useGain[ch] || _adsrState[ch] == 3)
                {
                    switch (_adsrState[ch])
                    {
                        case 0:
                            _gain[ch] += rate == 1 ? 1024 : 32;
                            if (_gain[ch] >= 0x7e0)
                            {
                                _adsrState[ch] = 1;
                            }
                            if (_gain[ch] > 0x7ff)
                            {
                                _gain[ch] = 0x7ff;
                            }
                            break;
                        case 1:
                            _gain[ch] -= ((_gain[ch] - 1) >> 8) + 1;
                            if (_gain[ch] < _sustainLevel[ch])
                            {
                                _adsrState[ch] = 2;
                            }
                            break;
                        case 2:
                            _gain[ch] -= ((_gain[ch] - 1) >> 8) + 1;
                            break;
                        case 3:
                            _gain[ch] -= 8;
                            if (_gain[ch] < 0)
                            {
                                _gain[ch] = 0;
                            }
                            break;
                        case 4:
                            switch (_gainMode[ch])
                            {
                                case 0:
                                    _gain[ch] -= 32;
                                    if (_gain[ch] < 0)
                                    {
                                        _gain[ch] = 0;
                                    }
                                    break;
                                case 1:
                                    _gain[ch] -= ((_gain[ch] - 1) >> 8) + 1;
                                    break;
                                case 2:
                                    _gain[ch] += 32;
                                    if (_gain[ch] > 0x7ff)
                                    {
                                        _gain[ch] = 0x7ff;
                                    }
                                    break;
                                case 3:
                                    _gain[ch] += _gain[ch] < 0x600 ? 32 : 8;
                                    if (_gain[ch] > 0x7ff)
                                    {
                                        _gain[ch] = 0x7ff;
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            if (_directGain[ch] && _useGain[ch] && _adsrState[ch] != 3)
            {
                _gain[ch] = _gainValue[ch];
            }
            int gainedVal = (sample * _gain[ch]) >> 11;
            _ram[(ch << 4) | 8] = (byte) (_gain[ch] >> 4);
            _ram[(ch << 4) | 9] = (byte) (gainedVal >> 7);
            _sampleOut[ch] = gainedVal;
        }
    }
}