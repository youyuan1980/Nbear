using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBear.Test.UnitTests.Common
{
    [TestClass]
    public class QueryProxyTest
    {
        public class Test
        {
            public delegate int TestHandler();

            public static event TestHandler OnTest;

            public static int DoTest()
            {
                if (OnTest != null)
                {
                    return OnTest();
                }
                return 0;
            }

            private static int Test1()
            {
                return 1;
            }

            private static int Test2()
            {
                return 2;
            }

            static Test()
            {
                OnTest += new TestHandler(Test2);
                OnTest += new TestHandler(Test1);
            }
        }

        //[TestMethod]
        public void TestMultiEventHandlerValueReturn()
        {
            Assert.AreEqual(Test.DoTest(), 1);
        }
    }
}
