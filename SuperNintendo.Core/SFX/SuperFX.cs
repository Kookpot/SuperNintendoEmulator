namespace SuperNintendo.Core.SFX
{
    public static class SuperFX
    {
        //byte vFlags;
        public static int pvRegisterPosition; // 768 bytes located in the memory at address 0x3000
        public static int nRamBanks;       // Number of 64kb-banks in GSU-RAM/BackupRAM (banks 0x70-0x73)
        public static int pvRamPosition;           // Pointer to GSU-RAM
        public static int nRomBanks;       // Number of 32kb-banks in Cart-ROM
        public static int pvRomPosition;           // Pointer to Cart-ROM
        public static int speedPerLine;
        //bool8 oneLineDone;
    }
}