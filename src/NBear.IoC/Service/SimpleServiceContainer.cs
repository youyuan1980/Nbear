using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Castle.MicroKernel;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Facilities.Startable;

using NBear.Common;
using NBear.IoC.Service.Services;
using NBear.IoC.Service.Services.Configuration;

namespace NBear.IoC.Service
{
    /// <summary>
    /// The simple service container.
    /// </summary>
    public sealed class SimpleServiceContainer : IServiceContainer
    {
        #region Const Members

        /// <summary>
        /// The default max try number.
        /// </summary>
        public const int DEFAULT_MAX_TRY_NUMBER = 30;

        #endregion

        #region Private Members

        private Castle.Windsor.IWindsorContainer container;
        private ServiceProxy serviceProxy;
        private IServiceMQ mq;
        private bool compress = false;

        private void Init(IServiceMQ mq, IDictionary serviceKeyTypes)
        {
            if (System.Configuration.ConfigurationManager.GetSection("castle") != null)
            {
                container = new WindsorContainer(new XmlInterpreter());
            }
            else
            {
                container = new WindsorContainer();
            }
            container.AddFacility("startable", new StartableFacility());
            this.mq = mq;
            container.Kernel.AddComponentInstance("NBear.IoC.Service.IServiceMQ", typeof(IServiceMQ), mq);
            container.AddFacility("service subscribe message reqMsg", new Facilities.ServiceSubscribeMessageRequestFacility(mq));

            if (serviceKeyTypes != null && serviceKeyTypes.Count > 0)
            {
                RegisterComponents(serviceKeyTypes);
            }

            serviceProxy = new ServiceProxy(mq, DEFAULT_MAX_TRY_NUMBER);
            serviceProxy.OnLog += new LogHandler(WriteLog);

            this.DiscoverServices();
        }

        private static ServiceNodeInfo[] ParseServiceNodes(GraphNode[] nodes)
        {
            if (nodes == null)
            {
                return null;
            }

            List<ServiceNodeInfo> serviceNodes = new List<ServiceNodeInfo>();

            for (int i = 0; i < nodes.Length; i++)
            {
                ComponentModel node = (ComponentModel)nodes[i];
                if (typeof(IService).IsAssignableFrom(node.Service))
                {
                    ServiceNodeInfo serviceNode = new ServiceNodeInfo();
                    serviceNode.Key = node.Name;
                    serviceNode.Sevice = node.Service.FullName;
                    serviceNode.Implementation = node.Implementation.FullName;
                    serviceNodes.Add(serviceNode);
                }
            }

            return serviceNodes.ToArray();
        }

        private ComponentModel GetComponentModelByKey(string key)
        {
            GraphNode[] nodes = Kernel.GraphNodes;
            foreach (ComponentModel node in nodes)
            {
                if (node.Name.Equals(key))
                {
                    return node;
                }
            }
            return null;
        }

        private IService GetLocalService(string serviceName)
        {
            ServiceNodeInfo[] serviceNodes = GetServiceNodes();
            foreach (ServiceNodeInfo serviceNode in serviceNodes)
            {
                IService obj = (IService)container[serviceNode.Key];
                if ((obj).ServiceName == serviceName)
                {
                    return obj;
                }
            }

            return null;
        }

        private void DiscoverServices()
        {
            GraphNode[] nodes = this.Kernel.GraphNodes;
            foreach (ComponentModel model in nodes)
            {
                bool markedWithServiceContract = typeof(IServiceInterface).IsAssignableFrom(model.Service);
                if (!markedWithServiceContract)
                {
                    foreach (object attr in model.Service.GetCustomAttributes(true))
                    {
                        if (attr.ToString().EndsWith("ServiceContractAttribute"))
                        {
                            markedWithServiceContract = true;
                            break;
                        }
                    }
                }
                if (markedWithServiceContract)
                {
                    DynamicService service = new DynamicService(this, model.Service);
                    Kernel.AddComponentInstance(Guid.NewGuid().ToString(), service);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleServiceContainer"/> class.
        /// </summary>
        public SimpleServiceContainer()
        {
            Init(new MemoryServiceMQ(), null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleServiceContainer"/> class.
        /// </summary>
        /// <param name="mq">The mq.</param>
        public SimpleServiceContainer(IServiceMQ mq)
        {
            Init(mq, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleServiceContainer"/> class.
        /// </summary>
        /// <param name="mq">The mq.</param>
        /// <param name="serviceKeyTypes">The service key types.</param>
        public SimpleServiceContainer(IServiceMQ mq, IDictionary serviceKeyTypes)
        {
            Init(mq, serviceKeyTypes);
        }

        #endregion

        #region IServiceContainer Members

        /// <summary>
        /// Gets or sets the max try num.
        /// </summary>
        /// <value>The max try num.</value>
        public int MaxTryNum
        {
            get
            {
                return serviceProxy.MaxTryNum;
            }
            set
            {
                serviceProxy.MaxTryNum = value;
            }
        }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        public Castle.MicroKernel.IKernel Kernel
        {
            get { return container.Kernel; }
        }

        /// <summary>
        /// Gets the MQ.
        /// </summary>
        /// <value>The MQ.</value>
        public IServiceMQ MQ
        {
            get
            {
                return mq;
            }
        }

        /// <summary>
        /// Registers the component.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="classType">Type of the class.</param>
        public void RegisterComponent(string key, Type serviceType, Type classType)
        {
            container.AddComponent(key, serviceType, classType);
        }

        /// <summary>
        /// Registers the component.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="classType">Type of the class.</param>
        public void RegisterComponent(string key, Type classType)
        {
            container.AddComponent(key, typeof(IService), classType);
        }

        /// <summary>
        /// Registers the components.
        /// </summary>
        /// <param name="serviceKeyTypes">The service key types.</param>
        public void RegisterComponents(System.Collections.IDictionary serviceKeyTypes)
        {
            System.Collections.IDictionaryEnumerator en = serviceKeyTypes.GetEnumerator();
            while (en.MoveNext())
            {
                string key = en.Key.ToString();
                Type type = (Type)(en.Value);
                container.AddComponent(key, typeof(IService), type);
            }
        }

        /// <summary>
        /// Releases the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Release(object obj)
        {
            container.Release(obj);
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get { return container[key]; }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified service type.
        /// </summary>
        /// <value></value>
        public object this[Type serviceType]
        {
            get { return container[serviceType]; }
        }

        /// <summary>
        /// Calls the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns>The msg.</returns>
        public ResponseMessage CallService(string serviceName, RequestMessage msg)
        {
            //check local service first
            IService localService = (IService)GetLocalService(serviceName);

            if (localService != null)
            {
                if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] Calling local service: {0}[{1}]", serviceName, localService.ClientId));
                return localService.CallService(msg);
            }

            //if no local service, call remote service
            if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] Calling remote service: {0}", serviceName));
            return serviceProxy.CallMethod(serviceName, msg);
        }

        /// <summary>
        /// Gets the service nodes.
        /// </summary>
        /// <returns>The service nodes.</returns>
        public NBear.IoC.Service.Services.Configuration.ServiceNodeInfo[] GetServiceNodes()
        {
            return ParseServiceNodes(Kernel.GraphNodes);
        }

        /// <summary>
        /// Gets the depender service nodes.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The service nodes.</returns>
        public NBear.IoC.Service.Services.Configuration.ServiceNodeInfo[] GetDependerServiceNodes(string key)
        {
            ComponentModel node = GetComponentModelByKey(key);
            if (node == null)
            {
                return null;
            }
            return ParseServiceNodes(node.Dependers);
        }

        /// <summary>
        /// Gets the dependent service nodes.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The service nodes.</returns>
        public NBear.IoC.Service.Services.Configuration.ServiceNodeInfo[] GetDependentServiceNodes(string key)
        {
            ComponentModel node = GetComponentModelByKey(key);
            if (node == null)
            {
                return null;
            }
            return ParseServiceNodes(node.Dependents);
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="logInfo">The log info.</param>
        public void WriteLog(string logInfo)
        {
            if (OnLog != null)
            {
                OnLog(logInfo);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether return value of service <see cref="IServiceContainer"/> is compress.
        /// </summary>
        /// <value><c>true</c> if compress; otherwise, <c>false</c>.</value>
        public bool Compress
        {
            get
            {
                return compress;
            }
            set
            {
                compress = value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            container.Dispose();
        }

        #endregion

        #region ILogable Members

        /// <summary>
        /// OnLog event.
        /// </summary>
        public event LogHandler OnLog;

        #endregion
    }
}
