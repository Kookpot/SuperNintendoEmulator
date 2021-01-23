using System;
using System.Windows.Forms;
using SimpleInjector;
using SNESFromScratch2.AudioProcessing;
using SNESFromScratch2.CPU;
using SNESFromScratch2.PictureProcessing;
using SNESFromScratch2.Rendering;
using SNESFromScratch2.ROM;
using SNESFromScratch2.SNESSystem;

namespace SNESFromScratch2
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Singleton;
            container.Register<IPPU, PPU>();
            container.Register<IROM, ROM.ROM>();
            container.Register<IFPS, FPS>();
            container.Register<IAPU, APU>();
            container.Register<ISNESSystem, SNESSystem.SNESSystem>();
            container.Register<ISystemManager, SystemMananger>();
            container.Register<ICPU, CPU.CPU>();
            container.Register<MainForm, MainForm>();
            container.Register<IKeyMapper, KeyMapper>();
            //container.Register<IRenderer, GDIRenderer>();
            container.Register<IRenderer, SharpDXRenderer>();
            container.Register<IDSP, DSP>();
            container.Register<ISPC700, SPC700>();
            container.Verify();
            Application.Run(container.GetInstance<MainForm>());
        }
    }
}