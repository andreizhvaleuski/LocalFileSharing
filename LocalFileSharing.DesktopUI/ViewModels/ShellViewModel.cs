using Caliburn.Micro;
using System.Diagnostics;

namespace LocalFileSharing.DesktopUI.ViewModels
{
    public class ShellViewModel : Screen
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
