using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using NBear.Common;

namespace NBear.Net.Remoting
{
    /// <summary>
    /// The Remoting Service Helper.
    /// </summary>
    public sealed class RemotingServiceHelper : IDisposable, ILogable
    {
        #region Private Members

        private RemotingChannelType channelType;
        private string serverAddress;
        private int serverPort;
        private IChannel serviceChannel;

        private void WriteLog(string logMsg)
        {
            if (OnLog != null)
            {
                OnLog(logMsg);
            }
        }

        private string BuildUrl(string notifyName)
        {
            StringBuilder url = new StringBuilder();
            url.Append(channelType.ToString().ToLower());
            url.Append("://");
            url.Append(serverAddress);
            url.Append(":");
            url.Append(serverPort.ToString());
            url.Append("/" + notifyName);

            return url.ToString();
        }

        #endregion

        /// <summary>
        /// OnLog event.
        /// </summary>
        public event LogHandler OnLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotingServiceHelper"/> class.
        /// </summary>
        /// <param name="channelType">Type of the channel.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="serverPort">The server port.</param>
        public RemotingServiceHelper(RemotingChannelType channelType, string serverAddress, int serverPort)
        {
            this.channelType = channelType;
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
            Init();
        }

        private void Init()
        {
            if (serviceChannel == null)
            {
                BinaryServerFormatterSinkProvider serverProvider = new
                    BinaryServerFormatterSinkProvider();
                BinaryClientFormatterSinkProvider clientProvider = new
                    BinaryClientFormatterSinkProvider();
                serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

                IDictionary props = new Hashtable();
                props["name"] = AppDomain.CurrentDomain.FriendlyName;
                props["port"] = serverPort;
                
                if (channelType == RemotingChannelType.TCP)
                {
                    serviceChannel = new TcpChannel(props, clientProvider, serverProvider);
                }
                else
                {
                    serviceChannel = new HttpChannel(props, clientProvider, serverProvider);
                }
                ChannelServices.RegisterChannel(serviceChannel, false);
            }
        }

        /// <summary>
        /// Publishes the well known service instance.
        /// </summary>
        /// <param name="notifyName">Name of the notify.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="mode">The mode.</param>
        public void PublishWellKnownServiceInstance(string notifyName, Type interfaceType, MarshalByRefObject instance, WellKnownObjectMode mode)
        {
            WriteLog("Instance URL --> " + BuildUrl(notifyName));
            RemotingConfiguration.RegisterWellKnownServiceType(interfaceType, BuildUrl(notifyName), mode);
            ObjRef objRef = RemotingServices.Marshal(instance, notifyName);
            WriteLog("(" + instance.ToString() + ") start listening at port: " + serverPort);
        }

        /// <summary>
        /// Publishes the activated service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public void PublishActivatedService(Type serviceType)
        {
            WriteLog("Registered Activated Service --> " + serviceType.ToString());
            RemotingConfiguration.RegisterActivatedServiceType(serviceType);
            WriteLog("[" + DateTime.Now.ToString() + "] Start listening at port: " + serverPort);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ChannelServices.UnregisterChannel(serviceChannel);
        }

        #endregion
    }
}
