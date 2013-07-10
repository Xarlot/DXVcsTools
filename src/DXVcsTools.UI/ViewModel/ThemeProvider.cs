using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.UI {
    public class ThemeProvider : BindableBase {
        [ThreadStatic]
        public static readonly ThemeProvider Instance;

        static ThemeProvider() {
            Instance = new ThemeProvider();
        }
        string themeName;

        public string ThemeName {
            get { return themeName; }
            set { SetProperty(ref themeName, value, () => ThemeName); }
        }
    }
}
