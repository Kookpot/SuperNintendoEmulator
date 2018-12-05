using System;

namespace SuperNintendo.Core.Sound
{
    public class RingBuffer
    {
        protected int size;
        protected int bufferSize;
        protected int start;
        protected byte[] buffer;

        public RingBuffer(int sizeOfBuffer)
        {
            bufferSize = sizeOfBuffer;
            buffer = new byte[bufferSize];
            size = 0;
            start = 0;
        }

        public bool Push(Array src, int numberOfBytes)
        {
            if (SpaceAvailable() < numberOfBytes)
                return false;

            var end = (start + size) % bufferSize;
            var firstWriteSize = Math.Min(numberOfBytes, bufferSize - end);

            Array.Copy(src, 0, buffer, end, firstWriteSize);

            if (numberOfBytes > firstWriteSize)
                Array.Copy(src, firstWriteSize, buffer, 0, numberOfBytes - firstWriteSize);

            size += numberOfBytes;
            return true;
        }

        public bool Pull(ref byte[] dst, int startOfDst, int numberOfBytes)
        {
            if (SpaceFilled() < numberOfBytes)
                return false;

            Array.Copy(buffer, start, dst, startOfDst, Math.Min(numberOfBytes, bufferSize - start));

            if (numberOfBytes > (bufferSize - start))
                Array.Copy(buffer, start + (bufferSize - start), dst, startOfDst + (bufferSize - start), numberOfBytes - (bufferSize - start));

            start = (start + numberOfBytes) % bufferSize;
            size -= numberOfBytes;

            return true;
        }

        protected int SpaceAvailable()
        {
            return bufferSize - size;
        }

        private int SpaceFilled()
        {
            return size;
        }

        private void Clear()
        {
            start = 0;
            size = 0;
            buffer = new byte[bufferSize];
        }

        public virtual void Resize(int size)
        {
            bufferSize = size;
            buffer = new byte[bufferSize];

            size = 0;
            start = 0;
        }

        private void CacheSilence()
        {
            Clear();
            size = bufferSize;
        }
    }
}