namespace SNESFromScratch2.AudioProcessing
{
    public interface IAPU
    {
        byte[] RAM { get; }
        //void SetSamples(byte[] left, byte[] right);
        void Cycle();
        void Write(int adr, byte value);
        byte Read(int adr);
        byte[] SpcWritePorts { get; }
        byte[] SpcReadPorts { get; set; }
        void Reset();
    }
}