using System;
using System.Windows.Forms;
using SimpleInjector;
using SNESFromScratch.AudioProcessing;
using SNESFromScratch.CentralMemory;
using SNESFromScratch.CPU;
using SNESFromScratch.IO;
using SNESFromScratch.PictureProcessing;
using SNESFromScratch.Rendering;
using SNESFromScratch.ROM;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch
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
            container.Register<IIO, IO.IO>();
            container.Register<IPPU, PPU>();
            container.Register<IROM, ROM.ROM>();
            container.Register<IFPS, FPS>();
            container.Register<ISNESSystem, SNESSystem.SNESSystem>();
            container.Register<ICPU, C65816>();
            container.Register<MainForm, MainForm>();
            container.Register<IKeyMapper, KeyMapper>();
            container.Register<IRenderer, Renderer>();
            container.Register<ISPC700, SPC700>();
            container.Register<IDMA, DMA>();
            container.Verify();
            Application.Run(container.GetInstance<MainForm>());
        }
    }
}