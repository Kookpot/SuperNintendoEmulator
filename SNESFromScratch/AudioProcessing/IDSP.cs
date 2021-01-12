namespace SNESFromScratch.AudioProcessing
{
    public interface IDSP
    {
        void ProcessSample();
        int Read8(int address);
        void Write8(int address, int value);
    }
}