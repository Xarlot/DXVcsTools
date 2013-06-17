using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Mvvm.UI;

namespace DXVcsTools.UI {
    public interface IDialogService {
        bool? ShowDialog(string contentTag, object parameter, string title = "", DialogButtons btn = DialogButtons.OkCancel);
    }

    public class DialogService : ViewServiceBase, IDialogService {
        public string ThemeName { get; set; }
        public bool? ShowDialog(string contentTag, object parameter, string title, DialogButtons btn) {
            var dialog = new DXDialog(title, btn) {
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.ToolWindow,
                ResizeMode = ResizeMode.NoResize,
                Content = CreateAndInitializeView(contentTag, null)
            };
            dialog.DataContext = parameter;
            ThemeManager.SetThemeName(dialog, ThemeName);
            return dialog.ShowDialog();
        }
    }
}