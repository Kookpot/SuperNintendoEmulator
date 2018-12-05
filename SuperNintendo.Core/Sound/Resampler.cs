namespace SuperNintendo.Core.Sound
{
    public class Resampler : RingBuffer
    {
        public Resampler(int sizeOfBuffer) : base(sizeOfBuffer) { }

        public bool Push(short[] src, int numberOfSamples)
        {
            if (MaxWrite() < numberOfSamples)
                return false;

            if (numberOfSamples != 0)
                Push(src, numberOfSamples << 1);

            return true;
        }

        protected int MaxWrite()
        {
            return SpaceAvailable() >> 1;
        }

        public override void Resize(int numberOfSamples)
        {
            base.Resize(numberOfSamples << 1);
        }

        public virtual void SetTimeRatio(double ratio) { }
    }
}