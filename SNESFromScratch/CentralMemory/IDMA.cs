using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.CentralMemory
{
    public interface IDMA : IHasAccessToSystem
    {
        int Read8(int address);
        void Write8(int address, int value);
        void DMATransfer();
        void HDMATransfer();
        void HDMAReset();
        void SetChannelEnabled(int channel, bool enabled);
    }
}