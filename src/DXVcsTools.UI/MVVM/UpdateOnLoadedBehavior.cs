﻿using System.Windows;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;

namespace DXVcsTools.UI {
    public class UpdateOnLoadedBehavior : Behavior<FrameworkElement> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectLoaded;
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
        }
        void AssociatedObjectLoaded(object sender, RoutedEventArgs e) {
            (AssociatedObject.DataContext as IUpdatableViewModel).Do(x => x.Update());
        }
    }
}