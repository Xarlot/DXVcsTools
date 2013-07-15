using System.Diagnostics;

namespace DXVcsTools.UI.ViewModel {
    public static class ShowLogHelper {
        public static void ShowLog(string path) {
            var pci = new ProcessStartInfo("notepad.exe");
            pci.Arguments = path;
            Process.Start(pci);
        }
    }
}
