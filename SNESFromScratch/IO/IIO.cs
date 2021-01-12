using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.IO
{
    public interface IIO : IHasAccessToSystem
    {
        void Reset();

        int NMITimeEnabled { get; set; }
        int HorizontalTime { get; set; }
        int VerticalTime { get; set; }
        int MDMAEnabled { get; set; }
        int HDMAEnabled { get; set; }
        int MemorySelection { get; set; }
        int HVIRQ { get; }

        int NMIRead { get; set; }
        int TimeUp { get; set; }
        int HVBJoy { get; set; }
        int Joy1 { get; set; }

        int Read8(int address);
        void Write8(int address, int value);
    }
}