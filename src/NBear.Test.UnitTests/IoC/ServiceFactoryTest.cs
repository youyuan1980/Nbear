using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NBear.IoC.Service;
using NBear.IoC.Service.Configuration;

namespace NBear.Test.UnitTests.IoC
{
    [TestClass]
    public class ServiceFactoryTest
    {
        //[TestMethod]
        public void TestCreateServiceFactoryConfigurationSection()
        {
            ServiceFactoryConfigurationSection section = new ServiceFactoryConfigurationSection();
            Configuration config = ConfigurationManager.OpenMachineConfiguration();
            config.Sections.Add("serviceFactory", section);
            config.SaveAs("c:\\test.config");
        }

        [TestMethod]
        public void TestLoadedServiceFactoryConfigurationSection()
        {
            ServiceFactoryConfigurationSection section = ServiceFactory.LoadServiceFactoryConfiguration();
            Assert.AreEqual(section.Type, ServiceFactoryType.Local);
            Assert.AreEqual(section.ServiceMQName, "testServiceFactory");
        }

        [TestMethod]
        public void TestServiceInterfaceImpl()
        {
            ISampleService service = ServiceFactory.Create().GetService<ISampleService>();
            service.Hello();
            service.Hello1();
            service.Hello2();
            service.HideBase();

            //IBaseGenericService<string> service2 = ServiceFactory.Create().GetService<IBaseGenericService<string>>();
            //service2.Hello();
        }
    }

    public interface IBaseService : IBaseGenericService<string>
    {
        string Hello1();
        int HideBase();
    }

    [ServiceContract]
    public interface ISampleService : IBaseService
    {
        string Hello2();
        new string HideBase();
    }

    [ServiceContract]
    public interface IBaseGenericService<T>
    {
        T Hello();
    }
}
