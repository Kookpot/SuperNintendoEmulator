using SuperNintendo.Core.Memory;

namespace SuperNintendo.Core.CPU
{
    public static class CPUState
    {
        public static uint Flags;
        public static int Cycles;
        public static int PrevCycles;
        public static int V_Counter;
        public static MappingData PCBase; //only reference to ROM
        public static bool NMIPending;
        public static bool IRQLine;
        public static bool IRQTransition;
        public static bool IRQLastState;
        public static bool IRQExternal;
        public static int IRQPending;
        public static int MemSpeed;
        public static int MemSpeedx2;
        public static int FastROMSpeed;
        public static bool InDMA;
        public static bool InHDMA;
        public static bool InDMAorHDMA;
        public static bool InWRAMDMAorHDMA;
        public static byte HDMARanInDMA;
        public static int CurrentDMAorHDMAChannel;
        public static HCEvents WhichEvent;
        public static int NextEvent;
        public static bool WaitingForInterrupt;
        public static uint AutoSaveTimer;
        public static bool SRAMModified;
    }
}