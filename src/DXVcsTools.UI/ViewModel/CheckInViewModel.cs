using DXVcsTools.UI.MVVM;

namespace DXVcsTools.UI {
    public class CheckInViewModel : BindableBaseEx {
        string comment;
        string filePath;
        bool staysChecked;
        public CheckInViewModel(string filePath, bool staysChecked) {
            FilePath = filePath;
            StaysChecked = staysChecked;
        }
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
    }
}