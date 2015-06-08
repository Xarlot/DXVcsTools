using System;

namespace DXVcsTools.Version {
    public sealed class VersionInfo {
        public const string Major = "2";
        public const string Minor = "1";
        public const string Build = "41";
        public const string Revision = "0";

        public const string FullVersion = Major + "." + Minor + "." + Build + "." + Revision;
        public static int ToIntVersion() {
            return Convert.ToInt32(Major) * 1000000 + Convert.ToInt32(Minor) * 10000 + Convert.ToInt32(Build) * 100 + Convert.ToInt32(Revision);
        }
    }
}