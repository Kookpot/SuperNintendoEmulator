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
        public static ushort PCw { get { return (ushort)PC.xPBPC; } set { PC.xPBPC |= value; } }
        public static byte PCh { get { return (byte)(PC.xPBPC >> 8); } set { PC.xPBPC |= (ushort)(value << 8); } }
        public static byte PCl { get { return (byte)PC.xPBPC; } set { PC.xPBPC |= value; } }
        public static byte PB { get { return (byte)(PC.xPBPC >> 16); } set { PC.xPBPC |= (uint)(value << 16); } }

        public static byte AL { get { return (byte)A.W; } set { A.W |= value; } }
        public static byte AH { get { return (byte)(A.W >> 8); } set { A.W |= (ushort)(value << 8); } }
        public static byte XL { get { return (byte)X.W; } set { X.W |= value; } }
        public static byte XH { get { return (byte)(X.W >> 8); } set { X.W |= (ushort)(value << 8); } }
        public static byte YL { get { return (byte)Y.W; } set { Y.W |= value; } }
        public static byte YH { get { return (byte)(Y.W >> 8); } set { Y.W |= (ushort)(value << 8); } }
        public static byte SL { get { return (byte)S.W; } set { S.W |= value; } }
        public static byte SH { get { return (byte)(S.W >> 8); } set { S.W |= (ushort)(value << 8); } }
        public static byte DL { get { return (byte)D.W; } set { D.W |= value; } }
        public static byte DH { get { return (byte)(D.W >> 8); } set { D.W |= (ushort)(value << 8); } }
        public static byte PL { get { return (byte)P.W; } set { P.W |= value; } }
        public static byte PH { get { return (byte)(P.W >> 8); } set { P.W |= (ushort)(value << 8); } }
    }
}