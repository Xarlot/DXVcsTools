using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.Core {
    public abstract class ProjectItemBase : BindableBase {
        string name;
        bool isChecked;
        bool isMerged = true;
        public virtual int Priority { get { return 0; } }
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        public bool IsChecked {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value, "IsChecked"); }
        }
        public bool IsMerged {
            get { return isMerged; }
            set { SetProperty(ref isMerged, value, "IsMerged"); }
        }
        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
    }
}
