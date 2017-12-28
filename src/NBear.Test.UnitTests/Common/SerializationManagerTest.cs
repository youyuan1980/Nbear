using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.Common;
using Newtonsoft.Json;

namespace NBear.Test.UnitTests.Common
{
    [TestClass]
    public class SerializationManagerTest
    {
        [TestMethod]
        public void TestBasicSerializationAndDeserializationUsage()
        {
            int i = 12;
            string data = SerializationManager.Serialize(i);
            Assert.AreEqual(data, "12");
            i = (int)SerializationManager.Deserialize(typeof(int) ,data);
            Assert.AreEqual(i, 12);
        }

        [TestMethod]
        public void TestJsonSerializationPerformance()
        {
            long repeat = 1000;
            Entities.OrderDetailsExtended obj = new Entities.OrderDetailsExtended();
            obj.ProductName = "test";

            long tickJsonSerialize = DateTime.Now.Ticks;
            for (int i = 0; i < repeat; i++)
            {
                string data = JavaScriptConvert.SerializeObject(obj);
            }
            tickJsonSerialize = DateTime.Now.Ticks - tickJsonSerialize;
            Console.WriteLine("json serialization time: " + tickJsonSerialize.ToString());

            long tickXmlSerialize = DateTime.Now.Ticks;
            for (int i = 0; i < repeat; i++)
            {
                string data = SerializationManager.Serialize(obj);
            }
            tickXmlSerialize = DateTime.Now.Ticks - tickXmlSerialize;
            Console.WriteLine("xml serialization time: " + tickXmlSerialize.ToString());

            string jsonData = JavaScriptConvert.SerializeObject(obj); ;
            string xmlData = SerializationManager.Serialize(obj);
            Console.WriteLine("serialized json size: " + jsonData.Length.ToString());
            Console.WriteLine(jsonData);
            Console.WriteLine("serialized xml size: " + xmlData.Length.ToString());
            Console.WriteLine(xmlData);

            long tickJsonDeserialize = DateTime.Now.Ticks;
            for (int i = 0; i < repeat; i++)
            {
                Entities.OrderDetailsExtended obj2 = (Entities.OrderDetailsExtended)JavaScriptConvert.DeserializeObject(jsonData, typeof(Entities.OrderDetailsExtended));
            }
            tickJsonDeserialize = DateTime.Now.Ticks - tickJsonDeserialize;
            Console.WriteLine("json deserialization time: " + tickJsonDeserialize.ToString());

            long tickXmlDeserialize = DateTime.Now.Ticks;
            for (int i = 0; i < repeat; i++)
            {
                Entities.OrderDetailsExtended obj2 = (Entities.OrderDetailsExtended)SerializationManager.Deserialize(typeof(Entities.OrderDetailsExtended), xmlData);
            }
            tickXmlDeserialize = DateTime.Now.Ticks - tickXmlDeserialize;
            Console.WriteLine("xml deserialization time: " + tickXmlDeserialize.ToString());
        }
    }
}
