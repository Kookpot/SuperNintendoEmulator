namespace SuperNintendo.Core.PPU
{
    public class ClipData
    {
        public byte Count;
        public byte[] DrawMode = new byte[6];
        public ushort[] Left = new ushort[6];
        public ushort[] Right = new ushort[6];
    }
}