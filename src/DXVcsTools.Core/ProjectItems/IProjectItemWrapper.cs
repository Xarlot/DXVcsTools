using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public interface IProjectItemWrapper {
        bool IsSaved { get; }
        void Save();
    }
}
