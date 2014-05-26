using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;

namespace DXVcsTools.UI {
    public class UpdateButtonsOnLoadedBehavior : Behavior<FrameworkElement> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectLoaded;
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
        }
        void AssociatedObjectLoaded(object sender, RoutedEventArgs e) {
            var root = (FrameworkElement)LayoutHelper.FindRoot(AssociatedObject);
            root.Do(x => x.Dispatcher.BeginInvoke(new Action(() => LayoutHelper.ForEachElement(x, element => (element as Button).Do(btn => btn.IsDefault = false)))));
        }
    }
}
