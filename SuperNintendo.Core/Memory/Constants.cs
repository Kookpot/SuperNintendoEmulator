namespace SuperNintendo.Core.Memory
{
    public static class Constants
    {
        public const int BLOCK_SIZE = 0x1000;
        public const int NUM_BLOCKS = 0x1000;
        public const int SHIFT = 12;
        public const int MASK = 0x1000 - 1;
        public const int MAX_ROM_SIZE = 0x800000;
        public const int ROM_NAME_LEN = 23;
        public const int SNES_HCOUNTER_MAX = 341;
        public const int ONE_DOT_CYCLE = 4;
        public const int SNES_CYCLES_PER_SCANLINE = SNES_HCOUNTER_MAX * ONE_DOT_CYCLE;
        public const int SNES_HBLANK_START_HC = 1096;				// H=274
        public const int SNES_HDMA_START_HC = 1106;				// FIXME: not true
        public const int SNES_HBLANK_END_HC = 4;						// H=1
        public const int SNES_HDMA_INIT_HC = 20;					// FIXME: not true
        public const int SNES_RENDER_START_HC = 128 * ONE_DOT_CYCLE;
        public const int SNES_MAX_NTSC_VCOUNTER = 262;
    }
}