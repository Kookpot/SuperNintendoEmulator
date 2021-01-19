using System;
using Newtonsoft.Json;

namespace SNESFromScratch.AudioProcessing
{
    public class DSP : IDSP
    {
        private const int MinS16 = -32768;
        private const int MaxS16 = 32767;
        private const int MaxU11 = 0x7FF;

        [JsonIgnore]
        private readonly int[] _gaussLUT = { 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x001,
            0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x001, 0x002, 0x002, 0x002, 0x002, 0x002, 0x002, 0x002, 0x003, 0x003, 0x003, 0x003,
            0x003, 0x004, 0x004, 0x004, 0x004, 0x004, 0x005, 0x005, 0x005, 0x005, 0x006, 0x006, 0x006, 0x006, 0x007, 0x007, 0x007, 0x008, 0x008, 0x008, 0x009,
            0x009, 0x009, 0x00A, 0x00A, 0x00A, 0x00B, 0x00B, 0x00B, 0x00C, 0x00C, 0x00D, 0x00D, 0x00E, 0x00E, 0x00F, 0x00F, 0x00F, 0x010, 0x010, 0x011, 0x011,
            0x012, 0x013, 0x013, 0x014, 0x014, 0x015, 0x015, 0x016, 0x017, 0x017, 0x018, 0x018, 0x019, 0x01A, 0x01B, 0x01B, 0x01C, 0x01D, 0x01D, 0x01E, 0x01F,
            0x020, 0x020, 0x021, 0x022, 0x023, 0x024, 0x024, 0x025, 0x026, 0x027, 0x028, 0x029, 0x02A, 0x02B, 0x02C, 0x02D, 0x02E, 0x02F, 0x030, 0x031, 0x032,
            0x033, 0x034, 0x035, 0x036, 0x037, 0x038, 0x03A, 0x03B, 0x03, 0x03D, 0x03E, 0x040, 0x041, 0x042, 0x043, 0x045, 0x046, 0x047, 0x049, 0x04A, 0x04C,
            0x04D, 0x04E, 0x050, 0x051, 0x053, 0x054, 0x056, 0x057, 0x059, 0x05A, 0x05C, 0x05E, 0x05F, 0x061, 0x063, 0x064, 0x066, 0x068, 0x06A, 0x06B, 0x06D,
            0x06F, 0x071, 0x073, 0x075, 0x076, 0x078, 0x07A, 0x07C, 0x07E, 0x080, 0x082, 0x084, 0x086, 0x089, 0x08B, 0x08D, 0x08F, 0x091, 0x093, 0x096, 0x098,
            0x09A, 0x09C, 0x09F, 0x0A1, 0x0A3, 0x0A6, 0x0A8, 0x0AB, 0x0AD, 0x0AF, 0x0B2, 0x0B4, 0x0B7, 0x0BA, 0x0BC, 0x0BF, 0x0C1, 0x0C4, 0x0C7, 0x0C9, 0x0CC,
            0x0CF, 0x0D2, 0x0D4, 0x0D7, 0x0DA, 0x0DD, 0x0E0, 0x0E3, 0x0E6, 0x0E9, 0x0EC, 0x0EF, 0x0F2, 0x0F5, 0x0F8, 0x0FB, 0x0FE, 0x101, 0x104, 0x107, 0x10B,
            0x10E, 0x111, 0x114, 0x118, 0x11B, 0x11E, 0x122, 0x125, 0x129, 0x12C, 0x130, 0x133, 0x137, 0x13A, 0x13E, 0x141, 0x145, 0x148, 0x14C, 0x150, 0x153,
            0x157, 0x15B, 0x15F, 0x162, 0x166, 0x16A, 0x16E, 0x172, 0x176, 0x17A, 0x17D, 0x181, 0x185, 0x189, 0x18D, 0x191, 0x195, 0x19A, 0x19E, 0x1A2, 0x1A6,
            0x1AA, 0x1AE, 0x1B2, 0x1B7, 0x1BB, 0x1BF, 0x1C3, 0x1C8, 0x1CC, 0x1D0, 0x1D5, 0x1D9, 0x1DD, 0x1E2, 0x1E6, 0x1EB, 0x1EF, 0x1F3, 0x1F8, 0x1FC, 0x201,
            0x205, 0x20A, 0x20F, 0x213, 0x218, 0x21C, 0x221, 0x226, 0x22A, 0x22F, 0x233, 0x238, 0x23D, 0x241, 0x246, 0x24B, 0x250, 0x254, 0x259, 0x25E, 0x263,
            0x267, 0x26C, 0x271, 0x276, 0x27B, 0x280, 0x284, 0x289, 0x28E, 0x293, 0x298, 0x29D, 0x2A2, 0x2A6, 0x2AB, 0x2B0, 0x2B5, 0x2BA, 0x2BF, 0x2C4, 0x2C9,
            0x2CE, 0x2D3, 0x2D8, 0x2DC, 0x2E1, 0x2E6, 0x2EB, 0x2F0, 0x2F5, 0x2FA, 0x2FF, 0x304, 0x309, 0x30E, 0x313, 0x318, 0x31D, 0x322, 0x326, 0x32B, 0x330,
            0x335, 0x33A, 0x33F, 0x344, 0x349, 0x34E, 0x353, 0x357, 0x35C, 0x361, 0x366, 0x36B, 0x370, 0x374, 0x379, 0x37E, 0x383, 0x388, 0x38C, 0x391, 0x396,
            0x39B, 0x39F, 0x3A4, 0x3A9, 0x3AD, 0x3B2, 0x3B7, 0x3BB, 0x3C0, 0x3C5, 0x3C9, 0x3CE, 0x3D2, 0x3D7, 0x3DC, 0x3E0, 0x3E5, 0x3E9, 0x3ED, 0x3F2, 0x3F6,
            0x3FB, 0x3FF, 0x403, 0x408, 0x40C, 0x410, 0x415, 0x419, 0x41D, 0x421, 0x425, 0x42A, 0x42E, 0x432, 0x436, 0x43A, 0x43E, 0x442, 0x446, 0x44A, 0x44E,
            0x452, 0x455, 0x459, 0x45D, 0x461, 0x465, 0x468, 0x46C, 0x470, 0x473, 0x477, 0x47A, 0x47E, 0x481, 0x485, 0x488, 0x48C, 0x48F, 0x492, 0x496, 0x499,
            0x49C, 0x49F, 0x4A2, 0x4A6, 0x4A9, 0x4AC, 0x4AF, 0x4B2, 0x4B5, 0x4B7, 0x4BA, 0x4BD, 0x4C0, 0x4C3, 0x4C5, 0x4C8, 0x4CB, 0x4CD, 0x4D0, 0x4D2, 0x4D5,
            0x4D7, 0x4D9, 0x4DC, 0x4DE, 0x4E0, 0x4E3, 0x4E5, 0x4E7, 0x4E9, 0x4EB, 0x4ED, 0x4EF, 0x4F1, 0x4F3, 0x4F5, 0x4F6, 0x4F8, 0x4FA, 0x4FB, 0x4FD, 0x4FF,
            0x500, 0x502, 0x503, 0x504, 0x506, 0x507, 0x508, 0x50A, 0x50B, 0x50C, 0x50D, 0x50E, 0x50F, 0x510, 0x511, 0x511, 0x512, 0x513, 0x514, 0x514, 0x515,
            0x516, 0x516, 0x517, 0x517, 0x517, 0x518, 0x518, 0x518, 0x518, 0x518, 0x519, 0x519 };

        [JsonIgnore]
        private readonly int[] _rateLUT = { -1, 2048, 1536, 1280, 1024, 768, 640, 512, 384, 320, 256, 192, 160, 128, 96, 80, 64, 48, 40, 32, 24, 20, 16, 12, 10, 8, 6, 5, 4, 3, 2, 1 };

        [JsonIgnore]
        private readonly int[] _offsetLUT = { 0, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 536, 0, 1040, 0, 0 };

        private readonly byte[] _soundBuffer = new byte[81920];
        private int _soundBuffAddress;
        private int _counter;

        private enum ADSR
        {
            Attack,
            Decay,
            Sustain,
            Release
        }

        private class DSPCh
        {
            public int VolL;
            public int VolR;
            public int P;
            public int SrcN;
            public int ADSR0;
            public int ADSR1;
            public int Gain;
            public int EnvX;
            public int OutX;
            public int UnusedA;
            public int UnusedB;
            public int UnusedE;
            public int Coef;

            // BRR Decoding stuff
            public int BRRFilter;
            public int BRRRange;
            public int BRRAddr;
            public int BRRNibCt;
            public int[] BRRRingBuff;
            public int BRRRingAddr;

            public int Env;
            public ADSR EnvState;
            public int InterpIdx;
            public bool Enabled;
        }

        private readonly DSPCh[] _channel = {new DSPCh(), new DSPCh(), new DSPCh(), new DSPCh(), new DSPCh(), new DSPCh(), new DSPCh(), new DSPCh()};
        private int _mVolL;
        private int _mVolR;
        private int _eVolL;
        private int _eVolR;
        private int _kOn;
        private int _kOff;
        private int _flag;
        private int _endX;
        private int _eFb;
        private int _unused;
        private int _pMOn;
        private int _nOn;
        private int _eOn;
        private int _dir;
        private int _eSA;
        private int _eDl;

        private ISPC700 _parent;

        public DSP(ISPC700 spc700)
        {
            _parent = spc700;
            for (int i = 0; i <= 7; i++)
            {
                _channel[i].BRRRingBuff = new int[12];
            }
            _counter = 0x77FF;
        }

        public void ProcessSample()
        {
            int mixL = 0;
            int mixR = 0;
            for (int channelIndex = 0; channelIndex <= 7; channelIndex++)
            {
                DSPCh currentChannel = _channel[channelIndex];
                if (currentChannel.Enabled)
                {
                    int pitch = currentChannel.P;
                    if ((_pMOn & ~_nOn & ~1 & (1 << channelIndex)) != 0)
                    {
                        pitch += ((_channel[channelIndex - 1].OutX >> 5) * currentChannel.P) >> 10;
                    }
                    currentChannel.InterpIdx += pitch;
                    if (currentChannel.InterpIdx > 0x7FFF)
                    {
                        currentChannel.InterpIdx = 0x7FFF;
                    }
                    if (currentChannel.InterpIdx > 0x3FFF)
                    {
                        DecodeBRRSamples(channelIndex);
                        currentChannel.InterpIdx -= 0x4000;
                    }
                    int sample = 0;
                    int lutIndex = (currentChannel.InterpIdx >> 4) & 0xFF;
                    int ringIndex = currentChannel.InterpIdx >> 12;
                    sample += (_gaussLUT[0xFF - lutIndex] * GetBuffer(channelIndex, ringIndex)) >> 11;
                    sample += (_gaussLUT[0x1FF - lutIndex] * GetBuffer(channelIndex, ringIndex + 1)) >> 11;
                    sample += (_gaussLUT[0x100 + lutIndex] * GetBuffer(channelIndex, ringIndex + 2)) >> 11;
                    sample = Sign16(sample & 0xFFFF) + ((_gaussLUT[lutIndex] * GetBuffer(channelIndex, ringIndex + 3)) >> 11);
                    sample = ClampS16(sample) & ~1;
                    ProcessADSR(channelIndex);
                    sample = (sample * currentChannel.Env) >> 11;
                    currentChannel.EnvX = (currentChannel.Env >> 4) & 0xFF;
                    currentChannel.OutX = (sample >> 8) & 0xFF;
                    int sampleL = sample;
                    int sampleR = sample;
                    sampleL = (sampleL * currentChannel.VolL) >> 7;
                    sampleR = (sampleR * currentChannel.VolR) >> 7;
                    mixL = ClampS16(mixL + sampleL);
                    mixR = ClampS16(mixR + sampleR);
                }
            }
            mixL = (mixL * _mVolL) >> 7;
            mixR = (mixR * _mVolR) >> 7;
            _soundBuffer[_soundBuffAddress + 0] = (byte) (mixL & 0xFF);
            _soundBuffer[_soundBuffAddress + 1] = (byte)((mixL >> 8) & 0xFF);
            _soundBuffer[_soundBuffAddress + 2] = (byte)(mixR & 0xFF);
            _soundBuffer[_soundBuffAddress + 3] = (byte)((mixR >> 8) & 0xFF);
            _soundBuffAddress = (_soundBuffAddress + 4) % _soundBuffer.Length;
            if (_counter > 0)
            {
                _counter -= 1;
            }
            else
            {
                _counter = 0x77FF;
            }
        }

        private void DecodeBRRSamples(int channelIndex)
        {
            DSPCh currentChannel = _channel[channelIndex];
            if (currentChannel.BRRNibCt == 0)
            {
                byte header = _parent.WRAM[currentChannel.BRRAddr];

                if ((header & 1) != 0)
                {
                    if ((header & 2) != 0)
                    {
                        int pointerAddress = (_dir << 8) + (currentChannel.SrcN << 2);
                        int addressLow = _parent.WRAM[pointerAddress + 2];
                        int addressHigh = _parent.WRAM[pointerAddress + 3];
                        currentChannel.BRRAddr = addressLow | (addressHigh << 8);
                    }
                    else
                    {
                        currentChannel.EnvState = ADSR.Release;
                    }
                    _endX |= (1 << channelIndex);
                }
                else
                {
                    currentChannel.BRRFilter = (header >> 2) & 3;
                    currentChannel.BRRRange = header >> 4;
                    currentChannel.BRRAddr = (currentChannel.BRRAddr + 1) & 0xFFFF;
                    currentChannel.BRRNibCt = 16;
                }
            }
            if (currentChannel.BRRNibCt != 0)
            {
                for (int i = 0; i <= 3; i++)
                {
                    int sample0 = GetBuffer(channelIndex, -1);
                    int sample1 = GetBuffer(channelIndex, -2);
                    int sample = _parent.WRAM[currentChannel.BRRAddr];
                    sample = (currentChannel.BRRNibCt & 1) != 0 ? Sign4(sample & 0xF) : Sign4(sample >> 4);
                    if (currentChannel.BRRRange > 12)
                    {
                        sample = (sample << 12) >> 3;
                    }
                    else
                    {
                        sample = (sample << currentChannel.BRRRange) >> 1;
                    }
                    switch (currentChannel.BRRFilter)
                    {
                        case 1:
                            sample = sample + sample0 * 1 + ((-sample0 * 1) >> 4);
                            break;
                        case 2:
                            sample = sample + sample0 * 2 + ((-sample0 * 3) >> 5) - sample1 + ((sample1 * 1) >> 4);
                            break;
                        case 3:
                            sample = sample + sample0 * 2 + ((-sample0 * 13) >> 6) - sample1 + ((sample1 * 3) >> 4);
                            break;
                    }
                    currentChannel.BRRAddr = (currentChannel.BRRAddr + (currentChannel.BRRNibCt & 1)) & 0xFFFF;
                    currentChannel.BRRNibCt -= 1;
                    PutBuffer(channelIndex, sample);
                }
            }
        }

        private int GetBuffer(int channelIndex, int offset)
        {
            DSPCh currentChannel = _channel[channelIndex];
            int address = currentChannel.BRRRingAddr + offset;
            if (address < 0x0)
            {
                address += 12;
            }
            if (address > 0xB)
            {
                address -= 12;
            }
            return currentChannel.BRRRingBuff[address];
        }

        private void PutBuffer(int channelIndex, int value)
        {
            DSPCh currentChannel = _channel[channelIndex];
            currentChannel.BRRRingBuff[currentChannel.BRRRingAddr] = value;
            if (currentChannel.BRRRingAddr < 0xB)
            {
                currentChannel.BRRRingAddr += 1;
            }
            else
            {
                currentChannel.BRRRingAddr = 0;
            }
        }

        private void ProcessADSR(int channelIndex)
        {
            DSPCh currentChannel = _channel[channelIndex];
            if (currentChannel.EnvState != ADSR.Release)
            {
                if ((currentChannel.ADSR0 & 0x80) != 0)
                {
                    switch (currentChannel.EnvState)
                    {
                        case ADSR.Attack:
                            int rate = currentChannel.ADSR0 & 0xF;
                            if (rate != 0xF)
                            {
                                if (RateMatches((rate << 1) | 1))
                                {
                                    currentChannel.Env += 32;
                                }
                            }
                            else if (RateMatches(0x1F))
                            {
                                currentChannel.Env += 1024;
                            }
                            break;
                        case ADSR.Decay:
                        case ADSR.Sustain:
                            if (currentChannel.EnvState == ADSR.Decay)
                            {
                                rate = (((currentChannel.ADSR0 >> 4) & 7) << 1) | 0x10;
                            }
                            else
                            {
                                rate = currentChannel.ADSR1 & 0x1F;
                            }
                            if (RateMatches(rate))
                            {
                                currentChannel.Env = currentChannel.Env - ((currentChannel.Env - 1) >> 8) + 1;
                            }
                            break;
                    }
                }
                else if ((currentChannel.Gain & 0x80) != 0)
                {
                    int rate = currentChannel.Gain & 0x1F;
                    if (RateMatches(rate))
                    {
                        switch ((currentChannel.Gain >> 5) & 3)
                        {
                            case 0:
                                currentChannel.Env -= 32; // Linear Dec.
                                break;
                            case 1:
                                currentChannel.Env = currentChannel.Env - ((currentChannel.Env - 1) >> 8) + 1; // Exp. Dec.
                                break;
                            case 2:
                                currentChannel.Env += 32; // Linear Inc.
                                break;
                            case 3:
                                currentChannel.Env = currentChannel.Env + currentChannel.Env < 0x600 ? 32 : 8; // Bent Inc.
                                break;
                        }
                    }
                }
                else
                {
                    currentChannel.Env = (currentChannel.Gain & 0x7F) << 4;
                }
            }
            else
            {
                currentChannel.Env -= 8;
                if (currentChannel.Env <= 0)
                {
                    currentChannel.Enabled = false;
                }
            }
            if (currentChannel.EnvState == ADSR.Attack & currentChannel.Env > 0x7FF)
            {
                currentChannel.EnvState = ADSR.Decay;
            }
            if (currentChannel.EnvState == ADSR.Decay & currentChannel.Env >> 8 <= currentChannel.ADSR1 >> 5)
            {
                currentChannel.EnvState = ADSR.Sustain;
            }
            currentChannel.Env = ClampU11(currentChannel.Env);
        }

        private bool RateMatches(int rate)
        {
            return _rateLUT[rate] != -1 & ((_counter + _offsetLUT[rate]) % _rateLUT[rate] == 0);
        }

        private static int ClampU11(int value)
        {
            return Math.Min(Math.Max(value, 0), MaxU11);
        }

        private static int ClampS16(int value)
        {
            return Math.Min(Math.Max(value, MinS16), MaxS16);
        }

        private static int Sign4(int value)
        {
            if ((value & 8) != 0)
            {
                return (int) (value | 0xFFFFFFF0);
            }
            return value;
        }

        private static int Sign16(int value)
        {
            if ((value & 0x8000) != 0)
            {
                return (int) (value | 0xFFFF0000);
            }
            return value;
        }

        public int Read8(int address)
        {
            address &= 0x7F;
            int returnVal = 0;
            if ((address & 0xF) < 0xC | (address & 0xF) > 0xD)
            {
                DSPCh currentChannel = _channel[address >> 4];
                switch (address & 0xF)
                {
                    case 0:
                        returnVal = currentChannel.VolL;
                        break;
                    case 1:
                        returnVal = currentChannel.VolR;
                        break;
                    case 2:
                        returnVal = currentChannel.P & 0xFF;
                        break;
                    case 3:
                        returnVal = currentChannel.P >> 8;
                        break;
                    case 4:
                        returnVal = currentChannel.SrcN;
                        break;
                    case 5:
                        returnVal = currentChannel.ADSR0;
                        break;
                    case 6:
                        returnVal = currentChannel.ADSR1;
                        break;
                    case 7:
                        returnVal = currentChannel.Gain;
                        break;
                    case 8:
                        returnVal = currentChannel.EnvX;
                        break;
                    case 9:
                        returnVal = currentChannel.OutX;
                        break;
                    case 0xA:
                        returnVal = currentChannel.UnusedA;
                        break;
                    case 0xB:
                        returnVal = currentChannel.UnusedB;
                        break;
                    case 0xE:
                        returnVal = currentChannel.UnusedE;
                        break;
                    case 0xF:
                        returnVal = currentChannel.Coef;
                        break;
                }
            }
            else
            {
                switch (address)
                {
                    case 0xC:
                        returnVal = _mVolL;
                        break;
                    case 0x1C:
                        returnVal = _mVolR;
                        break;
                    case 0x2C:
                        returnVal = _eVolL;
                        break;
                    case 0x3C:
                        returnVal = _eVolR;
                        break;
                    case 0x5C:
                        returnVal = _kOff;
                        break;
                    case 0x6C:
                        returnVal = _flag;
                        break;
                    case 0x7C:
                        returnVal = _endX;
                        break;
                    case 0xD:
                        returnVal = _eFb;
                        break;
                    case 0x1D:
                        returnVal = _unused;
                        break;
                    case 0x2D:
                        returnVal = _pMOn;
                        break;
                    case 0x3D:
                        returnVal = _nOn;
                        break;
                    case 0x4D:
                        returnVal = _eOn;
                        break;
                    case 0x5D:
                        returnVal = _dir;
                        break;
                    case 0x6D:
                        returnVal = _eSA;
                        break;
                    case 0x7D:
                        returnVal = _eDl;
                        break;
                }
            }
            return returnVal;
        }

        public void Write8(int address, int value)
        {
            if (address < 0x80)
            {
                if ((address & 0xF) < 0xC || (address & 0xF) > 0xD)
                {
                    DSPCh currentChannel = _channel[address >> 4];
                    switch (address & 0xF)
                    {
                        case 0:
                            currentChannel.VolL = value;
                            break;
                        case 1:
                            currentChannel.VolR = value;
                            break;
                        case 2:
                            currentChannel.P = value | (currentChannel.P & 0xFF00);
                            break;
                        case 3:
                            currentChannel.P = (value << 8) | (currentChannel.P & 0xFF);
                            break;
                        case 4:
                            currentChannel.SrcN = value;
                            break;
                        case 5:
                            currentChannel.ADSR0 = value;
                            break;
                        case 6:
                            currentChannel.ADSR1 = value;
                            break;
                        case 7:
                            currentChannel.Gain = value;
                            break;
                        case 8:
                            currentChannel.EnvX = value;
                            break;
                        case 0xA:
                            currentChannel.UnusedA = value;
                            break;
                        case 0xB:
                            currentChannel.UnusedB = value;
                            break;
                        case 0xE:
                            currentChannel.UnusedE = value;
                            break;
                        case 0xF:
                            currentChannel.Coef = value;
                            break;
                    }
                }
                else
                {
                    switch (address)
                    {
                        case 0xC:
                            _mVolL = value;
                            break;
                        case 0x1C:
                            _mVolR = value;
                            break;
                        case 0x2C:
                            _eVolL = value;
                            break;
                        case 0x3C:
                            _eVolR = value;
                            break;
                        case 0x4C:
                            for (int channelIndex = 0; channelIndex <= 7; channelIndex++)
                            {
                                if ((value & (1 << channelIndex)) != 0)
                                {
                                    DSPCh currentChannel = _channel[channelIndex];
                                    int pointerAddress = (_dir << 8) + (currentChannel.SrcN << 2);
                                    int addressLow = _parent.WRAM[pointerAddress + 0];
                                    int addressHigh = _parent.WRAM[pointerAddress + 1];
                                    currentChannel.BRRAddr = addressLow | (addressHigh << 8);
                                    currentChannel.BRRNibCt = 0;
                                    currentChannel.Enabled = true;
                                    currentChannel.EnvState = ADSR.Attack;
                                    _endX &= ~(1 << channelIndex);
                                    _channel[channelIndex] = currentChannel;
                                }
                            }
                            break;
                        case 0x5C:
                            for (int channelIndex = 0; channelIndex <= 7; channelIndex++)
                            {
                                if ((value & (1 << channelIndex)) != 0)
                                {
                                    _channel[channelIndex].EnvState = ADSR.Release;
                                }
                            }
                            _kOff = value;
                            break;
                        case 0x6C:
                            _flag = value;
                            break;
                        case 0x7C:
                            _endX = 0;
                            break;
                        case 0xD:
                            _eFb = value;
                            break;
                        case 0x1D:
                            _unused = value;
                            break;
                        case 0x2D:
                            _pMOn = value;
                            break;
                        case 0x3D:
                            _nOn = value;
                            break;
                        case 0x4D:
                            _eOn = value;
                            break;
                        case 0x5D:
                            _dir = value;
                            break;
                        case 0x6D:
                            _eSA = value;
                            break;
                        case 0x7D:
                            _eDl = value;
                            break;
                    }
                }
            }
        }
    }
}