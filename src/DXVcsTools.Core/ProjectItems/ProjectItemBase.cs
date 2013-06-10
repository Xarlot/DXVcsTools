using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.Core {
    public class ProjectItemBase : BindableBase {
        public ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
    }
}
