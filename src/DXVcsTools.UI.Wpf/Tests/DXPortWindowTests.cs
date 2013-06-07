#if UNIT_TEST

using NUnit.Framework;

namespace DXVcsTools.UI.Wpf.Tests {
    [TestFixture]
    public class DXPortWindowTests {
        [Test]
        [Explicit]
        public void ShowDialog() {
            var window = new DXPortWindow();
            window.ShowDialog();
        }
    }
}

#endif