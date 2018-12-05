namespace SuperNintendo.Core.Memory
{
    public class DirectMemorySetLink : IsMemoryLinked
    {
        public MemorySet Set;
        public int Position;

        public DirectMemorySetLink(MemorySet set, int position)
        {
            Set = set;
            Position = position;
        }

        public byte this[int i]
        {
            get => Set.bytes[Position + i];
            set => Set.bytes[Position + i] = value;
        }
    }
}