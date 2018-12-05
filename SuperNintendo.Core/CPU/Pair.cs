namespace SuperNintendo.Core.CPU
{
    public class Pair
    {
        public ushort W;
        public byte Low { get { return (byte)(W >> 8); } set { W |= (ushort)(value << 8); } }
        public byte High { get { return (byte)W; } set { W |= value; } }
    }
}