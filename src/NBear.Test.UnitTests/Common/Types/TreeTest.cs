using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.Common.Types;

namespace NBear.Test.UnitTests.Common.Types
{
    [TestClass]
    public class TreeTest
    {
        [TestMethod]
        public void TestTreeUsage()
        {
            Node<string> rootNode = new Node<string>("teddy");
            rootNode.Children.Add(new Node<string>("child1"));
            rootNode.Children.Add(new Node<string>("child2"));

            foreach (string s in rootNode.GetBreadthFirstEnumerator())
            {
                Console.WriteLine(s);
            }
        }
    }
}
