using System.Diagnostics;

namespace KSNES.GUI
{
    public class FPS : IFPS
    {
        private Stopwatch _stopWatchPerFrame;
        private Stopwatch _sw2;
        private int _getFramesPerSecond;
        private int _frameCount;

        public void HiResTimerInitialize()
        {
            _stopWatchPerFrame = new Stopwatch();
            _stopWatchPerFrame.Start();
            _sw2 = new Stopwatch();
            _sw2.Start();
        }

        public string GetFPS()
        {
            _frameCount += 1;
            if (_sw2.ElapsedMilliseconds > 1000)
            {
                _getFramesPerSecond = (int)(_frameCount * 1000 / _sw2.ElapsedMilliseconds);
                _frameCount = 0;
                _sw2.Restart();
            }
            return $"{_getFramesPerSecond} fps";
        }
    }
}