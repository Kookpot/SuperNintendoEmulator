namespace SuperNintendo.Core.Sound
{
    public static class Constants
    {
        public const int APU_DEFAULT_INPUT_RATE = 31950; // ~ 59.94Hz
        public const int APU_MINIMUM_SAMPLE_COUNT = 512;
        public const int APU_MINIMUM_SAMPLE_BLOCK = 128;
        public const int APU_NUMERATOR_NTSC = 15664;
        public const int APU_DENOMINATOR_NTSC = 328125;
        public const int APU_NUMERATOR_PAL = 34176;
        public const int APU_DENOMINATOR_PAL = 709379;
        public const int SoundBufferSize = 64;
    }
}