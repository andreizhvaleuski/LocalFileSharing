using System.Diagnostics;

using Caliburn.Micro;

namespace LocalFileSharing.DesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<Screen>
    {
        public ShellViewModel()
        {
            DisplayName = "Andrei";
        }

        public void Exit()
        {
            Debug.WriteLine(nameof(Exit));
        }

        public void Options()
        {
            Debug.WriteLine(nameof(Options));
        }
    }
}
