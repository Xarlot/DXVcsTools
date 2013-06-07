#if UNIT_TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DXVcsTools.UI.Wpf.Tests
{
    [TestFixture]
    public class DXPortWindowTests
    {
        [Test]
        [Explicit]
        public void ShowDialog()
        {
            DXPortWindow window = new DXPortWindow();
            window.ShowDialog();
        }
    }
}

#endif