using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;

namespace DXVcsTools.Core {
    public enum MergeState {
        None,
        Success,
        Conflict,
        InProgress,
        TargetDirectoryError,
    }

    public abstract class ProjectItemBase : BindableBase {
        string name;
        bool isChecked;
        bool isCheckOut;
        MergeState mergeState;
        string path;
        bool isSaved;

        public virtual int Priority { get { return 0; } }
        public string Path {
            get { return path; }
            set { SetProperty(ref path, value, "Path"); }
        }
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        public bool IsChecked {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value, "IsChecked"); }
        }
        public bool IsCheckOut {
            get { return isCheckOut; }
            set { SetProperty(ref isCheckOut, value, "IsCheckOut"); }
        }
        public MergeState MergeState {
            get { return mergeState; }
            set { SetProperty(ref mergeState, value, "MergeState"); }
        }
        public bool IsSaved { get { return ItemWrapper.If(x => x.IsSaved).ReturnSuccess(); } }
        public void Save() {
            if (!IsSaved)
                ItemWrapper.Do(x => x.Save());
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
        public IProjectItemWrapper ItemWrapper { get; set; }

        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
        }
    }
}
