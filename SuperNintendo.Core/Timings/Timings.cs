namespace SuperNintendo.Core.Timings
{
    public static class Timings
    {
        public static int H_Max_Master;
        public static int H_Max;
        public static int V_Max_Master;
        public static int V_Max;
        public static int HBlankStart;
        public static int HBlankEnd;
        public static int HDMAInit;
        public static int HDMAStart;
        public static int NMITriggerPos;
        public static int NextIRQTimer;
        public static int IRQTriggerCycles;
        public static int WRAMRefreshPos;
        public static int RenderPos;
        public static bool InterlaceField;
        public static int DMACPUSync;       // The cycles to synchronize DMA and CPU. Snes9x cannot emulate correctly.
        public static int NMIDMADelay;  // The delay of NMI trigger after DMA transfers. Snes9x cannot emulate correctly.
        public static CPU.IRQ IRQFlagChanging;  // This value is just a hack.
        public static int APUSpeedup;
        public static bool APUAllowTimeOverflow;
    }
}