using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.Core {
    public enum MergeState {
        None,
        Success,
        Conflict,
        InProgress,
    }

    public abstract class ProjectItemBase : BindableBase {
        string name;
        bool isChecked;
        bool isCheckOut;
        MergeState mergeState;

        public virtual int Priority { get { return 0; } }
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
        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
    }
}
