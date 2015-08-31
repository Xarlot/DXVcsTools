using System;
using System.Collections;
using System.Linq;
using DXVcsTools.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Mvvm.Native;

namespace DXVcsTools.UI {
    public class ChildNodesSelector : IChildNodesSelector {
        readonly Func<ProjectItemBase, bool> filterCallback;
        public ChildNodesSelector(Func<ProjectItemBase, bool> filterCallback) {
            this.filterCallback = filterCallback;
        }

        public IEnumerable SelectChildren(object item) {
            var children = (item as ProjectItemBase).With(x => x.Children);
            return children.Where(x => !filterCallback(x)).ToList();
        }
    }
}