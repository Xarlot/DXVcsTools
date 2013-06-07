using System;

namespace DXVcsTools.UI {
    public interface IDXBlameUI {
        void Show(Uri svnFile, int? lineNumber);
    }
}