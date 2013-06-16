using System.IO;
using System.Windows.Forms;
using DXVcsTools.UI.MVVM;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;

namespace DXVcsTools.UI {
    public class ManualMergeViewModel : BindableBaseEx {
        string originalFilePath;
        string targetFilePath;

        public RelayCommand<string> SpecifyTargetCommand { get; private set; }

        public string OriginalFilePath {
            get { return originalFilePath; }
            private set { SetProperty(ref originalFilePath, value); }
        }
        public string TargetFilePath {
            get { return targetFilePath; }
            set { SetProperty(ref targetFilePath, value); }
        }

        public ManualMergeViewModel(string originalFilePath, string targetFilePath = null) {
            OriginalFilePath = originalFilePath;
            TargetFilePath = targetFilePath;

            SpecifyTargetCommand = new RelayCommand<string>(SpecifyTarget, parameter => true);
        }
        void SpecifyTarget(string currentPath) {
            OpenFileDialog openFile = new OpenFileDialog();

            FileInfo info = new FileInfo(currentPath);
            if (info.Exists) {
                openFile.InitialDirectory = info.Directory.With(x => x.FullName);
                openFile.CheckFileExists = true;
                openFile.AddExtension = true;
            }
            if (openFile.ShowDialog() == DialogResult.OK) {
                TargetFilePath = openFile.FileName;
            }
        }
    }
}
