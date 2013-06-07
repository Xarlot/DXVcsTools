using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Company.DXVcsTools_VSIX {
    /// <summary>
    ///     Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl {
        public MyControl() {
            InitializeComponent();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        void button1_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", ToString()), "My Tool Window");
        }
    }
}