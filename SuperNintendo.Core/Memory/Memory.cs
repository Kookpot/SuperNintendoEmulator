using System;
using System.IO;

namespace SuperNintendo.Core.Memory
{
    /// <summary>
    /// memory
    /// </summary>
    public static class Memory
    {
        #region Public Members

        //private static byte[] NSRTHeader = new byte[32];
        //private static int HeaderCount;

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

        //private static byte[] Map = new byte[Constants.NUM_BLOCKS];
        //private static byte[] WriteMap = new byte[Constants.NUM_BLOCKS];
        //private static byte[] BlockIsRAM = new byte[Constants.NUM_BLOCKS];
        //private static byte[] BlockIsROM = new byte[Constants.NUM_BLOCKS];
        //private static byte ExtendedFormat;

        //private static string ROMFileName;
        //private static string ROMName;
        //private static string RawROMName;
        //private static string ROMId;

        //private static int CompanyId;
        //private static byte ROMRegion;
        //private static byte ROMSpeed;
        //private static byte ROMType;
        //private static byte ROMSize;
        //private static uint ROMChecksum;
        //private static uint ROMComplementChecksum;
        //private static uint ROMCRC32;

        //private static string ROMSHA256;
        //private static int ROMFramesPerSecond;

        //private static bool HiRom;
        private static bool LoROM;
        //private static byte SRAMSize;
        //private static uint SRAMMask;
        private static int CalculatedSize;
        //private static uint CalculatedChecksum;

        public static Action PostRomInitFunc;

        //private static SByte _objOpenBus;
        // bank     //offset
        //$00-$3F	$0000-$1FFF	LowRAM
        //$00-$3F   $2000-$2FFF	PPU1, APU
        //$00-$3F   $3000-$3FFF	DSP, SuperFX	hardware registers
        //$00-$3F   $4000-$41FF	controller	hardware registers
        //$00-$3F   $4200-$4FFF	DMA, PPU2?	hardware registers
        //$00-$3F   $6000-$7FFF	Chips	enhancement chips memory
        //$00-$3F   $8000-$FFFF	ROM	Data that is mapped here depends on the cartridge.
        //$40-$7D	$0000-$FFFF	ROM	Data that is mapped here depends on the cartridge.
        //$7E	    $0000-$1FFF	LowRAM
        //$7E       $2000-$7FFF	HighRAM
        //$7E       $8000-$FFFF	Extended RAM
        //$7F	    $0000-$FFFF	Extended RAM
        public static byte[] RAM = new byte[0x20000];
        public static byte[] SRAM = new byte[0x20000];
        public static byte[] VRAM = new byte[0x10000];
        public static byte[] ROM = new byte[Constants.MAX_ROM_SIZE + 0x200 + 0x8000];
        
        //private static Byte[] _objMemory = new Byte[16777216];

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

            //SuperFX.pvRegisters = FillRAM + 0x3000;
            //SuperFX.nRamBanks = 2; // Most only use 1.  1=64KB=512Mb, 2=128KB=1024Mb
            //SuperFX.pvRam = SRAM;
            //SuperFX.nRomBanks = (2 * 1024 * 1024) / (32 * 1024);
            //SuperFX.pvRom = (uint8*)ROM;

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

        private static int InitROM()
        {
            return 0;
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
