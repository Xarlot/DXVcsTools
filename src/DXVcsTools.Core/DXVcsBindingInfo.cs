using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DXVcsTools.Core {
    public class DXVcsBindingInfo : IDXVcsBindingInfo {
        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        internal string GetProjectSccFile(string projectFile) {
            return Path.Combine(Path.GetDirectoryName(projectFile), "mssccprj.scc");
        }

        #region IVcsBindingInfo Members
        public string GetProjectBinding(string projectFile, out string server) {
            if (string.IsNullOrEmpty(projectFile))
                throw new ArgumentException("projectFile");

            string sccFile = GetProjectSccFile(projectFile);
            string projectName = Path.GetFileName(projectFile);

            var buff = new StringBuilder(1024);

            GetPrivateProfileString(projectName, "SCC_Aux_Path", string.Empty, buff, (uint)buff.Capacity, sccFile);
            server = buff.ToString();

            GetPrivateProfileString(projectName, "SCC_Project_Name", string.Empty, buff, (uint)buff.Capacity, sccFile);
            return buff.ToString();
        }
        #endregion
    }
}