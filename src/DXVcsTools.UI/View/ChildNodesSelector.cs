using System.Collections;
using DXVcsTools.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.Native;

namespace DXVcsTools.UI {
    public class ChildNodesSelector : IChildNodesSelector {
        public IEnumerable SelectChildren(object item) {
            return (item as ProjectItemBase).With(x => x.Children);
        }
    }
}