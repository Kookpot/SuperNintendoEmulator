namespace KSNES.AudioProcessing
{
    public interface ISPC700
    {
        void SetAPU(IAPU apu);
        void Cycle();
        void Reset();
    }
}