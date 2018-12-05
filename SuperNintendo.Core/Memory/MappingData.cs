namespace SuperNintendo.Core.Memory
{
    public class MappingData
    {
        public MappingType? MappingType;
        public int Position;
        public IsMemoryLinked MemoryLink;

        public byte this[int i]
        {
            get => MemoryLink[Position + i];
            set => MemoryLink[Position + i] = value;
        }
    }
}