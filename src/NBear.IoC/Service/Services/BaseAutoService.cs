using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NBear.IoC.Service.Services
{
    /// <summary>
    /// The base class of all auto service.
    /// </summary>
    [Serializable]
    public abstract class BaseAutoService : BaseService, Castle.Core.IStartable
    {
        /// <summary>
        /// if service is started.
        /// </summary>
        protected bool started = false;
        /// <summary>
        /// if service is stopped.
        /// </summary>
        protected bool stopped = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAutoService"/> class.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="mq">The mq.</param>
        public BaseAutoService(string serviceName, IServiceMQ mq)
            : base(serviceName, mq)
        {
        }

        #region IStartable Members

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void Start()
        {
            started = true;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            stopped = true;
        }

        #endregion
    }
}
