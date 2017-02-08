using System;
using System.Diagnostics;
using System.Threading;

namespace SNESFromScratch
{
    public class FPS
    {
        private Stopwatch _sw;
        private Stopwatch _sw2;

        private int _getFramesPerSecond;
        private int _frameCount;

        public bool LimitFPS;

        private double[] _millis;
        private int _index;

        public bool HiResTimerInitialize()
        {
            _sw = new Stopwatch();
            _sw.Start();
            _sw2 = new Stopwatch();
            _sw2.Start();
            
            return true;
        }

        private double GetTotalMilies(double last, int targetFPS)
        {
            var index = _index + 1;
            if (index == targetFPS) index = 0;
            var sum = last;
            while(index!=_index)
            {
                sum += _millis[index];
                index++;
                if (index == targetFPS) index = 0;
            }
            return sum;
        }

        private void InitializeMilli(int targetFPS)
        {
            if (_millis != null && _millis.Length == targetFPS) return;
            _millis = new double[targetFPS];
            for (var i = 0; i < targetFPS; i++)
            {
                _millis[i] = (double) 1000/targetFPS;
            }
        }

        public void LockFramerate(int targetFPS)
        {
            InitializeMilli(targetFPS);
            var milli = _sw.ElapsedMilliseconds;
            var totalMilis = GetTotalMilies(milli, targetFPS);
            while (totalMilis<1000)
            {
                Thread.Sleep(1);
                milli = _sw.ElapsedMilliseconds;
                totalMilis = GetTotalMilies(milli, targetFPS);
            }
            _millis[_index] = totalMilis/targetFPS;
            _index++;
            if (_index == targetFPS) _index = 0;
            _sw.Restart();
        }

        public string GetFPS()
        {
            _frameCount = _frameCount + 1;
            if (_sw2.ElapsedMilliseconds > 1000)
            {
                _getFramesPerSecond = (int) (_frameCount * 1000 / _sw2.ElapsedMilliseconds);
                _frameCount = 0;
                _sw2.Restart();
            }

            return _getFramesPerSecond + " fps";
        }
    }
}