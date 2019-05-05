using System.Windows;
using Caliburn.Micro;
using LocalFileSharing.DesktopUI.ViewModels;

namespace LocalFileSharing.DesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
