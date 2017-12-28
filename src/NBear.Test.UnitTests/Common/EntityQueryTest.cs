using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Entities;
using NBear.Common;

namespace NBear.Test.UnitTests.Common
{
    [TestClass]
    public class EntityQueryTest
    {
        [TestMethod]
        public void TestEntityQueryBasicUsage()
        {
            WhereClip where = (!!(UserGroup._.ID > 0)) & (!(UserGroup._.Name == "teddy"));
            OrderByClip orderBy = UserGroup._.ID.Desc & UserGroup._.Name.Asc;
            Console.WriteLine(where.ToString());
            Console.WriteLine(orderBy.ToString());

            Console.WriteLine(PropertyItem.ParseExpressionByMetaData(where.ToString(), new PropertyToColumnMapHandler(SimplePropertyToColumn), "[", "]", "@"));
            Console.WriteLine(PropertyItem.ParseExpressionByMetaData(orderBy.ToString(), new PropertyToColumnMapHandler(SimplePropertyToColumn), "[", "]", "@"));

            Console.WriteLine(UserGroup._.ID.In(1, 2, 3));
            Console.WriteLine(UserGroup._.ID.Between(2, 6));
        }

        private string SimplePropertyToColumn(string propertyName)
        {
            return propertyName;
        }

        [TestMethod]
        public void TestSerializable()
        {
            PropertyItem pi = new PropertyItem("Teddy");
            Assert.AreEqual(pi.PropertyName, ((PropertyItem)SerializationManager.Deserialize(typeof(PropertyItem), SerializationManager.Serialize(pi))).PropertyName);
            OrderByClip orderBy = new OrderByClip("{ID} DESC");
            Assert.AreEqual(orderBy.OrderBy, ((OrderByClip)SerializationManager.Deserialize(typeof(OrderByClip), SerializationManager.Serialize(orderBy))).OrderBy);
            StoredProcedureParamItem spi = new StoredProcedureParamItem("Test");
            Assert.AreEqual(spi.Name, ((StoredProcedureParamItem)SerializationManager.Deserialize(typeof(StoredProcedureParamItem), SerializationManager.Serialize(spi))).Name);
            WhereClip where1 = WhereClip.All;
            Assert.AreEqual(where1.ToString(), ((WhereClip)SerializationManager.Deserialize(typeof(WhereClip), SerializationManager.Serialize(where1))).ToString());
            WhereClip where2 = new WhereClip("{ID} > @id and {Name} = @Name", 1, "test");
            WhereClip s_where2 = (WhereClip)SerializationManager.Deserialize(typeof(WhereClip), SerializationManager.Serialize(where2));
            Assert.AreEqual(where2.ToString(), s_where2.ToString());
            Assert.AreEqual(where2.ParamValues[0], s_where2.ParamValues[0]);
            Assert.AreEqual(where2.ParamValues[1], s_where2.ParamValues[1]);
        }

        [TestMethod]
        public void TestPropertyItemParam()
        {
            Assert.AreEqual("({CustomerID} + {EmployeeID}) + 1", (Order._.CustomerID + Order._.EmployeeID + 1).CustomValue);
            Assert.AreEqual("{EmployeeID} - 1", (Order._.EmployeeID - 1).CustomValue);
            Assert.AreEqual("(({CustomerID} * {EmployeeID}) + 1) + ({CustomerID} / {EmployeeID})", (Order._.CustomerID * Order._.EmployeeID + 1 + Order._.CustomerID / Order._.EmployeeID).CustomValue);
        }

        [TestMethod]
        public void TestEntityArrayQuery()
        {
            Order[] orders = NBear.Data.Gateway.Default.FindArray<Order>();
            EntityArrayQuery<Order> query = new EntityArrayQuery<Order>(orders);
            Assert.AreEqual(orders.Length, query.FindArray().Length);
            Assert.IsNotNull(query.Find(WhereClip.All));
            Assert.AreEqual(orders.Length, query.FindArray(Order._.OrderID.Desc).Length);
            Assert.AreEqual(orders.Length, query.FindArray(Order._.OrderID > 0).Length);
            Assert.AreEqual(orders.Length, query.FindArray(Order._.OrderID > 0, Order._.OrderID.Desc).Length);
            Assert.IsNotNull(query.Find(10248));
            object avgResult = query.Avg(Order._.OrderID, Order._.OrderID > 0);
            Assert.IsTrue(((int)avgResult) > 0);
            object sumResult = query.Sum(Order._.OrderID, Order._.OrderID > 0);
            Assert.IsTrue((Convert.ToDouble(sumResult)) > 0);
            Assert.IsTrue((Convert.ToInt32(query.Count(Order._.OrderID, Order._.OrderID > 0, true))) > 0);

            Assert.IsTrue(query.FindArray(Order._.ShipCity.Like("%r%")).Length > 0);
        }

        [TestMethod]
        public void TestAndAndOrOrOperator()
        {
            Console.WriteLine((Order._.Freight == 1 && Order._.CustomerID == 2).ToString());
            Console.WriteLine((Order._.Freight == 1 || Order._.CustomerID == 2).ToString());
        }
    }   
}
