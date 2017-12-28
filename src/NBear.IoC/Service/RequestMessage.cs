using System;
using System.Collections.Generic;
using System.Text;

namespace NBear.IoC.Service
{
    /// <summary>
    /// The request msg.
    /// </summary>
    [Serializable]
    public class RequestMessage
    {
        #region Private Members

        private string serviceName;
        private string subServiceName;
        private Guid transactionId;
        private ParameterCollection parameters = new ParameterCollection();
        private DateTime expiration;
        private Guid messageId;
        private byte priority;
        private DateTime timestamp;

        #endregion

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceName
        {
            get
            {
                return serviceName;
            }
            set
            {
                serviceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the sub service.
        /// </summary>
        /// <value>The name of the sub service.</value>
        public string SubServiceName
        {
            get
            {
                return subServiceName;
            }
            set
            {
                subServiceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        /// <value>The transaction id.</value>
        public Guid TransactionId
        {
            get
            {
                return transactionId;
            }
            set
            {
                transactionId = value;
            }
        }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public ParameterCollection Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime Expiration
        {
            get
            {
                return expiration;
            }
            set
            {
                expiration = value;
            }
        }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>The message id.</value>
        public Guid MessageId
        {
            get
            {
                return messageId;
            }
            set
            {
                messageId = value;
            }
        }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public byte Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
        }
    }
}
