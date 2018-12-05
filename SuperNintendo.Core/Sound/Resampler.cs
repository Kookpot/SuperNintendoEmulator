namespace SuperNintendo.Core.Sound
{
    public class Resampler
    {
        protected bool push(short[] src, int num_samples)
        {
            if (max_write() < num_samples)
                return false;

            !num_samples || ring_buffer::push((unsigned char *) src, num_samples << 1);

            return true;
        }

        inline int
        space_empty(void) const
        {
            return buffer_size - size;
        }

    inline int
    space_filled(void) const
        {
            return size;
        }

inline int
max_write(void) const
        {
            return space_empty() >> 1;
        }

        inline void
        resize(int num_samples)
{
    ring_buffer::resize(num_samples << 1);
}
    }
}