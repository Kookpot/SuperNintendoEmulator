using KSNES.AudioProcessing;
using KSNES.CPU;
using KSNES.PictureProcessing;
using KSNES.Rendering;
using KSNES.ROM;
using SimpleInjector;

namespace KSNES.SNESSystem
{
    public static class Injector
    {
        public static void Inject(Container container)
        {
            container.Register<IPPU, PPU>();
            container.Register<IROM, ROM.ROM>();
            container.Register<IAPU, APU>();
            container.Register<ISNESSystem, SNESSystem>();
            container.Register<ISystemManager, SystemMananger>();
            container.Register<ICPU, CPU.CPU>();
            container.Register<IDSP, DSP>();
            container.Register<ISPC700, SPC700>();
        }
    }
}