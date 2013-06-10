using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Mvvm.Native;

namespace DXVcsTools.UI {
    public class ChildNodesSelector : IChildNodesSelector {
        public IEnumerable SelectChildren(object item) {
            return (item as ProjectItemBase).With(x => x.Children);
        }
    }
}
