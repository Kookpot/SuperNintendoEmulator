namespace SuperNintendo.Core.Memory
{
    public class MemorySetLink : IsMemoryLinked
    {
        public IsMemoryLinked Set;
        public int Position;

        public MemorySetLink(IsMemoryLinked set, int position)
        {
            Set = set;
            Position = position;
        }

        public byte this[int i]
        {
            get => Set[Position + i];
            set => Set[Position + i] = value;
        }
    }
}