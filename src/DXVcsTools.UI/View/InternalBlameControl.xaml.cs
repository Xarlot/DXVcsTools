using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Bars;
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
            grid.Focus();
        }
    }

    public class LineIndexAttachedBehavior : Behavior<GridControl> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.CustomUnboundColumnData += AssociatedObject_CustomUnboundColumnData;
        }

        void AssociatedObject_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
            if (e.Column.FieldName == "LineNumber") {
                e.Value = e.ListSourceRowIndex + 1;
                return;
            }
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.CustomUnboundColumnData -= AssociatedObject_CustomUnboundColumnData;
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
        bool isInMenu;
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.PreviewMouseMove += AssociatedObjectPreviewMouseMove;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            AssociatedObject.ShowGridMenu += AssociatedObject_ShowGridMenu;
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
        void AssociatedObject_ShowGridMenu(object sender, GridMenuEventArgs e) {
            e.MenuInfo.Menu.Closed += Menu_Closed;
            isInMenu = true;
        }

        void Menu_Closed(object sender, EventArgs e) {
            ((DataControlPopupMenu)sender).Closed -= Menu_Closed;
            isInMenu = false;
            UpdateHighlighting(AssociatedObject.InputHitTest(Mouse.GetPosition(AssociatedObject)) as FrameworkElement);
        }
        void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            if (isInMenu)
                return;
            userCondition.Format = null;
            userCondition2.Format = null;
            revisionCondition.Format = null;
            revisionCondition2.Format = null;
        }
        void AssociatedObjectPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            UpdateHighlighting(e.OriginalSource as FrameworkElement);
        }
        void UpdateHighlighting(FrameworkElement element) {
            var result = AssociatedObject.CalcHitInfo(element);
            GridControl grid = AssociatedObject.Grid;
            if (isInMenu)
                return;
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

                    string expression = CriteriaOperator.And(new BinaryOperator("Revision", revisionValue), new BinaryOperator("User", userValue)).ToString();
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
