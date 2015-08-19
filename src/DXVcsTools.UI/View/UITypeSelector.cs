using System.Windows;
using System.Windows.Controls;

namespace DXVcsTools.UI.View {
    public class UITypeSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (item == null)
                return base.SelectTemplate(null, container);

            return ((FrameworkElement)container).FindResource("FlatDataTemplate") as DataTemplate;
        }
    }
}
