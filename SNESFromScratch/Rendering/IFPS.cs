namespace SNESFromScratch.Rendering
{
    public interface IFPS
    {
        void HiResTimerInitialize();
        string GetFPS();
        void LockFramerate();
    }
}