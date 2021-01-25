using System;
using System.Windows.Forms;
using KSNES.AudioProcessing;
using KSNES.Rendering;
using KSNES.SNESSystem;
using SimpleInjector;

namespace KSNES.GUI
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
            Injector.Inject(container);
            container.Register<IFPS, FPS>();
            container.Register<MainForm, MainForm>();
            container.Register<OpenSaveFilesDialog, OpenSaveFilesDialog>();
            container.Register<IKeyMapper, KeyMapper>();
            container.Register<IRenderer, SharpDXRenderer>();
            container.Register<IAudioHandler, XAudio2Handler>();
            container.Verify();
            Application.Run(container.GetInstance<MainForm>());
        }
    }
}