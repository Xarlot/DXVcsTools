using System.Runtime.CompilerServices;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.UI.MVVM {
    public class BindableBaseEx : BindableBase {
        public new void SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null) {
            base.SetProperty(ref storage, value, name);
        }
    }
}