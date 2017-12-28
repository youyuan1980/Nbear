using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBear.Common;
using NBear.Data;
using ManyToManyImpl2;

namespace NBear.Test.CaseTests
{
    [TestClass]
    public class ManyToManyCase2Tests
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 

        private Gateway gateway = null;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            gateway = new Gateway(new Database(Gateway.Default.Db.DbProvider));
            gateway.RegisterSqlLogger(Console.WriteLine);
        }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() 
        {
        }
        //
        #endregion

        [TestMethod]
        public void TestManyToManyCase2()
        {
            Role role = new Role();
            role.Name = "Role1";

            User user;
            for (int i = 0; i < 5; i++)
            {
                user = new User();
                gateway.Save(user);
                role.Users.Add(user);
            }
            gateway.Save<Role>(role);

            role = gateway.Find<Role>(role.ID);

            Assert.AreEqual(5, role.Users.Count);

            gateway.Delete<Role>(role);

            Assert.IsNull(gateway.Find<Role>(role.ID));
            Assert.AreEqual(0, (int)gateway.Count<UserRoles>(UserRoles._.RoleID == role.ID));
        }
    }
}
