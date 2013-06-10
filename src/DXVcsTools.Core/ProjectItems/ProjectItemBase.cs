using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.Core {
    public abstract class ProjectItemBase : BindableBase {
        string name;
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
    }
}
