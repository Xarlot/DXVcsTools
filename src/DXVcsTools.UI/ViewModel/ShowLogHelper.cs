using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.UI.ViewModel {
    public static class ShowLogHelper {
        public static void ShowLog(string path) {
            var pci = new ProcessStartInfo("notepad.exe");
            pci.Arguments = path;
            Process.Start(pci);
        }
    }
}
