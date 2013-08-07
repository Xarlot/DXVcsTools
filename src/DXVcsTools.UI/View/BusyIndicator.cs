using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;

namespace DXVcsTools.UI {
    public class BusyIndicator {
        static ProgressBusyIndicator indicator;
        public static void Show() {
            indicator.Do(x => x.Close());
            indicator = new ProgressBusyIndicator();
            ThemeManager.SetThemeName(indicator, ThemeProvider.Instance.ThemeName);
            indicator.Show();
        }
        public static void Close() {
            indicator.Do(x => x.Close());
            indicator = null;
        }
    }

    class ProgressBusyIndicator : Window {
        public ProgressBusyIndicator() {
            BackgroundPanel panel = new BackgroundPanel();
            TextBlock tb = new TextBlock() {Text = "Loading...", Margin = new Thickness(30)};
            panel.Content = tb;
            Content = panel;
            Topmost = true;
            BorderThickness = new Thickness(1);
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;
            SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
