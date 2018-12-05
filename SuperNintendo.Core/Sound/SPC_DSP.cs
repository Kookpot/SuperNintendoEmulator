namespace SuperNintendo.Core.Sound
{
    public static class SPC_DSP
    {
        private static byte[] _output;
        private static int begin;
        private static int end;

        public static void SetOutput(byte[] output, int size)
        {
            _output = output;
            begin = 0;
            end = size;
        }
    }
}