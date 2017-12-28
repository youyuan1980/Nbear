using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using NBear.Common;

namespace NBear.IoC.Service.Services
{
    /// <summary>
    /// The dynamic service.
    /// </summary>
    public class DynamicService : BaseService
    {
        /// <summary>
        /// The default  message expire minutes.
        /// </summary>
        public static int DefaultExpireMinutes = 30;
        private IServiceContainer container;
        private Type serviceInterfaceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicService"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="serviceInterfaceType">Type of the service interface.</param>
        public DynamicService(IServiceContainer container, Type serviceInterfaceType)
            : base(serviceInterfaceType.FullName, container.MQ)
        {
            this.container = container;
            this.OnLog += new LogHandler(container.WriteLog);
            this.serviceInterfaceType = serviceInterfaceType;
        }

        /// <summary>
        /// Runs the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>The msg.</returns>
        protected override ResponseMessage Run(RequestMessage msg)
        {
            if (container == null || msg == null)
            {
                return null;
            }

            object service = default(object);

            try
            {
                service = container[serviceInterfaceType];
            }
            catch
            {
            }

            if (service == null)
            {
                return null;
            }

            MethodInfo mi = null;
            foreach (MethodInfo item in serviceInterfaceType.GetMethods())
            {
                if (msg.SubServiceName == item.ToString())
                {
                    mi = item;
                    break;
                }
            }

            if (mi == null)
            {
                foreach (Type inheritedInterface in serviceInterfaceType.GetInterfaces())
                {
                    foreach (MethodInfo item in inheritedInterface.GetMethods())
                    {
                        if (item.ToString() == msg.SubServiceName)
                        {
                            mi = item;
                            break;
                        }
                    }
                }
            }

            if (mi == null)
            {
                return null;
            }

            ParameterInfo[] pis = mi.GetParameters();
            object[] parms = new object[pis.Length];

            for (int i = 0; i < pis.Length; i++)
            {
                Type type = pis[i].ParameterType;

                object val = SerializationManager.Deserialize(type, msg.Parameters[pis[i].Name]);

                parms[i] = val;
            }

            ResponseMessage resMsg = new ResponseMessage();
            resMsg.Request = msg;
            resMsg.Expiration = DateTime.Now.AddMinutes(DefaultExpireMinutes);
            resMsg.MessageId = Guid.NewGuid();
            resMsg.ServiceName = serviceInterfaceType.FullName;
            resMsg.SubServiceName = mi.Name;
            resMsg.Timestamp = DateTime.Now;
            resMsg.TransactionId = msg.TransactionId;

            object returnValue = null;

            try
            {
                returnValue = mi.Invoke(service, parms);
            }
            catch
            {
                return null;
            }

            if (returnValue != null)
            {
                Type returnType = mi.ReturnType;

                if (!container.Compress)
                {
                    if (returnType == typeof(System.Data.DataSet))
                    {
                        resMsg.Data = (DataSet)returnValue;
                    }
                    else
                    {
                        resMsg.Text = SerializationManager.Serialize(returnValue);
                    }
                }
                else
                {
                    string retText;
                    if (returnType == typeof(System.Data.DataSet))
                    {
                        retText = SerializationManager.Serialize(returnValue);
                    }
                    else
                    {
                        retText = SerializationManager.Serialize(returnValue);
                    }

                    resMsg.Text = CompressionManager.Compress7Zip(retText);
                }
            }

            return resMsg;
        }
    }
}
