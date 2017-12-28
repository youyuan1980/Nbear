using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.Common;
using Entities;

namespace NBear.Test.UnitTests.Common
{
    namespace EntityTest_temp
    {
        [Serializable]
        public partial class TestArray : EntityArrayList<Order> { }
    }

    [TestClass]
    public class EntityTest
    {
        [TestMethod]
        public void TestEntityBasicUsage()
        {
            PrivilegeOwner obj = new PrivilegeOwner();
            obj.Attach();
            Assert.AreEqual(obj.GetModifiedProperties(obj.GetType()).Count, 0);

            obj.Name = "teddy";
            Assert.AreEqual(new List<object>(obj.GetModifiedProperties(obj.GetType()).Values)[0], "teddy");

            obj.ResetModifiedPropertyStates();
            Assert.AreEqual(obj.GetModifiedProperties(obj.GetType()).Count, 0);

            obj.ID = 222;
            obj.Name = "teddy2";
            obj.ID = 222;
            obj.Name = "teddy2";
            Assert.AreEqual(obj.GetModifiedProperties(obj.GetType()).Count, 2);
        }

        [TestMethod]
        public void TestEntityArray()
        {
            EntityTest_temp.TestArray arr = new NBear.Test.UnitTests.Common.EntityTest_temp.TestArray();
            arr.Add(new Order());
            arr.Add(new Order());
            Console.WriteLine(SerializationManager.Serialize(arr));
        }

        [TestMethod]
        public void TestManualModifiedAndUpdate()
        {
            PrivilegeOwner obj = new PrivilegeOwner();
            obj.Name = "teddy";
            obj.ID = 222;

            obj.Attach();

            obj.SetAllPropertiesAsModified();

            Assert.AreEqual(obj.GetModifiedProperties().Count, 1);
        }
    }
}
