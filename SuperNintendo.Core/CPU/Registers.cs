namespace SuperNintendo.Core.CPU
{
    public class Registers
    {
        public static byte DB;
        public static Pair P = new Pair();
        public static Pair A = new Pair();
        public static Pair D = new Pair();
        public static Pair S = new Pair();
        public static Pair X = new Pair();
        public static Pair Y = new Pair();
        public static ProgramCounter PC = new ProgramCounter();

        public static uint PBPC {get { return PC.xPBPC; } set { PC.xPBPC = value; } }
        public static ushort PCw {
            get { return (ushort)PC.xPBPC; }
            set { PC.xPBPC = ((PC.xPBPC >> 16) << 16) + value; }
        }
        public static byte PCh { get { return (byte)(PC.xPBPC >> 8); } set { PC.xPBPC = (uint)(((PC.xPBPC >> 16) << 16) + (value << 8) + (byte)PC.xPBPC); } }
        public static byte PCl { get { return (byte)PC.xPBPC; } set { PC.xPBPC = ((PC.xPBPC >> 8) << 8) + value; } }
        public static byte PB { get { return (byte)(PC.xPBPC >> 16); } set { PC.xPBPC = (uint)(((ushort)PC.xPBPC) + (value << 16)); } }

        public static byte AL { get { return (byte)A.W; } set { A.W = (ushort)(((A.W >> 8) << 8) + value); } }
        public static byte AH { get { return (byte)(A.W >> 8); } set { A.W = (ushort)(((byte)A.W) + (value << 8)); } }
        public static byte XL { get { return (byte)X.W; } set { X.W = (ushort)(((X.W >> 8) << 8) + value); } }
        public static byte XH { get { return (byte)(X.W >> 8); } set { X.W = (ushort)(((byte)X.W) + (value << 8)); } }
        public static byte YL { get { return (byte)Y.W; } set { Y.W = (ushort)(((Y.W >> 8) << 8) + value); } }
        public static byte YH { get { return (byte)(Y.W >> 8); } set { Y.W = (ushort)(((byte)Y.W) + (value << 8)); } }
        public static byte SL { get { return (byte)S.W; } set { S.W = (ushort)(((S.W >> 8) << 8) + value); } }
        public static byte SH { get { return (byte)(S.W >> 8); } set { S.W = (ushort)(((byte)S.W) + (value << 8)); } }
        public static byte DL { get { return (byte)D.W; } set { D.W = (ushort)(((D.W >> 8) << 8) + value); } }
        public static byte DH { get { return (byte)(D.W >> 8); } set { D.W = (ushort)(((byte)D.W) + (value << 8)); } }
        public static byte PL { get { return (byte)P.W; } set { P.W = (ushort)(((P.W >> 8) << 8) + value); } }
        public static byte PH { get { return (byte)(P.W >> 8); } set { P.W = (ushort)(((byte)P.W) + (value << 8)); } }
    }
}