namespace SuperNintendo.Core.Memory
{
    public static class Constants
    {
        public const int BLOCK_SIZE = 0x1000;
        public const int NUM_BLOCKS = 0x1000;
        public const int SHIFT = 12;
        public const int MASK = 0x1000 - 1;
        public const int MAX_ROM_SIZE = 0x800000;
        public const int ROM_NAME_LEN = 23;
    }
}