using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.Test.UnitTests.temp.EntityDesignTemp1;

namespace NBear.Test.UnitTests.EntityDesignToEntity
{
    [TestClass]
    public class GenerateTest
    {
        //[TestMethod]
        public void TestReflectingEnum()
        {
            Type type = typeof(UserType);
            string[] names = Enum.GetNames(type);
            UserType[] values = (UserType[])Enum.GetValues(type);
        }

        //[TestMethod]
        public void TestReflectingNullableValueTypes()
        {
            Console.WriteLine(typeof(int?).ToString());
        }
    }
}
