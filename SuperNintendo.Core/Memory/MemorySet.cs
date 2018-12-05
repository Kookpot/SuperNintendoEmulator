namespace SuperNintendo.Core.Memory
{
    public class MemorySet : IsMemoryLinked
    {
        public byte[] bytes;
        public int Position;

        public MemorySet(int size)
        {
            bytes = new byte[size];
        }

        public byte this[int i]
        {
            get => bytes[Position + i];
            set => bytes[Position + i] = value;
        }
    }
}