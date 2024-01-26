namespace KSNES.SNESSystem;

public static class Injector
{
    public static void Inject(ServiceCollection container)
    {
        container.AddSingleton<IPPU, PPU>();
        container.AddSingleton<IROM, ROM.ROM>();
        container.AddSingleton<IAPU, APU>();
        container.AddSingleton<ISNESSystem, SNESSystem>();
        container.AddSingleton<ISystemManager, SystemMananger>();
        container.AddSingleton<ICPU, CPU.CPU>();
        container.AddSingleton<IDSP, DSP>();
        container.AddSingleton<ISPC700, SPC700>();
    }
}