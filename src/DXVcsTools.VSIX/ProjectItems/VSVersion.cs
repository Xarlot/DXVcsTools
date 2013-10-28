using System;
using System.Diagnostics;
using System.IO;

namespace DXVcsTools.Core {
    public static class VSVersion {
        static readonly object MLock = new object();
        static System.Version mVsVersion;
        static System.Version mOsVersion;

        public static System.Version FullVersion {
            get {
                lock (MLock) {
                    if (mVsVersion == null) {
                        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");

                        if (File.Exists(path)) {
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);

                            string verName = fvi.ProductVersion;

                            for (int i = 0; i < verName.Length; i++) {
                                if (!char.IsDigit(verName, i) && verName[i] != '.') {
                                    verName = verName.Substring(0, i);
                                    break;
                                }
                            }
                            mVsVersion = new System.Version(verName);
                        }
                        else
                            mVsVersion = new System.Version(0, 0); // Not running inside Visual Studio!
                    }
                }

                return mVsVersion;
            }
        }

        public static System.Version OSVersion {
            get { return mOsVersion ?? (mOsVersion = Environment.OSVersion.Version); }
        }

        public static bool VS2013OrLater {
            get { return FullVersion >= new System.Version(12, 0); }
        }
        public static bool VS2012 {
            get { return FullVersion.Major == 11; }
        }
        public static bool VS2013 {
            get { return FullVersion.Major == 12; }
        }
    }
}
