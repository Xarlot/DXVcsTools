using System;
using DevExpress.Mvvm;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class ThemeProvider : BindableBaseCore {
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
