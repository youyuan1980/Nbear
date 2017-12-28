using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

using NBear.Common;

namespace NBear.Test.UnitTests.Common
{
    [TestClass]
    public class MetaDataManagerTest
    {
        //[TestMethod]
        public void TestEntityConfigurationSectionHandler()
        {
            EntityConfigurationSection section = new EntityConfigurationSection();
            KeyValueConfigurationCollection includes = new KeyValueConfigurationCollection();
            includes.Add(new KeyValueConfigurationElement("Sample", "c:\\test.xml"));
            section.Includes = includes;
            Configuration config = ConfigurationManager.OpenMachineConfiguration();
            config.Sections.Add("entityConfig", section);
            config.SaveAs("c:\\test.config");
        }

        //[TestMethod]
        public void TestEntityConfiguration()
        {
            //EntityConfiguration returnEntityEc = MetaDataManager.GetEntityConfiguration(typeof(Entities.TestUser));
            //EntityConfiguration ec2 = MetaDataManager.GetEntityConfiguration(typeof(Entities.LocalUser));
        }
    }
}
