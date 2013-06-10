using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.Core {
    public abstract class FileItemBase : ProjectItemBase {
        string path;
        public string Path {
            get { return path; }
            set { SetProperty(ref path, value, "Path"); }
        }
        protected FileItemBase(IEnumerable<FileItemBase> items) : base(items) {
        }
    }
}
