using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.Common;

namespace NBear.Test.UnitTests.Common
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void TestParseRaletivePath()
        {
            string basePath = @"c:\abc\root";
            string relativePath = @"..\..\tmp\src";

            Assert.AreEqual(Util.ParseRelativePath(basePath, relativePath), @"c:\tmp\src");
        }
    }
}
