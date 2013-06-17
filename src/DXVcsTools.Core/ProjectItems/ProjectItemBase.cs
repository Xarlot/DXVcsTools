using System.Collections.Generic;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;

namespace DXVcsTools.Core {
    public enum MergeState {
        None,
        Success,
        Conflict,
        InProgress,
        TargetDirectoryError,
        UnknownError,
    }

    public abstract class ProjectItemBase : BindableBase {
        bool isCheckOut;
        bool isChecked;
        bool isSaved;
        MergeState mergeState;
        string name;
        string path;
        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
        }

        public virtual int Priority {
            get { return 0; }
        }
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
        public bool IsSaved {
            get { return ItemWrapper.If(x => x.IsSaved).ReturnSuccess(); }
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
        public IProjectItemWrapper ItemWrapper { get; set; }
        public void Save() {
            if (!IsSaved)
                ItemWrapper.Do(x => x.Save());
        }
    }
}