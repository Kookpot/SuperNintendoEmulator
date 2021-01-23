using System.Diagnostics;
using System.Threading;

namespace SNESFromScratch2.Rendering
{
    public class FPS : IFPS
    {
        private const int lockedFrameRate = 60;

        private Stopwatch _stopWatchPerFrame;
        private Stopwatch _sw2;
        private int _getFramesPerSecond;
        private int _frameCount;
        private double[] _millisecondsPerFrame;
        private int _currentFrameIndex;

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

        public void LockFramerate()
        {
            InitializeMillisecondsPerFrame();
            double totalMilis = GetTotalMillisecondsElapsedThisPastSecond(_stopWatchPerFrame.ElapsedMilliseconds);
            while (totalMilis < 1000)
            {
                Thread.Sleep(1);
                totalMilis = GetTotalMillisecondsElapsedThisPastSecond(_stopWatchPerFrame.ElapsedMilliseconds);
            }
            _millisecondsPerFrame[_currentFrameIndex] = totalMilis / lockedFrameRate;
            _currentFrameIndex++;
            if (_currentFrameIndex == lockedFrameRate)
            {
                _currentFrameIndex = 0;
            }
            _stopWatchPerFrame.Restart();
        }

        private double GetTotalMillisecondsElapsedThisPastSecond(double last)
        {
            int index = _currentFrameIndex + 1;
            if (index == lockedFrameRate)
            {
                index = 0;
            }
            double sum = last;
            while (index != _currentFrameIndex)
            {
                sum += _millisecondsPerFrame[index];
                index++;
                if (index == lockedFrameRate)
                {
                    index = 0;
                }
            }
            return sum;
        }

        private void InitializeMillisecondsPerFrame()
        {
            if (_millisecondsPerFrame == null || _millisecondsPerFrame.Length != lockedFrameRate)
            {
                _millisecondsPerFrame = new double[lockedFrameRate];
                for (var i = 0; i < lockedFrameRate; i++)
                {
                    _millisecondsPerFrame[i] = (double)1000 / lockedFrameRate;
                }
            }
        }
    }
}