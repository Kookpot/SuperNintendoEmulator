namespace SNESFromScratch.AudioProcessing
{
    public interface ISPC700
    {
        void Reset();
        void Execute(int targetCycles);
        void Write8IO(int address, int value);
        int Read8IO(int address);
        byte[] WRAM { get; }
        int Cycles { get; set; }
    }
}