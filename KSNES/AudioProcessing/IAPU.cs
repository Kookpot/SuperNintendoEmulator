namespace KSNES.AudioProcessing
{
    public interface IAPU
    {
        byte[] RAM { get; }
        void Attach();
        void Cycle();
        void Write(int adr, byte value);
        byte Read(int adr);
        byte[] SpcWritePorts { get; }
        byte[] SpcReadPorts { get; set; }
        void Reset();
        void SetSamples(float[] left, float[] right);
    }
}