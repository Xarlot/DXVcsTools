using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public class SolutionItem : ProjectItemBase {
        public SolutionItem(IEnumerable<ProjectItem> items) : base(items) {
        }
    }
}
