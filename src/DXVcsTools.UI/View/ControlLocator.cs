using System;
using System.Windows.Markup;
using DevExpress.Mvvm.UI;

namespace DXVcsTools.UI.View {
    public class ControlLocator : MarkupExtension, IViewLocator {
        public object ResolveView(string name) {
            if (name == "CheckInControl")
                return new CheckInControl();
            if (name == "MultipleCheckInControl")
                return new MultipleCheckInControl();
            if (name == "ManualMergeControl")
                return new ManualMergeControl();
            throw new ArgumentException("name");
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}