using Newtonsoft.Json;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.PictureProcessing
{
    public class PPU : IPPU
    {
        public int IniDisp { get; private set; }
        public int OAMAddress { get; set; }
        public int OAMReload { get; private set; }
        public int Stat77 { get; set; }
        public int Stat78 { get; set; }
        public int[] BackBuffer { get; private set; } = new int[57344];
        public byte[] VRAM { get; private set; } = new byte[65536];
        public int OpHorizontalCounter { get; set; }
        public int OpVerticalCounter { get; set; }


        private readonly int[] _pal = new int[256];
        private int BgCol;

        private class BackgroundStruct
        {
            public int SC;
            public int HorizontalOffset;
            public int VerticalOffset;
            public int HorizontalOffsetOld;
            public int VerticalOffsetOld;
            public int ChrBase;
        }

        private readonly int[,] _bppLUT = {
            { 2, 2, 2, 2 },
            { 4, 4, 2, 0 },
            { 4, 4, 0, 0 },
            { 8, 4, 0, 0 },
            { 8, 2, 0, 0 },
            { 4, 2, 0, 0 },
            { 4, 0, 0, 0 },
            { 8, 8, 0, 0 }
        };

        private readonly BackgroundStruct[] _backGrounds = {new BackgroundStruct(), new BackgroundStruct(), new BackgroundStruct(), new BackgroundStruct()};

        private int _objectSelection;
        private int _backgroundMode;
        private int _mosaic;
        private int _vmaIn;
        private int _vAddress;
        private int _m7Selection;
        private int _m7A;
        private int _m7B;
        private int _m7C;
        private int _m7D;
        private int _m7X;
        private int _m7Y;
        private int _m7H;
        private int _m7V;
        private int _cgAddress;
        private int _w12Selection;
        private int _w34Selection;
        private int _wObjectSelection;
        private int _wH0;
        private int _wH1;
        private int _wH2;
        private int _wH3;
        private int _wBgLog;
        private int _wObjectLog;
        private int _tm;
        private int _ts;
        private int _tmw;
        private int _tsw;
        private int _cgwSelection;
        private int _cgAdSub;
        private int _setIni;
        private int _multiply;

        private readonly byte[] CGRAM = new byte[512];
        private readonly byte[] OAM = new byte[544];

        private int _m7Old;
        private int _mode;
        private bool _hiRes;
        private bool _horizontalCtLoHi;
        private bool _verticalCtLoHi;
        private int _oamBuffer;
        private bool _oamPrimary;
        private int _vInc;
        private int _vPreFetch;
        private readonly int[] _secondaryScreen = new int[256];
        private readonly int[] _mainScreen = new int[256];
        private readonly int[] _secondaryZOrder = new int[256];
        private readonly int[] _mainZOrder = new int[256];
        private readonly bool[] _useMath = new bool[256];
        private int[] _brightLUT;

        [JsonIgnore]
        private ISNESSystem _system;

        public void SetSystem(ISNESSystem system)
        {
            _system = system;
        }

        public void Reset()
        {
            Stat77 = 1;
            Stat78 = 1;
            _brightLUT = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16 };
        }

        public void Render(int line)
        {
            for (int x = 0; x <= 255; x++)
            {
                _secondaryScreen[x] = BgCol;
                _mainScreen[x] = _pal[0];
                _secondaryZOrder[x] = 5;
                _mainZOrder[x] = 5;
                _useMath[x] = true;
            }
            switch (_mode)
            {
                case 0:
                    RenderCharacters(line, 3);
                    RenderBackground(line, 0, true);
                    RenderBackground(line, 1, true);
                    RenderCharacters(line, 2);
                    RenderBackground(line, 0);
                    RenderBackground(line, 1);
                    RenderCharacters(line, 1);
                    RenderBackground(line, 2, true);
                    RenderBackground(line, 3, true);
                    RenderCharacters(line, 0);
                    RenderBackground(line, 2);
                    RenderBackground(line, 3);
                    break;
                case 1:
                    if ((_backgroundMode & 8) != 0)
                    {
                        RenderBackground(line, 2, true);
                        RenderCharacters(line, 3);
                        RenderBackground(line, 0, true);
                        RenderBackground(line, 1, true);
                        RenderCharacters(line, 2);
                        RenderBackground(line, 0);
                        RenderBackground(line, 1);
                        RenderCharacters(line, 1);
                        RenderCharacters(line, 0);
                        RenderBackground(line, 2);
                    }
                    else
                    {
                        RenderCharacters(line, 3);
                        RenderBackground(line, 0, true);
                        RenderBackground(line, 1, true);
                        RenderCharacters(line, 2);
                        RenderBackground(line, 0);
                        RenderBackground(line, 1);
                        RenderCharacters(line, 1);
                        RenderBackground(line, 2, true);
                        RenderCharacters(line, 0);
                        RenderBackground(line, 2);
                    }
                    break;
                case object _ when 2 <= _mode && _mode <= 5:
                    RenderCharacters(line, 3);
                    RenderBackground(line, 0, true);
                    RenderCharacters(line, 2);
                    RenderBackground(line, 1, true);
                    RenderCharacters(line, 1);
                    RenderBackground(line, 0);
                    RenderCharacters(line, 0);
                    RenderBackground(line, 1);
                    break;
                case 6:
                    RenderCharacters(line, 3);
                    RenderBackground(line, 0, true);
                    RenderCharacters(line, 2);
                    RenderCharacters(line, 1);
                    RenderBackground(line, 0);
                    RenderCharacters(line, 0);
                    break;
                case 7:
                    RenderCharacters(line, 3);
                    RenderCharacters(line, 2);
                    RenderBackground(line, 1, true);
                    RenderCharacters(line, 1);
                    RenderBackground(line, 0);
                    RenderCharacters(line, 0);
                    RenderBackground(line, 1);
                    break;
            }
            RenderBuffer(line);
        }

        public int Read8(int address)
        {
            var returnVal = 0;

            switch (address)
            {
                case 0x2134:
                    returnVal = _multiply & 0xFF;
                    break;
                case 0x2135:
                    returnVal = (_multiply >> 8) & 0xFF;
                    break;
                case 0x2136:
                    returnVal = (_multiply >> 16) & 0xFF;
                    break;
                case 0x2137:
                    OpHorizontalCounter = _system.PPUDot;
                    OpVerticalCounter = _system.ScanLine;
                    break;
                case 0x2138:
                    returnVal = OAM[OAMAddress];
                    if ((OAMAddress & 0x200) != 0)
                    {
                        OAMAddress = ((OAMAddress + 1) & 0x1F) | 0x200;
                    }
                    else
                    {
                        OAMAddress += 1;
                    }
                    break;
                case 0x2139:
                    returnVal = _vPreFetch & 0xFF;
                    if (_vmaIn >> 7 == 0)
                    {
                        PreFetch();
                        _vAddress = (_vAddress + _vInc) & 0x7FFF;
                    }
                    break;
                case 0x213A:
                    returnVal = _vPreFetch >> 8;
                    if (_vmaIn >> 7 != 0)
                    {
                        PreFetch();
                        _vAddress = (_vAddress + _vInc) & 0x7FFF;
                    }
                    break;
                case 0x213B:
                    returnVal = CGRAM[_cgAddress];
                    _cgAddress = (_cgAddress + 1) & 0x1FF;
                    break;
                case 0x213C:
                    if (_horizontalCtLoHi)
                    {
                        returnVal = OpHorizontalCounter >> 8;
                    }
                    else
                    {
                        returnVal = OpHorizontalCounter & 0xFF;
                    }
                    _horizontalCtLoHi = !_horizontalCtLoHi;
                    break;
                case 0x213D:
                    if (_verticalCtLoHi)
                    {
                        returnVal = OpVerticalCounter >> 8;
                    }
                    else
                    {
                        returnVal = OpVerticalCounter & 0xFF;
                    }
                    _verticalCtLoHi = !_verticalCtLoHi;
                    break;
                case 0x213E:
                    returnVal = Stat77;
                    break;
                case 0x213F:
                    returnVal = Stat78;
                    _horizontalCtLoHi = false;
                    _verticalCtLoHi = false;
                    Stat78 &= ~0x40;
                    break;
            }
            return returnVal;
        }

        public void Write8(int address, int value)
        {
            //counter++;
            //Analyzer.WriteLine("PPU Write8:" + counter + ":" +  + address + ":" + value);
            switch (address)
            {
                case 0x2100:
                    if (((IniDisp ^ value) & 0x80) != 0 && (value & 0x80) == 0)
                    {
                        OAMAddress = OAMReload;
                    }
                    IniDisp = value;
                    break;
                case 0x2101:
                    _objectSelection = value;
                    break;
                case 0x2102:
                    OAMReload = (value << 1) | (OAMReload & 0x200);
                    OAMAddress = OAMReload;
                    break;
                case 0x2103:
                    OAMReload = ((value & 1) << 9) | (OAMReload & 0x1FE);
                    OAMAddress = OAMReload;
                    _oamPrimary = (value & 0x80) != 0;
                    break;
                case 0x2104:
                    if ((OAMAddress & 0x200) == 0)
                    {
                        if ((OAMAddress & 1) != 0)
                        {
                            OAM[OAMAddress - 1] = (byte) _oamBuffer;
                            OAM[OAMAddress] = (byte) value;
                        }
                    }
                    else
                    {
                        OAM[OAMAddress] = (byte) value;
                    }
                    if ((OAMAddress & 1) == 0)
                    {
                        _oamBuffer = value;
                    }
                    if ((OAMAddress & 0x200) != 0)
                    {
                        OAMAddress = ((OAMAddress + 1) & 0x1F) | 0x200;
                    }
                    else
                    {
                        OAMAddress += 1;
                    }
                    break;
                case 0x2105:
                    _backgroundMode = value;
                    _mode = value & 7;
                    _hiRes = _mode == 5 | _mode == 6;
                    break;
                case 0x2106:
                    _mosaic = value;
                    break;
                case 0x2107:
                    _backGrounds[0].SC = value;
                    break;
                case 0x2108:
                    _backGrounds[1].SC = value;
                    break;
                case 0x2109:
                    _backGrounds[2].SC = value;
                    break;
                case 0x210A:
                    _backGrounds[3].SC = value;
                    break;
                case 0x210B:
                    _backGrounds[0].ChrBase = (value & 0xF) << 13;
                    _backGrounds[1].ChrBase = (value >> 4) << 13;
                    break;
                case 0x210C:
                    _backGrounds[2].ChrBase = (value & 0xF) << 13;
                    _backGrounds[3].ChrBase = (value >> 4) << 13;
                    break;
                case 0x210D:
                    _m7H = _m7Old | (value << 8);
                    _m7Old = value;
                    BackgroundStruct backGround = _backGrounds[0];
                    backGround.HorizontalOffset = (value << 8) | (backGround.HorizontalOffsetOld & ~7) | ((backGround.HorizontalOffset >> 8) & 7);
                    backGround.HorizontalOffsetOld = value;
                    break;
                case 0x210E:
                    _m7V = _m7Old | (value << 8);
                    _m7Old = value;
                    backGround = _backGrounds[0];
                    backGround.VerticalOffset = (value << 8) | backGround.VerticalOffsetOld;
                    backGround.VerticalOffsetOld = value;
                    break;
                case 0x210F:
                    backGround = _backGrounds[1];
                    backGround.HorizontalOffset = (value << 8) | (backGround.HorizontalOffsetOld & ~7) | ((backGround.HorizontalOffset >> 8) & 7);
                    backGround.HorizontalOffsetOld = value;
                    break;
                case 0x2110:
                    backGround = _backGrounds[1];
                    backGround.VerticalOffset = (value << 8) | backGround.VerticalOffsetOld;
                    backGround.VerticalOffsetOld = value;
                    break;
                case 0x2111:
                    backGround = _backGrounds[2];
                    backGround.HorizontalOffset = (value << 8) | (backGround.HorizontalOffsetOld & ~7) | ((backGround.HorizontalOffset >> 8) & 7);
                    backGround.HorizontalOffsetOld = value;
                    break;
                case 0x2112:
                    backGround = _backGrounds[2];
                    backGround.VerticalOffset = (value << 8) | backGround.VerticalOffsetOld;
                    backGround.VerticalOffsetOld = value;
                    break;
                case 0x2113:
                    backGround = _backGrounds[3];
                    backGround.HorizontalOffset = (value << 8) | (backGround.HorizontalOffsetOld & ~7) | ((backGround.HorizontalOffset >> 8) & 7);
                    backGround.HorizontalOffsetOld = value;
                    break;
                case 0x2114:
                    backGround = _backGrounds[3];
                    backGround.VerticalOffset = (value << 8) | backGround.VerticalOffsetOld;
                    backGround.VerticalOffsetOld = value;
                    break;
                case 0x2115:
                    _vmaIn = value;
                    switch (_vmaIn & 3)
                    {
                        case 0:
                            _vInc = 1;
                            break;
                        case 1:
                            _vInc = 32;
                            break;
                        case 2:
                        case 3:
                            _vInc = 128;
                            break;
                    }
                    break;
                case 0x2116:
                    _vAddress = value | (_vAddress & 0xFF00); 
                    PreFetch();
                    break;
                case 0x2117:
                    _vAddress = (value << 8) | (_vAddress & 0xFF); 
                    PreFetch();
                    break;
                case 0x2118:
                    int addr = TransVAddr();
                    if (_vmaIn >> 7 == 0)
                    {
                        _vAddress = (_vAddress + _vInc) & 0x7FFF;
                    }
                    VRAM[addr << 1] = (byte) value;
                    break;
                case 0x2119:
                    addr = TransVAddr();
                    if (_vmaIn >> 7 != 0)
                    {
                        _vAddress = (_vAddress + _vInc) & 0x7FFF;
                    }
                    VRAM[(addr << 1) | 1] = (byte) value;
                    break;
                case 0x211A:
                    _m7Selection = value;
                    break;
                case object _ when 0x211B <= address && address <= 0x2120:
                    int value16 = _m7Old | (value << 8);
                    _m7Old = value;

                    switch (address)
                    {
                        case 0x211B:
                            _m7A = value16;
                            _multiply = Sign16(_m7A) * Sign8(_m7B);
                            break;
                        case 0x211C:
                            _m7B = value16; 
                            _multiply = Sign16(_m7A) * Sign8(_m7B >> 8);
                            break;
                        case 0x211D:
                            _m7C = value16;
                            break;
                        case 0x211E:
                            _m7D = value16;
                            break;
                        case 0x211F:
                            _m7X = value16;
                            break;
                        case 0x2120:
                            _m7Y = value16;
                            break;
                    }
                    break;
                case 0x2121:
                    _cgAddress = value << 1;
                    break;
                case 0x2122:
                    CGRAM[_cgAddress] = (byte) value;
                    int wAddr = _cgAddress & 0x1FE;
                    _pal[_cgAddress >> 1] = 0;
                    _pal[_cgAddress >> 1] = _pal[_cgAddress >> 1] | (CGRAM[wAddr & 0x1FE] & 0x1F) << 19;
                    _pal[_cgAddress >> 1] = _pal[_cgAddress >> 1] | (((CGRAM[wAddr] & 0xE0) >> 2) | ((CGRAM[wAddr + 1] & 3) << 6)) << 8;
                    _pal[_cgAddress >> 1] = _pal[_cgAddress >> 1] | (CGRAM[wAddr + 1] & 0x7C) << 1;
                    _cgAddress = (_cgAddress + 1) & 0x1FF;
                    break;
                case 0x2123:
                    _w12Selection = value;
                    break;
                case 0x2124:
                    _w34Selection = value;
                    break;
                case 0x2125:
                    _wObjectSelection = value;
                    break;
                case 0x2126:
                    _wH0 = value;
                    break;
                case 0x2127:
                    _wH1 = value;
                    break;
                case 0x2128:
                    _wH2 = value;
                    break;
                case 0x2129:
                    _wH3 = value;
                    break;
                case 0x212A:
                    _wBgLog = value;
                    break;
                case 0x212B:
                    _wObjectLog = value;
                    break;
                case 0x212C:
                    _tm = value;
                    break;
                case 0x212D:
                    _ts = value;
                    break;
                case 0x212E:
                    _tmw = value;
                    break;
                case 0x212F:
                    _tsw = value;
                    break;
                case 0x2130:
                    _cgwSelection = value;
                    break;
                case 0x2131:
                    _cgAdSub = value;
                    break;
                case 0x2132:
                    if ((value & 0x20) != 0)
                    {
                        BgCol = (BgCol & 0xFFFF) | (value & 0x1F) << 19;
                    }
                    if ((value & 0x40) != 0)
                    {
                        BgCol = (BgCol & 0xFF00FF) | (value & 0x1F) << 11;
                    }
                    if ((value & 0x80) != 0)
                    {
                        BgCol = (BgCol & 0xFFFF00) | (value & 0x1F) << 3;
                    }
                    break;
                case 0x2133:
                    _setIni = value;
                    break;
            }
        }

        private void RenderBackground(int line, int layer, bool foreGround = false)
        {
            if (((_tm | _ts) & (1 << layer)) != 0)
            {
                int bpp = _bppLUT[_mode, layer];
                int bppLSh = 0;
                switch (bpp)
                {
                    case 2:
                        bppLSh = 4;
                        break;
                    case 4:
                        bppLSh = 5;
                        break;
                    case 8:
                        bppLSh = 6;
                        break;
                }
                int mosCounter = 0;
                int mosSize = (_mosaic >> 4) + 1;
                bool mosEnabled = mosSize != 1 && (_mosaic & (1 << layer)) != 0;
                if (mosEnabled)
                {
                    line = line / mosSize * mosSize;
                }
                int mcgNum = 0;
                int mPalCol = 0;
                BackgroundStruct backGround = _backGrounds[layer];
                if (_mode == 7)
                {
                    int scrnOver = _m7Selection >> 6;
                    int a = Sign16(_m7A);
                    int b = Sign16(_m7B);
                    int c = Sign16(_m7C);
                    int d = Sign16(_m7D);
                    int x = Sign13(_m7X);
                    int y = Sign13(_m7Y);
                    int h = Sign13(_m7H);
                    int v = Sign13(_m7V);
                    int startX = 0;
                    int startY = line;
                    if ((_m7Selection & 1) != 0)
                    {
                        startX = 0xFF;
                    }
                    if ((_m7Selection & 2) != 0)
                    {
                        startY ^= 0xFF;
                    }
                    int a2 = (a * Clip10(h - x)) & ~0x3F;
                    int b2 = (b * Clip10(v - y)) & ~0x3F;
                    int c2 = (c * Clip10(h - x)) & ~0x3F;
                    int d2 = (d * Clip10(v - y)) & ~0x3F;
                    int bmpX = a2 + b2 + a * startX + ((b * startY) & ~0x3F) + (x << 8);
                    int bmpY = c2 + d2 + c * startX + ((d * startY) & ~0x3F) + (y << 8);
                    for (int scrnX = startX; scrnX <= (startX ^ 0xFF); scrnX++)
                    {
                        bool xOver = (bmpX & ~0x3FFFF) != 0;
                        bool yOver = (bmpY & ~0x3FFFF) != 0;
                        int tmx = (bmpX >> 11) & 0x7F;
                        int tmy = (bmpY >> 11) & 0x7F;
                        int pixelX = (bmpX >> 8) & 7;
                        int pixelY = (bmpY >> 8) & 7;
                        if (mosEnabled && mosCounter % mosSize == 0 || !mosEnabled)
                        {
                            if ((_m7Selection & 1) != 0)
                            {
                                bmpX -= a;
                                bmpY -= c;
                            }
                            else
                            {
                                bmpX += a;
                                bmpY += c;
                            }
                            if (mosEnabled)
                            {
                                mosCounter += 1;
                            }
                        }
                        int chrAddr = VRAM[(tmx + (tmy << 7)) << 1] * 128;
                        if (xOver || yOver)
                        {
                            if (scrnOver == 2)
                            {
                                continue;
                            }
                            if (scrnOver == 3)
                            {
                                chrAddr = 0;
                            }
                        }
                        byte color = VRAM[chrAddr + 1 + (pixelY << 4) + (pixelX << 1)];
                        bool pri = (color & 0x80) != 0;
                        if (layer == 0 || (_setIni & 0x40) != 0 && layer == 1 && pri == foreGround)
                        {
                            DrawPixel(layer, scrnX ^ startX, layer == 1 ? _pal[color & 0x7F] : _pal[color]);
                        }
                    }
                }
                else
                {
                    int tmBase = (backGround.SC & 0xFC) << 9;
                    bool t16 = (_backgroundMode & (0x10 << layer)) != 0;
                    int positionTileBit = t16 ? 9 : 8;
                    int startTileBits = t16 ? 4 : 3;
                    int horizontalMask = t16 ? 0xF : 7;
                    int tileMask = 0x1F << startTileBits;
                    int tileDimensions = t16 ? 16 : 8;
                    int tileAmount = t16 ? 16 : 32;
                    for (int tileIndex = 0; tileIndex <= tileAmount; tileIndex++)
                    {
                        int tileX = tileIndex;
                        int horizontalOffset = backGround.HorizontalOffset;
                        int y = line + backGround.VerticalOffset;
                        if (_hiRes)
                        {
                            horizontalOffset <<= 1;
                        }
                        if (_mode != 0 && (_mode & 1) == 0)
                        {
                            if (tileIndex != 0)
                            {
                                if (_mode == 4)
                                {
                                    int offset = GetBg3Tile((tileIndex - 1) * tileDimensions, 0);
                                    if ((offset & 0x8000) == 0)
                                    {
                                        tileX = tileIndex + (offset >> 3);
                                        horizontalOffset &= horizontalMask;
                                    }
                                    else
                                    {
                                        y = line + offset;
                                    }
                                }
                                else
                                {
                                    int maxTile = (tileIndex - 1) * tileDimensions;
                                    int optHorizontalOffset = GetBg3Tile(maxTile, 0);
                                    int optVerticalOffset = GetBg3Tile(maxTile, tileDimensions);
                                    if ((optHorizontalOffset & (0x2000 << layer)) != 0)
                                    {
                                        tileX = tileIndex + (optHorizontalOffset >> 3);
                                        horizontalOffset &= horizontalMask;
                                    }
                                    if ((optVerticalOffset & (0x2000 << layer)) != 0)
                                    {
                                        y = line + optVerticalOffset;
                                    }
                                }
                            }
                        }
                        int tmy = (y & tileMask) >> startTileBits;
                        int tmAddr = tmBase + (tmy << 6);
                        int tmsx = (tileX << startTileBits) + horizontalOffset;
                        int tmx = (tmsx & tileMask) >> startTileBits;
                        int tmsy = (y >> positionTileBit) & 1;
                        tmAddr += tmx << 1;
                        tmsx = (tmsx >> positionTileBit) & 1;
                        switch (backGround.SC & 3)
                        {
                            case 1:
                                tmAddr += tmsx << 11;
                                break;
                            case 2:
                                tmAddr += tmsy << 11;
                                break;
                            case 3:
                                tmAddr += (tmsx << 11) + (tmsy << 12);
                                break;
                        }
                        tmAddr &= 0xFFFE;
                        bool pri = (VRAM[tmAddr + 1] & 0x20) != 0;
                        if (pri == foreGround)
                        {
                            int verticalLow = VRAM[tmAddr];
                            int verticalHigh = VRAM[tmAddr + 1];
                            int tile = verticalLow | (verticalHigh << 8);
                            int chrNum = tile & 0x3FF;
                            int cgNum = ((tile & 0x1C00) >> 10) << bpp;
                            bool horizontalFlip = (tile & 0x4000) != 0;
                            bool verticalFlip = (tile & 0x8000) != 0;
                            int yOffset = y & 7;
                            if (verticalFlip)
                            {
                                yOffset ^= 7;
                            }
                            if (t16)
                            {
                                if (verticalFlip)
                                {
                                    if ((y & 8) == 0)
                                    {
                                        chrNum += 0x10;
                                    }
                                }
                                else if ((y & 8) != 0)
                                {
                                    chrNum += 0x10;
                                }
                            }
                            int chrAddr = (backGround.ChrBase + (chrNum << bppLSh) + (yOffset << 1)) & 0xFFFF;
                            for (int tbx = 0; tbx <= tileDimensions - 1; tbx += 8)
                            {
                                for (int x = 0; x <= 7; x++)
                                {
                                    int xBit = x;
                                    int tbxOffset = tbx;
                                    if (horizontalFlip)
                                    {
                                        xBit ^= 7;
                                    }
                                    if (horizontalFlip && t16)
                                    {
                                        tbxOffset ^= 8;
                                    }
                                    byte palCol = ReadChr(chrAddr, bpp, xBit);
                                    if (mosEnabled)
                                    {
                                        if (mosCounter % mosSize != 0)
                                        {
                                            palCol = (byte)mPalCol;
                                            cgNum = mcgNum;
                                        }
                                        else
                                        {
                                            mPalCol = palCol;
                                            mcgNum = cgNum;
                                        }
                                        mosCounter += 1;
                                    }
                                    if (palCol != 0)
                                    {
                                        int color = (cgNum + palCol) & 0xFF;
                                        int offset = tileIndex * tileDimensions + x + tbxOffset - (horizontalOffset & horizontalMask);
                                        if (offset >= 256)
                                        {
                                            break;
                                        }
                                        if (offset < 0)
                                        {
                                            continue;
                                        }
                                        DrawPixel(layer, offset, _pal[color]);
                                    }
                                }
                                chrAddr += bpp << 3;
                            }
                        }
                    }
                }
            }
        }

        private void RenderBuffer(int line)
        {
            int baseLine = (line - 1) * 256;
            if ((IniDisp & 0x80) == 0)
            {
                int bright = IniDisp & 0xF;
                for (int x = 0; x <= 255; x++)
                {
                    int w1Sel = (_wObjectSelection >> 4) & 3;
                    int w2Sel = (_wObjectSelection >> 6) & 3;
                    int maskLog = (_wObjectLog >> 2) & 3;
                    bool doMath = false;
                    bool isBlack = false;
                    switch ((_cgwSelection >> 4) & 3)
                    {
                        case 0:
                            doMath = true;
                            break;
                        case 1:
                        case 2:
                            doMath = GetWindow(x, w1Sel, w2Sel, maskLog);
                            if (((_cgwSelection >> 4) & 3) == 2)
                            {
                                doMath = !doMath;
                            }
                            break;
                    }
                    switch ((_cgwSelection >> 6) & 3)
                    {
                        case 1:
                        case 2:
                            isBlack = GetWindow(x, w1Sel, w2Sel, maskLog);
                            if (((_cgwSelection >> 6) & 3) == 1)
                            {
                                isBlack = !isBlack;
                            }
                            break;
                        case 3:
                            isBlack = true;
                            break;
                    }
                    if (isBlack)
                    {
                        _mainScreen[x] = 0;
                    }
                    int color;
                    if (doMath && _useMath[x] && (_cgAdSub & (1 << _mainZOrder[x])) != 0)
                    {
                        int mCol = (_cgwSelection & 2) != 0 ? _secondaryScreen[x] : BgCol;
                        bool div2 = (_cgAdSub & 0x40) != 0 && !(_secondaryZOrder[x] == 5 || isBlack);
                        int R;
                        int G;
                        int B;
                        if ((_cgAdSub & 0x80) != 0)
                        {
                            R = (((_mainScreen[x] >> 0) & 0xFF) - ((mCol >> 0) & 0xFF)) >> (div2 ? 1 : 0);
                            G = (((_mainScreen[x] >> 8) & 0xFF) - ((mCol >> 8) & 0xFF)) >> (div2 ? 1 : 0);
                            B = (((_mainScreen[x] >> 16) & 0xFF) - ((mCol >> 16) & 0xFF)) >> (div2 ? 1 : 0);
                        }
                        else
                        {
                            R = (((_mainScreen[x] >> 0) & 0xFF) + ((mCol >> 0) & 0xFF)) >> (div2 ? 1 : 0);
                            G = (((_mainScreen[x] >> 8) & 0xFF) + ((mCol >> 8) & 0xFF)) >> (div2 ? 1 : 0);
                            B = (((_mainScreen[x] >> 16) & 0xFF) + ((mCol >> 16) & 0xFF)) >> (div2 ? 1 : 0);
                        }
                        color = ClampU8(R) | (ClampU8(G) << 8) | (ClampU8(B) << 16);
                    }
                    else
                    {
                        color = _mainScreen[x];
                    }
                    if (bright < 0xF)
                    {
                        int R = (color >> 0) & 0xFF;
                        int G = (color >> 8) & 0xFF;
                        int B = (color >> 16) & 0xFF;
                        R = (R * _brightLUT[bright]) >> 4;
                        G = (G * _brightLUT[bright]) >> 4;
                        B = (B * _brightLUT[bright]) >> 4;
                        color = R | (G << 8) | (B << 16);
                    }
                    BackBuffer[baseLine + x] = (int) (color | 0xFF000000);
                }
            }
            else
            {
                for (int x = 0; x <= 255; x++)
                {
                    unchecked
                    {
                        BackBuffer[baseLine + x] = (int) 0xFF000000;
                    }
                }
            }
        }

        private bool GetWindow(int x, int w1Sel, int w2Sel, int maskLog)
        {
            bool w1Val = false;
            bool w2Val = false;
            switch (w1Sel)
            {
                case 2:
                    w1Val = x >= _wH0 && x <= _wH1;
                    break;
                case 3:
                    w1Val = x < _wH0 || x > _wH1; 
                    break;
            }
            switch (w2Sel)
            {
                case 2:
                    w2Val = x >= _wH2 && x <= _wH3;
                    break;
                case 3:
                    w2Val = x < _wH2 || x > _wH3;
                    break;
            }
            bool wVal = false;
            if (w1Sel > 1 && w2Sel > 1)
            {
                switch (maskLog)
                {
                    case 0:
                        wVal = w1Val || w2Val;
                        break;
                    case 1:
                        wVal = w1Val && w2Val;
                        break;
                    case 2:
                        wVal = w1Val ^ w2Val;
                        break;
                    case 3:
                        wVal = w1Val ^ w2Val ^ true;
                        break;
                }
            }
            else if (w1Sel > 1)
            {
                wVal = w1Val;
            }
            else
            {
                wVal = w2Val;
            }
            return wVal;
        }

        private static int ClampU8(int value)
        {
            if (value < 0)
            {
                return 0;
            }
            if (value > 0xFF)
            {
                return 0xFF;
            }
            return value;
        }

        private void DrawPixel(int layer, int x, int color, bool math = true)
        {
            bool sEnabled = true;
            bool mEnabled = true;
            if (((_tmw | _tsw) & (1 << layer)) != 0)
            {
                int w1Sel = 0;
                int w2Sel = 0;
                int maskLog = 0;
                switch (layer)
                {
                    case 0 :
                        w1Sel = (_w12Selection >> 0) & 3;
                        w2Sel = (_w12Selection >> 2) & 3;
                        maskLog = (_wBgLog >> 0) & 3;
                        break;
                    case 1 :
                        w1Sel = (_w12Selection >> 4) & 3; 
                        w2Sel = (_w12Selection >> 6) & 3;
                        maskLog = (_wBgLog >> 2) & 3;
                        break;
                    case 2 :
                        w1Sel = (_w34Selection >> 0) & 3;
                        w2Sel = (_w34Selection >> 2) & 3;
                        maskLog = (_wBgLog >> 4) & 3;
                        break;
                    case 3 :
                        w1Sel = (_w34Selection >> 4) & 3;
                        w2Sel = (_w34Selection >> 6) & 3;
                        maskLog = (_wBgLog >> 6) & 3;
                        break;
                    case 4 :
                        w1Sel = (_wObjectSelection >> 0) & 3;
                        w2Sel = (_wObjectSelection >> 2) & 3;
                        maskLog = (_wObjectLog >> 0) & 3;
                        break;
                }
                bool disable = GetWindow(x, w1Sel, w2Sel, maskLog);
                if ((_tsw & (1 << layer)) != 0)
                {
                    sEnabled = !disable;
                }
                if ((_tmw & (1 << layer)) != 0)
                {
                    mEnabled = !disable;
                }
            }
            bool colorMath = math && mEnabled;
            sEnabled = sEnabled && (_ts & (1 << layer)) != 0 && _secondaryZOrder[x] == 5;
            mEnabled = mEnabled && (_tm & (1 << layer)) != 0 && _mainZOrder[x] == 5;
            if (sEnabled)
            {
                _secondaryScreen[x] = color;
                _secondaryZOrder[x] = layer;
            }
            if (mEnabled)
            {
                _mainScreen[x] = color;
                _mainZOrder[x] = layer;
            }
            if (mEnabled)
            {
                _useMath[x] = colorMath;
            }
        }

        private int GetBg3Tile(int x, int y)
        {
            BackgroundStruct backGround = _backGrounds[2];
            bool t16 = (_backgroundMode & 0x40) != 0;
            int tmBase = (backGround.SC & 0xFC) << 9;
            int horizontalMask = t16 ? 0xF : 7;
            int sBit = t16 ? 9 : 8;
            int tBits = t16 ? 4 : 3;
            int tMask = 0x1F << tBits;
            int tmy = ((y + backGround.VerticalOffset) & tMask) >> tBits;
            int tmAddress = tmBase + (tmy << 6);
            int tmsx = x + (backGround.HorizontalOffset & ~horizontalMask);
            int tmx = (tmsx & tMask) >> tBits;
            int tmsy = ((y + backGround.VerticalOffset) >> sBit) & 1;
            tmAddress += tmx << 1;
            tmsx = (tmsx >> sBit) & 1;
            switch (backGround.SC & 3)
            {
                case 1:
                    tmAddress += tmsx << 11;
                    break;
                case 2:
                    tmAddress += tmsy << 11;
                    break;
                case 3:
                    tmAddress += (tmsx << 11) + (tmsy << 12);
                    break;
            }
            tmAddress &= 0xFFFE;
            int vl = VRAM[tmAddress];
            int vh = VRAM[tmAddress + 1];
            return vl | (vh << 8);
        }

        private byte ReadChr(int address, int bpp, int x)
        {
            byte color = 0;
            int bit = 0x80 >> x;
            if ((VRAM[address] & bit) != 0)
            {
                color |= 0x1;
            }
            if ((VRAM[address + 1] & bit) != 0)
            {
                color |= 0x2;
            }
            if (bpp != 2)
            {
                if ((VRAM[address + 16] & bit) != 0)
                {
                    color |= 0x4;
                }
                if ((VRAM[address + 17] & bit) != 0)
                {
                    color |= 0x8;
                }
                if (bpp == 8)
                {
                    if ((VRAM[address + 32] & bit) != 0)
                    {
                        color |= 0x10;
                    }
                    if ((VRAM[address + 33] & bit) != 0)
                    {
                        color |= 0x20;
                    }
                    if ((VRAM[address + 48] & bit) != 0)
                    {
                        color |= 0x40;
                    }
                    if ((VRAM[address + 49] & bit) != 0)
                    {
                        color |= 0x80;
                    }
                }
            }
            return color;
        }

        private static int Clip10(int value)
        {
            if ((value & 0x2000) != 0)
            {
                return value | ~0x3FF;
            }
            return value & 0x3FF;
        }

        private void RenderCharacters(int line, int pri)
        {
            line -= 1;
            int rOverCounter = 0;
            int tOverCounter = 0; 
            int characterBase = (_objectSelection & 3) << 14;
            int offset;
            if (_oamPrimary)
            {
                offset = (((OAMAddress >> 2) - 1) & 0x7F) << 2;
            }
            else
            {
                offset = 0;
            }
            for (int i = 0; i <= 0x7F; i++)
            {
                int x = OAM[offset];
                int y = OAM[offset + 1];
                int characterNum = OAM[offset + 2];
                int attribute = OAM[offset + 3];
                int oam32Address = 0x200 + (offset >> 4);
                int oam32Bit = (offset & 0xC) >> 1;
                offset = (offset + 4) & 0x1FF;
                if (y >= 224)
                {
                    y = (int) (y | 0xFFFFFF00);
                }
                if (line < y)
                {
                    continue;
                }
                characterNum |= (attribute & 1) << 8;
                int palNum = (attribute >> 1) & 7;
                int priority = (attribute >> 4) & 3;
                bool horizontalFlip = (attribute & 0x40) != 0;
                bool verticalFlip = (attribute & 0x80) != 0;
                if (priority == pri)
                {
                    bool tSize = (OAM[oam32Address] & (1 << (oam32Bit + 1))) != 0;
                    bool xHigh = (OAM[oam32Address] & (1 << oam32Bit)) != 0;
                    if (xHigh)
                    {
                        x = (int) (x | 0xFFFFFF00);
                    }
                    int tx = 0;
                    int ty = 0;
                    switch (_objectSelection >> 5)
                    {
                        case 0:
                            if (tSize)
                            {
                                tx = 1;
                                ty = 1;
                            }
                            else
                            {
                                tx = 0;
                                ty = 0;
                            }
                            break;
                        case 1:
                            if (tSize)
                            {
                                tx = 3; 
                                ty = 3;
                            }
                            else
                            {
                                tx = 0;
                                ty = 0;
                            }
                            break;
                        case 2:
                            if (tSize)
                            {
                                tx = 7;
                                ty = 7;
                            }
                            else
                            {
                                tx = 0;
                                ty = 0;
                            }
                            break;
                        case 3:
                            if (tSize)
                            {
                                tx = 3;
                                ty = 3;
                            }
                            else
                            {
                                tx = 1; 
                                ty = 1;
                            }
                            break;
                        case 4:
                            if (tSize)
                            {
                                tx = 7;
                                ty = 7;
                            }
                            else
                            {
                                tx = 1;
                                ty = 1;
                            }
                            break;
                        case 5:
                            if (tSize)
                            {
                                tx = 7;
                                ty = 7;
                            }
                            else
                            {
                                tx = 3;
                                ty = 3;
                            }
                            break;
                        case 6:
                            if (tSize)
                            {
                                tx = 3;
                                ty = 7;
                            }
                            else
                            {
                                tx = 1;
                                ty = 3;
                            }
                            break;
                        case 7:
                            if (tSize)
                            {
                                tx = 3;
                                ty = 3;
                            }
                            else
                            {
                                tx = 1;
                                ty = 3;
                            }
                            break;
                    }
                    bool xOffsetScreen = x >= 256 || x + ((tx + 1) << 3) < 0;
                    bool YOffsetLine = line >= y + ((ty + 1) << 3);
                    if (xOffsetScreen || YOffsetLine)
                    {
                        continue;
                    }
                    rOverCounter += 1;
                    tOverCounter += tx + 1;
                    if (tOverCounter > 34 | rOverCounter > 32)
                    {
                        break;
                    }
                    if (((_tm | _ts) & 0x10) != 0)
                    {
                        int yPos = line - y;
                        if (verticalFlip)
                        {
                            yPos ^= ((ty + 1) << 3) - 1;
                        }
                        int tileY = yPos >> 3;
                        int yOffset = yPos & 7;
                        if (horizontalFlip)
                        {
                            x += tx << 3;
                        }
                        for (int tileX = 0; tileX <= tx; tileX++)
                        {
                            int characterAddress = characterBase + (yOffset << 1) + ((characterNum + (tileY << 4) + tileX) << 5);
                            for (int xOffset = 0; xOffset <= 7; xOffset++)
                            {
                                int posX = x + xOffset;
                                if (posX < 0)
                                {
                                    continue;
                                }
                                if (posX > 255)
                                {
                                    break;
                                }
                                int xBit = xOffset;
                                if (horizontalFlip)
                                {
                                    xBit ^= 7;
                                }
                                byte color = ReadChr(characterAddress, 4, xBit);
                                if (color != 0)
                                {
                                    DrawPixel(4, posX, _pal[0x80 | (palNum << 4) | color], palNum > 3);
                                }
                            }
                            if (horizontalFlip)
                            {
                                x -= 8;
                            }
                            else
                            {
                                x += 8;
                            }
                        }
                    }
                }
            }
            if (tOverCounter > 34)
            {
                Stat77 |= 0x80;
            }
            if (rOverCounter > 32)
            {
                Stat77 |= 0x40;
            }
        }

        private void PreFetch()
        {
            int addr = TransVAddr();
            int low = VRAM[addr << 1];
            int high = VRAM[(addr << 1) | 1];
            _vPreFetch = low | (high << 8);
        }

        private int TransVAddr()
        {
            int addr = _vAddress;
            switch ((_vmaIn & 0xC) >> 2)
            {
                case 1:
                    addr = ROL3(addr & 0xFF, 8) | (addr & 0xFF00);
                    break;
                case 2:
                    addr = ROL3(addr & 0x1FF, 9) | (addr & 0xFE00);
                    break;
                case 3:
                    addr = ROL3(addr & 0x3FF, 10) | (addr & 0xFC00);
                    break;
            }
            return addr & 0x7FFF;
        }

        private static int ROL3(int Value, int Bits)
        {
            return ((Value << 3) | (Value >> (Bits - 3))) & ((1 << Bits) - 1);
        }

        // Integer Sign
        private static int Sign8(int value)
        {
            if ((value & 0x80) != 0)
            {
                return (int) (value | 0xFFFFFF00);
            }
            return value;
        }

        private static int Sign13(int value)
        {
            if ((value & 0x1000) != 0)
            {
                return (int) (value | 0xFFFFE000);
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
    }
}