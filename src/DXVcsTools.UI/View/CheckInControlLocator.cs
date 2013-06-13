using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using DevExpress.Xpf.Mvvm.UI;

namespace DXVcsTools.UI.View {
    public class CheckInControlLocator : MarkupExtension, IViewLocator  {
        public object ResolveView(string name) {
            if (name == "CheckInControl")
                return new CheckInControl();
            if (name == "MultipleCheckInControl")
                return new MultipleCheckInControl();
            throw new ArgumentException("name");
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
