namespace SuperNintendo.Core.PPU
{
    public static class SPPU
    {
        public static VMA VMA = new VMA();
        public static uint WRAM;
        public static BackGround[] BG = new BackGround[4];
        public static byte BGMode;
        public static byte BG3Priority;

        public static bool CGFLIP;
        public static byte CGFLIPRead;
        public static byte CGADD;
        public static byte CGSavedByte;
        public static ushort[] CGDATA = new ushort[256];

        public static ScreenObject[] OBJ = new ScreenObject[128];
        public static bool OBJThroughMain;
        public static bool OBJThroughSub;
        public static bool OBJAddition;
        public static ushort OBJNameBase;
        public static ushort OBJNameSelect;
        public static byte OBJSizeSelect;

        public static ushort OAMAddr;
        public static ushort SavedOAMAddr;
        public static byte OAMPriorityRotation;
        public static byte OAMFlip;
        public static byte OAMReadFlip;
        public static ushort OAMTileAddress;
        public static ushort OAMWriteRegister;
        public static byte[] OAMData = new byte[512 + 32];

        public static byte FirstSprite;
        public static byte LastSprite;
        public static byte RangeTimeOver;

        public static bool HTimerEnabled;
        public static bool VTimerEnabled;
        public static short HTimerPosition;
        public static short VTimerPosition;
        public static ushort IRQHBeamPos;
        public static ushort IRQVBeamPos;

        public static byte HBeamFlip;
        public static byte VBeamFlip;
        public static ushort HBeamPosLatched;
        public static ushort VBeamPosLatched;
        public static ushort GunHLatch;
        public static ushort GunVLatch;
        public static byte HVBeamCounterLatched;

        public static bool Mode7HFlip;
        public static bool Mode7VFlip;
        public static byte Mode7Repeat;
        public static short MatrixA;
        public static short MatrixB;
        public static short MatrixC;
        public static short MatrixD;
        public static short CentreX;
        public static short CentreY;
        public static short M7HOFS;
        public static short M7VOFS;

        public static byte Mosaic;
        public static byte MosaicStart;
        public static bool[] BGMosaic = new bool[4];

        public static byte Window1Left;
        public static byte Window1Right;
        public static byte Window2Left;
        public static byte Window2Right;
        public static bool RecomputeClipWindows;
        public static byte[] ClipCounts = new byte[6];
        public static ClippingType[] ClipWindowOverlapLogic = new ClippingType[6];
        public static bool[] ClipWindow1Enable = new bool[6];
        public static bool[] ClipWindow2Enable = new bool[6];
        public static bool[] ClipWindow1Inside = new bool[6];
        public static bool[] ClipWindow2Inside = new bool[6];

        public static Memory.IsMemoryLinked[] HDMAMemPointers = new Memory.IsMemoryLinked[8];
        public static ushort[] BlackColourMap = new ushort[256];
        public static ushort[,] DirectColourMaps = new ushort[8,256];

        public static bool ForcedBlanking;

        public static byte FixedColourRed;
        public static byte FixedColourGreen;
        public static byte FixedColourBlue;
        public static byte Brightness;
        public static ushort ScreenHeight;

        public static bool Need16x8Mulitply;
        public static byte BGnxOFSbyte;
        public static byte M7byte;

        public static byte HDMA;
        public static byte HDMAEnded;

        public static byte OpenBus1;
        public static byte OpenBus2;

        public static ushort VRAMReadBuffer;

        public static void ResetPPU()
        {
            SoftResetPPU();
            //ControlsReset();
            M7HOFS = 0;
            M7VOFS = 0;
            M7byte = 0;
        }

        public static void UpdateIRQPositions(bool initial)
        {
            HTimerPosition = (short)(IRQHBeamPos * Memory.Constants.ONE_DOT_CYCLE + Timings.Timings.IRQTriggerCycles);
            HTimerPosition -= (short)(IRQHBeamPos > 0 ? 0 : Memory.Constants.ONE_DOT_CYCLE);
            HTimerPosition += (short)(IRQHBeamPos > 322 ? (Memory.Constants.ONE_DOT_CYCLE / 2) : 0);
            HTimerPosition += (short)(IRQHBeamPos > 326 ? (Memory.Constants.ONE_DOT_CYCLE / 2) : 0);
            VTimerPosition = (short)IRQVBeamPos;

            if (VTimerEnabled && (VTimerPosition >= (Timings.Timings.V_Max + (IPPU.Interlace ? 1 : 0))))
                Timings.Timings.NextIRQTimer = 0x0fffffff;
            else if (!HTimerEnabled && !VTimerEnabled)
                Timings.Timings.NextIRQTimer = 0x0fffffff;
            else if (HTimerEnabled && !VTimerEnabled)
            {
                var v_pos = CPU.CPUState.V_Counter;
                Timings.Timings.NextIRQTimer = HTimerPosition;
                if (CPU.CPUState.Cycles > Timings.Timings.NextIRQTimer - Timings.Timings.IRQTriggerCycles)
                {
                    Timings.Timings.NextIRQTimer += Timings.Timings.H_Max;
                    v_pos++;
                }

                // Check for short dot scanline
                if (v_pos == 240 && Timings.Timings.InterlaceField && !IPPU.Interlace)
                {
                    Timings.Timings.NextIRQTimer -= IRQHBeamPos <= 322 ? Memory.Constants.ONE_DOT_CYCLE / 2 : 0;
                    Timings.Timings.NextIRQTimer -= IRQHBeamPos <= 326 ? Memory.Constants.ONE_DOT_CYCLE / 2 : 0;
                }
            }
            else if (!HTimerEnabled && VTimerEnabled)
            {
                if (CPU.CPUState.V_Counter == VTimerPosition && initial)
                    Timings.Timings.NextIRQTimer = CPU.CPUState.Cycles + Timings.Timings.IRQTriggerCycles - Memory.Constants.ONE_DOT_CYCLE;
                else
                    Timings.Timings.NextIRQTimer = CyclesUntilNext(Timings.Timings.IRQTriggerCycles - Memory.Constants.ONE_DOT_CYCLE, VTimerPosition);
            }
            else
            {
                Timings.Timings.NextIRQTimer = CyclesUntilNext(HTimerPosition, VTimerPosition);

                // Check for short dot scanline
                var field = Timings.Timings.InterlaceField;

                if (VTimerPosition < CPU.CPUState.V_Counter || (VTimerPosition == CPU.CPUState.V_Counter && Timings.Timings.NextIRQTimer > Timings.Timings.H_Max))
                    field = !field;

                if (VTimerPosition == 240 && field && !IPPU.Interlace)
                {
                    Timings.Timings.NextIRQTimer -= IRQHBeamPos <= 322 ? Memory.Constants.ONE_DOT_CYCLE / 2 : 0;
                    Timings.Timings.NextIRQTimer -= IRQHBeamPos <= 326 ? Memory.Constants.ONE_DOT_CYCLE / 2 : 0;
                }
            }
        }

        private static int CyclesUntilNext(int hc, int vc)
        {
            var total = 0;
            int vpos = CPU.CPUState.V_Counter;

            if (vc - vpos > 0)
            {
                // It's still in this frame */
                // Add number of lines
                total += (vc - vpos) * Timings.Timings.H_Max_Master;
                // If line 240 is in there and we're odd, subtract a dot
                if (vpos <= 240 && vc > 240 && Timings.Timings.InterlaceField & !IPPU.Interlace)
                    total -= Memory.Constants.ONE_DOT_CYCLE;
            }
            else
            {
                if (vc == vpos && (hc > CPU.CPUState.Cycles))
                    return hc;

                total += (Timings.Timings.V_Max - vpos) * Timings.Timings.H_Max_Master;
                if (vpos <= 240 && Timings.Timings.InterlaceField && !IPPU.Interlace)
                    total -= Memory.Constants.ONE_DOT_CYCLE;

                total += (vc) * Timings.Timings.H_Max_Master;
                if (vc > 240 && !Timings.Timings.InterlaceField && !IPPU.Interlace)
                    total -= Memory.Constants.ONE_DOT_CYCLE;
            }

            total += hc;
            return total;
        }

        public static void SetPPU(byte value, ushort address)
        {
            // MAP_PPU: $2000-$3FFF

            if (CPU.CPUState.InDMAorHDMA)
            {
                if (CPU.CPUState.CurrentDMAorHDMAChannel >= 0 && DMA.DMA.SDMA[CPU.CPUState.CurrentDMAorHDMAChannel].ReverseTransfer)
                    return;
                else
                {
                    if (address > 0x21ff)
                        address = (ushort)(0x2100 + (address & 0xff));
                }
            }


            //if (Settings.MSU1 && (address & 0xfff8) == 0x2000) // MSU-1
                //MSU1WritePort(address & 7, value);
            //else if ((address & 0xffc0) == 0x2140) // APUIO0, APUIO1, APUIO2, APUIO3 write_port will run the APU until given clock before writing value
                //APUWritePort(address & 3, value);
             //else
            if (address <= 0x2183)
            {
                switch (address)
                {
                    case 0x2100: // INIDISP
                        if (value != Memory.Memory.FillRAM[0x2100])
                        {
                            FLUSH_REDRAW();

                            if (Brightness != (value & 0xf))
                            {
                                IPPU.ColorsChanged = true;
                                Brightness = (byte)(value & 0xf);
                                FixColourBrightness();
                                BuildDirectColourMaps();
                                if (Brightness > IPPU.MaxBrightness)
                                    IPPU.MaxBrightness = Brightness;
                            }

                            if ((Memory.Memory.FillRAM[0x2100] & 0x80) != (value & 0x80))
                            {
                                IPPU.ColorsChanged = true;
                                ForcedBlanking = ((value >> 7) & 1) > 0;
                            }
                        }

                        if (((Memory.Memory.FillRAM[0x2100] & 0x80) > 0) && CPU.CPUState.V_Counter == ScreenHeight + Constants.FIRST_VISIBLE_LINE)
                        {
                            OAMAddr = SavedOAMAddr;

                            byte tmp = 0;
                            if (OAMPriorityRotation > 0)
                                tmp = (byte)((OAMAddr & 0xfe) >> 1);

                            if ((OAMFlip & 1) > 0 || FirstSprite != tmp)
                            {
                                FirstSprite = tmp;
                                IPPU.OBJChanged = true;
                            }
                            OAMFlip = 0;
                        }

                        break;

                    case 0x2101: // OBSEL
                        if (value != Memory.Memory.FillRAM[0x2101])
                        {
                            FLUSH_REDRAW();
                            OBJNameBase = (ushort)((value & 3) << 14);
                            OBJNameSelect = (ushort)(((value >> 3) & 3) << 13);
                            OBJSizeSelect = (byte)((value >> 5) & 7);
                            IPPU.OBJChanged = true;
                        }

                        break;

                    case 0x2102: // OAMADDL
                        OAMAddr = (ushort)(((Memory.Memory.FillRAM[0x2103] & 1) << 8) | value);
                        OAMFlip = 0;
                        OAMReadFlip = 0;
                        SavedOAMAddr = OAMAddr;
                        if (OAMPriorityRotation > 0 && FirstSprite != (OAMAddr >> 1))
                        {
                            FirstSprite = (byte)((OAMAddr & 0xfe) >> 1);
                            IPPU.OBJChanged = true;
                        }

                        break;

                    case 0x2103: // OAMADDH
                        OAMAddr = (ushort)(((value & 1) << 8) | Memory.Memory.FillRAM[0x2102]);
                        OAMPriorityRotation = (byte)((value & 0x80) > 0 ? 1 : 0);
                        if (OAMPriorityRotation > 0)
                        {
                            if (FirstSprite != (OAMAddr >> 1))
                            {
                                FirstSprite = (byte)((OAMAddr & 0xfe) >> 1);
                                IPPU.OBJChanged = true;
                            }
                        }
                        else if (FirstSprite != 0)
                        {
                            FirstSprite = 0;
                            IPPU.OBJChanged = true;
                        }

                        OAMFlip = 0;
                        OAMReadFlip = 0;
                        SavedOAMAddr = OAMAddr;

                        break;

                    case 0x2104: // OAMDATA
                        //REGISTER_2104(value);
                        break;

                    case 0x2105: // BGMODE
                        if (value != Memory.Memory.FillRAM[0x2105])
                        {
                            FLUSH_REDRAW();
                            BG[0].BGSize = (byte)((value >> 4) & 1);
                            BG[1].BGSize = (byte)((value >> 5) & 1);
                            BG[2].BGSize = (byte)((value >> 6) & 1);
                            BG[3].BGSize = (byte)((value >> 7) & 1);
                            BGMode = (byte)(value & 7);
                            // BJ: BG3Priority only takes effect if BGMode == 1 and the bit is set
                            BG3Priority = (byte)((value & 0x0f) == 0x09 ? 1 : 0);
                            if (BGMode == 6 || BGMode == 5 || BGMode == 7)
                                IPPU.Interlace = ((Memory.Memory.FillRAM[0x2133] & 1) > 0);
                            else
                                IPPU.Interlace = false;
                        }

                        break;

                    case 0x2106: // MOSAIC
                        if (value != Memory.Memory.FillRAM[0x2106])
                        {
                            FLUSH_REDRAW();
                            MosaicStart = (byte)CPU.CPUState.V_Counter;
                            if (MosaicStart > ScreenHeight)
                                MosaicStart = 0;

                            Mosaic = (byte)((value >> 4) + 1);
                            BGMosaic[0] = (value & 1) > 0;
                            BGMosaic[1] = (value & 2) > 0;
                            BGMosaic[2] = (value & 4) > 0;
                            BGMosaic[3] = (value & 8) > 0;
                        }

                        break;

                    case 0x2107: // BG1SC
                        if (value != Memory.Memory.FillRAM[0x2107])
                        {
                            FLUSH_REDRAW();
                            BG[0].SCSize = (ushort)(value & 3);
                            BG[0].SCBase = (ushort)((value & 0x7c) << 8);
                        }

                        break;

                    case 0x2108: // BG2SC
                        if (value != Memory.Memory.FillRAM[0x2108])
                        {
                            FLUSH_REDRAW();
                            BG[1].SCSize = (ushort)(value & 3);
                            BG[1].SCBase = (ushort)((value & 0x7c) << 8);
                        }

                        break;

                    case 0x2109: // BG3SC
                        if (value != Memory.Memory.FillRAM[0x2109])
                        {
                            FLUSH_REDRAW();
                            BG[2].SCSize = (ushort)(value & 3);
                            BG[2].SCBase = (ushort)((value & 0x7c) << 8);
                        }

                        break;

                    case 0x210a: // BG4SC
                        if (value != Memory.Memory.FillRAM[0x210a])
                        {
                            FLUSH_REDRAW();
                            BG[3].SCSize = (ushort)(value & 3);
                            BG[3].SCBase = (ushort)((value & 0x7c) << 8);
                        }

                        break;

                    case 0x210b: // BG12NBA
                        if (value != Memory.Memory.FillRAM[0x210b])
                        {
                            FLUSH_REDRAW();
                            BG[0].NameBase = (ushort)((value & 7) << 12);
                            BG[1].NameBase = (ushort)(((value >> 4) & 7) << 12);
                        }

                        break;

                    case 0x210c: // BG34NBA
                        if (value != Memory.Memory.FillRAM[0x210c])
                        {
                            FLUSH_REDRAW();
                            BG[2].NameBase = (ushort)((value & 7) << 12);
                            BG[3].NameBase = (ushort)(((value >> 4) & 7) << 12);
                        }

                        break;

                    case 0x210d: // BG1HOFS, M7HOFS
                        BG[0].HOffset = (ushort)((value << 8) | (BGnxOFSbyte & ~7) | ((BG[0].HOffset >> 8) & 7));
                        M7HOFS = (short)((value << 8) | M7byte);
                        BGnxOFSbyte = value;
                        M7byte = value;
                        break;

                    case 0x210e: // BG1VOFS, M7VOFS
                        BG[0].VOffset = (ushort)((value << 8) | BGnxOFSbyte);
                        M7VOFS = (short)((value << 8) | M7byte);
                        BGnxOFSbyte = value;
                        M7byte = value;
                        break;

                    case 0x210f: // BG2HOFS
                        BG[1].HOffset = (ushort)((value << 8) | (BGnxOFSbyte & ~7) | ((BG[1].HOffset >> 8) & 7));
                        BGnxOFSbyte = value;
                        break;

                    case 0x2110: // BG2VOFS
                        BG[1].VOffset = (ushort)((value << 8) | BGnxOFSbyte);
                        BGnxOFSbyte = value;
                        break;

                    case 0x2111: // BG3HOFS
                        BG[2].HOffset = (ushort)((value << 8) | (BGnxOFSbyte & ~7) | ((BG[2].HOffset >> 8) & 7));
                        BGnxOFSbyte = value;
                        break;

                    case 0x2112: // BG3VOFS
                        BG[2].VOffset = (ushort)((value << 8) | BGnxOFSbyte);
                        BGnxOFSbyte = value;
                        break;

                    case 0x2113: // BG4HOFS
                        BG[3].HOffset = (ushort)((value << 8) | (BGnxOFSbyte & ~7) | ((BG[3].HOffset >> 8) & 7));
                        BGnxOFSbyte = value;
                        break;

                    case 0x2114: // BG4VOFS
                        BG[3].VOffset = (ushort)((value << 8) | BGnxOFSbyte);
                        BGnxOFSbyte = value;
                        break;

                    case 0x2115: // VMAIN
                        VMA.High = (value & 0x80) != 0;
                        switch (value & 3)
                        {
                            case 0: VMA.Increment = 1; break;
                            case 1: VMA.Increment = 32; break;
                            case 2: VMA.Increment = 128; break;
                            case 3: VMA.Increment = 128; break;
                        }

                        if ((value & 0x0c) > 0)
                        {
                            var i = (byte)((value & 0x0c) >> 2);
                            VMA.FullGraphicCount = Constants.IncCount[i];
                            VMA.Mask1 = (ushort)(Constants.IncCount[i] * 8 - 1);
                            VMA.Shift = Constants.Shift[i];
                        }
                        else
                            VMA.FullGraphicCount = 0;
                        break;

                    case 0x2116: // VMADDL
                        VMA.Address &= 0xff00;
                        VMA.Address |= value;

                        //UpdateVRAMReadBuffer();

                        break;

                    case 0x2117: // VMADDH
                        VMA.Address &= 0x00ff;
                        VMA.Address |= (ushort)(value << 8);

                        //UpdateVRAMReadBuffer();

                        break;

                    case 0x2118: // VMDATAL
                        //REGISTER_2118(value);
                        break;

                    case 0x2119: // VMDATAH
                        //REGISTER_2119(value);
                        break;

                    case 0x211a: // M7SEL
                        if (value != Memory.Memory.FillRAM[0x211a])
                        {
                            FLUSH_REDRAW();
                            Mode7Repeat = (byte)(value >> 6);
                            if (Mode7Repeat == 1)
                                Mode7Repeat = 0;

                            Mode7VFlip = ((value & 2) >> 1) > 0;
                            Mode7HFlip = (value & 1) > 0;
                        }

                        break;

                    case 0x211b: // M7A
                        MatrixA = (short)(M7byte | (value << 8));
                        Need16x8Mulitply = true;
                        M7byte = value;
                        break;

                    case 0x211c: // M7B
                        MatrixB = (short)(M7byte | (value << 8));
                        Need16x8Mulitply = true;
                        M7byte = value;
                        break;

                    case 0x211d: // M7C
                        MatrixC = (short)(M7byte | (value << 8));
                        M7byte = value;
                        break;

                    case 0x211e: // M7D
                        MatrixD = (short)(M7byte | (value << 8));
                        M7byte = value;
                        break;

                    case 0x211f: // M7X
                        CentreX =(short)(M7byte | (value << 8));
                        M7byte = value;
                        break;

                    case 0x2120: // M7Y
                        CentreY = (short)(M7byte | (value << 8));
                        M7byte = value;
                        break;

                    case 0x2121: // CGADD
                        CGFLIP = false;
                        CGFLIPRead = 0;
                        CGADD = value;
                        break;

                    case 0x2122: // CGDATA
                        //REGISTER_2122(value);
                        break;

                    case 0x2123: // W12SEL
                        if (value != Memory.Memory.FillRAM[0x2123])
                        {
                            FLUSH_REDRAW();
                            ClipWindow1Enable[0] = (value & 0x02) > 0;
                            ClipWindow1Enable[1] = (value & 0x20) > 0;
                            ClipWindow2Enable[0] = (value & 0x08) > 0;
                            ClipWindow2Enable[1] = (value & 0x80) > 0;
                            ClipWindow1Inside[0] = (value & 0x01) == 0;
                            ClipWindow1Inside[1] = (value & 0x10) == 0;
                            ClipWindow2Inside[0] = (value & 0x04) == 0;
                            ClipWindow2Inside[1] = (value & 0x40) == 0;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2124: // W34SEL
                        if (value != Memory.Memory.FillRAM[0x2124])
                        {
                            FLUSH_REDRAW();
                            ClipWindow1Enable[2] = (value & 0x02) > 0;
                            ClipWindow1Enable[3] = (value & 0x20) > 0;
                            ClipWindow2Enable[2] = (value & 0x08) > 0;
                            ClipWindow2Enable[3] = (value & 0x80) > 0;
                            ClipWindow1Inside[2] = (value & 0x01) == 0;
                            ClipWindow1Inside[3] = (value & 0x10) == 0;
                            ClipWindow2Inside[2] = (value & 0x04) == 0;
                            ClipWindow2Inside[3] = (value & 0x40) == 0;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2125: // WOBJSEL
                        if (value != Memory.Memory.FillRAM[0x2125])
                        {
                            FLUSH_REDRAW();
                            ClipWindow1Enable[4] = (value & 0x02) > 0;
                            ClipWindow1Enable[5] = (value & 0x20) > 0;
                            ClipWindow2Enable[4] = (value & 0x08) > 0;
                            ClipWindow2Enable[5] = (value & 0x80) > 0;
                            ClipWindow1Inside[4] = (value & 0x01) == 0;
                            ClipWindow1Inside[5] = (value & 0x10) == 0;
                            ClipWindow2Inside[4] = (value & 0x04) == 0;
                            ClipWindow2Inside[5] = (value & 0x40) == 0;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2126: // WH0
                        if (value != Memory.Memory.FillRAM[0x2126])
                        {
                            FLUSH_REDRAW();
                            Window1Left = value;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2127: // WH1
                        if (value != Memory.Memory.FillRAM[0x2127])
                        {
                            FLUSH_REDRAW();
                            Window1Right = value;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2128: // WH2
                        if (value != Memory.Memory.FillRAM[0x2128])
                        {
                            FLUSH_REDRAW();
                            Window2Left = value;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2129: // WH3
                        if (value != Memory.Memory.FillRAM[0x2129])
                        {
                            FLUSH_REDRAW();
                            Window2Right = value;
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x212a: // WBGLOG
                        if (value != Memory.Memory.FillRAM[0x212a])
                        {
                            FLUSH_REDRAW();
                            ClipWindowOverlapLogic[0] = (ClippingType)(value & 0x03);
                            ClipWindowOverlapLogic[1] = (ClippingType)((value & 0x0c) >> 2);
                            ClipWindowOverlapLogic[2] = (ClippingType)((value & 0x30) >> 4);
                            ClipWindowOverlapLogic[3] = (ClippingType)((value & 0xc0) >> 6);
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x212b: // WOBJLOG
                        if (value != Memory.Memory.FillRAM[0x212b])
                        {
                            FLUSH_REDRAW();
                            ClipWindowOverlapLogic[4] = (ClippingType)(value & 0x03);
                            ClipWindowOverlapLogic[5] = (ClippingType)((value & 0x0c) >> 2);
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x212c: // TM
                        if (value != Memory.Memory.FillRAM[0x212c])
                        {
                            FLUSH_REDRAW();
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x212d: // TS
                        if (value != Memory.Memory.FillRAM[0x212d])
                        {
                            FLUSH_REDRAW();
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x212e: // TMW
                        if (value != Memory.Memory.FillRAM[0x212e])
                        {
                            FLUSH_REDRAW();
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x212f: // TSW
                        if (value != Memory.Memory.FillRAM[0x212f])
                        {
                            FLUSH_REDRAW();
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2130: // CGWSEL
                        if (value != Memory.Memory.FillRAM[0x2130])
                        {
                            FLUSH_REDRAW();
                            RecomputeClipWindows = true;
                        }

                        break;

                    case 0x2131: // CGADSUB
                        if (value != Memory.Memory.FillRAM[0x2131])
                        {
                            FLUSH_REDRAW();
                        }

                        break;

                    case 0x2132: // COLDATA
                        if (value != Memory.Memory.FillRAM[0x2132])
                        {
                            FLUSH_REDRAW();
                            if ((value & 0x80) > 0)
                                FixedColourBlue = (byte)(value & 0x1f);
                            if ((value & 0x40) > 0)
                                FixedColourGreen = (byte)(value & 0x1f);
                            if ((value & 0x20) > 0)
                                FixedColourRed = (byte)(value & 0x1f);
                        }

                        break;

                    case 0x2133: // SETINI
                        if (value != Memory.Memory.FillRAM[0x2133])
                        {
                            if (((Memory.Memory.FillRAM[0x2133] ^ value) & 8) > 0)
                            {
                                FLUSH_REDRAW();
                                IPPU.PseudoHires = (value & 8) > 0;
                            }

                            if ((value & 0x04) > 0)
                            {
                                ScreenHeight = Constants.HEIGHT_EXTENDED;
                                if (IPPU.DoubleHeightPixels)
                                    IPPU.RenderedScreenHeight = ScreenHeight << 1;
                                else
                                    IPPU.RenderedScreenHeight = ScreenHeight;
                            }
                            else
                            {
                                ScreenHeight = Constants.HEIGHT;
                                if (IPPU.DoubleHeightPixels)
                                    IPPU.RenderedScreenHeight = ScreenHeight << 1;
                                else
                                    IPPU.RenderedScreenHeight = ScreenHeight;
                            }

                            if (((Memory.Memory.FillRAM[0x2133] ^ value) & 3) > 0)
                            {
                                FLUSH_REDRAW();
                                if (((Memory.Memory.FillRAM[0x2133] ^ value) & 2) > 0)
                                    IPPU.OBJChanged = true;

                                IPPU.Interlace = (value & 1) > 0;
                                IPPU.InterlaceOBJ = (value & 2) > 0;
                            }
                        }

                        break;

                    case 0x2180: // WMDATA
                        //if (!CPU.CPUState.InWRAMDMAorHDMA)
                            //REGISTER_2180(value);

                        break;

                    case 0x2181: // WMADDL
                        if (!CPU.CPUState.InWRAMDMAorHDMA)
                        {
                            WRAM &= 0x1ff00;
                            WRAM |= value;
                        }

                        break;

                    case 0x2182: // WMADDM
                        if (!CPU.CPUState.InWRAMDMAorHDMA)
                        {
                            WRAM &= 0x100ff;
                            WRAM |= (uint)(value << 8);
                        }

                        break;

                    case 0x2183: // WMADDH
                        if (!CPU.CPUState.InWRAMDMAorHDMA)
                        {
                            WRAM &= 0x0ffff;
                            WRAM |= (uint)(value << 16);
                            WRAM &= 0x1ffff;
                        }

                        break;

                    case 0x2134: // MPYL
                    case 0x2135: // MPYM
                    case 0x2136: // MPYH
                    case 0x2137: // SLHV
                    case 0x2138: // OAMDATAREAD
                    case 0x2139: // VMDATALREAD
                    case 0x213a: // VMDATAHREAD
                    case 0x213b: // CGDATAREAD
                    case 0x213c: // OPHCT
                    case 0x213d: // OPVCT
                    case 0x213e: // STAT77
                    case 0x213f: // STAT78
                        return;
                }
            }
            else
            {
                //if (Settings.SuperFX && Address >= 0x3000 && Address <= 0x32ff)
                //{
                //    S9xSetSuperFX(Byte, Address);
                //    return;
                //}
                //else
                //if (Settings.SA1 && Address >= 0x2200)
                //{
                //    if (Address <= 0x23ff)
                //        S9xSetSA1(Byte, Address);
                //    else
                //        Memory.FillRAM[Address] = Byte;
                //    return;
                //}
                //else
                //if (Settings.BS && Address >= 0x2188 && Address <= 0x219f)
                //    S9xSetBSXPPU(Byte, Address);
                //else
                //if (Settings.SRTC && Address == 0x2801)
                //    S9xSetSRTC(Byte, Address);
            }

            Memory.Memory.FillRAM[address] = value;
        }

        public static byte GetPPU(ushort address)
        {
            // MAP_PPU: $2000-$3FFF
            //if (Settings.MSU1 && (Address & 0xfff8) == 0x2000)
            //    return MSU1ReadPort(Address & 7);
            //else if (Address < 0x2100)
            //    return Memory.Memory.OpenBus;

            if (CPU.CPUState.InDMAorHDMA)
            {
                if (CPU.CPUState.CurrentDMAorHDMAChannel >= 0 && !DMA.DMA.SDMA[CPU.CPUState.CurrentDMAorHDMAChannel].ReverseTransfer)
                {
                    // S9xGetPPU() is called to read from DMA[].AAddress
                    if ((address & 0xff00) == 0x2100)
                        // Cannot access to Address Bus B ($2100-$21FF) via (H)DMA
                        return Memory.Memory.OpenBus;
                    else
                        // $2200-$3FFF are connected to Address Bus A
                        // SA1, SuperFX and SRTC are mapped here
                        // I don't bother for now...
                        return Memory.Memory.OpenBus;
                }
                else
                {
                    // S9xGetPPU() is called to write to $21xx
                    // Take care of DMA wrapping
                    if (address > 0x21ff)
                        address = (ushort)(0x2100 + (address & 0xff));
                }
            }

            if ((address & 0xffc0) == 0x2140) // APUIO0, APUIO1, APUIO2, APUIO3
                                              // read_port will run the APU until given APU time before reading value
                                              //return APUReadPort(address & 3);
                return 0;
            else if (address <= 0x2183)
            {
                byte value;
                switch (address)
                {
                    case 0x2104: // OAMDATA
                    case 0x2105: // BGMODE
                    case 0x2106: // MOSAIC
                    case 0x2108: // BG2SC
                    case 0x2109: // BG3SC
                    case 0x210a: // BG4SC
                    case 0x2114: // BG4VOFS
                    case 0x2115: // VMAIN
                    case 0x2116: // VMADDL
                    case 0x2118: // VMDATAL
                    case 0x2119: // VMDATAH
                    case 0x211a: // M7SEL
                    case 0x2124: // W34SEL
                    case 0x2125: // WOBJSEL
                    case 0x2126: // WH0
                    case 0x2128: // WH2
                    case 0x2129: // WH3
                    case 0x212a: // WBGLOG
                        return OpenBus1;

                    case 0x2134: // MPYL
                    case 0x2135: // MPYM
                    case 0x2136: // MPYH
                        if (Need16x8Mulitply)
                        {
                            var r = (MatrixA * (MatrixB >> 8));
                            Memory.Memory.FillRAM[0x2134] = (byte)r;
                            Memory.Memory.FillRAM[0x2135] = (byte)(r >> 8);
                            Memory.Memory.FillRAM[0x2136] = (byte)(r >> 16);
                            Need16x8Mulitply = false;
                        }
                        return OpenBus1 = Memory.Memory.FillRAM[address];

                    case 0x2137: // SLHV
                        //LatchCounters(0);
                        return OpenBus1;

                    case 0x2138: // OAMDATAREAD
                        if ((OAMAddr & 0x100) > 0)
                        {
                            if ((OAMFlip & 1) == 0)
                                value = OAMData[(OAMAddr & 0x10f) << 1];
                            else
                            {
                                value = OAMData[((OAMAddr & 0x10f) << 1) + 1];
                                OAMAddr = (ushort)((OAMAddr + 1) & 0x1ff);
                                if (OAMPriorityRotation > 0 && FirstSprite != (OAMAddr >> 1))
                                {
                                    FirstSprite = (byte)((OAMAddr & 0xfe) >> 1);
                                    IPPU.OBJChanged = true;
                                }
                            }
                        }
                        else
                        {
                            if ((OAMFlip & 1) == 0)
                                value = OAMData[OAMAddr << 1];
                            else
                            {
                                value = OAMData[(OAMAddr << 1) + 1];
                                ++OAMAddr;
                                if (OAMPriorityRotation > 0 && FirstSprite != (OAMAddr >> 1))
                                {
                                    FirstSprite = (byte)((OAMAddr & 0xfe) >> 1);
                                    IPPU.OBJChanged = true;
                                }
                            }
                        }

                        OAMFlip ^= 1;
                        return OpenBus1 = value;

                    case 0x2139: // VMDATALREAD
                        value = (byte)(VRAMReadBuffer & 0xff);
                        if (!VMA.High)
                        {
                            //UpdateVRAMReadBuffer();

                            VMA.Address += VMA.Increment;
                        }

                        return OpenBus1 = value;

                    case 0x213a: // VMDATAHREAD
                        value = (byte)((VRAMReadBuffer >> 8) & 0xff);
                        if (VMA.High)
                        {
                            //UpdateVRAMReadBuffer();

                            VMA.Address += VMA.Increment;
                        }
                        return OpenBus1 = value;

                    case 0x213b: // CGDATAREAD
                        if ((CGFLIPRead) > 0)
                            value = (byte)((OpenBus2 & 0x80) | ((CGDATA[CGADD++] >> 8) & 0x7f));
                        else
                            value = (byte)(CGDATA[CGADD] & 0xff);

                        CGFLIPRead ^= 1;
                        return OpenBus2 = value;

                    case 0x213c: // OPHCT
                        //TryGunLatch(false);
                        if (HBeamFlip > 0)
                            value = (byte)((OpenBus2 & 0xfe) | ((HBeamPosLatched >> 8) & 0x01));
                        else
                            value = (byte)HBeamPosLatched;

                        HBeamFlip ^= 1;
                        return OpenBus2 = value;

                    case 0x213d: // OPVCT
                        //TryGunLatch(false);
                        if (VBeamFlip > 0)
                            value = (byte)((OpenBus2 & 0xfe) | ((VBeamPosLatched >> 8) & 0x01));
                        else
                            value = (byte)VBeamPosLatched;

                        VBeamFlip ^= 1;
                        return OpenBus2 = value;

                    case 0x213e: // STAT77
                        FLUSH_REDRAW();
                        value = (byte)((OpenBus1 & 0x10) | RangeTimeOver | SnesModel._5C77);
                        return OpenBus1 = value;

                    case 0x213f: // STAT78
                        //TryGunLatch(false);
                        VBeamFlip = HBeamFlip = 0;
                        value = (byte)((OpenBus2 & 0x20) | (Memory.Memory.FillRAM[0x213f] & 0xc0) | (Settings.PAL ? 0x10 : 0) | SnesModel._5C78);
                        Memory.Memory.FillRAM[0x213f] = (byte)(Memory.Memory.FillRAM[0x213f] & ~0x40);
                        return OpenBus2 = value;

                    case 0x2180: // WMDATA
                        if (!CPU.CPUState.InWRAMDMAorHDMA)
                        {
                            value = Memory.Memory.RAM[(int) WRAM++];
                            WRAM &= 0x1ffff;
                        }
                        else
                            value = Memory.Memory.OpenBus;
                        return value;

                    default:
                        return Memory.Memory.OpenBus;
                }
            }
            else
            {
                //if (Settings.SuperFX && address >= 0x3000 && address <= 0x32ff)
                //    return GetSuperFX(address);
                //else
                //if (Settings.SA1 && address >= 0x2200)
                //    return GetSA1(address);
                //else
                //if (Settings.BS && address >= 0x2188 && address <= 0x219f)
                //    return GetBSXPPU(address);
                //else
                //if (Settings.SRTC && address == 0x2800)
                //    return GetSRTC(address);
                //else
                //    switch (address)
                //    {
                //        case 0x21c2:
                //            if (SnesModel._5C77 == 2)
                //                return 0x20;

                //            return Memory.Memory.OpenBus;

                //        case 0x21c3:
                //            if (SnesModel._5C77 == 2)
                //                return 0;

                //            return Memory.Memory.OpenBus;

                //        default:
                //            return Memory.Memory.OpenBus;
                //    }
                return 0;
            }
        }

        public static void SetCPU(byte value, ushort address)
        {
            if (address < 0x4200)
            {
                switch (address)
                {
                    case 0x4016: // JOYSER0
                        //SetJoypadLatch(value & 1);
                        break;

                    case 0x4017: // JOYSER1
                        return;

                    default:
                        break;
                }
            }
            else
            if ((address & 0xff80) == 0x4300)
            {
                if (CPU.CPUState.InDMAorHDMA)
                    return;

                int d = (address >> 4) & 0x7;

                switch (address & 0xf)
                {
                    case 0x0: // 0x43x0: DMAPx
                        DMA.DMA.SDMA[d].ReverseTransfer = (value & 0x80) > 0;
                        DMA.DMA.SDMA[d].HDMAIndirectAddressing = (value & 0x40) > 0;
                        DMA.DMA.SDMA[d].UnusedBit43x0 = (value & 0x20) > 0;
                        DMA.DMA.SDMA[d].AAddressDecrement = (value & 0x10) > 0;
                        DMA.DMA.SDMA[d].AAddressFixed = (value & 0x08) > 0;
                        DMA.DMA.SDMA[d].TransferMode = (byte)(value & 7);
                        return;

                    case 0x1: // 0x43x1: BBADx
                        DMA.DMA.SDMA[d].BAddress = value;
                        return;

                    case 0x2: // 0x43x2: A1TxL
                        DMA.DMA.SDMA[d].AAddress &= 0xff00;
                        DMA.DMA.SDMA[d].AAddress |= value;
                        return;

                    case 0x3: // 0x43x3: A1TxH
                        DMA.DMA.SDMA[d].AAddress &= 0xff;
                        DMA.DMA.SDMA[d].AAddress |= (ushort)(value << 8);
                        return;

                    case 0x4: // 0x43x4: A1Bx
                        DMA.DMA.SDMA[d].ABank = value;
                        HDMAMemPointers[d] = null;
                        return;

                    case 0x5: // 0x43x5: DASxL
                        DMA.DMA.SDMA[d].DMACount_Or_HDMAIndirectAddress &= 0xff00;
                        DMA.DMA.SDMA[d].DMACount_Or_HDMAIndirectAddress |= value;
                        HDMAMemPointers[d] = null;
                        return;

                    case 0x6: // 0x43x6: DASxH
                        DMA.DMA.SDMA[d].DMACount_Or_HDMAIndirectAddress &= 0xff;
                        DMA.DMA.SDMA[d].DMACount_Or_HDMAIndirectAddress |= (ushort)(value << 8);
                        HDMAMemPointers[d] = null;
                        return;

                    case 0x7: // 0x43x7: DASBx
                        DMA.DMA.SDMA[d].IndirectBank = value;
                        HDMAMemPointers[d] = null;
                        return;

                    case 0x8: // 0x43x8: A2AxL
                        DMA.DMA.SDMA[d].Address &= 0xff00;
                        DMA.DMA.SDMA[d].Address |= value;
                        HDMAMemPointers[d] = null;
                        return;

                    case 0x9: // 0x43x9: A2AxH
                        DMA.DMA.SDMA[d].Address &= 0xff;
                        DMA.DMA.SDMA[d].Address |= (ushort)(value << 8);
                        HDMAMemPointers[d] = null;
                        return;

                    case 0xa: // 0x43xa: NLTRx
                        if ((value & 0x7f) > 0)
                        {
                            DMA.DMA.SDMA[d].LineCount = (byte)(value & 0x7f);
                            DMA.DMA.SDMA[d].Repeat = (byte)((value & 0x80) == 0 ? 1 : 0);
                        }
                        else
                        {
                            DMA.DMA.SDMA[d].LineCount = 128;
                            DMA.DMA.SDMA[d].Repeat = (byte)(((value & 0x80) > 0) ? 1 : 0);
                        }

                        return;

                    case 0xb: // 0x43xb: ????x
                    case 0xf: // 0x43xf: mirror of 0x43xb
                        DMA.DMA.SDMA[d].UnknownByte = value;
                        return;

                    default:
                        break;
                }
            }
            else
            {
                ushort pos;

                switch (address)
                {
                    case 0x4200: // NMITIMEN

                        if (value == Memory.Memory.FillRAM[0x4200])
                            break;

                        if ((value & 0x20) > 0)
                            VTimerEnabled = true;
                        else
                            VTimerEnabled = false;

                        if ((value & 0x10) > 0)
                            HTimerEnabled = true;
                        else
                            HTimerEnabled = false;

                        if ((value & 0x10) == 0 && (value & 0x20) == 0)
                        {
                            CPU.CPUState.IRQLine = false;
                            CPU.CPUState.IRQTransition = false;
                        }

                        //if ((value & 0x30) != (Memory.Memory.FillRAM[0x4200] & 0x30))
                            //UpdateIRQPositions(true);

                        // NMI can trigger immediately during VBlank as long as NMI_read ($4210) wasn't cleard.
                        if ((value & 0x80) > 0 && (Memory.Memory.FillRAM[0x4200] & 0x80) == 0 &&
                            (CPU.CPUState.V_Counter >= ScreenHeight + Constants.FIRST_VISIBLE_LINE) && (Memory.Memory.FillRAM[0x4210] & 0x80) > 0)
                        {
                            // FIXME: triggered at HC+=6, checked just before the final CPU cycle,
                            // then, when to call S9xOpcode_NMI()?
                            CPU.CPUState.NMIPending = true;
                            Timings.Timings.NMITriggerPos = CPU.CPUState.Cycles + 6 + 6;

                        }

                        break;

                    case 0x4201: // WRIO
                        //if ((value & 0x80) == 0 && (Memory.Memory.FillRAM[0x4213] & 0x80) == 0x80)
                        //    LatchCounters(1);
                        //else
                        //    TryGunLatch((value & 0x80) > 0 ? true : false);
                        Memory.Memory.FillRAM[0x4201] = Memory.Memory.FillRAM[0x4213] = value;
                        break;

                    case 0x4202: // WRMPYA
                        break;

                    case 0x4203: // WRMPYB
                        {
                            var res = Memory.Memory.FillRAM[0x4202] * value;
                            // FIXME: The update occurs 8 machine cycles after $4203 is set.
                            Memory.Memory.FillRAM[0x4216] = (byte)res;
                            Memory.Memory.FillRAM[0x4217] = (byte)(res >> 8);
                            break;
                        }

                    case 0x4204: // WRDIVL
                    case 0x4205: // WRDIVH
                        break;

                    case 0x4206: // WRDIVB
                        {
                            var a = (ushort)(Memory.Memory.FillRAM[0x4204] + (Memory.Memory.FillRAM[0x4205] << 8));
                            var div = (ushort)(value > 0 ? a / value : 0xffff);
                            var rem = (ushort)(value > 0 ? a % value : a);
                            // FIXME: The update occurs 16 machine cycles after $4206 is set.
                            Memory.Memory.FillRAM[0x4214] = (byte)div;
                            Memory.Memory.FillRAM[0x4215] = (byte)(div >> 8);
                            Memory.Memory.FillRAM[0x4216] = (byte)rem;
                            Memory.Memory.FillRAM[0x4217] = (byte)(rem >> 8);
                            break;
                        }

                    case 0x4207: // HTIMEL
                        pos = IRQHBeamPos;
                        IRQHBeamPos = (ushort)((IRQHBeamPos & 0xff00) | value);
                        //if (IRQHBeamPos != pos)
                        //    UpdateIRQPositions(false);
                        break;

                    case 0x4208: // HTIMEH
                        pos = IRQHBeamPos;
                        IRQHBeamPos = (ushort)((IRQHBeamPos & 0xff) | ((value & 1) << 8));
                        //if (IRQHBeamPos != pos)
                        //    UpdateIRQPositions(false);
                        break;

                    case 0x4209: // VTIMEL
                        pos = IRQVBeamPos;
                        IRQVBeamPos = (ushort)((IRQVBeamPos & 0xff00) | value);
                        //if (IRQVBeamPos != pos)
                        //    UpdateIRQPositions(true);
                        break;

                    case 0x420a: // VTIMEH
                        pos = IRQVBeamPos;
                        IRQVBeamPos = (ushort)((IRQVBeamPos & 0xff) | ((value & 1) << 8));
                        //if (IRQVBeamPos != pos)
                        //    UpdateIRQPositions(true);
                        break;

                    case 0x420b: // MDMAEN
                        if (CPU.CPUState.InDMAorHDMA)
                            return;

                        // XXX: Not quite right...
                        if (value > 0)
                            CPU.CPUState.Cycles += Timings.Timings.DMACPUSync;

                        //if ((value & 0x01) > 0)
                        //    DoDMA(0);
                        //if ((value & 0x02) > 0)
                        //    DoDMA(1);
                        //if ((value & 0x04) > 0)
                        //    DoDMA(2);
                        //if ((value & 0x08) > 0)
                        //    DoDMA(3);
                        //if ((value & 0x10) > 0)
                        //    DoDMA(4);
                        //if ((value & 0x20) > 0)
                        //    DoDMA(5);
                        //if ((value & 0x40) > 0)
                        //    DoDMA(6);
                        //if ((value & 0x80) > 0)
                        //    DoDMA(7);
                        break;

                    case 0x420c: // HDMAEN
                        if (CPU.CPUState.InDMAorHDMA)
                            return;

                        Memory.Memory.FillRAM[0x420c] = value;
                        // Yoshi's Island, Genjyu Ryodan, Mortal Kombat, Tales of Phantasia
                        HDMA = (byte)(value & ~HDMAEnded);
                        break;

                    case 0x420d: // MEMSEL
                        if ((value & 1) != (Memory.Memory.FillRAM[0x420d] & 1))
                        {
                            if ((value & 1) > 0)
                            {
                                CPU.CPUState.FastROMSpeed = CPU.Constants.ONE_CYCLE;
                            }
                            else
                                CPU.CPUState.FastROMSpeed = CPU.Constants.SLOW_ONE_CYCLE;
                            // we might currently be in FastROMSpeed region, S9xSetPCBase will update CPU.MemSpeed
                            Memory.Memory.SetPCBase(CPU.Registers.PBPC);
                        }

                        break;

                    case 0x4210: // RDNMI
                    case 0x4211: // TIMEUP
                    case 0x4212: // HVBJOY
                    case 0x4213: // RDIO
                    case 0x4214: // RDDIVL
                    case 0x4215: // RDDIVH
                    case 0x4216: // RDMPYL
                    case 0x4217: // RDMPYH
                    case 0x4218: // JOY1L
                    case 0x4219: // JOY1H
                    case 0x421a: // JOY2L
                    case 0x421b: // JOY2H
                    case 0x421c: // JOY3L
                    case 0x421d: // JOY3H
                    case 0x421e: // JOY4L
                    case 0x421f: // JOY4H
                        return;

                    default:
                        //if (Settings.SPC7110 && address >= 0x4800)
                        //    SetSPC7110(value, address);
                        //else if (Settings.SDD1 && address >= 0x4804 && address <= 0x4807)
                        //    SetSDD1MemoryMap(address - 0x4804, value & 7);
                        break;
                }
            }

            Memory.Memory.FillRAM[address] = value;
        }

        public static byte GetCPU(ushort address)
        {
            if (address < 0x4200)
            {
		        bool pad_read;
		        if (address == 0x4016 || address == 0x4017)
		        {
			        //OnSNESPadRead();
                    pad_read = true;
		        }

		        switch (address)
		        {
			        //case 0x4016: // JOYSER0
			        //case 0x4017: // JOYSER1
				       // return ReadJOYSERn(address);

			        default:
				        return Memory.Memory.OpenBus;
		        }
	        }
	        else if ((address & 0xff80) == 0x4300)
	        {
		        if (CPU.CPUState.InDMAorHDMA)
			        return Memory.Memory.OpenBus;

		        var d = (address >> 4) & 0x7;

		        switch (address & 0xf)
		        {
			        case 0x0: // 0x43x0: DMAPx
				        return (byte)((DMA.DMA.SDMA[d].ReverseTransfer? 0x80 : 0) |
						        (DMA.DMA.SDMA[d].HDMAIndirectAddressing? 0x40 : 0) |
						        (DMA.DMA.SDMA[d].UnusedBit43x0? 0x20 : 0) |
						        (DMA.DMA.SDMA[d].AAddressDecrement? 0x10 : 0) |
						        (DMA.DMA.SDMA[d].AAddressFixed? 0x08 : 0) |
						        (DMA.DMA.SDMA[d].TransferMode & 7));

			        case 0x1: // 0x43x1: BBADx
				        return DMA.DMA.SDMA[d].BAddress;

			        case 0x2: // 0x43x2: A1TxL
				        return (byte)(DMA.DMA.SDMA[d].AAddress & 0xff);

			        case 0x3: // 0x43x3: A1TxH
				        return (byte)(DMA.DMA.SDMA[d].AAddress >> 8);

			        case 0x4: // 0x43x4: A1Bx
				        return DMA.DMA.SDMA[d].ABank;

			        case 0x5: // 0x43x5: DASxL
				        return (byte)(DMA.DMA.SDMA[d].DMACount_Or_HDMAIndirectAddress & 0xff);

			        case 0x6: // 0x43x6: DASxH
				        return (byte)(DMA.DMA.SDMA[d].DMACount_Or_HDMAIndirectAddress >> 8);

			        case 0x7: // 0x43x7: DASBx
				        return DMA.DMA.SDMA[d].IndirectBank;

			        case 0x8: // 0x43x8: A2AxL
				        return (byte)(DMA.DMA.SDMA[d].Address & 0xff);

			        case 0x9: // 0x43x9: A2AxH
				        return (byte)(DMA.DMA.SDMA[d].Address >> 8);

			        case 0xa: // 0x43xa: NLTRx
				        return (byte)(DMA.DMA.SDMA[d].LineCount ^ (DMA.DMA.SDMA[d].Repeat > 0 ? 0x00 : 0x80));

			        case 0xb: // 0x43xb: ????x
			        case 0xf: // 0x43xf: mirror of 0x43xb
				        return DMA.DMA.SDMA[d].UnknownByte;

			        default:
				        return Memory.Memory.OpenBus;
		        }
	        }
	        else
	        {
		        byte value;

		        switch (address)
		        {
			        case 0x4210: // RDNMI
                        value = Memory.Memory.FillRAM[0x4210];
                        Memory.Memory.FillRAM[0x4210] = SnesModel._5A22;
				        return (byte)((value & 0x80) | (Memory.Memory.OpenBus & 0x70) | SnesModel._5A22);

			        case 0x4211: // TIMEUP
                        value = 0;
				        if (CPU.CPUState.IRQLine)
				        {
                            value = 0x80;
					        CPU.CPUState.IRQLine = false;
					        CPU.CPUState.IRQTransition = false;
				        }

				        return (byte)(value | (Memory.Memory.OpenBus & 0x7f));

			        //case 0x4212: // HVBJOY
				       // return REGISTER_4212() | (Memory.Memory.OpenBus & 0x3e);

			        case 0x4213: // RDIO
				        return Memory.Memory.FillRAM[0x4213];

			        case 0x4214: // RDDIVL
			        case 0x4215: // RDDIVH
			        case 0x4216: // RDMPYL
			        case 0x4217: // RDMPYH
				        return Memory.Memory.FillRAM[address];

			        case 0x4218: // JOY1L
			        case 0x4219: // JOY1H
			        case 0x421a: // JOY2L
			        case 0x421b: // JOY2H
			        case 0x421c: // JOY3L
			        case 0x421d: // JOY3H
			        case 0x421e: // JOY4L
			        case 0x421f: // JOY4H
				        bool pad_read;
				        if ((Memory.Memory.FillRAM[0x4200] & 1) > 0)
				        {
					        //OnSNESPadRead();
                            pad_read = true;
				        }
				        return Memory.Memory.FillRAM[address];

			        default:
				        //if (Settings.SPC7110 && address >= 0x4800)
					       // return GetSPC7110(address);

				        if (Settings.SDD1 && address >= 0x4800 && address <= 0x4807)
					        return Memory.Memory.FillRAM[address];

				        return Memory.Memory.OpenBus;
		        }
	        }
        }

        public static ushort BuildPixel(uint R, uint G, uint B)
        {
            return (ushort)((R << 10) + (G << 5) + B);
        }

        public static void SetUIColor(uint R, uint G, uint B)
        {
            //TODO
        }

        private static void SoftResetPPU()
        {
            //ControlsSoftReset();

            VMA.High = false;
            VMA.Increment = 1;
            VMA.Address = 0;
            VMA.FullGraphicCount = 0;
            VMA.Shift = 0;

            WRAM = 0;

            for (var c = 0; c < 4; c++)
                BG[c] = new BackGround();

            BGMode = 0;
            BG3Priority = 0;

            CGFLIP = false;
            CGFLIPRead = 0;
            CGADD = 0;

            for (var c = 0; c < 256; c++)
            {
                IPPU.Red[c] = (uint)((c & 7) << 2);
                IPPU.Green[c] = (uint)(((c >> 3) & 7) << 2);
                IPPU.Blue[c] = (uint)(((c >> 6) & 2) << 3);
                CGDATA[c] = (ushort)(IPPU.Red[c] | (IPPU.Green[c] << 5) | (IPPU.Blue[c] << 10));
            }

            for (var c = 0; c < 128; c++)
                OBJ[c] = new ScreenObject();

            OBJThroughMain = false;
            OBJThroughSub = false;
            OBJAddition = false;
            OBJNameBase = 0;
            OBJNameSelect = 0;
            OBJSizeSelect = 0;

            OAMAddr = 0;
            SavedOAMAddr = 0;
            OAMPriorityRotation = 0;
            OAMFlip = 0;
            OAMReadFlip = 0;
            OAMTileAddress = 0;
            OAMWriteRegister = 0;
            OAMData = new byte[512 + 32];

            FirstSprite = 0;
            LastSprite = 127;
            RangeTimeOver = 0;

            HTimerEnabled = false;
            VTimerEnabled = false;
            HTimerPosition = (short)(Timings.Timings.H_Max + 1);
            VTimerPosition = (short)(Timings.Timings.V_Max + 1);
            IRQHBeamPos = 0x1ff;
            IRQVBeamPos = 0x1ff;

            HBeamFlip = 0;
            VBeamFlip = 0;
            HBeamPosLatched = 0;
            VBeamPosLatched = 0;
            GunHLatch = 0;
            GunVLatch = 1000;
            HVBeamCounterLatched = 0;

            Mode7HFlip = false;
            Mode7VFlip = false;
            Mode7Repeat = 0;
            MatrixA = 0;
            MatrixB = 0;
            MatrixC = 0;
            MatrixD = 0;
            CentreX = 0;
            CentreY = 0;

            Mosaic = 0;
            BGMosaic[0] = false;
            BGMosaic[1] = false;
            BGMosaic[2] = false;
            BGMosaic[3] = false;

            Window1Left = 1;
            Window1Right = 0;
            Window2Left = 1;
            Window2Right = 0;
            RecomputeClipWindows = true;

            for (var c = 0; c < 6; c++)
            {
                ClipCounts[c] = 0;
                ClipWindowOverlapLogic[c] = ClippingType.CLIP_OR;
                ClipWindow1Enable[c] = false;
                ClipWindow2Enable[c] = false;
                ClipWindow1Inside[c] = true;
                ClipWindow2Inside[c] = true;
            }

            ForcedBlanking = true;

            FixedColourRed = 0;
            FixedColourGreen = 0;
            FixedColourBlue = 0;
            Brightness = 0;
            ScreenHeight = Constants.HEIGHT;

            Need16x8Mulitply = false;
            BGnxOFSbyte = 0;

            HDMA = 0;
            HDMAEnded = 0;

            OpenBus1 = 0;
            OpenBus2 = 0;

            IPPU.Clip = new ClipData[2, 6];
            IPPU.ColorsChanged = true;
            IPPU.OBJChanged = true;
            IPPU.TileCached = new byte[7][] {
                new byte[Constants.MAX_2BIT_TILES],
                new byte[Constants.MAX_4BIT_TILES],
                new byte[Constants.MAX_8BIT_TILES],
                new byte[Constants.MAX_2BIT_TILES],
                new byte[Constants.MAX_2BIT_TILES],
                new byte[Constants.MAX_4BIT_TILES],
                new byte[Constants.MAX_4BIT_TILES]
            };
            VRAMReadBuffer = 0; // XXX: FIXME: anything better?
                                //GFX.InterlaceFrame = 0;
            IPPU.Interlace = false;
            IPPU.InterlaceOBJ = false;
            IPPU.DoubleWidthPixels = false;
            IPPU.DoubleHeightPixels = false;
            IPPU.CurrentLine = 0;
            IPPU.PreviousLine = 0;
            IPPU.XB = null;
            for (var c = 0; c < 256; c++)
                IPPU.ScreenColors[c] = (byte)c;

            IPPU.MaxBrightness = 0;
            IPPU.RenderThisFrame = true;
            IPPU.RenderedScreenWidth = Constants.WIDTH;
            IPPU.RenderedScreenHeight = Constants.HEIGHT;
            IPPU.FrameCount = 0;
            IPPU.RenderedFramesCount = 0;
            IPPU.DisplayedRenderedFrameCount = 0;
            IPPU.SkippedFrames = 0;
            IPPU.FrameSkip = 0;

            FixColourBrightness();
            BuildDirectColourMaps();

            for (int c = 0; c < 0x8000; c += 0x100)
                for (int j = 0; j < 0x100; j++)
                    Memory.Memory.FillRAM[c + j] = (byte)(c >> 8);

            for (int j = 0; j < 0x100; j++)
                Memory.Memory.FillRAM[0x2100 + j] = 0;

            for (int j = 0; j < 0x100; j++)
                Memory.Memory.FillRAM[0x4200 + j] = 0;

            for (int j = 0; j < 0x100; j++)
                Memory.Memory.FillRAM[0x4000 + j] = 0;

            for (int j = 0; j < 0x100; j++)
                Memory.Memory.FillRAM[0x1000 + j] = 0;

            Memory.Memory.FillRAM[0x4201] = Memory.Memory.FillRAM[0x4213] = 0xff;
            Memory.Memory.FillRAM[0x2126] = Memory.Memory.FillRAM[0x2128] = 1;
        }

        private static void FixColourBrightness()
        {
            IPPU.XB = Constants.mul_brightness[Brightness];

            for (int i = 0; i < 256; i++)
            {
                IPPU.Red[i] = IPPU.XB[(CGDATA[i]) & 0x1f];
                IPPU.Green[i] = IPPU.XB[(CGDATA[i] >> 5) & 0x1f];
                IPPU.Blue[i] = IPPU.XB[(CGDATA[i] >> 10) & 0x1f];
                IPPU.ScreenColors[i] = BuildPixel(IPPU.Red[i], IPPU.Green[i], IPPU.Blue[i]);
            }
        }

        private static void BuildDirectColourMaps()
        {
            IPPU.XB = Constants.mul_brightness[Brightness];

            for (int p = 0; p < 8; p++)
                for (int c = 0; c < 256; c++)
                    DirectColourMaps[p,c] = BuildPixel(IPPU.XB[((c & 7) << 2) | ((p & 1) << 1)], IPPU.XB[((c & 0x38) >> 1) | (p & 2)], IPPU.XB[((c & 0xc0) >> 3) | (p & 4)]);
        }

        private static void FLUSH_REDRAW()
        {

        }
    }
}