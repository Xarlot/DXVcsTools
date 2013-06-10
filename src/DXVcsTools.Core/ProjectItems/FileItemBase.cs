using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.Core {
    public abstract class FileItemBase : ProjectItemBase {
        protected FileItemBase(IEnumerable<FileItemBase> items) : base(items) {
        }
    }
}
