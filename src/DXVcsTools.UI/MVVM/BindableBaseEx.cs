using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DXVcsTools.Core;

namespace DXVcsTools.UI.MVVM {
    public class BindableBaseEx : BindableBaseCore {
        public new void SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = null) {
            base.SetProperty(ref storage, value, name);
        }
    }
}