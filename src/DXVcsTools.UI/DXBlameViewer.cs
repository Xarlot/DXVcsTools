using System;
using System.Diagnostics;
using DXVcsTools.Core;

namespace DXVcsTools.UI.Wpf {
    //public class DXBlameViewer : IDXBlameUI {
    //    readonly string _tortoiseProc;

    //    public DXBlameViewer(string tortoiseProc) {
    //        if (string.IsNullOrEmpty(tortoiseProc))
    //            throw new ArgumentException("tortoiseProc");
    //        _tortoiseProc = PathHelper.ResolvePath(tortoiseProc);
    //    }

    //    #region IDXBlameUI Members
    //    public void Show(Uri svnFile, int? lineNumber) {
    //        if (string.IsNullOrEmpty(svnFile.ToString()))
    //            throw new ArgumentException("svnFile");

    //        var startInfo = new ProcessStartInfo();
    //        startInfo.FileName = _tortoiseProc;
    //        startInfo.Arguments = string.Format("/command:blame /path:{0} /startrev:0 /endrev:-1", svnFile.AbsoluteUri);

    //        if (lineNumber.HasValue)
    //            startInfo.Arguments += string.Format(" /line:{0}", lineNumber);

    //        using (Process process = Process.Start(startInfo)) {
    //            process.WaitForExit();
    //        }
    //    }
    //    #endregion
    //}
}