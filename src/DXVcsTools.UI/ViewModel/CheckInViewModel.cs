using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.UI.MVVM;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.UI {
    public class CheckInViewModel : BindableBaseEx {
        string filePath;
        string comment;
        bool staysChecked;
        public string FilePath {
            get { return filePath; }
            private set { SetProperty(ref filePath, value); }
        }
        public string Comment {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }
        public bool StaysChecked {
            get { return staysChecked; }
            set { SetProperty(ref staysChecked, value); }
        }

        public CheckInViewModel(string filePath, bool staysChecked) {
            FilePath = filePath;
            StaysChecked = staysChecked;
        }
    }
}
