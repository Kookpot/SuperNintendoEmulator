namespace SuperNintendo.Core.Sound
{
    public static class MSU
    {
        public static int buffer_size;
        public static byte[] landing_buffer;
        public static Resampler resampler;
        public static int resample_buffer_size = -1;
        public static byte[] resample_buffer;

        public static byte MSU1_STATUS;
        public static uint MSU1_DATA_SEEK;
        public static uint MSU1_DATA_POS;
        public static ushort MSU1_TRACK_SEEK;
        public static ushort MSU1_CURRENT_TRACK;
        public static uint MSU1_RESUME_TRACK;
        public static byte MSU1_VOLUME;
        public static byte MSU1_CONTROL;
        public static uint MSU1_AUDIO_POS;
        public static uint MSU1_RESUME_POS;
    }
}