using SuperNintendo.Core.SFX;
using System;
using System.IO;

namespace SuperNintendo.Core.Memory
{
    public static class Memory
    {
        #region Public Members

        private static int RAMPosition;
        private static int ROMPosition;
        private static int SRAMPosition;
        private static int VRAMPosition;
        private static int FillRAMPosition; //maps to rom
        private static int BWRAMPosition;
        private static int C4RAMPosition;
        private static int OBC1RAMPosition;
        private static int BSRAMPosition;
        private static int BIOSROM;
        private static byte ROMRegion;
        private static byte ROMSpeed;
        private static byte ROMType;
        private static byte ROMSize;
        private static byte SRAMSize;
        private static uint SRAMMask;

        private static int[] Map = new int[Constants.NUM_BLOCKS];
        private static int[] WriteMap = new int[Constants.NUM_BLOCKS];
        private static bool[] BlockIsRAM = new bool[Constants.NUM_BLOCKS];
        private static bool[] BlockIsROM = new bool[Constants.NUM_BLOCKS];
        private static int ROMFramesPerSecond;

        private static bool LoROM;
        private static int CalculatedSize;

        public static Action PostRomInitFunc;

        public static byte[] RAM = new byte[0x20000];
        public static byte[] SRAM = new byte[0x20000];
        public static byte[] VRAM = new byte[0x10000];
        public static byte[] ROM = new byte[Constants.MAX_ROM_SIZE + 0x200 + 0x8000];

        #endregion

        #region Public Methods

        public static bool Init()
        {
            ROMPosition = 0;
            SRAMPosition = 0;
            VRAMPosition = 0;
            RAMPosition = 0;
            FillRAMPosition = 0;
            ROMPosition += 0x8000;
            C4RAMPosition = ROMPosition + 0x400000 + 8192 * 8;
            OBC1RAMPosition = ROMPosition + 0x400000;
            BIOSROM = ROMPosition + 0x300000;
            BSRAMPosition = ROMPosition + 0x400000;

            SuperFX.pvRegisterPosition = FillRAMPosition + 0x3000;
            SuperFX.nRamBanks = 2; // Most only use 1.  1=64KB=512Mb, 2=128KB=1024Mb
            SuperFX.pvRamPosition = SRAMPosition;
            SuperFX.nRomBanks = (2 * 1024 * 1024) / (32 * 1024);
            SuperFX.pvRomPosition = ROMPosition;

            PostRomInitFunc = null;
            return true;
        }

        public static void LoadROM(Stream stream)
        {
            ROM = new byte[0x8000 + stream.Length];
            stream.Read(ROM, ROMPosition, (int) stream.Length);
            LoadROM((int) stream.Length);
        }

        private static void LoadROM(int ROMfillSize)
        {
            Settings.DisplayColor = GUI.BuildPixel(31, 31, 31);
            GUI.SetUIColor(255, 255, 255);

            CalculatedSize = 0;

            int hi_score, lo_score;
            int score_headered;
            int score_nonheadered;

            hi_score = ScoreHiROM(false);
            lo_score = ScoreLoROM(false);
            score_nonheadered = Math.Max(hi_score, lo_score);
            score_headered = Math.Max(ScoreHiROM(true), ScoreLoROM(true));

            var size_is_likely_headered = ((ROMfillSize - 512) & 0xFFFF) == 0;
            if (size_is_likely_headered)
                score_headered += 2;
            else
                score_headered -= 2;

            if (First512BytesCountZeroes() >= 0x1E0)
                score_headered += 2;
            else
                score_headered -= 2;

            var headered_score_highest = score_headered > score_nonheadered;

            CalculatedSize = (ROMfillSize / 0x2000) * 0x2000;

            // CalculatedSize is now set, so rescore
            hi_score = ScoreHiROM(false);
            lo_score = ScoreLoROM(false);

            if (lo_score >= hi_score)
                LoROM = true;
            else
                LoROM = false;

            InitROM();
            Reset();
        }

        private static int ScoreLoROM(bool skip_header)
        {
            var position = ROMPosition + 0x7f00 + (skip_header ? 0x200 : 0);
            int score = 0;

            if ((ROM[position + 0xd5] & 0x1) == 0)
                score += 3;

            // Mode23 is SA-1
            if (ROM[position + 0xd5] == 0x23)
                score += 2;

            if ((ROM[position + 0xdc] + (ROM[position + 0xdd] << 8)) + (ROM[position + 0xde] + (ROM[position + 0xdf] << 8)) == 0xffff)
            {
                score += 2;
                if (0 != (ROM[position + 0xde] + (ROM[position + 0xdf] << 8)))
                    score++;
            }

            if (ROM[position + 0xda] == 0x33)
                score += 2;

            if ((ROM[position + 0xd5] & 0xf) < 4)
                score += 2;

            if ((ROM[position + 0xfd] & 0x80) == 0)
                score -= 6;

            if ((ROM[position + 0xfc] + (ROM[position + 0xfd] << 8)) > 0xffb0)
                score -= 2; // reduced per Cowering suggestion

            if (CalculatedSize <= 1024 * 1024 * 16)
                score += 2;

            if ((1 << (ROM[position + 0xd7] - 7)) > 48)
                score -= 1;

            if (!AllASCII(position + 0xb0, 6))
                score -= 1;

            if (!AllASCII(position + 0xc0, Constants.ROM_NAME_LEN - 1))
                score -= 1;

            return score;
        }

        private static int ScoreHiROM(bool skip_header)
        {
            var position = 0xff00 + ROMPosition + (skip_header ? 0x200 : 0);
            int score = 0;

            if ((ROM[position + 0xd5] & 0x1) > 0)
                score += 2;

            // Mode23 is SA-1
            if (ROM[position + 0xd5] == 0x23)
                score -= 2;

            if (ROM[position + 0xd4] == 0x20)
                score += 2;

            if ((ROM[position + 0xdc] + (ROM[position + 0xdd] << 8)) + (ROM[position + 0xde] + (ROM[position + 0xdf] << 8)) == 0xffff)
            {
                score += 2;
                if (0 != (ROM[position + 0xde] + (ROM[position + 0xdf] << 8)))
                    score++;
            }

            if (ROM[position + 0xda] == 0x33)
                score += 2;

            if ((ROM[position + 0xd5] & 0xf) < 4)
                score += 2;

            if ((ROM[position + 0xfd] & 0x80) == 0)
                score -= 6;

            if ((ROM[position + 0xfc] + (ROM[position + 0xfd] << 8)) > 0xffb0)
                score -= 2; // reduced after looking at a scan by Cowering

            if (CalculatedSize > 1024 * 1024 * 3)
                score += 4;

            if ((1 << (ROM[position + 0xd7] - 7)) > 48)
                score -= 1;

            if (!AllASCII(position + 0xb0, 6))
                score -= 1;

            if (!AllASCII(position +0xc0, Constants.ROM_NAME_LEN - 1))
                score -= 1;

            return score;
        }

        private static bool AllASCII(int position, int length)
        {
            for (int i = 0; i < length; i++)
                if (ROM[position + i] < 32 || ROM[position + i] > 126)
                    return false;

            return true;
        }

        private static int First512BytesCountZeroes()
        {
            var position = ROMPosition;
            var zeroCount = 0;
	        for (int i = 0; i< 512; i++)
		        if (ROM[position + i] == 0)
			        zeroCount++;

	        return zeroCount;
        }

        private static void InitBSX()
        {
            Settings.BSXItself = false;
            Settings.BS = false;
        }

        private static void ParseSNESHeader(int position)
        {
            ROMSize = ROM[position + 0x27];
            SRAMSize = ROM[position + 0x28];
            ROMSpeed = ROM[position + 0x25];
            ROMType = ROM[position + 0x26];
            ROMRegion = ROM[position + 0x29];
        }

        private static void MapInitialize()
        {
            for (int c = 0; c < 0x1000; c++)
            {
                Map[c] = (int) MappingType.MAP_NONE;
                WriteMap[c] = (int)MappingType.MAP_NONE;
                BlockIsROM[c] = false;
                BlockIsRAM[c] = false;
            }
        }

        private static void MapSpace(uint bank_s, uint bank_e, uint addr_s, uint addr_e, int dataPosition)
        {
            uint c, i, p;

            for (c = bank_s; c <= bank_e; c++)
            {
                for (i = addr_s; i <= addr_e; i += 0x1000)
                {
                    p = (c << 4) | (i >> 12);
                    Map[p] = dataPosition;
                    BlockIsROM[p] = false;
                    BlockIsRAM[p] = true;
                }
            }
        }

        private static void MapIndex(uint bank_s, uint bank_e, uint addr_s, uint addr_e, MappingType index, MapType type)
        {
            uint c, i, p;
            bool isROM, isRAM;

            isROM = ((type == MapType.MAP_TYPE_I_O) || (type == MapType.MAP_TYPE_RAM)) ? false : true;
            isRAM = ((type == MapType.MAP_TYPE_I_O) || (type == MapType.MAP_TYPE_ROM)) ? false : true;

            for (c = bank_s; c <= bank_e; c++)
            {
                for (i = addr_s; i <= addr_e; i += 0x1000)
                {
                    p = (c << 4) | (i >> 12);
                    Map[p] = (int) index;
                    BlockIsROM[p] = isROM;
                    BlockIsRAM[p] = isRAM;
                }
            }
        }

        private static void MapWRAM()
        {
            // will overwrite others
            MapSpace(0x7e, 0x7e, 0x0000, 0xffff, RAMPosition);
            MapSpace(0x7f, 0x7f, 0x0000, 0xffff, RAMPosition + 0x10000);
        }

        private static void MapSystem()
        {
            // will be overwritten
            MapSpace(0x00, 0x3f, 0x0000, 0x1fff, RAMPosition);
            MapIndex(0x00, 0x3f, 0x2000, 0x3fff, MappingType.MAP_PPU, MapType.MAP_TYPE_I_O);
            MapIndex(0x00, 0x3f, 0x4000, 0x5fff, MappingType.MAP_CPU, MapType.MAP_TYPE_I_O);
            MapSpace(0x80, 0xbf, 0x0000, 0x1fff, RAMPosition);
            MapIndex(0x80, 0xbf, 0x2000, 0x3fff, MappingType.MAP_PPU, MapType.MAP_TYPE_I_O);
            MapIndex(0x80, 0xbf, 0x4000, 0x5fff, MappingType.MAP_CPU, MapType.MAP_TYPE_I_O);
        }

        private static uint MapMirror(uint size, uint pos)
        {
            // from bsnes
            if (size == 0)
                return (0);

            if (pos < size)
                return (pos);

            uint mask = (uint) 1 << 31;
            while ((pos & mask) == 0)
                mask >>= 1;

            if (size <= (pos & mask))
                return MapMirror(size, pos - mask);
            else
                return mask + MapMirror(size - mask, pos - mask);
        }

        private static void MapLoROM(uint bank_s, uint bank_e, uint addr_s, uint addr_e, uint size)
        {
            uint c, i, p, addr;

            for (c = bank_s; c <= bank_e; c++)
            {
                for (i = addr_s; i <= addr_e; i += 0x1000)
                {
                    p = (c << 4) | (i >> 12);
                    addr = (c & 0x7f) * 0x8000;
                    Map[p] = (int) (ROMPosition + MapMirror(size, addr) - (i & 0x8000));
                    BlockIsROM[p] = true;
                    BlockIsRAM[p] = true;
                }
            }
        }

        private static void MapLoROMSRAM()
        {
            uint hi;

            if (SRAMSize == 0)
                return;

            if (ROMSize > 11 || SRAMSize > 5)
                hi = 0x7fff;
            else
                hi = 0xffff;

            MapIndex(0x70, 0x7d, 0x0000, hi, MappingType.MAP_LOROM_SRAM, MapType.MAP_TYPE_RAM);
            MapIndex(0xf0, 0xff, 0x0000, hi, MappingType.MAP_LOROM_SRAM, MapType.MAP_TYPE_RAM);
        }

        private static void MapWriteProtectROM()
        {
            for (var i = 0; i < Map.Length; i++)
                WriteMap[i] = Map[i];

            for (int c = 0; c < 0x1000; c++)
                if (BlockIsROM[c])
                    WriteMap[c] = (int) MappingType.MAP_NONE;
        }

        private static void MapLoROMMap()
        {
            MapSystem();

            MapLoROM(0x00, 0x3f, 0x8000, 0xffff, (uint) CalculatedSize);
            MapLoROM(0x40, 0x7f, 0x0000, 0xffff, (uint) CalculatedSize);
            MapLoROM(0x80, 0xbf, 0x8000, 0xffff, (uint) CalculatedSize);
            MapLoROM(0xc0, 0xff, 0x0000, 0xffff, (uint) CalculatedSize);

            MapLoROMSRAM();
            MapWRAM();

            MapWriteProtectROM();
        }

        private static int InitROM()
        {
            Settings.SuperFX = false;
            Settings.DSP = 0;
            Settings.SA1 = false;
            Settings.C4 = false;
            Settings.SDD1 = false;
            Settings.SPC7110 = false;
            Settings.SPC7110RTC = false;
            Settings.OBC1 = false;
            Settings.SETA = 0;
            Settings.SRTC = false;
            Settings.BS = false;
            Settings.MSU1 = false;

            SuperFX.nRomBanks = CalculatedSize >> 15;

            //// Parse ROM header and read ROM informatoin
            InitBSX(); // Set BS header before parsing

            var romHeaderPosition = ROMPosition + 0x7FB0;
            ParseSNESHeader(romHeaderPosition);

            //// Detect and initialize chips
            //// detection codes are compatible with NSRT

            // DSP1/2/3/4
            DSP.DSP.SetDSP = null;
            DSP.DSP.GetDSP = null;

            // MSU1
            Settings.MSU1 = false;

            //// Map memory and calculate checksum

            MapInitialize();
            MapLoROMMap();

            Settings.PAL = false;
            Settings.FrameTime = Settings.FrameTimeNTSC;
            ROMFramesPerSecond = 60;

            // SRAM size
            SRAMMask = SRAMSize > 0 ? (uint) ((1 << (SRAMSize + 3)) * 128) - 1 : 0;

            //// Initialize emulation

            Timings.H_Max_Master = SNES_CYCLES_PER_SCANLINE;
            Timings.H_Max = Timings.H_Max_Master;
            Timings.HBlankStart = SNES_HBLANK_START_HC;
            Timings.HBlankEnd = SNES_HBLANK_END_HC;
            Timings.HDMAInit = SNES_HDMA_INIT_HC;
            Timings.HDMAStart = SNES_HDMA_START_HC;
            Timings.RenderPos = SNES_RENDER_START_HC;
            Timings.V_Max_Master = Settings.PAL ? SNES_MAX_PAL_VCOUNTER : SNES_MAX_NTSC_VCOUNTER;
            Timings.V_Max = Timings.V_Max_Master;
            /* From byuu: The total delay time for both the initial (H)DMA sync (to the DMA clock),
               and the end (H)DMA sync (back to the last CPU cycle's mcycle rate (6, 8, or 12)) always takes between 12-24 mcycles.
               Possible delays: { 12, 14, 16, 18, 20, 22, 24 }
               XXX: Snes9x can't emulate this timing :( so let's use the average value... */
            Timings.DMACPUSync = 18;
            /* If the CPU is halted (i.e. for DMA) while /NMI goes low, the NMI will trigger
               after the DMA completes (even if /NMI goes high again before the DMA
               completes). In this case, there is a 24-30 cycle delay between the end of DMA
               and the NMI handler, time enough for an instruction or two. */
            // Wild Guns, Mighty Morphin Power Rangers - The Fighting Edition
            Timings.NMIDMADelay = 24;

            IPPU.TotalEmulatedFrames = 0;

            //// Hack games

            ApplyROMFixes();

            //// Show ROM information
            char displayName[ROM_NAME_LEN];

            strcpy(RawROMName, ROMName);
            sprintf(displayName, "%s", SafeANK(ROMName));
            sprintf(ROMName, "%s", Safe(ROMName));
            sprintf(ROMId, "%s", Safe(ROMId));

            sprintf(String, "\"%s\" [%s] %s, %s, %s, %s, SRAM:%s, ID:%s, CRC32:%08X",
                displayName, isChecksumOK ? "checksum ok" : ((Multi.cartType == 4) ? "no checksum" : "bad checksum"),
                MapType(), Size(), KartContents(), Settings.PAL ? "PAL" : "NTSC", StaticRAMSize(), ROMId, ROMCRC32);
            S9xMessage(S9X_INFO, S9X_ROM_INFO, String);

            Settings.ForceLoROM = FALSE;
            Settings.ForceHiROM = FALSE;
            Settings.ForceHeader = FALSE;
            Settings.ForceNoHeader = FALSE;
            Settings.ForceInterleaved = FALSE;
            Settings.ForceInterleaved2 = FALSE;
            Settings.ForceInterleaveGD24 = FALSE;
            Settings.ForceNotInterleaved = FALSE;
            Settings.ForcePAL = FALSE;
            Settings.ForceNTSC = FALSE;

            Settings.TakeScreenshot = FALSE;

            if (stopMovie)
                S9xMovieStop(TRUE);

            if (PostRomInitFunc)
                PostRomInitFunc();

            S9xVerifyControllers();
        }

        private static int Reset()
        {

        }

        ///// <summary>
        ///// get word from address
        ///// </summary>
        //public static UInt16 objGet16(UInt32 intAddress)
        //{
        //    return (UInt16) (objGetByte(intAddress) | (objGetByte(intAddress+1) << 8));
        //}

        ///// <summary>
        ///// get byte
        ///// </summary>
        ///// <param name="intAddress">addres to get</param>
        ///// <returns>byte at place</returns>
        //public static Byte objGetByte(UInt32 intAddress)
        //{
        //    return _objMemory[intAddress];
        //}

        ///// <summary>
        ///// memory move
        ///// </summary>
        ///// <param name="arr1"></param>
        ///// <param name="?"></param>
        ///// <param name="arr2"></param>
        ///// <param name="?"></param>
        ///// <param name="?"></param>
        //public static void memmove(ref SByte[] arr1, Int32 intI, ref SByte[] arr2, Int32 intJ, Int32 intLength)
        //{
        //    for(Int32 intK=0;intK<intLength;intK++)
        //    {
        //        arr1[intI+intK] = arr2[intJ+intK];
        //    }
        //}

        #endregion
    }
}
