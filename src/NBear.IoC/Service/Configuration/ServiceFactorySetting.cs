using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;

using NBear.Net.Remoting;

namespace NBear.IoC.Service.Configuration
{
    /// <summary>
    /// The service factory configuration.
    /// </summary>
    public class ServiceFactoryConfigurationSection : ConfigurationSection
    {
        private ServiceFactoryType type = ServiceFactoryType.Local;
        private RemotingChannelType protocol = RemotingChannelType.HTTP;
        private string server = "127.0.0.1";
        private int port = 8888;
        private string serviceMQName = "MMQ";
        private bool debug = true;
        private bool compress = false;
        private int maxTry = SimpleServiceContainer.DEFAULT_MAX_TRY_NUMBER;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsRequired = true, IsDefaultCollection = false)]
        public ServiceFactoryType Type
        {
            get { return this["type"] == null ? type : (ServiceFactoryType)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        [ConfigurationProperty("protocol")]
        public RemotingChannelType Protocol
        {
            get { return this["protocol"] == null ? protocol : (RemotingChannelType)this["protocol"]; }
            set { this["protocol"] = value; }
        }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        [ConfigurationProperty("server")]
        public string Server
        {
            get { return this["server"] == null ? server : (string)this["server"]; }
            set { this["server"] = value; }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return this["port"] == null ? port : (int)this["port"]; }
            set { this["port"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the service MQ.
        /// </summary>
        /// <value>The name of the service MQ.</value>
        [ConfigurationProperty("name")]
        public string ServiceMQName
        {
            get { return this["name"] == null ? serviceMQName : (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ServiceFactoryConfigurationSection"/> is debug.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("debug")]
        public bool Debug
        {
            get { return this["debug"] == null ? debug : (bool)this["debug"]; }
            set { this["debug"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data dispatched by this service factory is compressed.
        /// </summary>
        /// <value><c>true</c> if compress; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("compress")]
        public bool Compress
        {
            get { return this["compress"] == null ? compress : (bool)this["compress"]; }
            set { this["compress"] = value; }
        }

        /// <summary>
        /// Gets or sets the max try.
        /// </summary>
        /// <value>The max try.</value>
        [ConfigurationProperty("maxTry")]
        public int MaxTry
        {
            get { return this["maxTry"] == null ? maxTry : (int)this["maxTry"]; }
            set { this["maxTry"] = value; }
        }
    }

    /// <summary>
    /// Service facrory type
    /// </summary>
    public enum ServiceFactoryType
    {
        /// <summary>
        /// Local
        /// </summary>
        Local,
        /// <summary>
        /// Remoting
        /// </summary>
        Remoting
    }
}
