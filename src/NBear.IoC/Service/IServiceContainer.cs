using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Castle.Core;
using Castle.MicroKernel;
using NBear.IoC.Service.Services.Configuration;
using NBear.Common;

namespace NBear.IoC.Service
{
    /// <summary>
    /// The service container interface.
    /// </summary>
    public interface IServiceContainer : IDisposable, ILogable
    {
        /// <summary>
        /// Gets or sets the max try num.
        /// </summary>
        /// <value>The max try num.</value>
        int MaxTryNum { get; set; }
        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        IKernel Kernel { get; }
        /// <summary>
        /// Gets the MQ.
        /// </summary>
        /// <value>The MQ.</value>
        IServiceMQ MQ { get; }
        /// <summary>
        /// Registers the component.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="classType">Type of the class.</param>
        void RegisterComponent(string key, Type classType);
        /// <summary>
        /// Registers the component.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="classType">Type of the class.</param>
        void RegisterComponent(string key, Type serviceType, Type classType);
        /// <summary>
        /// Registers the components.
        /// </summary>
        /// <param name="serviceKeyTypes">The service key types.</param>
        void RegisterComponents(IDictionary serviceKeyTypes);
        /// <summary>
        /// Releases the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Release(object obj);
        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        object this[string Key] { get; }
        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified service type.
        /// </summary>
        /// <value></value>
        object this[Type serviceType] { get; }
        /// <summary>
        /// Calls the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns>The response msg.</returns>
        ResponseMessage CallService(string serviceName, RequestMessage msg);
        /// <summary>
        /// Gets the service nodes.
        /// </summary>
        /// <returns>The service nodes.</returns>
        ServiceNodeInfo[] GetServiceNodes();
        /// <summary>
        /// Gets the depender service nodes.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The service nodes.</returns>
        ServiceNodeInfo[] GetDependerServiceNodes(string key);
        /// <summary>
        /// Gets the dependent service nodes.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The service nodes.</returns>
        ServiceNodeInfo[] GetDependentServiceNodes(string key);
        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="logInfo">The log info.</param>
        void WriteLog(string logInfo);

        /// <summary>
        /// Gets or sets a value indicating whether return value of service <see cref="IServiceContainer"/> is compress.
        /// </summary>
        /// <value><c>true</c> if compress; otherwise, <c>false</c>.</value>
        bool Compress { get; set; }
    }
}
