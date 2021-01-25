namespace SNESFromScratch2.AudioProcessing
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