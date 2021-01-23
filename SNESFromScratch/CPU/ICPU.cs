using System.Runtime.CompilerServices;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.CPU
{
    public interface ICPU : IHasAccessToSystem
    {
        int Read8(int address, bool incCycles = true);
        int Read16(int address, bool incCycles = true);
        void Write8(int address, int value, bool incCycles = true, [CallerFilePath] string filePath = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int sourceLineNumber = 0);
        int Cycles { get; set; }
        void Reset();
        void ExecuteStep();
        bool IRQPending { get; set; }
        void DoNMI();
        void LoadSRAM();
        byte[] WRAM { get; }
    }
}