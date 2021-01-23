namespace SNESFromScratch2.Rendering
{
    public interface IFPS
    {
        void HiResTimerInitialize();
        string GetFPS();
        void LockFramerate();
    }
}