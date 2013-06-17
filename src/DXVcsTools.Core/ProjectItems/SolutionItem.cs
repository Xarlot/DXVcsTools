using System.Collections.Generic;

namespace DXVcsTools.Core {
    public class SolutionItem : ProjectItemBase {
        public SolutionItem(IEnumerable<ProjectItem> items) : base(items) {
        }
    }
}