using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.UI.Interactivity;

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

    public class MouseOverHighlightBehavior : Behavior<TableView> {
        FormatCondition userCondition;
        FormatCondition userCondition2;
        FormatCondition revisionCondition;
        FormatCondition revisionCondition2;
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.PreviewMouseMove += AssociatedObjectPreviewMouseMove;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            userCondition = new FormatCondition() { FieldName = "User" };
            userCondition2 = new FormatCondition() { FieldName = "User" };
            revisionCondition = new FormatCondition() { FieldName = "Revision" };
            revisionCondition2 = new FormatCondition() { FieldName = "Revision" };
            
            AssociatedObject.AddFormatCondition(userCondition);
            AssociatedObject.AddFormatCondition(userCondition2);
            AssociatedObject.AddFormatCondition(revisionCondition);
            AssociatedObject.AddFormatCondition(revisionCondition2);
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.PreviewMouseMove -= AssociatedObjectPreviewMouseMove;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
        void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            userCondition.Format = null;
            userCondition2.Format = null;
            revisionCondition.Format = null;
            revisionCondition2.Format = null;
        }
        void AssociatedObjectPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            var result = AssociatedObject.CalcHitInfo(e.OriginalSource as DependencyObject);
            GridControl grid = AssociatedObject.Grid;
            if (result.InRowCell) {
                if (result.Column.FieldName == "Revision" || result.Column.FieldName == "User") {
                    var revisionValue = grid.GetCellValue(result.RowHandle, "Revision");
                    var userValue = grid.GetCellValue(result.RowHandle, "User");
                    var halfHighlightExpression = new BinaryOperator("User", userValue).ToString();
                    var halfHighlightFormat = new Format() {
                        Background = new SolidColorBrush(!string.IsNullOrEmpty(userValue as string) && userValue.ToString().Contains("Serov") ? Color.FromArgb(50, 255, 0, 0) : Color.FromArgb(50, 0, 0, 255))
                    };
                    userCondition.Expression = halfHighlightExpression;
                    userCondition.Format = halfHighlightFormat;
                    revisionCondition.Expression = halfHighlightExpression;
                    revisionCondition.Format = halfHighlightFormat;
                    
                    string expression = CriteriaOperator.And(
                        new BinaryOperator("Revision", revisionValue),
                        new BinaryOperator("User", userValue)).ToString();
                    var highlightFormat = new Format() {
                        Background = new SolidColorBrush(!string.IsNullOrEmpty(userValue as string) && userValue.ToString().Contains("Serov") ? Color.FromArgb(150, 255, 0, 0) : Color.FromArgb(100, 0, 0, 255))
                    };
                    userCondition2.Format = highlightFormat;
                    userCondition2.Expression = expression;
                    revisionCondition2.Format = highlightFormat;
                    revisionCondition2.Expression = expression;
                }
                else {
                    userCondition.Format = null;
                    userCondition2.Format = null;
                    revisionCondition.Format = null;
                    revisionCondition2.Format = null;
                }
            }
        }
    }
}
