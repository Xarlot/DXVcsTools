using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXVcsTools.UI
{
    public interface IDXBlameUI
    {
        void Show(Uri svnFile, int? lineNumber);
    }
}
