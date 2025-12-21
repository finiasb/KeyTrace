using System.Configuration;
using System.Data;
using System.Windows;

namespace ServiceApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // PORNEȘTI HOOK-UL AICI
            KeyboardHook.StartHook();

            // Nicio fereastră nu se deschide

        }

        protected override void OnExit(ExitEventArgs e)
        {
            KeyboardHook.StopHook();
            base.OnExit(e);
        }


    }
}