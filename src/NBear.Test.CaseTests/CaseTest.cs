using System;
using System.Data.Common;
using System.Text;
using System.Transactions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Entities;
using Test.Entities;
using NBear.Common;
using NBear.Data;

using NBear.Test.CaseTests.shared;

namespace NBear.Test.CaseTests
{
    [TestClass]
    public class CaseTest
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
            gateway = new Gateway("CaseTests");
            gateway.RegisterSqlLogger(new LogHandler(Console.Write));
        }
        
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestCreate()
        {
            LocalUser newLocalUser = new LocalUser();
            newLocalUser.ID = Guid.NewGuid();
            newLocalUser.LoginName = newLocalUser.ID.ToString();
            UserName name = new UserName();
            name.FirstName = "first name of local user";
            name.LastName = "last name of local user";
            newLocalUser.Name = name;
            newLocalUser.Password = "password";
            newLocalUser.Status = UserStatus.Normal;

            gateway.Save<LocalUser>(newLocalUser);

            newLocalUser = gateway.Find<LocalUser>(newLocalUser.ID);
            Assert.IsNotNull(newLocalUser);

            AgentUser newAgentUser = new AgentUser();
            gateway.Save((User)newAgentUser);
        }

        [TestMethod]
        public void TestUpdate()
        {
            //delete users without teamid
            //gateway.BatchDelete<User>(User._.TeamID == new Guid() | User._.TeamID == null);

            LocalUser user = gateway.FindArray<LocalUser>(WhereClip.All, LocalUser._.Password.Desc)[0];
            user.Password = "12345";
            UserName newName = new UserName();
            newName.FirstName = "12345";
            user.Name = newName;

            Assert.IsTrue(user.Groups.Count >= 0);

            Console.WriteLine(user.Domains.Count.ToString());

            gateway.Save<LocalUser>(user);
            user = gateway.Find<LocalUser>(user.ID);
            Assert.AreEqual(user.Password, "12345");

            LocalUser newLocalUser = new LocalUser();
            newLocalUser.ID = Guid.NewGuid();
            newLocalUser.LoginName = newLocalUser.ID.ToString();
            UserName name = new UserName();
            name.FirstName = "first name of local user";
            name.LastName = "last name of local user";
            newLocalUser.Name = name;
            newLocalUser.Password = "password";
            newLocalUser.Status = UserStatus.Normal;

            gateway.Save<LocalUser>(newLocalUser);

            UserProfile profile = new UserProfile();
            profile.ContentXml = "test content";
            newLocalUser.Profile = profile;
            gateway.Save<LocalUser>(newLocalUser);

            newLocalUser = gateway.Find<LocalUser>(newLocalUser.ID);
            Assert.IsNotNull(newLocalUser.Profile);
        }

        [TestMethod]
        public void TestDelete()
        {
            LocalUser newLocalUser = new LocalUser();
            newLocalUser.ID = Guid.NewGuid();
            newLocalUser.LoginName = newLocalUser.ID.ToString();
            UserName name = new UserName();
            name.FirstName = "first name of local user";
            name.LastName = "last name of local user";
            newLocalUser.Name = name;
            newLocalUser.Password = "password";
            newLocalUser.Status = UserStatus.Normal;

            Group g = new Group();
            g.Name = "test group";
            gateway.Save(g);

            newLocalUser.Groups.Add(g);

            gateway.Save<LocalUser>(newLocalUser);

            Guid id = newLocalUser.ID;
            AgentUser user = gateway.Find<AgentUser>(id);
            gateway.Delete<AgentUser>(user);
            Assert.IsNull(gateway.Find<User>(id));
            Assert.IsNull(gateway.Find<AgentUser>(id));
            Assert.IsNull(gateway.Find<LocalUser>(id));
        }

        private Guid CreateSampleData(DbTransaction tran)
        {
            //create local user
            LocalUser newLocalUser = new LocalUser();
            newLocalUser.ID = Guid.NewGuid();
            newLocalUser.LoginName = newLocalUser.ID.ToString();
            UserName name = new UserName();
            name.FirstName = "first name of local user";
            name.LastName = "last name of local user";
            newLocalUser.Name = name;
            newLocalUser.Password = "password";
            newLocalUser.Status = UserStatus.Normal;

            //create user profile
            UserProfile newUserProfile = new UserProfile();
            newUserProfile.ContentXml = "sample content xml";
            newUserProfile.UserID = newLocalUser.ID;

            newLocalUser.Profile = newUserProfile;

            gateway.Save<LocalUser>(newLocalUser, tran);

            return newLocalUser.ID;
        }

        [TestMethod]
        public void TestAsp20Transaction()
        {
            Guid id = default(Guid);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                id = CreateSampleData(null);

                scope.Complete();
            }

            AgentUser user = gateway.Find<AgentUser>(id);
            Assert.AreNotEqual(user.Profile, null);

            user.Status = UserStatus.Deleted;
            user.Profile.ContentXml = "modified";

            gateway.Save<AgentUser>(user);
            AgentUser anotherThisUser = gateway.Find<AgentUser>(user.ID);
            Assert.AreEqual(anotherThisUser.Status, user.Status);
            Assert.AreEqual(anotherThisUser.Profile.ContentXml, user.Profile.ContentXml);

            gateway.Delete<User>(user);
            Assert.IsNull(gateway.Find<LocalUser>(id));
            Assert.IsNull(gateway.Find<UserProfile>(id));
        }

        [TestMethod]
        public void TestAsp11Transaction()
        {
            Guid id = default(Guid);
            DbTransaction tran = gateway.BeginTransaction();
            try
            {
                id = CreateSampleData(tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            finally
            {
                gateway.CloseTransaction(tran);
            }

            AgentUser user = gateway.Find<AgentUser>(id);
            Assert.AreNotEqual(user.Profile, null);

            user.Status = UserStatus.Deleted;
            user.Profile.ContentXml = "modified";

            gateway.Save<AgentUser>(user);
            AgentUser anotherThisUser = gateway.Find<AgentUser>(user.ID);
            Assert.AreEqual(anotherThisUser.Status, user.Status);
            Assert.AreEqual(anotherThisUser.Profile.ContentXml, user.Profile.ContentXml);

            gateway.Delete<User>(user);
            Assert.IsNull(gateway.Find<LocalUser>(id));
            Assert.IsNull(gateway.Find<UserProfile>(id));
        }

        [TestMethod]
        public void TestQueryPropertyArray()
        {
            //create local user
            LocalUser newLocalUser = new LocalUser();
            newLocalUser.LoginName = "sample login name";
            UserName name = new UserName();
            name.FirstName = "first name of local user";
            name.LastName = "last name of local user";
            newLocalUser.Name = name;
            newLocalUser.Password = "password";
            newLocalUser.Status = UserStatus.Normal;

            //create user profile
            UserProfile newUserProfile = new UserProfile();
            newUserProfile.ContentXml = "sample content xml";

            newLocalUser.Profile = newUserProfile;

            gateway.Save<LocalUser>(newLocalUser);

            Domain newDomain = new Domain();
            newDomain.Desc = "sample domain desc";
            newDomain.Name = "sample domain name";
            newLocalUser.Domains.Add(newDomain);

            gateway.Save<LocalUser>(newLocalUser);

            AgentUser agentUser = gateway.Find<AgentUser>(newLocalUser.ID);

            Assert.AreEqual(agentUser.Domains.Count, 1);

            agentUser.Domains.Clear();

            gateway.Save<AgentUser>(agentUser);

            agentUser = gateway.Find<AgentUser>(newLocalUser.ID);

            Assert.AreEqual(agentUser.Domains.Count, 0);

            newDomain.Detach();
            agentUser.Domains.Add(newDomain);

            gateway.Save<AgentUser>(agentUser);

            Assert.IsNotNull(gateway.Find<Domain>(newDomain.ID));

            gateway.Delete<AgentUser>(agentUser);

            Assert.IsNull(gateway.Find<Domain>(newDomain.ID));
        }

        [TestMethod]
        public void TestMasterDetails()
        {
            Master newMaster = new Master();
            newMaster.Name = "master name";
            Detail newDetail = new Detail();
            newDetail.Name = "detail name";
            newMaster.Details.Add(newDetail);
            gateway.Save(newMaster);

            newMaster.Name = "modified master name";
            newMaster.Details[0].Name = "modified detail name";

            gateway.Save(newMaster);

            newMaster.Details = null;
            gateway.Save(newMaster);
        }

        [TestMethod]
        public void TestTemp()
        {
            Team t = new Team();
            t.Name = "test"; ;
            gateway.Save(t);

            User u = new User();
            u.Team = t;
            gateway.Save(u);

            cms_Articles entity = new cms_Articles();
            entity.Author = "11";
            entity.Body = "11";
            entity.ChannelId = 1;
            entity.CreateTime = DateTime.Now;
            entity.Editor = "11";
            entity.Picture = "11";
            entity.Source = "11";
            entity.Title = "11";
            entity.UpdateTime = DateTime.Now;
            entity.Statistics = new cms_ArticleStatistics();

            cms_Channels newChannel = new cms_Channels();
            newChannel.Dir = "dir";
            newChannel.Title = "11";
            gateway.Save(newChannel);

            entity.ChannelId = newChannel.Id;
            gateway.Save(entity);

            entity = gateway.Find<cms_Articles>(entity.Id);

            object a = entity.Statistics;
            a = entity.Statistics;
            object b = entity.Channel;
        }

        //[TestMethod]
        public void TestManyToMany()
        {
            m_User user1 = new m_User();
            user1.Name = "teddy1";
            m_User user2 = new m_User();
            user2.Name = "teddy2";

            m_Group group = new m_Group();
            group.Name = "group";
            group.Users.Add(user1);
            group.Users.Add(user2);
            gateway.Save(group);
            Assert.AreEqual(2, gateway.Count<m_User>(WhereClip.All));
            Assert.AreEqual(2, gateway.Count<m_UserGroup>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_Group>(WhereClip.All));

            user1 = gateway.Find<m_User>(user1.ID);
            user1.Groups = new m_GroupArrayList();
            gateway.Save(user1);
            Assert.AreEqual(2, gateway.Count<m_User>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_UserGroup>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_Group>(WhereClip.All));

            user1.Groups.Add(group);
            gateway.Save(user1);
            Assert.AreEqual(2, gateway.Count<m_User>(WhereClip.All));
            Assert.AreEqual(2, gateway.Count<m_UserGroup>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_Group>(WhereClip.All));

            gateway.Delete(user1);
            Assert.AreEqual(1, gateway.Count<m_User>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_UserGroup>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_Group>(WhereClip.All));

            group = gateway.Find<m_Group>(group.ID);
            group.Users = null;
            gateway.Save(group);
            Assert.AreEqual(0, gateway.Count<m_User>(WhereClip.All));
            Assert.AreEqual(0, gateway.Count<m_UserGroup>(WhereClip.All));
            Assert.AreEqual(1, gateway.Count<m_Group>(WhereClip.All));

            gateway.Delete(group);
            Assert.AreEqual(0, gateway.Count<m_User>(WhereClip.All));
            Assert.AreEqual(0, gateway.Count<m_UserGroup>(WhereClip.All));
            Assert.AreEqual(0, gateway.Count<m_Group>(WhereClip.All));
        }

        [TestMethod]
        public void TestTeamFkQueryUsers()
        {
            Team t = new Team();
            t.Name = "test team";
            User u = new User();
            t.Users.Add(u);
            gateway.Save(t);
        }

        [TestMethod]
        public void TestOrderAndItem()
        {
            Orders orders = new Orders();
            OrderItem[] item = new OrderItem[2];
            item[0] = new OrderItem();
            item[1] = new OrderItem();
            OrderItemArrayList itemArray = new OrderItemArrayList();
            itemArray.AddRange(item);
            orders.OrderItem = itemArray;
            int actual = gateway.Save(orders);
            Console.WriteLine(actual.ToString());

            orders = gateway.Find<Orders>(orders.ID);
            object obj = orders.OrderItem;
            obj = orders.OrderItem;
            obj = orders.OrderItem[0].Order;
            obj = orders.OrderItem[0].Order;            
        }

        [TestMethod]
        public void TestFindPreLoadedEntities()
        {
            LocalUser user = gateway.Find<LocalUser>(WhereClip.All);
            Assert.IsNotNull(user);
            LocalUser[] users = gateway.FindArray<LocalUser>(LocalUser._.Status == UserStatus.Normal, LocalUser._.LoginName.Desc);
            Assert.IsTrue(users.Length >= 0);
            users = gateway.GetPageSelector<LocalUser>(LocalUser._.Status == UserStatus.Normal, LocalUser._.LoginName.Desc, 3).FindPage(1);
            Assert.IsTrue(users.Length >= 0);
            users = gateway.GetPageSelector<LocalUser>(LocalUser._.Status == UserStatus.Normal, OrderByClip.Default, 3).FindPage(2);
            Assert.IsTrue(users.Length >= 0);
            int count = gateway.Count<LocalUser>(WhereClip.All);
            object obj = gateway.FindScalar<LocalUser>(WhereClip.All, OrderByClip.Default, LocalUser._.Name);
            object[] objs = gateway.FindSinglePropertyArray<LocalUser>(LocalUser._.Status == UserStatus.Normal, LocalUser._.LoginName.Desc, LocalUser._.Name);
            Assert.IsTrue(objs.Length >= 0);
        }

        [TestMethod]
        public void TestEntityRelatedPropertyAssignment()
        {
            User user = new User();
            Team team = new Team();
            team.ID = Guid.NewGuid();
            user.Team = team;
            user.Team = null;
        }

        [TestMethod]
        public void TestManyToMany2()
        {
            Group g = new Group();
            g.Name = "mtm2";

            User u1 = new User();
            User u2 = new User();
            g.Users.Add(u1);

            gateway.Save(g);

            g = gateway.Find<Group>(g.ID);
            Assert.AreEqual(1, g.Users.Count);

            g.Users.Add(u2);

            gateway.Save(g);

            g = gateway.Find<Group>(g.ID);
            Assert.AreEqual(2, g.Users.Count);

            gateway.Delete(g);

            Assert.AreEqual(0, gateway.Count<UserGroup>(UserGroup._.GroupID == g.ID));
            Assert.IsNull(gateway.Find<Group>(g.ID));
        }

        [TestMethod]
        public void TestEntityEquals()
        {
            ManyToManyImpl.User user1 = new ManyToManyImpl.User();
            ManyToManyImpl.User user2 = new ManyToManyImpl.User();
            user1.Attach();
            user2.Attach();
            Assert.AreEqual(user1, user2);
            Assert.IsTrue(user1 == user2);
            //Assert.IsTrue((object)user1 == (object)user2);
            Assert.IsTrue(((object)user1).Equals((object)user2));

            List<object> list = new List<object>();
            list.Add(user1);
            Assert.IsTrue(list.Contains(user2));
        }

        [TestMethod]
        public void TestContractEntities()
        {
            SampleEntityWithContract obj = new SampleEntityWithContract();
            SampleEntityWithContract c1 = new SampleEntityWithContract();
            SampleEntityWithContract c2 = new SampleEntityWithContract();
            SampleEntityWithContract p = new SampleEntityWithContract();
            gateway.Save(p);
            obj.Childs.Add(c1);
            obj.Childs.Add(c2);
            obj.Parent = p;
            gateway.Save(obj);
            obj = gateway.Find<SampleEntityWithContract>(obj.ID);
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Parent);
            Assert.IsTrue(obj.Childs.Count == 2);
        }

        [TestMethod]
        public void TestContractEntities2()
        {
            game_SiteNodes obj = new game_SiteNodes();
            game_SiteNodes c1 = new game_SiteNodes();
            game_SiteNodes c2 = new game_SiteNodes();
            game_SiteNodes p = new game_SiteNodes();
            gateway.Save(p);
            obj.Childs.Add(c1);
            obj.Childs.Add(c2);
            obj.Parent = p;
            gateway.Save(obj);
            obj = gateway.Find<game_SiteNodes>(obj.Id);
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Parent);
            Assert.IsTrue(obj.Childs.Count == 2);
        }

        [TestMethod]
        public void TestSaveEntityWithSqlDefaultValueColumn()
        {
            gateway.Save(new Group());
            Assert.IsNotNull(gateway.Find<Group>(Group._.Name == "default group name"));
        }

        [TestMethod]
        public void TestCategoryTree()
        {
            Category rootCategory = new Category();
            rootCategory.Name = "root";
            rootCategory.Childs = new CategoryArrayList();
            Category subCategory1 = new Category();
            subCategory1.Name = "sub 1";
            rootCategory.Childs.Add(subCategory1);
            Category subCategory2 = new Category();
            subCategory2.Name = "sub 2";
            rootCategory.Childs.Add(subCategory2);
            Category subCategory11 = new Category();
            subCategory11.Name = "sub 1.1";
            subCategory1.Childs.Add(subCategory11);
            Category subCategory12 = new Category();
            subCategory12.Name = "sub 1.2";
            subCategory1.Childs.Add(subCategory12);

            gateway.Save(rootCategory);

            rootCategory = gateway.Find<Category>(rootCategory.ID);
            Assert.AreEqual(2, rootCategory.Childs.Count);
            Assert.AreEqual(2, rootCategory.Childs[0].Childs.Count);
        }
    }
}
