using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.UI {
    public class CheckInViewModel : BindableBase {
        string filePath;
        string comment;
        bool staysChecked;
        public string FilePath {
            get { return filePath; }
            private set { SetProperty(ref filePath, value, "FilePath"); }
        }
        public string Comment {
            get { return comment; }
            set { SetProperty(ref comment, value, "Comment"); }
        }
        public bool StaysChecked {
            get { return staysChecked; }
            set { SetProperty(ref staysChecked, value, "StaysChecked"); }
        }

        public CheckInViewModel(string filePath, bool staysChecked) {
            FilePath = filePath;
            StaysChecked = staysChecked;
        }
    }
}
