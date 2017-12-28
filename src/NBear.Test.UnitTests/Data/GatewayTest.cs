using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;

using NBear.Common;
using NBear.Data;
using Entities;

namespace NBear.Test.UnitTests.Data
{
    namespace temp_design
    {
        [NBear.Common.Design.ReadOnly]
        [NBear.Common.Design.MappingName("Order Details Extended")]
        public interface OrderDetailsExtended : NBear.Common.Design.Entity
        {
            int OrderID { get; }
            int ProductID { get; }
            string ProductName { get; }
            decimal UnitPrice { get; }
            short Quantity { get; }
            float Discount { get; }
            decimal ExtendedPrice { get; }
        }

        [NBear.Common.Design.MappingName("Categories")]
        public interface Category : NBear.Common.Design.Entity
        {
            [NBear.Common.Design.PrimaryKey]
            int CategoryID { get; }
            string CategoryName { get; set; }
            string Description { get; set; }
            byte[] Picture { get; set; }
        }
    }

    [TestClass]
    public class GatewayTest
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
            gateway = new Gateway("Northwind");
            gateway.RegisterSqlLogger(new LogHandler(Console.Write));
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestFindReadOnlyEntity()
        {
            OrderDetailsExtended orderDetail = gateway.Find<OrderDetailsExtended>(10248);
            Assert.IsNotNull(orderDetail);
        }

        [TestMethod]
        public void TestFindScalar()
        {
            object obj = gateway.FindScalar<Order>(Order._.OrderID == 10248, OrderByClip.Default, Order._.ShippedDate);
            Assert.IsTrue(((DateTime)obj) > DateTime.Parse("2006-10-28"));
        }

        [TestMethod]
        public void TestSelectAggregation()
        {
            int minVal = (int)gateway.Min<Order>(WhereClip.All, Order._.OrderID);
            int maxVal = (int)gateway.Max<Order>(WhereClip.All, Order._.OrderID);
            int sumVal = (int)gateway.Sum<Order>(WhereClip.All, Order._.OrderID);
            int avgVal = (int)gateway.Avg<Order>(WhereClip.All, Order._.OrderID);

            Assert.IsTrue(minVal < avgVal && avgVal < maxVal && maxVal < sumVal);
        }

        [TestMethod]
        public void TestFindSinglePropertyArray()
        {
            object[] ids = gateway.FindSinglePropertyArray<Order>(Order._.OrderID > 10, Order._.OrderID.Desc, Order._.OrderID);
            Assert.IsTrue(ids.Length > 0);
        }

        [TestMethod]
        public void TestFindViewPage()
        {
            OrderDetailsExtended[] orders = gateway.GetPageSelector<OrderDetailsExtended>(WhereClip.All, OrderByClip.Default, 10).FindPage(2);
        }

        [TestMethod]
        public void TestEntitySerialization()
        {
            OrderDetailsExtended[] orders = gateway.GetPageSelector<OrderDetailsExtended>(WhereClip.All, OrderByClip.Default, 10).FindPage(2);

            Console.WriteLine(SerializationManager.Serialize(orders[0]));
            Console.WriteLine(SerializationManager.Serialize(orders));
        }

        [TestMethod]
        public void TestEntityArrayToDataTable()
        {
            OrderDetailsExtended[] orders = gateway.GetPageSelector<OrderDetailsExtended>(WhereClip.All, OrderByClip.Default, 10).FindPage(2);
            System.Data.DataTable dt = Entity.EntityArrayToDataTable(orders);
            Assert.AreEqual(dt.Rows.Count, orders.Length);
            dt = Entity.EntityArrayToDataTable(new OrderDetailsExtended[0]);
            Assert.AreEqual(dt.Rows.Count, 0);
            dt = Entity.EntityArrayToDataTable<OrderDetailsExtended>(null);
            Assert.AreEqual(dt.Rows.Count, 0);
        }

        [TestMethod]
        public void TestBatchUpdate()
        {
            DbTransaction tran = gateway.BeginTransaction();
            gateway.Update<OrderDetailsExtended>(new PropertyItem[] { OrderDetailsExtended._.UnitPrice }, new object[] { 100 }, OrderDetailsExtended._.UnitPrice == 100, tran);
            tran.Commit();
            gateway.CloseTransaction(tran);
            gateway.Update<OrderDetailsExtended>(new PropertyItem[] { OrderDetailsExtended._.UnitPrice }, new object[] { OrderDetailsExtended._.ExtendedPrice }, OrderDetailsExtended._.UnitPrice == 100);
            gateway.Update<OrderDetailsExtended>(new PropertyItem[] { OrderDetailsExtended._.UnitPrice }, new object[] { new PropertyItemParam("{ExtendedPrice} + {ExtendedPrice} + 100") }, OrderDetailsExtended._.UnitPrice == 100);
        }

        [TestMethod]
        public void TestManualSave()
        {
            Order order = gateway.Find<Order>(WhereClip.All);

            //test cache
            order = gateway.Find<Order>(WhereClip.All);
            gateway.RemoveCaches(typeof(Order).Name);
            order = gateway.Find<Order>(WhereClip.All);

            //test manual save
            Order manualOrder = new Order();
            manualOrder.OrderID = order.OrderID;
            manualOrder.Attach();
            manualOrder.OrderDate = DateTime.Now;
            gateway.Save(manualOrder);
        }

        [TestMethod]
        public void TestSaveNewEntityWithAutoProperty()
        {
            Category newCat = new Category();
            newCat.CategoryName = "test";
            newCat.Description = "desc";
            gateway.Save(newCat);

            Assert.IsTrue(newCat.CategoryID > 0);

            newCat.Picture = null;

            gateway.Save(newCat);

            gateway.Delete(newCat);
        }

        [TestMethod]
        public void TestAccessDatabaseRelativePath()
        {
            Gateway g1 = new Gateway("access1");
            Gateway g2 = new Gateway("access2");
            Gateway g3 = new Gateway("access3");
            Console.WriteLine(g1.Db.ConnectionString);
            Console.WriteLine(g2.Db.ConnectionString);
            Console.WriteLine(g3.Db.ConnectionString);
        }

        [TestMethod]
        public void TestInSubQuery()
        {
            object obj = gateway.FindArray<Order>(gateway.InSubQuery<OrderDetailsExtended>(Order._.OrderID, OrderDetailsExtended._.OrderID, OrderDetailsExtended._.ExtendedPrice > 100), OrderByClip.Default);
        }

        [TestMethod]
        public void TestCompression()
        {
            string inStr = SerializationManager.Serialize(gateway.FindArray<Order>());
            string outGZip = CompressionManager.Compress(inStr);
            string out7Zip = CompressionManager.Compress7Zip(inStr);
            Console.WriteLine("Input Size: " + inStr.Length.ToString());
            Console.WriteLine("GZip Output Size: " + outGZip.Length.ToString());
            Console.WriteLine("7Zip Output Size: " + out7Zip.Length.ToString());

            Assert.AreEqual(inStr.Length, CompressionManager.Decompress7Zip(out7Zip).Length);
            Assert.AreEqual(inStr.Length, CompressionManager.Decompress(outGZip).Length);
        }

        [TestMethod]
        public void TestFindDataTable()
        {
            System.Data.DataTable dt = gateway.FindDataTable<OrderDetailsExtended>(WhereClip.All, OrderByClip.Default);
            Assert.IsNotNull(dt);
        }

        /// <summary>
        /// Tests the oracle.
        /// </summary>
        //[TestMethod]
        public void TestOracle()
        {
            Gateway oracle = new Gateway(DatabaseType.Oracle, "Data Source=localhost;User ID=system;Password=sa;Unicode=True");
            oracle.RegisterSqlLogger(new LogHandler(Console.WriteLine));
            TempTable obj = new TempTable();
            obj.ColName = "test";
            obj.Guid = Guid.NewGuid();
            //obj.ID = 998;
            oracle.Save(obj);
            Assert.IsTrue(obj.ID > 0);
            oracle.Delete<TempTable>(TempTable._.ID == 998 & TempTable._.ColName == "test");
            TempTable[] objs = oracle.GetPageSelector<TempTable>(WhereClip.All, OrderByClip.Default, 10).FindPage(1);
            objs = oracle.GetPageSelector<TempTable>(WhereClip.All, OrderByClip.Default, 10).FindPage(2);
        }

        [TestMethod]
        public void TestCache()
        {
            Order[] orders = gateway.FindArray<Order>(WhereClip.All, OrderByClip.Default);
            gateway.Update<Order>(new PropertyItem[] { Order._.RequiredDate }, new object[] { DateTime.Now }, Order._.OrderID == 10000);
            orders = gateway.FindArray<Order>(WhereClip.All, OrderByClip.Default);
        }

        [TestMethod]
        public void TestAccessPageSplit()
        {
            Gateway access = new Gateway(DatabaseType.MsAccess, @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=C:\Teddy\NBearV3\cases\SimpleGuestbook\website\App_Data\SimpleGuestbook.mdb");
            access.RegisterSqlLogger(Console.WriteLine);
            Message[] objs = access.GetPageSelector<Message>(Message._.PostTime > new DateTime(2000, 1, 1), Message._.ID.Desc, 5).FindPage(2);
            objs = access.GetPageSelector<Message>(Message._.PostTime > new DateTime(2000, 1, 1), Message._.ID.Desc, 5).FindPage(1);
        }

        //[TestMethod]
        public void TestPerformanceRedutionOfPageSplitSqlCache()
        {
            TimeSpan t = TestLoader.GetMultiThreadSpend(5000, new TestHandler1(DoTestPerformanceRedutionOfPageSplitSqlCache));
            Console.WriteLine(t.ToString());
        }

        private object DoTestPerformanceRedutionOfPageSplitSqlCache()
        {
            Gateway g = new Gateway(new Database(gateway.Db.DbProvider));
            OrderDetailsExtended[] orders = g.GetPageSelector<OrderDetailsExtended>(WhereClip.All, OrderByClip.Default, 10).FindPage(2);
            return null;
        }

        [TestMethod]
        public void TestPropertyItemParamQuery()
        {
            gateway.FindArray<Order>(Order._.RequiredDate - Order._.OrderDate < new TimeSpan(5, 0, 0, 0));
            gateway.FindArray<Order>((Order._.OrderDate - DateTime.Parse("2006-1-1")) < (Order._.RequiredDate - DateTime.Parse("2007-1-2")));
            gateway.Update<Order>(new PropertyItem[] { Order._.Freight }, new object[] { Order._.Freight + 1 - 1 }, Order._.Freight == (Order._.Freight + 1) / 1 * 1);
        }

        [TestMethod]
        public void TestWhereClipWithAtCharInStringParam()
        {
            WhereClip where = new WhereClip("{Name} = @Name and {ID} = @ID and {a} = @a", "teddy@ID.com", "1234@1234.com", 1);
            WhereClip flat = gateway.ToFlatWhereClip(where, new Order().GetEntityConfiguration());
            Console.WriteLine(flat.ToString());
        }
    }
}
