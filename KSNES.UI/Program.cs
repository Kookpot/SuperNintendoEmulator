namespace KSNES.GUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var serviceProvider = CreateHostBuilder;
            Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }

        private static IServiceProvider CreateHostBuilder
        {
            get
            {
                var services = new ServiceCollection();

                services.AddSingleton<IFPS, FPS>();
                services.AddSingleton<MainForm, MainForm>();
                services.AddSingleton<OpenSaveFilesDialog, OpenSaveFilesDialog>();
                services.AddSingleton<IKeyMapper, KeyMapper>();
                services.AddSingleton<IRenderer, SharpDXRenderer>();
                services.AddSingleton<IAudioHandler, XAudio2Handler>();
                Injector.Inject(services);

                return services.BuildServiceProvider();
            }
        }
    }
}