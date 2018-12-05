namespace SuperNintendo.Core.CPU
{
    public static class Constants
    {
        //#define SNES_TR_MASK		(1 <<  4)
        //#define SNES_TL_MASK		(1 <<  5)
        //#define SNES_X_MASK			(1 <<  6)
        //#define SNES_A_MASK			(1 <<  7)
        //#define SNES_RIGHT_MASK		(1 <<  8)
        //#define SNES_LEFT_MASK		(1 <<  9)
        //#define SNES_DOWN_MASK		(1 << 10)
        //#define SNES_UP_MASK		(1 << 11)
        //#define SNES_START_MASK		(1 << 12)
        //#define SNES_SELECT_MASK	(1 << 13)
        //#define SNES_Y_MASK			(1 << 14)
        //#define SNES_B_MASK			(1 << 15)
        //public static byte ONE_DOT_CYCLE = 4;

        public const byte DEBUG_MODE_FLAG = 1 << 0;	// debugger
        public const byte TRACE_FLAG = 1 << 1; // debugger
        public const byte SINGLE_STEP_FLAG = 1 << 2;	// debugger
        public const byte BREAK_FLAG = 1 << 3;	// debugger
        public const byte SCAN_KEYS_FLAG = 1 << 4;	// CPU
        public const ushort HALTED_FLAG = 1 << 12;	// APU
        public const ushort FRAME_ADVANCE_FLAG = 1 << 9;

        public const int ONE_CYCLE = 6;
        public const int SLOW_ONE_CYCLE = 8;
        public const int TWO_CYCLES = 12;

        public static readonly  byte Carry = 1;
        public static readonly byte Zero = 2;
        public static readonly byte IRQ = 4;
        public static readonly byte Decimal = 8;
        public static readonly byte IndexFlag = 16;
        public static readonly byte MemoryFlag = 32;
        public static readonly byte Overflow = 64;
        public static readonly byte Negative = 128;
        public static readonly ushort Emulation = 256;

        public const int SNES_WRAM_REFRESH_HC_v1 = 530;
        public const int SNES_WRAM_REFRESH_HC_v2 = 538;
        public const int SNES_WRAM_REFRESH_CYCLES = 40;
    }
}