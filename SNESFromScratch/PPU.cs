using System;
using System.Drawing;
using System.Windows.Forms;

namespace SNESFromScratch
{
    public class PPU
    {
        private struct ColorPalette
        {
            public byte R;
            public byte G;
            public byte B;
        }

        private struct PPUBackground
        {
            public int Address;
            public int CHRAddress;
            public byte Size;
            public bool Tile16X16;
            public int HScroll, VScroll;
            public bool HLowHighToggle, VLowHighToggle;
            public bool Mosaic;
        }

        private readonly ColorPalette[] _palette = new ColorPalette[256];
        private readonly PPUBackground[] _background = new PPUBackground[4];
        private byte _bgMainEnabled;
        private byte _bgSubEnabled;
        private int _palAddress;
        private byte _ppuMode;
        private bool _bg3Priority;

        private Bitmap img;
        private byte _colorMathEnable;
        private bool _colorMathObj;
        private bool _colorMathAddSub;
        private bool _colorMathDiv2;
        private byte _colorMathBGs;
        private ColorPalette _fixedColor;

        private int _objSize, _objName, _objChrOffset;
        public int ObjRAMAddress, ObjRAMFirstAddress;
        public byte[] ObjRAM = new byte[0x220];
        private bool _objLowHighToggle, _firstReadObj;

        private int _vramAddress, _vramIncrement;
        private bool _increment2119213A;
        private bool _firstReadVram;

        private int _vLatch;

        private int _mode7Multiplicand, _mode7Multiplier;
        private byte _mode7C, _mode7D, _mode7X, _mode7Y;
        private bool _mode7LowHigh;
        private int _multResult;

        private byte _mosaicSize;

        public byte[] VRAM = new byte[0x10000];
        private readonly byte[] _cgRAM = new byte[0x200];

        private bool _screenEnabled;

        private readonly int[] _videoBuffer = new int[(256*224)];
        private readonly int[]  _videoBufferSub = new int[(256*224)];
        public int[] PowerOf2 = new int[32];

        public bool TakeScreenshot;

        public C65816 C65816;
        public ROM ROM;
        public Form1 FrmMain;

        public void ResetPPU()
        {
            for (var i = 0; i <= 30; i++)
            {
                PowerOf2[i] = (int) Math.Pow(2,i);
            }
            PowerOf2[31] = -2147483648;

            for (var bgNum = 0; bgNum <= 3; bgNum++)
            {
                var bg = _background[bgNum];
                bg.Address = 0;
                bg.Size = 0;
                bg.CHRAddress = 0;
                bg.HScroll = 0;
                bg.HLowHighToggle = false;
                bg.VScroll = 0;
                bg.VLowHighToggle = false;
                _background[bgNum] = bg;
            }
            ObjRAMAddress = 0;
            ObjRAMFirstAddress = 0;
            _objLowHighToggle = false;
            _firstReadObj = false;
            Array.Clear(ObjRAM, 0, ObjRAM.Length);
            Array.Clear(_palette, 0, _palette.Length);
        }

        public void WritePPU(int address,byte value)
        {
            PPUBackground bg;
            switch (address)
            {
                case 0x2100:
                    _screenEnabled = ((value & 0x80) <= 0);
                    break;
                case 0x2101:
                    _objChrOffset = (value & 3)*0x4000;
                    _objName = ((value >> 3) & 3) << 13;
                    _objSize = value/0x20;
                    break;
                case 0x2102:
                    ObjRAMAddress = value + (ObjRAMAddress & 0x100);
                    ObjRAMFirstAddress = ObjRAMAddress;
                    break;
                case 0x2103:
                    if ((value & 0x1) != 0)
                    {
                        ObjRAMAddress = ObjRAMAddress | 0x100;
                    }
                    else
                    {
                        ObjRAMAddress = ObjRAMAddress & ~0x100;
                    }
                    ObjRAMFirstAddress = ObjRAMAddress;
                    _objLowHighToggle = true;
                    break;
                case 0x2104:
                    if (ObjRAMAddress > 0x10F)
                    {
                        ObjRAMAddress = 0;
                        _objLowHighToggle = true;
                    }
                    if (_objLowHighToggle)
                    {
                        ObjRAM[ObjRAMAddress*2] = value;
                    }
                    else
                    {
                        ObjRAM[(ObjRAMAddress*2) + 1] = value;
                        ObjRAMAddress += 1;
                    }
                    _objLowHighToggle = !_objLowHighToggle;
                    break;
                case 0x2105:
                    _ppuMode = (byte) (value & 0x7);
                    _bg3Priority = (value & 0x8) != 0;
                    _background[0].Tile16X16 = (value & 0x10) != 0;
                    _background[1].Tile16X16 = (value & 0x20) != 0;
                    _background[2].Tile16X16 = (value & 0x40) != 0;
                    _background[3].Tile16X16 = (value & 0x80) != 0;
                    break;
                case 0x2106: //Mosaic
                    _mosaicSize = (byte) ((value & 0xF0) >> 4);
                    _background[0].Mosaic = (value & 0x1) != 0;
                    _background[1].Mosaic = (value & 0x2) != 0;
                    _background[2].Mosaic = (value & 0x4) != 0;
                    _background[3].Mosaic = (value & 0x8) != 0;
                    break;
                case 0x2107: //Address
                    _background[0].Address = (value & 0x7C)*0x200;
                    _background[0].Size = (byte) (value & 3);
                    break;
                case 0x2108:
                    _background[1].Address = (value & 0x7C)*0x200;
                    _background[1].Size = (byte) (value & 3);
                    break;
                case 0x2109:
                    _background[2].Address = (value & 0x7C)*0x200;
                    _background[2].Size = (byte) (value & 3);
                    break;
                case 0x210A:
                    _background[3].Address = (value & 0x7C)*0x200;
                    _background[3].Size = (byte) (value & 3);
                    break;
                case 0x210B: //CHR Address
                    _background[0].CHRAddress = (value & 7)*0x2000;
                    _background[1].CHRAddress = (value >> 4)*0x2000;
                    break;
                case 0x210C:
                    _background[2].CHRAddress = (value & 7)*0x2000;
                    _background[3].CHRAddress = (value >> 4)*0x2000;
                    break;
                case 0x210D:
                    bg = _background[0];
                    if (bg.HLowHighToggle)
                    {
                        bg.HScroll = (value*0x100) + (bg.HScroll & 0xFF);
                    }
                    else
                    {
                        bg.HScroll = value + (bg.HScroll & 0xFF00);
                    }
                    bg.HLowHighToggle = !bg.HLowHighToggle;
                    _background[0] = bg;
                    break;
                case 0x210E:
                    bg = _background[0];
                    if (bg.VLowHighToggle)
                    {
                        bg.VScroll = (value*0x100) + (bg.VScroll & 0xFF);
                    }
                    else
                    {
                        bg.VScroll = value + (bg.VScroll & 0xFF00);
                    }
                    bg.VLowHighToggle = !bg.VLowHighToggle;
                    _background[0] = bg;
                    break;
                case 0x210F:
                    bg = _background[1];
                    if (bg.HLowHighToggle)
                    {
                        bg.HScroll = (value*0x100) + (bg.HScroll & 0xFF);
                    }
                    else
                    {
                        bg.HScroll = value + (bg.HScroll & 0xFF00);
                    }
                    bg.HLowHighToggle = !bg.HLowHighToggle;
                    _background[1] = bg;
                    break;
                case 0x2110:
                    bg = _background[1];
                    if (bg.VLowHighToggle)
                    {
                        bg.VScroll = (value*0x100) + (bg.VScroll & 0xFF);
                    }
                    else
                    {
                        bg.VScroll = value + (bg.VScroll & 0xFF00);
                    }
                    bg.VLowHighToggle = !bg.VLowHighToggle;
                    _background[1] = bg;
                    break;
                case 0x2111:
                    bg = _background[2];
                    if (bg.HLowHighToggle)
                    {
                        bg.HScroll = (value*0x100) + (bg.HScroll & 0xFF);
                    }
                    else
                    {
                        bg.HScroll = value + (bg.HScroll & 0xFF00);
                    }
                    bg.HLowHighToggle = !bg.HLowHighToggle;
                    _background[2] = bg;
                    break;
                case 0x2112:
                    bg = _background[2];
                    if (bg.VLowHighToggle)
                    {
                        bg.VScroll = (value*0x100) + (bg.VScroll & 0xFF);
                    }
                    else
                    {
                        bg.VScroll = value + (bg.VScroll & 0xFF00);
                    }
                    bg.VLowHighToggle = !bg.VLowHighToggle;
                    _background[2] = bg;
                    break;
                case 0x2113:
                    bg = _background[3];
                    if (bg.HLowHighToggle)
                    {
                        bg.HScroll = (value*0x100) + (bg.HScroll & 0xFF);
                    }
                    else
                    {
                        bg.HScroll = value + (bg.HScroll & 0xFF00);
                    }
                    bg.HLowHighToggle = !bg.HLowHighToggle;
                    _background[3] = bg;
                    break;
                case 0x2114:
                    bg = _background[3];
                    if (bg.VLowHighToggle)
                    {
                        bg.VScroll = (value*0x100) + (bg.VScroll & 0xFF);
                    }
                    else
                    {
                        bg.VScroll = value + (bg.VScroll & 0xFF00);
                    }
                    bg.VLowHighToggle = !bg.VLowHighToggle;
                    _background[3] = bg;
                    break;
                case 0x2115: //VRAM Control
                    switch (value & 3)
                    {
                        case 0:
                            _vramIncrement = 1;
                            break;
                        case 1:
                            _vramIncrement = 32;
                            break;
                        case 2:
                            _vramIncrement = 128;
                            break;
                        case 3:
                            _vramIncrement = 256;
                            break;
                    }
                    _increment2119213A = (value & 0x80) != 0;
                    break;
                case 0x2116: //VRAM Access
                    _vramAddress = value + (_vramAddress & 0xFF00);
                    _firstReadVram = true;
                    break;
                case 0x2117:
                    _vramAddress = (value*0x100) + (_vramAddress & 0xFF);
                    _firstReadVram = true;
                    break;
                case 0x2118:
                    VRAM[(_vramAddress << 1) & 0xFFFF] = value;
                    if (!_increment2119213A)
                    {
                        _vramAddress += _vramIncrement;
                    }
                    _firstReadVram = true;
                    break;
                case 0x2119:
                    VRAM[((_vramAddress << 1) + 1) & 0xFFFF] = value;
                    if (_increment2119213A)
                    {
                        _vramAddress += _vramIncrement;
                    }
                    _firstReadVram = true;
                    break;
                case 0x211B:
                    if (_mode7LowHigh)
                    {
                        _mode7Multiplicand = (value*0x100) + (_mode7Multiplicand & 0xFF);
                    }
                    else
                    {
                        _mode7Multiplicand = value + (_mode7Multiplicand & 0xFF00);
                    }
                    _mode7LowHigh = !_mode7LowHigh;
                    break;
                case 0x211C:
                    _mode7Multiplier = value;
                    _multResult = _mode7Multiplicand*_mode7Multiplier;
                    break;
                case 0x211D:
                    _mode7C = value;
                    break;
                case 0x211E:
                    _mode7D = value;
                    break;
                case 0x211F:
                    _mode7X = value;
                    break;
                case 0x2120:
                    _mode7Y = value;
                    break;
                case 0x2121:
                    _palAddress = value*2;
                    break;
                case 0x2122:
                    _cgRAM[_palAddress & 0x1FF] = value;
                    var paletteValue = _cgRAM[_palAddress & 0x1FE] + (_cgRAM[(_palAddress & 0x1FE) + 1] * 0x100);
                    _palette[(_palAddress/2) & 0xFF].R = (byte) ((paletteValue & 0x1F)*8);
                    _palette[(_palAddress/2) & 0xFF].G = (byte) (((paletteValue >> 5) & 0x1F)*8);
                    _palette[(_palAddress/2) & 0xFF].B = (byte) (((paletteValue >> 10) & 0x1F)*8);
                    _palAddress += 1;
                    break;
                case 0x212C:
                    _bgMainEnabled = value;
                    break;
                case 0x212D:
                    _bgSubEnabled = value;
                    break;
                case 0x2130:
                    _colorMathObj = (value & 0x2) != 0;
                    _colorMathEnable = (byte) ((value & 0x30)/0x10);
                    break;
                case 0x2131:
                    _colorMathAddSub = (value & 0x80) != 0;
                    _colorMathDiv2 = (value & 0x40) != 0;
                    _colorMathBGs = (byte) (value & 0x3F);
                    break;
                case 0x2132:
                    if ((value & 0x20) != 0)
                    {
                        _fixedColor.R = (byte) ((value & 0x1F)*8);
                    }
                    if ((value & 0x40) != 0)
                    {
                        _fixedColor.G = (byte) ((value & 0x1F)*8);
                    }
                    if ((value & 0x80) != 0)
                    {
                        _fixedColor.B = (byte) ((value & 0x1F)*8);
                    }
                    break;
            }
        }

        public byte ReadPPU(int address)
        {
            byte value;
            switch (address)
            {
                case 0x2134:
                    return (byte) (_multResult & 0xFF);
                case 0x2135:
                    return (byte) ((_multResult & 0xFF00)/0x100);
                case 0x2136:
                    return (byte) ((_multResult & 0xFF0000)/0x10000);
                case 0x2137:
                    _vLatch = C65816.CurrentLine;
                    break;
                case 0x2138:
                    if (_firstReadObj)
                    {
                        _firstReadObj = false;
                        return ObjRAM[ObjRAMAddress << 1];
                    }
                    value = ObjRAM[((ObjRAMAddress << 1) + 1) & 0x10F];
                    ObjRAMAddress = (ObjRAMAddress + 1) & 0x10F;
                    return value;
                case 0x2139:
                    if (_firstReadVram)
                    {
                        _firstReadVram = false;
                        return VRAM[(_vramAddress << 1) & 0xFFFF];
                    }
                    value = VRAM[((_vramAddress << 1) - 2) & 0xFFFF];
                    if (!_increment2119213A)
                    {
                        _vramAddress += _vramIncrement;
                    }
                    return value;
                case 0x213A:
                    if (_firstReadVram)
                    {
                        _firstReadVram = false;
                        return VRAM[((_vramAddress << 1) + 1) & 0xFFFF];
                    }
                    value = VRAM[((_vramAddress << 1) - 1) & 0xFFFF];
                    if (_increment2119213A)
                    {
                        _vramAddress += _vramIncrement;
                    }
                    return value;
                case 0x213D:
                    value = (byte) (_vLatch & 0xFF);
                    _vLatch >>= 8;
                    return value;
            }
            return 0;
        }

        public void RenderScanline(int scanline)
        {
            if (!_screenEnabled) return;
            RenderBgLayer(scanline, 3, false);
            RenderBgLayer(scanline, 2, false);
            RenderSprites(scanline, 0);
            if (_bg3Priority)
            {
                RenderBgLayer(scanline, 3, true);
                RenderSprites(scanline, 1);
                RenderBgLayer(scanline, 1, false);
                RenderBgLayer(scanline, 0, false);
                RenderBgLayer(scanline, 1, true);
                RenderSprites(scanline, 2);
                RenderBgLayer(scanline, 0, true);
                RenderSprites(scanline, 3);
                RenderBgLayer(scanline, 2, true);
            }
            else
            {
                RenderBgLayer(scanline, 3, true);
                RenderBgLayer(scanline, 2, true);
                RenderSprites(scanline, 1);
                RenderBgLayer(scanline, 1, false);
                RenderBgLayer(scanline, 0, false);
                RenderSprites(scanline, 2);
                RenderBgLayer(scanline, 1, true);
                RenderBgLayer(scanline, 0, true);
                RenderSprites(scanline, 3);
            }
        }

        private void RenderBgLayer(int scanline, int layer, bool foreground)
        {
            if (((_bgMainEnabled | _bgSubEnabled) & PowerOf2[layer]) == 0) return;
            var colorMath = false;
            if (_colorMathEnable != 3)
            {
                if ((_bgMainEnabled & PowerOf2[layer]) != 0)
                {
                    colorMath = (_colorMathBGs & PowerOf2[layer]) != 0;
                }
                if (((_colorMathBGs & 0x20) != 0) && (layer == 1 && !foreground))
                {
                    colorMath = true;
                }
            }
            var BPP = 0;
            switch (layer)
            {
                case 0:
                    switch (_ppuMode)
                    {
                        case 0:
                            BPP = 2;
                            break;
                        case 1:
                        case 2:
                        case 5:
                        case 6:
                            BPP = 4;
                            break;
                        case 3:
                        case 4:
                            BPP = 8;
                            break;
                    }
                    break;
                case 1:
                    switch (_ppuMode)
                    {
                        case 0:
                        case 4:
                        case 5:
                            BPP = 2;
                            break;
                        case 1:
                        case 2:
                        case 3:
                            BPP = 4;
                            break;
                    }
                    break;
                case 2:
                    if (_ppuMode < 2)
                    {
                        BPP = 2;
                    }
                    break;
                case 3:
                    if (_ppuMode == 0)
                    {
                        BPP = 2;
                    }
                    break;
            }

            var bg = _background[layer];
            var reverseX = ((bg.HScroll/256)%2) <= 0;
            var reverseY = ((bg.VScroll/256)%2) <= 0;

            var scrollY = 0;
            if (scanline >= (256 - (bg.VScroll%256)))
            {
                scrollY = 1;
            }

            if (_ppuMode == 7 && layer == 0)
            {
                //Mode 7
                var baseCharNum = ((((scanline + (bg.VScroll%8))/8) + ((bg.VScroll%1024)/8))%128)*256;
                var temp = (scanline + (bg.VScroll%8))%8;
                for (var x = 0; x <= 127; x++)
                {
                    var characterNumber = baseCharNum + (x*2);
                    for (var scrollX = 0; scrollX <= 1; scrollX++)
                    {
                        var tempX = ((x*8) + (scrollX*1024) - (bg.HScroll%1024));
                        if (tempX <= -8 || tempX >= 256) continue;
                        int tileNumber = VRAM[characterNumber];
                        var baseTile = (tileNumber*128) + 1;
                        baseTile += temp*16;
                        for (var tileX = 0; tileX <= 7; tileX++)
                        {
                            var color = VRAM[baseTile + (tileX*2)];
                            DrawPixel(((x*8) + tileX) + (scrollX*1024) - (bg.HScroll%1024), scanline, color);
                        }
                    }
                }
            }
            else
            {
                if (BPP != 0)
                {
                    if (bg.Tile16X16)
                    {
                        var baseCharNum = ((((scanline + (bg.VScroll%16))/16) + ((bg.VScroll%256)/16))%16)*64;
                        var temp = (scanline + (bg.VScroll%16))%16;
                        for (var x = 0; x <= 16; x++)
                        {
                            var characterNumber = baseCharNum + (x*2);
                            for (var scrollX = 0; scrollX <= 1; scrollX++)
                            {
                                var tempX = ((x*16) + (scrollX*256) - (bg.HScroll%256));
                                if (tempX <= -16 || tempX >= 256) continue;
                                var tileOffset = bg.Address + characterNumber;
                                switch (bg.Size)
                                {
                                    case 1:
                                        tileOffset += (512*(reverseX ? scrollX : 1 - scrollX));
                                        break;
                                    case 2:
                                        tileOffset += (512*(reverseY ? scrollY : 1 - scrollY));
                                        break;
                                    case 3:
                                        tileOffset += (512*(reverseY ? scrollY : 1 - scrollY)*2) + (512*(reverseX ? scrollX : 1 - scrollX));
                                        break;
                                }
                                var tileData = VRAM[tileOffset] + VRAM[tileOffset + 1]*0x100;
                                var tileNumber = tileData & 0x3FF;
                                var palNum = (tileData & 0x1C00) >> 10;
                                var priority = (tileData & 0x2000) != 0;
                                var hFlip = (tileData & 0x4000) != 0;
                                var vFlip = (tileData & 0x8000) != 0;
                                if (vFlip)
                                {
                                    if (((scanline + (bg.VScroll%16))%16) < 8)
                                    {
                                        tileNumber += 16;
                                    }
                                }
                                else
                                {
                                    if (((scanline + (bg.VScroll%16))%16) > 7)
                                    {
                                        tileNumber += 16;
                                    }
                                }
                                if (priority != foreground) continue;
                                for (var tx = 0; tx <= 1; tx++)
                                {
                                    var baseTile = bg.CHRAddress + (tileNumber*(BPP*8));
                                    if (temp > 7)
                                    {
                                        temp -= 8;
                                    }
                                    baseTile += vFlip ? (7 - temp)*2 : temp*2;
                                    byte byte2 = 0,byte3 = 0,byte4 = 0,byte5 = 0,byte6 = 0,byte7 = 0;
                                    var byte0 = VRAM[baseTile];
                                    var byte1 = VRAM[baseTile + 1];
                                    if (BPP == 4 || BPP == 8)
                                    {
                                        byte2 = VRAM[baseTile + 16];
                                        byte3 = VRAM[baseTile + 17];
                                        if (BPP == 8)
                                        {
                                            byte4 = VRAM[baseTile + 32];
                                            byte5 = VRAM[baseTile + 33];
                                            byte6 = VRAM[baseTile + 48];
                                            byte7 = VRAM[baseTile + 49];
                                        }
                                    }
                                    int xFlip;
                                    if (hFlip)
                                    {
                                        xFlip = 8*(1 - tx);
                                    }
                                    else
                                    {
                                        xFlip = 8*tx;
                                    }
                                    for (var tileX = 0; tileX <= 7; tileX++)
                                    {
                                        var pixelColor = 0;
                                        var bitToTest = PowerOf2[hFlip ? tileX : 7 - tileX];
                                        if ((byte0 & bitToTest) != 0)
                                        {
                                            pixelColor += 1;
                                        }
                                        if ((byte1 & bitToTest) != 0)
                                        {
                                            pixelColor += 2;
                                        }
                                        if (BPP == 4 || BPP == 8)
                                        {
                                            if ((byte2 & bitToTest) != 0)
                                            {
                                                pixelColor += 4;
                                            }
                                            if ((byte3 & bitToTest) != 0)
                                            {
                                                pixelColor += 8;
                                            }
                                            if (BPP == 8)
                                            {
                                                if ((byte4 & bitToTest) != 0)
                                                {
                                                    pixelColor += 16;
                                                }
                                                if ((byte5 & bitToTest) != 0)
                                                {
                                                    pixelColor += 32;
                                                }
                                                if ((byte6 & bitToTest) != 0)
                                                {
                                                    pixelColor += 64;
                                                }
                                                if ((byte7 & bitToTest) != 0)
                                                {
                                                    pixelColor += 128;
                                                }
                                            }
                                        }
                                        var color = (byte) ((palNum*PowerOf2[BPP]) + pixelColor);
                                        if (pixelColor != 0 || (layer == 1 && !foreground))
                                        {
                                            DrawPixel(((x*16) + tileX + xFlip) + (scrollX*256) - (bg.HScroll%256), scanline, color, colorMath, pixelColor == 0);
                                        }
                                    }
                                    tileNumber += 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        var baseCharNum = ((((scanline + (bg.VScroll%8))/8) + ((bg.VScroll%256)/8))%32)*64;
                        var temp = (scanline + (bg.VScroll%8))%8;
                        for (var x = 0; x <= 31; x++)
                        {
                            var characterNumber = baseCharNum + (x*2);
                            for (var scrollX = 0; scrollX <= 1; scrollX++)
                            {
                                var tempX = ((x*8) + (scrollX*256) - (bg.HScroll%256));
                                if (tempX <= -8 || tempX >= 256) continue;
                                var tileOffset = bg.Address + characterNumber;
                                switch (bg.Size)
                                {
                                    case 1:
                                        tileOffset += (2048*(reverseX ? scrollX : 1 - scrollX));
                                        break;
                                    case 2:
                                        tileOffset += (2048*(reverseY ? scrollY : 1 - scrollY));
                                        break;
                                    case 3:
                                        tileOffset += (2048*(reverseY ? scrollY : 1 - scrollY)*2) + (2048*(reverseX ? scrollX : 1 - scrollX));
                                        break;
                                }
                                var tileData = VRAM[tileOffset] + (VRAM[tileOffset + 1]*0x100);
                                var tileNumber = tileData & 0x3FF;
                                var palNum = (tileData & 0x1C00) >> 10;
                                var priority = (tileData & 0x2000) != 0;
                                var hFlip = (tileData & 0x4000) != 0;
                                var vFlip = (tileData & 0x8000) != 0;
                                if (priority != foreground) continue;
                                var baseTile = bg.CHRAddress + (tileNumber*(BPP*8));
                                baseTile += vFlip ? (7 - temp)*2 : temp*2;
                                byte byte2 = 0, byte3 = 0, byte4 = 0, byte5 = 0, byte6 = 0, byte7= 0;
                                var byte0 = VRAM[baseTile];
                                var byte1 = VRAM[baseTile + 1];
                                if (BPP == 4 || BPP == 8)
                                {
                                    byte2 = VRAM[baseTile + 16];
                                    byte3 = VRAM[baseTile + 17];
                                    if (BPP == 8)
                                    {
                                        byte4 = VRAM[baseTile + 32];
                                        byte5 = VRAM[baseTile + 33];
                                        byte6 = VRAM[baseTile + 48];
                                        byte7 = VRAM[baseTile + 49];
                                    }
                                }
                                for (var tileX = 0; tileX <= 7; tileX++)
                                {
                                    var pixelColor = 0;
                                    var bitToTest = PowerOf2[hFlip ? tileX : 7 - tileX];
                                    if ((byte0 & bitToTest) != 0)
                                    {
                                        pixelColor += 1;
                                    }
                                    if ((byte1 & bitToTest) != 0)
                                    {
                                        pixelColor += 2;
                                    }
                                    if (BPP == 4 || BPP == 8)
                                    {
                                        if ((byte2 & bitToTest) != 0)
                                        {
                                            pixelColor += 4;
                                        }
                                        if ((byte3 & bitToTest) != 0)
                                        {
                                            pixelColor += 8;
                                        }
                                        if (BPP == 8)
                                        {
                                            if ((byte4 & bitToTest) != 0)
                                            {
                                                pixelColor += 16;
                                            }
                                            if ((byte5 & bitToTest) != 0)
                                            {
                                                pixelColor += 32;
                                            }
                                            if ((byte6 & bitToTest) != 0)
                                            {
                                                pixelColor += 64;
                                            }
                                            if ((byte7 & bitToTest) != 0)
                                            {
                                                pixelColor += 128;
                                            }
                                        }
                                    }
                                    var color = (byte) ((palNum*PowerOf2[BPP]) + pixelColor);
                                    if ((pixelColor != 0) || (layer == 1 && !foreground))
                                    {
                                        DrawPixel(((x*8) + tileX) + (scrollX*256) - (bg.HScroll%256), scanline, color, colorMath, pixelColor == 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (bg.Mosaic && _mosaicSize != 0)
            {
                ApplyMosaic(scanline);
            }
            _background[layer] = bg;
        }

        private void RenderSprites(int scanline, int priority)
        {
            if (((_bgMainEnabled | _bgSubEnabled) & 0x10) <= 0) return;
            var tbl2Byte = 0;
            var tbl2Shift = 1;
            var temp = 0;
            for (var offset = 0; offset <= 0x1FF; offset += 4)
            {
                int tempX = ObjRAM[offset];
                int y = ObjRAM[offset + 1];
                int tileNumber = ObjRAM[offset + 2];
                var attributes = ObjRAM[offset + 3];
                if ((attributes & 0x1) != 0)
                {
                    tileNumber = tileNumber | 0x100;
                }
                var palNum = (attributes & 0xE) >> 1;
                var objPriority = (attributes & 0x30) >> 4;
                var hFlip = (attributes & 0x40) != 0;
                var vFlip = (attributes & 0x80) != 0;
                var tbl2Data = ObjRAM[0x200 + tbl2Byte];
                if ((tbl2Data & PowerOf2[tbl2Shift - 1]) != 0)
                {
                    tempX = tempX | 0x100;
                }
                var x = tempX;
                var tileSize = (tbl2Data & PowerOf2[tbl2Shift]) != 0;
                int tX = 0, tY = 0;
                switch (_objSize)
                {
                    case 0:
                        if (tileSize)
                        {
                            tX = 1;
                            tY = 1;
                        }
                        else
                        {
                            tX = 0;
                            tY = 0; //8x8/16x16
                        }
                        break;
                    case 1:
                        if (tileSize)
                        {
                            tX = 3;
                            tY = 3;
                        }
                        else
                        {
                            tX = 0;
                            tY = 0; //8x8/32x32
                        }
                        break;
                    case 2:
                        if (tileSize)
                        {
                            tX = 7;
                            tY = 7;
                        }
                        else
                        {
                            tX = 0;
                            tY = 0; //8x8/64x64
                        }
                        break;
                    case 3:
                        if (tileSize)
                        {
                            tX = 3;
                            tY = 3;
                        }
                        else
                        {
                            tX = 1;
                            tY = 1; //16x16/32x32
                        }
                        break;
                    case 4:
                        if (tileSize)
                        {
                            tX = 7;
                            tY = 7;
                        }
                        else
                        {
                            tX = 1;
                            tY = 1; //16x16/64x64
                        }
                        break;
                    case 5:
                        if (tileSize)
                        {
                            tX = 7;
                            tY = 7;
                        }
                        else
                        {
                            tX = 3;
                            tY = 3; //32x32/64x64
                        }
                        break;
                }
                if (objPriority == priority)
                {
                    if (vFlip)
                    {
                        y += (8*tY);
                    }
                    for (var tileNumY = 0; tileNumY <= tY; tileNumY++)
                    {
                        if (hFlip)
                        {
                            x += (8*tX);
                        }
                        for (var tileNumX = 0; tileNumX <= tX; tileNumX++)
                        {
                            for (var tileY = 0; tileY <= 7; tileY++)
                            {
                                if (y + (vFlip ? (7 - tileY) : tileY) != scanline) continue;
                                var byte0 = VRAM[_objChrOffset + (tileY*2) + ((tileNumber + (tileNumY*16) + tileNumX)*32)];
                                var byte1 = VRAM[_objChrOffset + (tileY*2) + ((tileNumber + (tileNumY*16) + tileNumX)*32) + 1];
                                var byte2 = VRAM[_objChrOffset + (tileY*2) + ((tileNumber + (tileNumY*16) + tileNumX)*32) + 16];
                                var byte3 =VRAM[_objChrOffset + (tileY*2) + ((tileNumber + (tileNumY*16) + tileNumX)*32) + 17];
                                for (var tileX = 0; tileX <= 7; tileX++)
                                {
                                    var pixelColor = 0;
                                    var bitToTest = PowerOf2[hFlip ? tileX : 7 - tileX];
                                    if ((byte0 & bitToTest) != 0)
                                    {
                                        pixelColor += 1;
                                    }
                                    if ((byte1 & bitToTest) != 0)
                                    {
                                        pixelColor += 2;
                                    }
                                    if ((byte2 & bitToTest) != 0)
                                    {
                                        pixelColor += 4;
                                    }
                                    if ((byte3 & bitToTest) != 0)
                                    {
                                        pixelColor += 8;
                                    }
                                    if (vFlip)
                                    {
                                        DrawPixel(x + tileX, y + (7 - tileY), (byte) (128 + (palNum*16) + pixelColor), false, pixelColor == 0);
                                    }
                                    else
                                    {
                                        DrawPixel(x + tileX, y + tileY, (byte) (128 + (palNum*16) + pixelColor), false, pixelColor == 0);
                                    }
                                }
                            }
                            if (hFlip)
                            {
                                x -= 8;
                            }
                            else
                            {
                                x += 8;
                            }
                        }
                        x = tempX;
                        if (vFlip)
                        {
                            y -= 8;
                        }
                        else
                        {
                            y += 8;
                        }
                    }
                }
                if (temp < 3)
                {
                    temp += 1;
                    tbl2Shift += 2;
                }
                else
                {
                    temp = 0;
                    tbl2Byte += 1;
                    tbl2Shift = 1;
                }
            }
        }

        private void DrawPixel(int x, int y, byte colorIndex, bool colorMath = false, bool transparent = false)
        {
            if ((x < 0 || x >= 256) || (y < 0 || y >= 224)) return;
            var bufferPosition = x + (y*256);
            var color = _palette[colorIndex];
            if (colorMath && transparent)
            {
                if (_colorMathAddSub)
                {
                    color.R = (byte) (SubColor(color.R, _fixedColor.R));
                    color.G = (byte) (SubColor(color.G, _fixedColor.G));
                    color.B = (byte) (SubColor(color.B, _fixedColor.B));
                }
                else
                {
                    color.R =(byte) (AddColor(color.R, _fixedColor.R));
                    color.G = (byte) (AddColor(color.G, _fixedColor.G));
                    color.B = (byte) (AddColor(color.B, _fixedColor.B));
                }
                if (_colorMathDiv2)
                {
                    color.R = (byte) (color.R *0.5);
                    color.G = (byte) (color.G *0.5);
                    color.B = (byte) (color.B *0.5);
                }
            }
            if (!transparent || colorMath)
            {
                _videoBuffer[bufferPosition] = (color.R*0x10000) + (color.G*0x100) + color.B;
            }
            _palette[colorIndex] = color;
        }

        private void ApplyMosaic(int scanline)
        {
            var srcY = scanline - (scanline%_mosaicSize);
            for (var x = 0; x <= 255; x += _mosaicSize)
            {
                var srcColor = _videoBuffer[x + (srcY*256)];
                for (var copyX = 0; copyX <= _mosaicSize - 1; copyX++)
                {
                    if (x + copyX >= 0 && x + copyX < 256)
                    {
                        _videoBuffer[(x + copyX) + (scanline*256)] = srcColor;
                    }
                }
            }
        }

        //Color Math (Note to Mike: this is not working right, so I disabled for now...)
        //private void DrawPixelCM(int x, int y, byte colorIndex, bool colorMath = false, bool transparent = false)
        //{
        //    if ((x < 0 || x >= 256) || (y < 0 || y >= 224)) return;
        //    var bufferPosition = x + (y*256);
        //    var color = _palette[colorIndex];
        //    if (!transparent)
        //    {
        //        _videoBuffer[bufferPosition] = (color.R*0x10000) + (color.G*0x100) + color.B;
        //    }
        //    if (colorMath)
        //    {
        //        DoColorMath(bufferPosition);
        //    }
        //}

        //private void DrawPixelCMSub(int x, int y, byte colorIndex, bool colorMath = false, bool transparent = false)
        //{
        //    if ((x < 0 || x >= 256) || (y < 0 || y >= 224)) return;
        //    var bufferPosition = x + (y*256);
        //    var color = _palette[colorIndex];
        //    if (!transparent)
        //    {
        //        _videoBufferSub[bufferPosition] = (color.R*0x10000) + (color.G*0x100) + color.B;
        //    }
        //    if (colorMath)
        //    {
        //        DoColorMath(bufferPosition);
        //    }
        //}

        //private void DoColorMath(int bufferPosition)
        //{
        //    ColorPalette color;
        //    if (_colorMathAddSub)
        //    {
        //        color.R = (byte) (SubColor((_videoBuffer[bufferPosition] & 0xFF0000)/0x10000, (_videoBufferSub[bufferPosition] & 0xFF0000)/0x10000));
        //        color.G = (byte) (SubColor((_videoBuffer[bufferPosition] & 0xFF00)/0x100, (_videoBufferSub[bufferPosition] & 0xFF00)/0x100));
        //        color.B = (byte) (SubColor(_videoBuffer[bufferPosition] & 0xFF, _videoBufferSub[bufferPosition] & 0xFF));
        //    }
        //    else
        //    {
        //        color.R = (byte) (AddColor((_videoBuffer[bufferPosition] & 0xFF0000)/0x10000, (_videoBufferSub[bufferPosition] & 0xFF0000)/0x10000));
        //        color.G = (byte) (AddColor((_videoBuffer[bufferPosition] & 0xFF00)/0x100, (_videoBufferSub[bufferPosition]) & 0xFF00)/0x100));
        //        color.B = (byte) (AddColor(_videoBuffer[bufferPosition] & 0xFF, _videoBufferSub[bufferPosition] & 0xFF));
        //    }
        //    if (_colorMathDiv2)
        //    {
        //        color.R = (byte) (color.R * 0.5);
        //        color.G = (byte) (color.G * 0.5);
        //        color.B = (byte) (color.B * 0.5);
        //    }
        //    _videoBuffer[bufferPosition] = (color.R*0x10000) + (color.G*0x100) + color.B;
        //}

        private static int AddColor(int val1, int val2)
        {
            var result = val1 + val2;
            return result > 0xFF ? 0xFF : result;
        }

        private static int SubColor(int val1, int val2)
        {
            var result = val1 - val2;
            return result < 0 ? 0 : result;
        }

        public void Blit()
        {
            if (img == null)
            {
                img = new Bitmap(256, 224, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            }
            var bitmapData1 = img.LockBits(new Rectangle(0, 0, 256, 224),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var scan0 = bitmapData1.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(_videoBuffer, 0, scan0, 256*224);
            img.UnlockBits(bitmapData1);
            FrmMain.PicScreen.Image = img;

            //clears the screen
            Array.Clear(_videoBuffer, 0, _videoBuffer.Length);
            for (var position = 0; position <= _videoBufferSub.Length - 1; position++)
            {
                _videoBufferSub[position] = (_fixedColor.R*0x10000) + (_fixedColor.G*0x100) + _fixedColor.B;
            }
        }

        public void Screenshot()
        {
            TakeScreenshot = false;
            if (img == null)
            {
                img = new Bitmap(256, 224, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            }
            var bitmapData1 = img.LockBits(new Rectangle(0, 0, 256, 224),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var scan0 = bitmapData1.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(_videoBuffer, 0, scan0, 256*224);
            img.UnlockBits(bitmapData1);
            var saveDlg = new SaveFileDialog{Title = "Save Screenshot", Filter = "Image|*.png", FileName = ROM.Header.Name};
            saveDlg.ShowDialog();
            if (saveDlg.FileName != null)
            {
                img.Save(saveDlg.FileName);
            }
        }   
    }
}