using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.Data;

namespace DXVcsTools.UI.ViewModel {
    public class InternalBlameViewModel : BindableBase {
        IEnumerable<IBlameLine> blame;
        public IEnumerable<IBlameLine> Blame {
            get { return blame; }
            set { SetProperty(ref blame, value, () => Blame); }
        }

        public InternalBlameViewModel(IEnumerable<IBlameLine> blame) {
            Blame = blame;
        }
    }
}
