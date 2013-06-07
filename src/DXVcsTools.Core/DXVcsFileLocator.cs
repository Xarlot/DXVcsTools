using System;

namespace DXVcsTools.Core {
    public class DXVcsFileLocator {
        readonly IDXVcsBindingInfo _bindingInfo;

        public DXVcsFileLocator(IDXVcsBindingInfo bindingInfo) {
            if (bindingInfo == null)
                throw new ArgumentException("bindingInfo");

            _bindingInfo = bindingInfo;
        }

        public string GetVcsLocation(string file, string projectFile, out string server) {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("file");

            if (string.IsNullOrEmpty(projectFile))
                throw new ArgumentException("projectFile");

            Uri projectBinding = GetProjectBinding(projectFile, out server);
            Uri fileRelative = GetFileRelativeUri(file, projectFile);
            var fileBinding = new Uri(projectBinding, fileRelative);

            return fileBinding.ToString().Replace("dxvcs://", "$/");
        }

        Uri GetFileRelativeUri(string file, string projectFile) {
            return new Uri(projectFile).MakeRelativeUri(new Uri(file));
        }

        Uri GetProjectBinding(string projectFile, out string server) {
            string projectBinding = _bindingInfo.GetProjectBinding(projectFile, out server);

            if (!projectBinding.EndsWith("/"))
                projectBinding += "/";

            projectBinding = projectBinding.Replace("$/", "dxvcs://");
            return new Uri(projectBinding);
        }
    }
}