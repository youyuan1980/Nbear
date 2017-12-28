using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.Common;
using NBear.Data;
using Entities;

namespace NBear.Test.UnitTests.Data
{
    namespace temp_design
    {
        [NBear.Common.Design.MappingName("Orders")]
        public interface Order : NBear.Common.Design.Entity
        {
            [NBear.Common.Design.PrimaryKey]
            int OrderID { get; }
            string CustomerID { get; set; }
            int EmployeeID { get; set; }
            DateTime OrderDate { get; set; }
            DateTime RequiredDate { get; set; }
            DateTime ShippedDate { get; set; }
            int ShipVia { get; set; }
            decimal Freight { get; set; }
            string ShipName { get; set; }
            string ShipAddress { get; set; }
            string ShipCity { get; set; }
            string ShipRegion { get; set; }
            string ShipPostalCode { get; set; }
            string ShipCountry { get; set; }
        }
    }

    [TestClass]
    public class PerformenceComparisonTest
    {
        private Gateway gateway = null;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            gateway = new Gateway("Northwind");
            //gateway.RegisterSqlLogger(new LogHandler(Console.Write));
        }

        private const int REPEAT_TIME = 1;//100;

        #region Read

        private object DaabRead()
        {
            DataSet ds = gateway.SelectDataSet("select OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry from Orders where OrderID > @OrderID Order By OrderID desc", new object[] { 10 });
            ds = null;
            return null;
        }

        private object DbHelperRead()
        {
            DataSet ds = gateway.DbHelper.Select("Orders", new string[] { "OrderID", "CustomerID", "EmployeeID", "OrderDate", "RequiredDate", "ShippedDate", "ShipVia", "Freight", "ShipName", "ShipAddress", "ShipCity", "ShipRegion", "ShipPostalCode", "ShipCountry" }, "OrderID > @OrderID", new object[] { 10 }, "OrderID desc");
            ds = null;
            return null;
        }

        private object GatewayRead()
        {
            Order[] orders = gateway.FindArray<Order>(Order._.OrderID > 10, Order._.OrderID.Desc);
            orders = null;
            return null;
        }

        private object DaabReadFirstPage()
        {
            DataSet ds = gateway.SelectDataSet("select top 100 * from Orders where OrderID > @OrderID Order By OrderID desc", new object[] { 10 });
            ds = null;
            return null;
        }

        private void DaabReadSecondPage(object val)
        {
            DataSet ds = gateway.SelectDataSet("select top 100 * from Orders where OrderID > @OrderID and OrderID Not In (select top 100 OrderID from Orders where OrderID > @OrderID_Inside) Order By OrderID desc", new object[] { 10, 10 });
            ds = null;
        }

        private object DbHelperReadFirstPage()
        {
            IPageSplit ps = gateway.DbHelper.SelectPageSplit("Orders", new string[] { "*" }, "OrderID > @OrderID", "OrderID desc", "OrderID", new object[] { 10 });
            ps.PageSize = 100;
            DataSet ds = ps.GetPage(1);
            ds = null;
            ps = null;
            return null;
        }

        private void DbHelperReadSecondPage(object val)
        {
            IPageSplit ps = gateway.DbHelper.SelectPageSplit("Orders", new string[] { "*" }, "OrderID > @OrderID", "OrderID desc", "OrderID", new object[] { 10 });
            ps.PageSize = 100;
            DataSet ds = ps.GetPage(2);
            ds = null;
            ps = null;
        }

        private object GatewayReadFirstPage()
        {
            PageSelector<Order> ps = gateway.GetPageSelector<Order>(Order._.OrderID > 10, OrderByClip.Default, 100);
            ps.PageSize = 100;
            Order[] orders = ps.FindPage(1);
            orders = null;
            ps = null;
            return null;
        }

        private void GatewayReadSecondPage(object val)
        {
            PageSelector<Order> ps = gateway.GetPageSelector<Order>(Order._.OrderID > 10, OrderByClip.Default, 100);
            ps.PageSize = 100;
            Order[] orders = ps.FindPage(2);
            orders = null;
            ps = null;
        }

        [TestMethod]
        public void TestReadPerformenceComparison()
        {
            DaabRead();
            DbHelperRead();
            GatewayRead();

            Console.WriteLine(string.Format("Daab read (Single Thread):\t\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(DaabRead))));
            Console.WriteLine(string.Format("DbHelper read (Single Thread):\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(DbHelperRead))));
            Console.WriteLine(string.Format("Gateway read (Single Thread):\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(GatewayRead))));
            Console.WriteLine(string.Format("Daab read (Multi Thread):\t\t{0}", TestLoader.GetMultiThreadSpend(REPEAT_TIME, new TestHandler1(DaabRead))));
            Console.WriteLine(string.Format("DbHelper read (Multi Thread):\t\t{0}", TestLoader.GetMultiThreadSpend(REPEAT_TIME, new TestHandler1(DbHelperRead))));
            Console.WriteLine(string.Format("Gateway read (Multi Thread):\t\t{0}", TestLoader.GetMultiThreadSpend(REPEAT_TIME, new TestHandler1(GatewayRead))));
        }

        [TestMethod]
        public void TestReadPagePerformenceComparison()
        {
            DaabReadFirstPage();
            DaabReadSecondPage(null);
            DbHelperReadFirstPage();
            DbHelperReadSecondPage(null);
            GatewayReadFirstPage();
            GatewayReadSecondPage(null);

            Console.WriteLine(string.Format("Daab read page (Single Thread):\t\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(DaabReadFirstPage), new TestHandler2(DaabReadSecondPage))));
            Console.WriteLine(string.Format("DbHelper read page (Single Thread):\t\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(DbHelperReadFirstPage), new TestHandler2(DbHelperReadSecondPage))));
            Console.WriteLine(string.Format("Gateway read page (Single Thread):\t\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(GatewayReadFirstPage), new TestHandler2(GatewayReadSecondPage))));
            Console.WriteLine(string.Format("Daab read page (Multi Thread):\t\t{0}", TestLoader.GetMultiThreadSpend(REPEAT_TIME, new TestHandler1(DaabReadFirstPage), new TestHandler2(DaabReadSecondPage))));
            Console.WriteLine(string.Format("DbHelper read page (Multi Thread):\t\t{0}", TestLoader.GetMultiThreadSpend(REPEAT_TIME, new TestHandler1(DbHelperReadFirstPage), new TestHandler2(DbHelperReadSecondPage))));
            Console.WriteLine(string.Format("Gateway read page (Multi Thread):\t\t{0}", TestLoader.GetMultiThreadSpend(REPEAT_TIME, new TestHandler1(GatewayReadFirstPage), new TestHandler2(GatewayReadSecondPage))));
        }

        #endregion

        #region Write

        private object DaabWrite()
        {
            gateway.ExecuteNonQuery("update Orders set ShippedDate = @ShipDate where OrderID = @OrderID", new object[] { DateTime.Now, 10248 });
            gateway.ExecuteNonQuery("delete from Orders where OrderID = @OrderID", new object[] { 0 });
            return null;
        }

        private object DbHelperWrite()
        {
            gateway.DbHelper.Update("Orders", new string[] { "ShippedDate" }, new object[] { DateTime.Now }, "OrderID = @OrderID", new object[] { 10248 });
            gateway.DbHelper.Delete("Orders", "OrderID = @OrderID", new object[] { 0 });
            return null;
        }

        private Order orderToBeUpdated = null;

        private object GatewayWrite()
        {
            orderToBeUpdated.ShippedDate = DateTime.MaxValue;
            orderToBeUpdated.ShippedDate = DateTime.Now;
            gateway.Save(orderToBeUpdated);
            gateway.Delete(new Order());
            return null;
        }

        private Gateway batchGateway;

        private object GatewayBatchWrite()
        {
            orderToBeUpdated.ShippedDate = DateTime.Now;
            batchGateway.Save(orderToBeUpdated);
            batchGateway.Delete(new Order());
            return null;
        }

        [TestMethod]
        public void TestWritePerformenceComparison()
        {
            DaabWrite();
            DbHelperWrite();
            orderToBeUpdated = gateway.Find<Order>(10248);
            GatewayWrite();
            batchGateway = gateway.BeginBatchGateway(10);
            GatewayBatchWrite();
            batchGateway.EndBatch();
            batchGateway.BeginBatch(10);

            Console.WriteLine(string.Format("Daab write (Single Thread):\t\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(DaabWrite))));
            Console.WriteLine(string.Format("DbHelper write (Single Thread):\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(DbHelperWrite))));
            Console.WriteLine(string.Format("Gateway write (Single Thread):\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(GatewayWrite))));
            Console.WriteLine(string.Format("Gateway batch write (Single Thread):\t{0}", TestLoader.GetSingleThreadSpend(REPEAT_TIME, new TestHandler1(GatewayBatchWrite))));
            batchGateway.EndBatch();
            batchGateway = null;
        }

        #endregion
    }
}
