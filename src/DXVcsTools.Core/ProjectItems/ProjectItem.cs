using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public class ProjectItem : ProjectItemBase {
        public ProjectItem(IEnumerable<FileItemBase> items) : base(items) {
        }
    }
}
