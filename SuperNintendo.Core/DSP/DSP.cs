using System;

namespace SuperNintendo.Core.DSP
{
    public static class DSP
    {
        public static Func<ushort, byte> GetDSP;
        public static Action<byte ushort> SetDSP;
    }
}
