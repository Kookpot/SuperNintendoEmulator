namespace KSNES.AudioProcessing
{
    public interface IAudioHandler
    {
        float[] SampleBufferL { get; set; }
        float[] SampleBufferR { get; set; }
        void NextBuffer();
        void Pauze();
        void Resume();
    }
}