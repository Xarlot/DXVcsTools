using System.Collections.Generic;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;

namespace DXVcsTools.Core {
    public enum MergeState {
        None,
        Success,
        Conflict,
        InProgress,
        TargetFileError,
        CheckOutFileError,
        UnknownError,
    }

    public abstract class ProjectItemBase : BindableBase {
        bool isCheckOut;
        bool isChecked;
        bool isNew;
        MergeState mergeState;
        string name;
        string path;
        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
            if (Children != null) {
                foreach (var child in Children)
                    child.Parent = this;
            }
        }

        public ProjectItemBase Parent { get; internal set; }
        public virtual int Priority {
            get { return 0; }
        }
        public bool IsNew {
            get { return isNew; }
            set { SetProperty(ref isNew, value, () => IsNew); }
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
        public string FullPath { get { return ItemWrapper.Return(x => x.FullPath, () => string.Empty); } }

        public void Save() {
            if (!IsSaved)
                ItemWrapper.Do(x => x.Save());
        }
        public void Open() {
            ItemWrapper.Do(x => x.Open());
        }
    }
}