using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.UI.Navigator;
using log4net.Repository.Hierarchy;

namespace DXVcsTools.UI {
    public class NavigatePreset {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
