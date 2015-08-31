using System.Windows;
using System.Windows.Controls;
using DXVcsTools.Core;

namespace DXVcsTools.UI.View {
    public class UITypeSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            IToolWindowViewModel model = item as IToolWindowViewModel;
            if (model == null)
                return base.SelectTemplate(null, container);
            if (model.Options.UseFlatUI)
                return ((FrameworkElement)container).FindResource("FlatDataTemplate") as DataTemplate;
            return ((FrameworkElement)container).FindResource("TreeDataTemplate") as DataTemplate;
        }
    }
}
