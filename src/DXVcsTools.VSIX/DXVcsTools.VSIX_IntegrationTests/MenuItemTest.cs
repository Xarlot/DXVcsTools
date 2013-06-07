using System.ComponentModel.Design;
using System.Globalization;
using DXVcsTools.VSIX;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;

namespace DXVcsTools.VSIX_IntegrationTests {
    [TestClass]
    public class MenuItemTest {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///     A test for lauching the command and closing the associated dialogbox
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void LaunchCommand() {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate {
                var menuItemCmd = new CommandID(GuidList.guidDXVcsTools_VSIXCmdSet, (int)PkgCmdIDList.cmdidDXVcsToolsRoot);

                // Create the DialogBoxListener Thread.
                string expectedDialogBoxText = string.Format(CultureInfo.CurrentCulture, "{0}\n\nInside {1}.MenuItemCallback()", "DXVcsTools.VSIX", "Company.DXVcsTools_VSIX.DXVcsTools_VSIXPackage");
                var purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try {
                    purger.Start();

                    var testUtils = new TestUtils();
                    testUtils.ExecuteCommand(menuItemCmd);
                }
                finally {
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The dialog box has not shown");
                }
            });
        }

        delegate void ThreadInvoker();
    }
}