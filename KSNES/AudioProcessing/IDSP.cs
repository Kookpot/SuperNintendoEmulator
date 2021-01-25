namespace KSNES.AudioProcessing
{
    public interface IDSP
    {
        void Cycle();
        byte Read(int adr);
        void Write(int adr, byte value);
        void SetAPU(IAPU apu);
        float[] SamplesL { get; }
        float[] SamplesR { get; }
        int SampleOffset { get; set; }
        void Reset();
    }
}