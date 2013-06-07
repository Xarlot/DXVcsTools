// Guids.cs
// MUST match guids.h

using System;

namespace DXVcsTools.VSIX {
    static class GuidList {
        public const string guidDXVcsTools_VSIXPkgString = "2631d8dd-813b-41eb-8f9a-fe0fc88ecba9";
        public const string guidDXVcsTools_VSIXCmdSetString = "8f625cb3-1d17-45ca-814a-baa4c18152dc";
        public const string guidToolWindowPersistanceString = "c170e42d-6d77-44b1-a643-29d22df9f286";

        public static readonly Guid guidDXVcsTools_VSIXCmdSet = new Guid(guidDXVcsTools_VSIXCmdSetString);
    };
}