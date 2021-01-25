using Newtonsoft.Json;
using KSNES.SNESSystem;

namespace KSNES.PictureProcessing
{
    public class PPU : IPPU
    {
        private ISNESSystem _snes;
        private ushort[] _vram;
        private ushort[] _cgram;
        private ushort[] _oam;
        private ushort[] _highOam;
        private byte[] _spriteLineBuffer;
        private byte[] _spritePrioBuffer;
        private int[] _mode7Xcoords;
        private int[] _mode7Ycoords;
        private int[] _pixelOutput;

        [JsonIgnore]
        private readonly int[] _layersPerMode = {
            4, 0, 1, 4, 0, 1, 4, 2, 3, 4, 2, 3,
            4, 0, 1, 4, 0, 1, 4, 2, 4, 2, 5, 5,
            4, 0, 4, 1, 4, 0, 4, 1, 5, 5, 5, 5,
            4, 0, 4, 1, 4, 0, 4, 1, 5, 5, 5, 5,
            4, 0, 4, 1, 4, 0, 4, 1, 5, 5, 5, 5,
            4, 0, 4, 1, 4, 0, 4, 1, 5, 5, 5, 5,
            4, 0, 4, 4, 0, 4, 5, 5, 5, 5, 5, 5,
            4, 4, 4, 0, 4, 5, 5, 5, 5, 5, 5, 5,
            2, 4, 0, 1, 4, 0, 1, 4, 2, 4, 5, 5,
            4, 4, 1, 4, 0, 4, 1, 5, 5, 5, 5, 5
        };

        [JsonIgnore]
        private readonly int[] _prioPerMode = {
            3, 1, 1, 2, 0, 0, 1, 1, 1, 0, 0, 0,
            3, 1, 1, 2, 0, 0, 1, 1, 0, 0, 5, 5,
            3, 1, 2, 1, 1, 0, 0, 0, 5, 5, 5, 5,
            3, 1, 2, 1, 1, 0, 0, 0, 5, 5, 5, 5,
            3, 1, 2, 1, 1, 0, 0, 0, 5, 5, 5, 5,
            3, 1, 2, 1, 1, 0, 0, 0, 5, 5, 5, 5,
            3, 1, 2, 1, 0, 0, 5, 5, 5, 5, 5, 5,
            3, 2, 1, 0, 0, 5, 5, 5, 5, 5, 5, 5,
            1, 3, 1, 1, 2, 0, 0, 1, 0, 0, 5, 5,
            3, 2, 1, 1, 0, 0, 0, 5, 5, 5, 5, 5
        };

        [JsonIgnore]
        private readonly int[] _bitPerMode = {
            2, 2, 2, 2,
            4, 4, 2, 5,
            4, 4, 5, 5,
            8, 4, 5, 5,
            8, 2, 5, 5,
            4, 2, 5, 5,
            4, 5, 5, 5,
            8, 5, 5, 5,
            4, 4, 2, 5,
            8, 7, 5, 5
        };

        [JsonIgnore]
        private readonly int[] _layercountPerMode = { 12, 10, 8, 8, 8, 8, 6, 5, 10, 7 };

        [JsonIgnore]
        private readonly double[] _brightnessMults = { 0.1, 0.5, 1.1, 1.6, 2.2, 2.7, 3.3, 3.8, 4.4, 4.9, 5.5, 6, 6.6, 7.1, 7.6, 8.2 };

        [JsonIgnore]
        private readonly int[] _spriteTileOffsets = { 
            0, 1, 2, 3, 4, 5, 6, 7,
            16, 17, 18, 19, 20, 21, 22, 23,
            32, 33, 34, 35, 36, 37, 38, 39,
            48, 49, 50, 51, 52, 53, 54, 55,
            64, 65, 66, 67, 68, 69, 70, 71,
            80, 81, 82, 83, 84, 85, 86, 87,
            96, 97, 98, 99, 100, 101, 102, 103,
            112, 113, 114, 115, 116, 117, 118, 119
        };

        [JsonIgnore]
        private readonly int[] _spriteSizes = {
            1, 1, 1, 2, 2, 4, 2, 2,
            2, 4, 8, 4, 8, 8, 4, 4
        };

        private int _cgramAdr;
        private bool _cgramSecond;
        private int _cgramBuffer;

        private int _vramInc;
        private int _vramRemap;
        private bool _vramIncOnHigh;
        private int _vramAdr;
        private int _vramReadBuffer;
        private bool[] _tilemapWider;
        private bool[] _tilemapHigher;
        private int[] _tilemapAdr;
        private int[] _tileAdr;

        private int[] _bgHoff;
        private int[] _bgVoff;

        private int _offPrev1;
        private int _offPrev2;
        private int _mode;
        private bool _layer3Prio;
        private bool[] _bigTiles;
        private bool[] _mosaicEnabled;
        private int _mosaicSize;
        private int _mosaicStartLine;
        private bool[] _mainScreenEnabled;
        private bool[] _subScreenEnabled;
        private bool _forcedBlank;
        private int _brightness;

        private int _oamAdr;
        private int _oamRegAdr;
        private bool _oamInHigh;
        private bool _oamRegInHigh;
        private bool _objPriority;
        private bool _oamSecond;
        private int _oamBuffer;


        private int _sprAdr1;
        private int _sprAdr2;
        private int _objSize;

        private bool _rangeOver;
        private bool _timeOver;

        private bool _mode7ExBg;
        private bool _pseudoHires;
        private bool _overscan;
        private bool _objInterlace;
        private bool _interlace;

        public bool FrameOverscan { get; private set; }
        private bool _evenFrame;

        public int LatchedHpos { get; set; }
        public int LatchedVpos { get; set; }
        private bool _latchHsecond;
        private bool _latchVsecond;
        public bool CountersLatched { get; set; }

        private int _mode7Hoff;
        private int _mode7Voff;
        private int _mode7A;
        private int _mode7B;
        private int _mode7C;
        private int _mode7D;
        private int _mode7X;
        private int _mode7Y;
        private int _mode7Prev;
        private int _multResult;

        private bool _mode7LargeField;
        private bool _mode7Char0fill;
        private bool _mode7FlipX;
        private bool _mode7FlipY;

        private bool[] _window1Inversed;
        private bool[] _window1Enabled;
        private bool[] _window2Inversed;
        private bool[] _window2Enabled;
        private int[] _windowMaskLogic;
        private int _window1Left;
        private int _window1Right;
        private int _window2Left;
        private int _window2Right;
        private bool[] _mainScreenWindow;
        private bool[] _subScreenWindow;

        private int _colorClip;
        private int _preventMath;
        private bool _addSub;
        private bool _directColor;

        private bool _subtractColors;
        private bool _halfColors;
        private bool[] _mathEnabled;
        private int _fixedColorB ;
        private int _fixedColorG;
        private int _fixedColorR;

        private int[] _tilemapBuffer;
        private int[] _tileBufferP1;
        private int[] _tileBufferP2;
        private int[] _tileBufferP3;
        private int[] _tileBufferP4;
        private int[] _lastTileFetchedX;
        private int[] _lastTileFetchedY;
        private int[] _optHorBuffer;
        private int[] _optVerBuffer;
        private int[] _lastOrigTileX;

        public void Reset()
        {
            _vram = new ushort[0x8000];
            _cgram = new ushort[0x100];
            _oam = new ushort[0x100];
            _highOam = new ushort[0x10];
            _spriteLineBuffer = new byte[256];
            _spritePrioBuffer = new byte[256];
            _pixelOutput = new int[57344];
            _mode7Xcoords = new int[256];
            _mode7Ycoords = new int[256];
            _cgramAdr= 0;
            _cgramSecond = false;
            _cgramBuffer = 0;
            _vramInc = 0;
            _vramRemap = 0;
            _vramIncOnHigh = false;
            _vramAdr = 0;
            _vramReadBuffer = 0;
            _tilemapWider = new bool[4];
            _tilemapHigher = new bool[4];
            _tilemapAdr = new int[4];
            _tileAdr = new int[4];
            _bgHoff = new int[5];
            _bgVoff = new int[5];
            _offPrev1 = 0;
            _offPrev2 = 0;
            _mode = 0;
            _layer3Prio = false;
            _bigTiles = new bool[4];
            _mosaicEnabled = new bool[5];
            _mosaicSize = 1;
            _mosaicStartLine = 1;
            _mainScreenEnabled = new bool[5];
            _subScreenEnabled = new bool[5];
            _forcedBlank = true;
            _brightness = 0;
            _oamAdr = 0;
            _oamRegAdr = 0;
            _oamInHigh = false;
            _oamRegInHigh = false;
            _objPriority = false;
            _oamSecond = false;
            _oamBuffer = 0;
            _sprAdr1 = 0;
            _sprAdr2 = 0;
            _objSize = 0;
            _rangeOver = false;
            _timeOver = false;
            _mode7ExBg = false;
            _pseudoHires = false;
            _overscan = false;
            _objInterlace = false;
            _interlace = false;
            FrameOverscan = false;
            _evenFrame = false;
            LatchedHpos = 0;
            LatchedVpos = 0;
            _latchHsecond = false;
            _latchVsecond = false;
            CountersLatched = false;
            _mode7Hoff = 0;
            _mode7Voff = 0;
            _mode7A = 0;
            _mode7B = 0;
            _mode7C = 0;
            _mode7D = 0;
            _mode7X = 0;
            _mode7Y = 0;
            _mode7Prev = 0;
            _multResult = 0;
            _mode7LargeField = false;
            _mode7Char0fill = false;
            _mode7FlipX = false;
            _mode7FlipY = false;
            _window1Inversed = new bool[6];
            _window1Enabled = new bool[6];
            _window2Inversed = new bool[6];
            _window2Enabled = new bool[6];
            _windowMaskLogic = new int[6];
            _window1Left = 0;
            _window1Right = 0;
            _window2Left = 0;
            _window2Right = 0;
            _mainScreenWindow = new bool[5];
            _subScreenWindow = new bool[5];
            _colorClip = 0;
            _preventMath = 0;
            _addSub = false;
            _directColor = false;
            _subtractColors = false;
            _halfColors = false;
            _mathEnabled = new bool[6];
            _fixedColorB = 0;
            _fixedColorG = 0;
            _fixedColorR = 0;
            _tilemapBuffer = new int[4];
            _tileBufferP1 = new int[4];
            _tileBufferP2 = new int[4];
            _tileBufferP3 = new int[4];
            _tileBufferP4 = new int[4];
            _lastTileFetchedX = new[] { -1, -1, -1, -1 };
            _lastTileFetchedY = new[] { -1, -1, -1, -1 };
            _optHorBuffer = new int[2];
            _optVerBuffer = new int[2];
            _lastOrigTileX = new[] { -1, -1 };
        }

        public void SetSystem(ISNESSystem snes)
        {
            _snes = snes;
        }

        public int Read(int adr)
        {
            switch (adr)
            {
                case 0x34:
                    return _multResult & 0xff;
                case 0x35:
                    return (_multResult & 0xff00) >> 8;
                case 0x36:
                    return (_multResult & 0xff0000) >> 16;
                case 0x37:
                    if (_snes.PPULatch)
                    {
                        LatchedHpos = _snes.XPos >> 2;
                        LatchedVpos = _snes.YPos;
                        CountersLatched = true;
                    }
                    return _snes.OpenBus;
                case 0x38:
                    int val;
                    if (!_oamSecond)
                    {
                        if (_oamInHigh)
                        {
                            val = _highOam[_oamAdr & 0xf] & 0xff;
                        }
                        else
                        {
                            val = _oam[_oamAdr] & 0xff;
                        }
                        _oamSecond = true;
                    }
                    else
                    {
                        if (_oamInHigh)
                        {
                            val = _highOam[_oamAdr & 0xf] >> 8;
                        }
                        else
                        {
                            val = _oam[_oamAdr] >> 8;
                        }
                        _oamAdr++;
                        _oamAdr &= 0xff;
                        _oamInHigh = _oamAdr == 0 ? !_oamInHigh : _oamInHigh;
                        _oamSecond = false;
                    }
                    return val;
                case 0x39:
                    int val2 = _vramReadBuffer;
                    _vramReadBuffer = _vram[GetVramRemap()];
                    if (!_vramIncOnHigh)
                    {
                        _vramAdr += _vramInc;
                        _vramAdr &= 0xffff;
                    }
                    return val2 & 0xff;
                case 0x3a:
                    int val3 = _vramReadBuffer;
                    _vramReadBuffer = _vram[GetVramRemap()];
                    if (_vramIncOnHigh)
                    {
                        _vramAdr += _vramInc;
                        _vramAdr &= 0xffff;
                    }
                    return (val3 & 0xff00) >> 8;
                case 0x3b:
                    int val4;
                    if (!_cgramSecond)
                    {
                        val4 = _cgram[_cgramAdr] & 0xff;
                        _cgramSecond = true;
                    }
                    else
                    {
                        val4 = _cgram[_cgramAdr++] >> 8;
                        _cgramAdr &= 0xff;
                        _cgramSecond = false;
                    }
                    return val4;
                case 0x3c:
                    int val5;
                    if (!_latchHsecond)
                    {
                        val5 = LatchedHpos & 0xff;
                        _latchHsecond = true;
                    }
                    else
                    {
                        val5 = (LatchedHpos & 0xff00) >> 8;
                        _latchHsecond = false;
                    }
                    return val5;
                case 0x3d:
                    int val6;
                    if (!_latchVsecond)
                    {
                        val6 = LatchedVpos & 0xff;
                        _latchVsecond = true;
                    }
                    else
                    {
                        val6 = (LatchedVpos & 0xff00) >> 8;
                        _latchVsecond = false;
                    }
                    return val6;
                case 0x3e:
                    int val7 = _timeOver ? 0x80 : 0;
                    val7 |= _rangeOver ? 0x40 : 0;
                    return val7 | 0x1;
                case 0x3f:
                    int val8 = _evenFrame ? 0x80 : 0;
                    val8 |= CountersLatched ? 0x40 : 0;
                    if (_snes.PPULatch)
                    {
                        CountersLatched = false;
                    }
                    _latchHsecond = false;
                    _latchVsecond = false;
                    return val8 | 0x2;
            }
            return _snes.OpenBus;
        }

        public void Write(int adr, int value)
        {
            switch (adr)
            {
                case 0x00:
                    _forcedBlank = (value & 0x80) > 0;
                    _brightness = value & 0xf;
                    return;
                case 0x01:
                    _sprAdr1 = (value & 0x7) << 13;
                    _sprAdr2 = ((value & 0x18) + 8) << 9;
                    _objSize = (value & 0xe0) >> 5;
                    return;
                case 0x02:
                    _oamAdr = value;
                    _oamRegAdr = _oamAdr;
                    _oamInHigh = _oamRegInHigh;
                    _oamSecond = false;
                    return;
                case 0x03:
                    _oamInHigh = (value & 0x1) > 0;
                    _objPriority = (value & 0x80) > 0;
                    _oamAdr = _oamRegAdr;
                    _oamRegInHigh = _oamInHigh;
                    _oamSecond = false;
                    return;
                case 0x04:
                    if (!_oamSecond)
                    {
                        if (_oamInHigh)
                        {
                            var adress = _oamAdr & 0xf;
                            _highOam[adress] = (ushort) ((_highOam[adress] & 0xff00) | value);
                        }
                        else
                        {
                            _oamBuffer = (_oamBuffer & 0xff00) | value;
                        }
                        _oamSecond = true;
                    }
                    else
                    {
                        if (_oamInHigh)
                        {
                            var adress = _oamAdr & 0xf;
                            _highOam[adress] = (ushort) ((_highOam[adress] & 0xff) | (value << 8));
                        }
                        else
                        {
                            _oamBuffer = (_oamBuffer & 0xff) | (value << 8);
                            _oam[_oamAdr] = (ushort) _oamBuffer;
                        }
                        _oamAdr++;
                        _oamAdr &= 0xff;
                        _oamInHigh = _oamAdr == 0 ? !_oamInHigh : _oamInHigh;
                        _oamSecond = false;
                    }
                    return;
                case 0x05:
                    _mode = value & 0x7;
                    _layer3Prio = (value & 0x08) > 0;
                    _bigTiles[0] = (value & 0x10) > 0;
                    _bigTiles[1] = (value & 0x20) > 0;
                    _bigTiles[2] = (value & 0x40) > 0;
                    _bigTiles[3] = (value & 0x80) > 0;
                    return;
                case 0x06:
                    _mosaicEnabled[0] = (value & 0x1) > 0;
                    _mosaicEnabled[1] = (value & 0x2) > 0;
                    _mosaicEnabled[2] = (value & 0x4) > 0;
                    _mosaicEnabled[3] = (value & 0x8) > 0;
                    _mosaicSize = ((value & 0xf0) >> 4) + 1;
                    _mosaicStartLine = _snes.YPos;
                    return;
                case 0x07:
                case 0x08:
                case 0x09:
                case 0x0a:
                    _tilemapWider[adr - 7] = (value & 0x1) > 0;
                    _tilemapHigher[adr - 7] = (value & 0x2) > 0;
                    _tilemapAdr[adr - 7] = (value & 0xfc) << 8;
                    return;
                case 0x0b:
                    _tileAdr[0] = (value & 0xf) << 12;
                    _tileAdr[1] = (value & 0xf0) << 8;
                    return;
                case 0x0c:
                    _tileAdr[2] = (value & 0xf) << 12;
                    _tileAdr[3] = (value & 0xf0) << 8;
                    return;
                case 0x0d:
                    _mode7Hoff = Get13Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    _bgHoff[(adr - 0xd) >> 1] = (value << 8) | (_offPrev1 & 0xf8) | (_offPrev2 & 0x7);
                    _offPrev1 = value;
                    _offPrev2 = value;
                    return;
                case 0x0f:
                case 0x11:
                case 0x13:
                    _bgHoff[(adr - 0xd) >> 1] = (value << 8) | (_offPrev1 & 0xf8) | (_offPrev2 & 0x7);
                    _offPrev1 = value;
                    _offPrev2 = value;
                    return;
                case 0x0e:
                    _mode7Voff = Get13Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    _bgVoff[(adr - 0xe) >> 1] = (value << 8) | (_offPrev1 & 0xff);
                    _offPrev1 = value;
                    return;
                case 0x10:
                case 0x12:
                case 0x14:
                    _bgVoff[(adr - 0xe) >> 1] = (value << 8) | (_offPrev1 & 0xff);
                    _offPrev1 = value;
                    return;
                case 0x15:
                    var incVal = value & 0x3;
                    if (incVal == 0)
                    {
                        _vramInc = 1;
                    }
                    else if (incVal == 1)
                    {
                        _vramInc = 32;
                    }
                    else
                    {
                        _vramInc = 128;
                    }
                    _vramRemap = (value & 0xc0) >> 2;
                    _vramIncOnHigh = (value & 0x80) > 0;
                    return;
                case 0x16:
                    _vramAdr = (_vramAdr & 0xff00) | value;
                    _vramReadBuffer = _vram[GetVramRemap()];
                    return;
                case 0x17:
                    _vramAdr = (_vramAdr & 0xff) | (value << 8);
                    _vramReadBuffer = _vram[GetVramRemap()];
                    return;
                case 0x18:
                    int adr2 = GetVramRemap();
                    _vram[adr2] = (ushort) ((_vram[adr2] & 0xff00) | value);
                    if (!_vramIncOnHigh)
                    {
                        _vramAdr += _vramInc;
                        _vramAdr &= 0xffff;
                    }
                    return;
                case 0x19:
                    int adr3 = GetVramRemap();
                    _vram[adr3] = (ushort) ((_vram[adr3] & 0xff) | (value << 8));
                    if (_vramIncOnHigh)
                    {
                        _vramAdr += _vramInc;
                        _vramAdr &= 0xffff;
                    }
                    return;
                case 0x1a:
                    _mode7LargeField = (value & 0x80) > 0;
                    _mode7Char0fill = (value & 0x40) > 0;
                    _mode7FlipY = (value & 0x2) > 0;
                    _mode7FlipX = (value & 0x1) > 0;
                    return;
                case 0x1b:
                    _mode7A = Get16Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    _multResult = GetMultResult(_mode7A, _mode7B);
                    return;
                case 0x1c:
                    _mode7B = Get16Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    _multResult = GetMultResult(_mode7A, _mode7B);
                    return;
                case 0x1d:
                    _mode7C = Get16Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    return;
                case 0x1e:
                    _mode7D = Get16Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    return;
                case 0x1f:
                    _mode7X = Get13Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    return;
                case 0x20:
                    _mode7Y = Get13Signed((value << 8) | _mode7Prev);
                    _mode7Prev = value;
                    return;
                case 0x21:
                    _cgramAdr = value;
                    _cgramSecond = false;
                    return;
                case 0x22:
                    if (!_cgramSecond)
                    {
                        _cgramBuffer = (_cgramBuffer & 0xff00) | value;
                        _cgramSecond = true;
                    }
                    else
                    {
                        _cgramBuffer = (_cgramBuffer & 0xff) | (value << 8);
                        _cgram[_cgramAdr++] = (ushort) _cgramBuffer;
                        _cgramAdr &= 0xff;
                        _cgramSecond = false;
                    }
                    return;
                case 0x23:
                    _window1Inversed[0] = (value & 0x01) > 0;
                    _window1Enabled[0] = (value & 0x02) > 0;
                    _window2Inversed[0] = (value & 0x04) > 0;
                    _window2Enabled[0] = (value & 0x08) > 0;
                    _window1Inversed[1] = (value & 0x10) > 0;
                    _window1Enabled[1] = (value & 0x20) > 0;
                    _window2Inversed[1] = (value & 0x40) > 0;
                    _window2Enabled[1] = (value & 0x80) > 0;
                    return;
                case 0x24:
                    _window1Inversed[2] = (value & 0x01) > 0;
                    _window1Enabled[2] = (value & 0x02) > 0;
                    _window2Inversed[2] = (value & 0x04) > 0;
                    _window2Enabled[2] = (value & 0x08) > 0;
                    _window1Inversed[3] = (value & 0x10) > 0;
                    _window1Enabled[3] = (value & 0x20) > 0;
                    _window2Inversed[3] = (value & 0x40) > 0;
                    _window2Enabled[3] = (value & 0x80) > 0;
                    return;
                case 0x25:
                    _window1Inversed[4] = (value & 0x01) > 0;
                    _window1Enabled[4] = (value & 0x02) > 0;
                    _window2Inversed[4] = (value & 0x04) > 0;
                    _window2Enabled[4] = (value & 0x08) > 0;
                    _window1Inversed[5] = (value & 0x10) > 0;
                    _window1Enabled[5] = (value & 0x20) > 0;
                    _window2Inversed[5] = (value & 0x40) > 0;
                    _window2Enabled[5] = (value & 0x80) > 0;
                    return;
                case 0x26:
                    _window1Left = value;
                    return;
                case 0x27:
                    _window1Right = value;
                    return;
                case 0x28:
                    _window2Left = value;
                    return;
                case 0x29:
                    _window2Right = value;
                    return;
                case 0x2a:
                    _windowMaskLogic[0] = value & 0x3;
                    _windowMaskLogic[1] = (value & 0xc) >> 2;
                    _windowMaskLogic[2] = (value & 0x30) >> 4;
                    _windowMaskLogic[3] = (value & 0xc0) >> 6;
                    return;
                case 0x2b:
                    _windowMaskLogic[4] = value & 0x3;
                    _windowMaskLogic[5] = (value & 0xc) >> 2;
                    return;
                case 0x2c:
                    _mainScreenEnabled[0] = (value & 0x1) > 0;
                    _mainScreenEnabled[1] = (value & 0x2) > 0;
                    _mainScreenEnabled[2] = (value & 0x4) > 0;
                    _mainScreenEnabled[3] = (value & 0x8) > 0;
                    _mainScreenEnabled[4] = (value & 0x10) > 0;
                    return;
                case 0x2d:
                    _subScreenEnabled[0] = (value & 0x1) > 0;
                    _subScreenEnabled[1] = (value & 0x2) > 0;
                    _subScreenEnabled[2] = (value & 0x4) > 0;
                    _subScreenEnabled[3] = (value & 0x8) > 0;
                    _subScreenEnabled[4] = (value & 0x10) > 0;
                    return;
                case 0x2e:
                    _mainScreenWindow[0] = (value & 0x1) > 0;
                    _mainScreenWindow[1] = (value & 0x2) > 0;
                    _mainScreenWindow[2] = (value & 0x4) > 0;
                    _mainScreenWindow[3] = (value & 0x8) > 0;
                    _mainScreenWindow[4] = (value & 0x10) > 0;
                    return;
                case 0x2f:
                    _subScreenWindow[0] = (value & 0x1) > 0;
                    _subScreenWindow[1] = (value & 0x2) > 0;
                    _subScreenWindow[2] = (value & 0x4) > 0;
                    _subScreenWindow[3] = (value & 0x8) > 0;
                    _subScreenWindow[4] = (value & 0x10) > 0;
                    return;
                case 0x30:
                    _colorClip = (value & 0xc0) >> 6;
                    _preventMath = (value & 0x30) >> 4;
                    _addSub = (value & 0x2) > 0;
                    _directColor = (value & 0x1) > 0;
                    return;
                case 0x31:
                    _subtractColors = (value & 0x80) > 0;
                    _halfColors = (value & 0x40) > 0;
                    _mathEnabled[0] = (value & 0x1) > 0;
                    _mathEnabled[1] = (value & 0x2) > 0;
                    _mathEnabled[2] = (value & 0x4) > 0;
                    _mathEnabled[3] = (value & 0x8) > 0;
                    _mathEnabled[4] = (value & 0x10) > 0;
                    _mathEnabled[5] = (value & 0x20) > 0;
                    return;
                case 0x32:
                    if ((value & 0x80) > 0)
                    {
                        _fixedColorB = value & 0x1f;
                    }
                    if ((value & 0x40) > 0)
                    {
                        _fixedColorG = value & 0x1f;
                    }
                    if ((value & 0x20) > 0)
                    {
                        _fixedColorR = value & 0x1f;
                    }
                    return;
                case 0x33:
                    _mode7ExBg = (value & 0x40) > 0;
                    _pseudoHires = (value & 0x08) > 0;
                    _overscan = (value & 0x04) > 0;
                    _objInterlace = (value & 0x02) > 0;
                    _interlace = (value & 0x01) > 0;
                    return;
            }
        }

        public int[] GetPixels()
        {
            return _pixelOutput;
        }

        public void CheckOverscan(int line) 
        {
            if (line == 225 && _overscan)
            {
                FrameOverscan = true;
            }
        }

        public void RenderLine(int line) 
        {
            if (line == 0)
            {
                _rangeOver = false;
                _timeOver = false;
                FrameOverscan = false;
                _spriteLineBuffer =  new byte[256];
                if (!_forcedBlank)
                {
                    EvaluateSprites(0);
                }
            }
            else if (line == (FrameOverscan ? 240 : 225))
            {
                if (!_forcedBlank)
                {
                    _oamAdr = _oamRegAdr;
                    _oamInHigh = _oamRegInHigh;
                }
                _evenFrame = !_evenFrame;
            }
            else if (line > 0 && line < (FrameOverscan ? 240 : 225))
            {
                if (line == 1)
                {
                    _mosaicStartLine = 1;
                }
                if (_mode == 7)
                {
                    GenerateMode7Coords(line);
                }
                _lastTileFetchedX = new[] { -1, -1, -1, -1 };
                _lastTileFetchedY = new[] { -1, -1, -1, -1 };
                _optHorBuffer = new[] { 0, 0 };
                _optVerBuffer = new[] { 0, 0 };
                _lastOrigTileX = new[] { -1, -1 };
                double bMult = _brightnessMults[_brightness];
                var i = 0;
                while (i < 256)
                {
                    var r1 = 0;
                    var g1 = 0;
                    var b1 = 0;
                    var r2 = 0;
                    var g2 = 0;
                    var b2 = 0;
                    if (!_forcedBlank)
                    {
                        var (color, item2, item3) = GetColor(false, i, line);
                        r2 = color & 0x1f;
                        g2 = (color & 0x3e0) >> 5;
                        b2 = (color & 0x7c00) >> 10;
                        if (_colorClip == 3 || _colorClip == 2 && GetWindowState(i, 5) || _colorClip == 1 && !GetWindowState(i, 5))
                        {
                            r2 = 0;
                            g2 = 0;
                            b2 = 0;
                        }
                        var secondLay = (0, 5, 0);
                        if (_mode == 5 || _mode == 6 || _pseudoHires || GetMathEnabled(i, item2, item3) && _addSub)
                        {
                            secondLay = GetColor(true, i, line);
                            r1 = secondLay.Item1 & 0x1f;
                            g1 = (secondLay.Item1 & 0x3e0) >> 5;
                            b1 = (secondLay.Item1 & 0x7c00) >> 10;
                        }
                        if (GetMathEnabled(i, item2, item3))
                        {
                            if (_subtractColors)
                            {
                                r2 -= _addSub && secondLay.Item2 < 5 ? r1 : _fixedColorR;
                                g2 -= _addSub && secondLay.Item2 < 5 ? g1 : _fixedColorG;
                                b2 -= _addSub && secondLay.Item2 < 5 ? b1 : _fixedColorB;
                            }
                            else
                            {
                                r2 += _addSub && secondLay.Item2 < 5 ? r1 : _fixedColorR;
                                g2 += _addSub && secondLay.Item2 < 5 ? g1 : _fixedColorG;
                                b2 += _addSub && secondLay.Item2 < 5 ? b1 : _fixedColorB;
                            }
                            if (_halfColors && (secondLay.Item2 < 5 || !_addSub))
                            {
                                r2 >>= 1;
                                g2 >>= 1;
                                b2 >>= 1;
                            }
                            r2 = r2 > 31 ? 31 : r2;
                            r2 = r2 < 0 ? 0 : r2;
                            g2 = g2 > 31 ? 31 : g2;
                            g2 = g2 < 0 ? 0 : g2;
                            b2 = b2 > 31 ? 31 : b2;
                            b2 = b2 < 0 ? 0 : b2;
                        }
                    }
                    var realColor = ((byte) (b2 * bMult) & 0xff) | (((byte) (g2 * bMult) & 0xff) << 8) | (((byte) (r2 * bMult) & 0xff) << 16);
                    _pixelOutput[(line - 1) * 256 + i] = (int) (realColor | 0xFF000000);
                    i++;
                }
                _spriteLineBuffer = new byte[256];
                if (!_forcedBlank)
                {
                    EvaluateSprites(line);
                }
            }
        }

        private (ushort, int, int) GetColor(bool sub, int x, int y) 
        {
            int modeIndex = _layer3Prio && _mode == 1 ? 96 : 12 * _mode;
            modeIndex = _mode7ExBg && _mode == 7 ? 108 : modeIndex;
            int count = _layercountPerMode[_mode];
            int j;
            var pixel = 0;
            var layer = 5;
            if (_interlace && (_mode == 5 || _mode == 6))
            {
                y = y * 2 + (_evenFrame ? 1 : 0);
            }
            for (j = 0; j < count; j++)
            {
                int lx = x;
                int ly = y;
                layer = _layersPerMode[modeIndex + j];
                if (!sub && _mainScreenEnabled[layer] && (!_mainScreenWindow[layer] || !GetWindowState(lx, layer)) || sub && _subScreenEnabled[layer] && (!_subScreenWindow[layer] || !GetWindowState(lx, layer)))
                {
                    if (_mosaicEnabled[layer])
                    {
                        lx -= lx % _mosaicSize;
                        ly -= (ly - _mosaicStartLine) % _mosaicSize;
                    }
                    lx += _mode == 7 ? 0 : _bgHoff[layer];
                    ly += _mode == 7 ? 0 : _bgVoff[layer];
                    int optX = lx - _bgHoff[layer];
                    if ((_mode == 5 || _mode == 6) && layer < 4)
                    {
                        lx = lx * 2 + (sub ? 0 : 1);
                        optX = optX * 2 + (sub ? 0 : 1);
                    }
                    if ((_mode == 2 || _mode == 4 || _mode == 6) && layer < 2)
                    {
                        int andVal = layer == 0 ? 0x2000 : 0x4000;
                        if (x == 0)
                        {
                            _lastOrigTileX[layer] = lx >> 3;
                        }
                        int tileStartX = optX - (lx - (lx & 0xfff8));
                        if (lx >> 3 != _lastOrigTileX[layer] && x > 0)
                        {
                            FetchTileInBuffer(_bgHoff[2] + ((tileStartX - 1) & 0x1f8), _bgVoff[2], 2, true);
                            _optHorBuffer[layer] = _tilemapBuffer[2];
                            if (_mode == 4)
                            {
                                if ((_optHorBuffer[layer] & 0x8000) > 0)
                                {
                                    _optVerBuffer[layer] = _optHorBuffer[layer];
                                    _optHorBuffer[layer] = 0;
                                }
                                else
                                {
                                    _optVerBuffer[layer] = 0;
                                }
                            }
                            else
                            {
                                FetchTileInBuffer(_bgHoff[2] + ((tileStartX - 1) & 0x1f8), _bgVoff[2] + 8, 2, true);
                                _optVerBuffer[layer] = _tilemapBuffer[2];
                            }
                            _lastOrigTileX[layer] = lx >> 3;
                        }
                        if ((_optHorBuffer[layer] & andVal) > 0)
                        {
                            int add = (tileStartX + 7) & 0x1f8;
                            lx = (lx & 0x7) + ((_optHorBuffer[layer] + add) & 0x1ff8);
                        }
                        if ((_optVerBuffer[layer] & andVal) > 0)
                        {
                            ly = (_optVerBuffer[layer] & 0x1fff) + (ly - _bgVoff[layer]);
                        }
                    }
                    pixel = GetPixelForLayer(lx, ly, layer, _prioPerMode[modeIndex + j]);
                }
                if ((pixel & 0xff) > 0)
                {
                    break;
                }
            }
            layer = j == count ? 5 : layer;
            ushort color = _cgram[pixel & 0xff];
            if (_directColor && layer < 4 && _bitPerMode[_mode * 4 + layer] == 8)
            {
                int r = ((pixel & 0x7) << 2) | ((pixel & 0x100) >> 7);
                int g = ((pixel & 0x38) >> 1) | ((pixel & 0x200) >> 8);
                int b = ((pixel & 0xc0) >> 3) | ((pixel & 0x400) >> 8);
                color = (ushort) ((b << 10) | (g << 5) | r);
            }
            return (color, layer, pixel);
        }

        private bool GetMathEnabled(int x, int l, int pal) 
        {
            if (_preventMath == 3 || (_preventMath == 2 && GetWindowState(x, 5)) || (_preventMath == 1 && !GetWindowState(x, 5)))
            {
                return false;
            }
            if (_mathEnabled[l] && (l != 4 || pal >= 0xc0))
            {
                return true;
            }
            return false;
        }

        private bool GetWindowState(int x, int l) 
        {
            if (!_window1Enabled[l] && !_window2Enabled[l])
            {
                return false;
            }
            if (_window1Enabled[l] && !_window2Enabled[l])
            {
                bool test = x >= _window1Left && x <= _window1Right;
                return _window1Inversed[l] ? !test : test;
            }
            if (!_window1Enabled[l] && _window2Enabled[l])
            {
                bool test = x >= _window2Left && x <= _window2Right;
                return _window2Inversed[l] ? !test : test;
            }
            bool w1test = x >= _window1Left && x <= _window1Right;
            w1test = _window1Inversed[l] ? !w1test : w1test;
            bool w2test = x >= _window2Left && x <= _window2Right;
            w2test = _window2Inversed[l] ? !w2test : w2test;
            switch (_windowMaskLogic[l])
            {
                case 0:
                    return w1test || w2test;
                case 1:
                    return w1test && w2test;
                case 2:
                    return w1test != w2test;
                case 3:
                    return w1test == w2test;
            }
            return false;
        }

        private int GetPixelForLayer(int x, int y, int l, int p) 
        {
            if (l > 3)
            {
                if (_spritePrioBuffer[x] != p)
                {
                    return 0;
                }
                return _spriteLineBuffer[x];
            }
            if (_mode == 7)
            {
                return GetMode7Pixel(x, y, l, p);
            }
            if (x >> 3 != _lastTileFetchedX[l] || y != _lastTileFetchedY[l])
            {
                FetchTileInBuffer(x, y, l, false);
                _lastTileFetchedX[l] = x >> 3;
                _lastTileFetchedY[l] = y;
            }
            int mapWord = _tilemapBuffer[l];
            if ((mapWord & 0x2000) >> 13 != p)
            {
                return 0;
            }
            int paletteNum = (mapWord & 0x1c00) >> 10;
            int xShift = (mapWord & 0x4000) > 0 ? x & 0x7 : 7 - (x & 0x7);
            paletteNum += _mode == 0 ? l * 8 : 0;
            int bits = _bitPerMode[_mode * 4 + l];
            int mul = 4;
            int tileData = (_tileBufferP1[l] >> xShift) & 0x1;
            tileData |= ((_tileBufferP1[l] >> (8 + xShift)) & 0x1) << 1;
            if (bits > 2)
            {
                mul = 16;
                tileData |= ((_tileBufferP2[l] >> xShift) & 0x1) << 2;
                tileData |= ((_tileBufferP2[l] >> (8 + xShift)) & 0x1) << 3;
            }
            if (bits > 4)
            {
                mul = 256;
                tileData |= ((_tileBufferP3[l] >> xShift) & 0x1) << 4;
                tileData |= ((_tileBufferP3[l] >> (8 + xShift)) & 0x1) << 5;
                tileData |= ((_tileBufferP4[l] >> xShift) & 0x1) << 6;
                tileData |= ((_tileBufferP4[l] >> (8 + xShift)) & 0x1) << 7;
            }
            return tileData > 0 ? paletteNum * mul + tileData : 0;
        }

        private void FetchTileInBuffer(int x, int y, int l, bool offset) 
        {
            int rx = x;
            int ry = y;
            bool useXbig = _bigTiles[l] | _mode == 5 | _mode == 6;
            x >>= useXbig ? 1 : 0;
            y >>= _bigTiles[l] ? 1 : 0;
            int adr = _tilemapAdr[l] + (((y & 0xff) >> 3) << 5 | ((x & 0xff) >> 3));
            adr += (x & 0x100) > 0 && _tilemapWider[l] ? 1024 : 0;
            adr += (y & 0x100) > 0 && _tilemapHigher[l] ? _tilemapWider[l] ? 2048 : 1024 : 0;
            _tilemapBuffer[l] = _vram[adr & 0x7fff];
            if (offset)
            {
                return;
            }
            bool yFlip = (_tilemapBuffer[l] & 0x8000) > 0;
            bool xFlip = (_tilemapBuffer[l] & 0x4000) > 0;
            int yRow = yFlip ? 7 - (ry & 0x7) : ry & 0x7;
            int tileNum = _tilemapBuffer[l] & 0x3ff;
            tileNum += useXbig && (rx & 0x8) == (xFlip ? 0 : 8) ? 1 : 0;
            tileNum += _bigTiles[l] && (ry & 0x8) == (yFlip ? 0 : 8) ? 0x10 : 0;
            int bits = _bitPerMode[_mode * 4 + l];
            _tileBufferP1[l] = _vram[(_tileAdr[l] + tileNum * 4 * bits + yRow) & 0x7fff];
            if (bits > 2)
            {
                _tileBufferP2[l] = _vram[(_tileAdr[l] + tileNum * 4 * bits + yRow + 8) & 0x7fff];
            }
            if (bits > 4)
            {
                _tileBufferP3[l] = _vram[(_tileAdr[l] + tileNum * 4 * bits + yRow + 16) & 0x7fff];
                _tileBufferP4[l] = _vram[(_tileAdr[l] + tileNum * 4 * bits + yRow + 24) & 0x7fff];
            }
        }

        private void EvaluateSprites(int line) 
        {
            var spriteCount = 0;
            var sliverCount = 0;
            int index = _objPriority ? ((_oamAdr & 0xfe) - 2) & 0xff : 254;
            for (var i = 0; i < 128; i++)
            {
                int x = _oam[index] & 0xff;
                int y = (_oam[index] & 0xff00) >> 8;
                int tile = _oam[index + 1] & 0xff;
                int ex = (_oam[index + 1] & 0xff00) >> 8;
                x |= (_highOam[index >> 4] >> (index & 0xf) & 0x1) << 8;
                bool big = (_highOam[index >> 4] >> (index & 0xf) & 0x2) > 0;
                x = x > 255 ? -(512 - x) : x;
                int size = _spriteSizes[_objSize + (big ? 8 : 0)];
                int sprRow = line - y;
                if (sprRow < 0 || sprRow >= size * (_objInterlace ? 4 : 8))
                {
                    sprRow = line + (256 - y);
                }
                if (sprRow >= 0 && sprRow < size * (_objInterlace ? 4 : 8) && x > -(size * 8))
                {
                    if (spriteCount == 32)
                    {
                        _rangeOver = true;
                        break;
                    }
                    sprRow = _objInterlace ? sprRow * 2 + (_evenFrame ? 1 : 0) : sprRow;
                    int adr = _sprAdr1 + ((ex & 0x1) > 0 ? _sprAdr2 : 0);
                    sprRow = (ex & 0x80) > 0 ? size * 8 - 1 - sprRow : sprRow;
                    int tileRow = sprRow >> 3;
                    sprRow &= 0x7;
                    for (var k = 0; k < size; k++)
                    {
                        if (x + k * 8 > -7 && x + k * 8 < 256)
                        {
                            if (sliverCount == 34)
                            {
                                sliverCount = 35;
                                break;
                            }
                            int tileColumn = (ex & 0x40) > 0 ? size - 1 - k : k;
                            int tileNum = tile + _spriteTileOffsets[tileRow * 8 + tileColumn];
                            tileNum &= 0xff;
                            int tileP1 = _vram[(adr + tileNum * 16 + sprRow) & 0x7fff];
                            int tileP2 = _vram[(adr + tileNum * 16 + sprRow + 8) & 0x7fff];
                            for (var j = 0; j < 8; j++)
                            {
                                int shift = (ex & 0x40) > 0 ? j : 7 - j;
                                int tileData = (tileP1 >> shift) & 0x1;
                                tileData |= ((tileP1 >> (8 + shift)) & 0x1) << 1;
                                tileData |= ((tileP2 >> shift) & 0x1) << 2;
                                tileData |= ((tileP2 >> (8 + shift)) & 0x1) << 3;
                                int color = tileData + 16 * ((ex & 0xe) >> 1);
                                int xInd = x + k * 8 + j;
                                if (tileData > 0 && xInd < 256 && xInd >= 0)
                                {
                                    _spriteLineBuffer[xInd] = (byte) (0x80 + color);
                                    _spritePrioBuffer[xInd] = (byte) ((ex & 0x30) >> 4);
                                }
                            }
                            sliverCount++;
                        }
                    }
                    if (sliverCount == 35)
                    {
                        _timeOver = true;
                        break;
                    }
                    spriteCount++;
                }
                index = (index - 2) & 0xff;
            }
        }

         private void GenerateMode7Coords(int y)
         {
            int rY = _mode7FlipY ? 255 - y : y;
            int clippedH = _mode7Hoff - _mode7X;
            clippedH = (clippedH & 0x2000) > 0 ? clippedH | ~0x3ff : clippedH & 0x3ff;
            int clippedV = _mode7Voff - _mode7Y;
            clippedV = (clippedV & 0x2000) > 0 ? clippedV | ~0x3ff : clippedV & 0x3ff;
            int lineStartX = ((_mode7A * clippedH) & ~63) + ((_mode7B * rY) & ~63) + ((_mode7B * clippedV) & ~63) + (_mode7X << 8);
            int lineStartY = ((_mode7C * clippedH) & ~63) + ((_mode7D * rY) & ~63) + ((_mode7D * clippedV) & ~63) + (_mode7Y << 8);
            _mode7Xcoords[0] = lineStartX;
            _mode7Ycoords[0] = lineStartY;
            for (var i = 1; i < 256; i++)
            {
                _mode7Xcoords[i] = _mode7Xcoords[i - 1] + _mode7A;
                _mode7Ycoords[i] = _mode7Ycoords[i - 1] + _mode7C;
            }
        }

        private int GetMode7Pixel(int x, int y, int l, int p) 
        {
            int pixelData = _tilemapBuffer[0];
            if (x != _lastTileFetchedX[0] || y != _lastTileFetchedY[0])
            {
                int rX = _mode7FlipX ? 255 - x : x;
                int px = _mode7Xcoords[rX] >> 8;
                int py = _mode7Ycoords[rX] >> 8;
                var pixelIsTransparent = false;
                if (_mode7LargeField && (px < 0 || px >= 1024 || py < 0 || py >= 1024))
                {
                    if (_mode7Char0fill)
                    {
                        px &= 0x7;
                        py &= 0x7;
                    }
                    else
                    {
                        pixelIsTransparent = true;
                    }
                }
                int tileX = (px & 0x3f8) >> 3;
                int tileY = (py & 0x3f8) >> 3;
                int tileByte = _vram[tileY * 128 + tileX] & 0xff;
                pixelData = _vram[tileByte * 64 + (py & 0x7) * 8 + (px & 0x7)];
                pixelData >>= 8;
                pixelData = pixelIsTransparent ? 0 : pixelData;
                _tilemapBuffer[0] = pixelData;
                _lastTileFetchedX[0] = x;
                _lastTileFetchedY[0] = y;
            }
            if (l == 1 && pixelData >> 7 != p)
            {
                return 0;
            }
            if (l == 1)
            {
                return pixelData & 0x7f;
            }
            return pixelData;
        }

        private int GetVramRemap() 
        {
            int adr = _vramAdr & 0x7fff;
            if (_vramRemap == 1)
            {
                adr = (adr & 0xff00) | ((adr & 0xe0) >> 5) | ((adr & 0x1f) << 3);
            }
            else if (_vramRemap == 2)
            {
                adr = (adr & 0xfe00) | ((adr & 0x1c0) >> 6) | ((adr & 0x3f) << 3);
            }
            else if (_vramRemap == 3)
            {
                adr = (adr & 0xfc00) | ((adr & 0x380) >> 7) | ((adr & 0x7f) << 3);
            }
            return adr;
        }

        private static int Get13Signed(int val)
        {
            if ((val & 0x1000) != 0)
            {
                return -(8192 - (val & 0xfff));
            }
            return val;
        }

       private static int Get16Signed(int val) 
       {
           if ((val & 0x8000) != 0)
           {
               return -(65536 - val);
           }
           return val;
        }

        private static int GetMultResult(int a, int b) {
            b = b < 0 ? 65536 + b : b;
            b >>= 8;
            b = (b & 0x80) > 0 ? -(256 - b) : b;
            int ans = a * b;
            if (ans < 0)
            {
                return 16777216 + ans;
            }
            return ans;
        }
    }
}