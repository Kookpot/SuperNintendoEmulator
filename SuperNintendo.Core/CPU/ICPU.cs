using System;

namespace SuperNintendo.Core.CPU
{
    public static class ICPU
    {
        public static Action[] Opcodes;
        public static byte[] OpLengths;
        public static byte _Carry;
        public static byte _Zero;
        public static byte _Negative;
        public static byte _Overflow;
        public static uint ShiftedPB;
        public static uint ShiftedDB;
        public static uint Frame;
        public static uint FrameAdvanceCount;
    }
}