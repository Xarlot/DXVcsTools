using System;
using System.ComponentModel.Design;
using Company.DXVcsTools_VSIX;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;

namespace DXVcsTools.VSIX_IntegrationTests {
    [TestClass]
    public class ToolWindowTest {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///     A test for showing the toolwindow
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void ShowToolWindow() {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate {
                var toolWindowCmd = new CommandID(GuidList.guidDXVcsTools_VSIXCmdSet, (int)PkgCmdIDList.cmdidMyTool);

                var testUtils = new TestUtils();
                testUtils.ExecuteCommand(toolWindowCmd);

                Assert.IsTrue(testUtils.CanFindToolwindow(new Guid(GuidList.guidToolWindowPersistanceString)));
            });
        }

        delegate void ThreadInvoker();
    }
}