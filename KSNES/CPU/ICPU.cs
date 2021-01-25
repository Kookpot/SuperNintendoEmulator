using KSNES.SNESSystem;

namespace KSNES.CPU
{
    public interface ICPU : IHasAccessToSystem
    {
        void Cycle();
        bool IrqWanted { get; set; }
        bool NmiWanted { get; set; }
        int CyclesLeft { get; set; }
        void Reset();
    }
}