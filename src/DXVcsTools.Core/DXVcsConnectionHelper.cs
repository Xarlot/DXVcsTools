using System;
using DXVcsTools.DXVcsClient;

namespace DXVcsTools.Core {
    public static class DXVcsConnectionHelper {
        public static IDXVcsRepository Connect(string file, string projectFile, out string vcsService, out string vcsFile) {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("file");

            if (string.IsNullOrEmpty(projectFile))
                throw new ArgumentException("projectFile");

            var locator = new DXVcsFileLocator(new DXVcsBindingInfo());
            vcsFile = locator.GetVcsLocation(file, projectFile, out vcsService);
            return Connect(vcsService);
        }

        public static IDXVcsRepository Connect(string file, string projectFile, out string vcsFile) {
            string vcsService;
            return Connect(file, projectFile, out vcsService, out vcsFile);
        }

        public static IDXVcsRepository Connect(string vcsService) {
            if (string.IsNullOrEmpty(vcsService))
                throw new ArgumentException("vcsService");

            return DXVcsRepositoryFactory.Create(vcsService);
        }
    }
}