using System.IO;
using System.Windows.Forms;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.UI.MVVM;
using DevExpress.Xpf.Mvvm.Native;

namespace DXVcsTools.UI {
    public class ManualMergeViewModel : BindableBaseEx {
        string originalFilePath;
        string targetFilePath;
        public ManualMergeViewModel(string originalFilePath, string targetFilePath = null) {
            OriginalFilePath = originalFilePath;
            TargetFilePath = targetFilePath;

            SpecifyTargetCommand = new DelegateCommand<string>(SpecifyTarget);
        }

        public DelegateCommand<string> SpecifyTargetCommand { get; private set; }

        public string OriginalFilePath {
            get { return originalFilePath; }
            private set { SetProperty(ref originalFilePath, value); }
        }
        public string TargetFilePath {
            get { return targetFilePath; }
            set { SetProperty(ref targetFilePath, value); }
        }

        void SpecifyTarget(string currentPath) {
            var openFile = new OpenFileDialog();

            var info = new FileInfo(currentPath);
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