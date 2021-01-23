using SNESFromScratch2.SNESSystem;

namespace SNESFromScratch2.ROM
{
    public interface IROM : IHasAccessToSystem
    {
        byte Read(int bank, int adr);
        void Write(int bank, int adr, byte value);
        Header Header { get; }
        void LoadROM(byte[] data, Header header);
        void LoadSRAM();
    }
}