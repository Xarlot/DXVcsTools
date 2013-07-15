using System;
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
