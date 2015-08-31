using System.Collections.Generic;

namespace DXVcsTools.Core {
    public class SolutionItem : ProjectItemBase {
        public SolutionItem(IEnumerable<ProjectItem> items) : base(items) {
        }
        public override bool IsNew {
            get { return false; }
            set { }
        }
    }
}