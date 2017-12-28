using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBear.MVP;
using NBear.IoC.Service;

namespace NBear.Test.UnitTests.MVP
{
    public interface ITestGenericPresentor<GenericType> : IPresenter
    {
    }

    public interface ITestPresenter : IPresenter
    {
    }

    public class TestPresenter : Presenter<ITestView, ITestService>, ITestPresenter, ITestGenericPresentor<Entities.Address>
    {
    }

    [ServiceContract]
    public interface ITestService// : IServiceInterface
    {
    }

    public interface ITestView
    {
    }

    public class TestView : ITestView
    {
    }

    [TestClass]
    public class MVPTest
    {
        [TestMethod]
        public void TestPresenterFactory()
        {
            ITestPresenter presenter = PresenterFactory.Create().GetPresenter<ITestPresenter>(new TestView());
            Assert.IsNotNull(presenter);

            ITestGenericPresentor<Entities.Address> presenter2 = PresenterFactory.Create().GetPresenter<ITestGenericPresentor<Entities.Address>>(new TestView());
            Assert.IsNotNull(presenter2);
        }
    }
}
