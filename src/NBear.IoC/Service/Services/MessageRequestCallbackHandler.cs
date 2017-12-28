using System;
using System.Collections.Generic;
using System.Text;

namespace NBear.IoC.Service.Services
{
    /// <summary>
    /// The MessageRequestCallbackHandler.
    /// </summary>
    [Serializable]
    public class MessageRequestCallbackHandler : MarshalByRefObject
    {
        private IServiceMQ mq;
        private IService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRequestCallbackHandler"/> class.
        /// </summary>
        /// <param name="mq">The mq.</param>
        /// <param name="service">The service.</param>
        public MessageRequestCallbackHandler(IServiceMQ mq, IService service)
        {
            this.mq = mq;
            this.service = service;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        public IService Service
        {
            get
            {
                return service;
            }
        }

        #region IMessageRequestCallback Members

        /// <summary>
        /// Called when [message request callback].
        /// </summary>
        /// <param name="header">The header.</param>
        public void OnMessageRequestCallback(RequestMessage header)
        {
            RequestMessage msg = mq.ReceiveRequestFromQueue(header.TransactionId);
            if (msg != null)
            {
                mq.SendResponseToQueue(service.CallService(msg));
            }
        }
        #endregion

        #region MarshalByRefObject

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"></see> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"></see> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/></PermissionSet>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion
    }
}
