using System;

namespace SuperNintendo.Core.Sound
{
    public static class SPC
    {
        public static Action sa_callback;

        public static bool sound_in_sync = true;
        public static bool sound_enabled = false;

        public static int buffer_size;
        public static int lag_master = 0;
        public static int lag = 0;

        public static byte[] landing_buffer;
        public static byte[] shrink_buffer;

        //static Resampler* resampler = NULL;

        public static int reference_time;
        public static uint remainder;

        public const int timing_hack_numerator = 256;
        public static int timing_hack_denominator = 256;
        public static uint ratio_numerator = Constants.APU_NUMERATOR_NTSC;
        public static uint ratio_denominator = Constants.APU_DENOMINATOR_NTSC;

        static double dynamic_rate_multiplier = 1.0;
    }
}