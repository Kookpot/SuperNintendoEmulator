namespace SNESFromScratch.ROM
{
    public interface IROM
    {
        void LoadRom(string fileName);

        byte[,] ROMData { get; }
        bool IsHiRom();
        byte GetBanks();
        byte GetSRAMSize();
        bool IsPAL();
        bool IsExHiRom();
        string GetName();
    }
}