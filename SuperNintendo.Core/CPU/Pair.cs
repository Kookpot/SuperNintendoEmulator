namespace SuperNintendo.Core.CPU
{
    public class Pair
    {
        public ushort W;
        public byte Low { get { return (byte)W; } set { W = (ushort)(((W >> 8) << 8) + value); } }
        public byte High { get { return (byte)(W >> 8); } set { W = (ushort)(((byte)W) + (value << 8)); } }
    }
}