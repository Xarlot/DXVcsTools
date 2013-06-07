#if UNIT_TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DXVcsTools.Core.Tests
{
    [TestFixture]
    public class DXVcsBindingInfoTests
    {
        [Test]
        public void GetProjectSccFileTest()
        {
            DXVcsBindingInfo bindingReader = new DXVcsBindingInfo();

            Assert.AreEqual(@"c:\projects\HelloWorld\mssccprj.scc", bindingReader.GetProjectSccFile(@"c:\projects\HelloWorld\HelloWorld.csproj"));
            Assert.AreEqual(@"c:\mssccprj.scc", new DXVcsBindingInfo().GetProjectSccFile(@"c:\Test.csproj"));
        }
    }
}

#endif // UNIT_TEST
