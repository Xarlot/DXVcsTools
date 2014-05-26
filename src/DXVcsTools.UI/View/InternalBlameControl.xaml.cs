using System;
using System.Linq;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Mvvm.UI.Interactivity;

namespace DXVcsTools.UI.View {
    /// <summary>
    /// Interaction logic for InternalBlameControl.xaml
    /// </summary>
    public partial class InternalBlameControl : UserControl {
        public InternalBlameControl() {
            InitializeComponent();
        }
    }

    public class CustomDisplayTextAttachedBehavior : Behavior<GridControl> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.CustomColumnDisplayText += AssociatedObject_CustomColumnDisplayText;
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.CustomColumnDisplayText -= AssociatedObject_CustomColumnDisplayText;
        }

        void AssociatedObject_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
            if (e.Column.FieldName == "Comment") {
                e.DisplayText = string.IsNullOrEmpty(e.DisplayText) ? e.DisplayText : e.DisplayText.Replace(Environment.NewLine, " ");
            }
        }
    }
}
