using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBear.Common;
using NBear.Data;
using ManyToManyImpl;

namespace NBear.Test.CaseTests
{
    [TestClass]
    public class ManyToManyCaseTests
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
            gateway.Delete<UserGroup>(WhereClip.All);
            gateway.Delete<User>(WhereClip.All);
            gateway.Delete<Group>(WhereClip.All);
        }
        //
        #endregion

        [TestMethod]
        public void TestManyToManyCase()
        {
            User user1 = new User();
            user1.Name = "user1";
            gateway.Save(user1);

            User user2 = new User();
            user2.Name = "user2";
            gateway.Save(user2);

            User user3 = new User();
            user3.Name = "user3";
            //gateway.Save(user3); - user3 not saved

            Group group1 = new Group();
            group1.Name = "group1";
            gateway.Save(group1);

            Group group2 = new Group();
            group2.Name = "group2";
            gateway.Save(group2);

            Group group3 = new Group();
            group3.Name = "group3";
            //gateway.Save(group3); - group3 not saved

            //check saved users and groups
            Assert.AreEqual(gateway.Count<User>(WhereClip.All), 2);
            Assert.AreEqual(gateway.Count<Group>(WhereClip.All), 2);
            Assert.AreEqual(gateway.Count<UserGroup>(WhereClip.All), 0);

            //add user1 to group1 and group2
            user1.Groups.Add(group1);
            gateway.Save(user1);
            Assert.AreEqual(gateway.Count<UserGroup>(WhereClip.All), 1);
            user1 = gateway.Find<User>(user1.ID);

            user1.Groups.Add(group2);
            user1.Groups.Add(group3);   //althrough group3 is added to user1, it will not be saved, the relation row for user1 and group3 will not be saved, either.
            gateway.Save(user1);

            //check
            user1 = gateway.Find<User>(user1.ID);
            Assert.AreEqual(user1.Groups.Count, 2);
            Assert.IsNull(gateway.Find<UserGroup>(user1.ID, group3.ID));
            Assert.AreEqual(gateway.Count<Group>(WhereClip.All), 2);

            //save group3 and add user1, user2 to group3
            group3.Users.Add(user1);
            group3.Users.Add(user2);
            group3.Users.Add(user3);    //althrough user3 is added to group3, it will not be saved, the relation row for user3 and group3 will not be saved, either.
            gateway.Save(group3);

            //check
            Assert.AreEqual(gateway.Count<User>(WhereClip.All), 2);
            Assert.AreEqual(gateway.Count<Group>(WhereClip.All), 3);
            Assert.AreEqual(gateway.Count<UserGroup>(WhereClip.All), 4);
            user1 = gateway.Find<User>(user1.ID);
            Assert.AreEqual(user1.Groups.Count, 3);

            //delete user1, cascade delete group1,group2 and group3 related to user1
            gateway.Delete(user1);

            //check
            Assert.AreEqual(gateway.Count<User>(WhereClip.All), 1);
            Assert.AreEqual(gateway.Count<Group>(WhereClip.All), 3);
            Assert.AreEqual(gateway.Count<UserGroup>(WhereClip.All), 1);
        }
    }
}
