﻿using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;

namespace DXVcsTools.VSIX_IntegrationTests.IntegrationTests {
    [TestClass]
    public class SolutionTests {
        #region fields
        delegate void ThreadInvoker();
        #endregion

        #region properties
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }
        #endregion

        #region ctors
        #endregion

        [TestMethod]
        [HostType("VS IDE")]
        public void CreateEmptySolution() {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate {
                var testUtils = new TestUtils();
                testUtils.CloseCurrentSolution(__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave);
                testUtils.CreateEmptySolution(TestContext.TestDir, "EmptySolution");
            });
        }
    }
}