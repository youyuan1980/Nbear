using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;

using NBear.Test.UnitTests.AnalysisTests.NullableSerializeTest_temp;

namespace NBear.Test.UnitTests.AnalysisTests
{
    namespace NullableSerializeTest_temp
    {
        public class User
        {
            private int? id;
            private string name;
            private Address? addr;

            public int? ID
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public Address? Addr
            {
                get
                {
                    return addr;
                }
                set
                {
                    addr = value;
                }
            }
        }

        public struct Address
        {
            public string Addr1;
            public string Addr2;
        }
    }

    /// <summary>
    /// Test Nullable object's serialization ability
    /// </summary>
    [TestClass]
    public class NullableSerializeTest
    {
        [TestMethod]
        public void TestNullableSerialize()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(User));
            User obj = new User();
            obj.ID = 1;
            obj.Name = "teddy";
            serializer.Serialize(sw, obj);
            Console.WriteLine(sb.ToString());

            sb.Remove(0, sb.Length);
            obj.ID = null;
            obj.Name = null;
            serializer.Serialize(sw, obj);
            Console.WriteLine(sb.ToString());
        }
    }
}
