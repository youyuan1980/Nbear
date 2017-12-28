using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NBear.Common;

namespace NBear.IoC.Service
{
    /// <summary>
    /// A memory service mq impl.
    /// </summary>
    public class MemoryServiceMQ : MarshalByRefObject, IServiceMQ
    {
        #region Private Members

        private IDictionary requests = Hashtable.Synchronized(new Hashtable());
        private IDictionary responses = Hashtable.Synchronized(new Hashtable());
        private Dictionary<string, Dictionary<Guid, ServiceRequestNotifyHandler>> onServiceRequests = new Dictionary<string, Dictionary<Guid, ServiceRequestNotifyHandler>>();
        private IBroadCastStrategy broadCastStrategy;

        private void WriteLog(string logInfo)
        {
            if (OnLog != null) OnLog(logInfo);
        }

        private object GetData(IDictionary map, Guid transactionId)
        {
            lock (map)
            {
                if (map.Contains(transactionId))
                {
                    object retObj = map[transactionId];
                    map.Remove(transactionId);
                    return retObj;
                }
                else
                {
                    return null;
                }
            }
        }

        private object GetData(IDictionary map, string serviceName)
        {
            object retMsg = null;

            if (serviceName != null)
            {
                lock (map)
                {
                    foreach (RequestMessage msg in map.Values)
                    {
                        if (msg.ServiceName == serviceName)
                        {
                            retMsg = map[msg.TransactionId];
                            map.Remove(msg.TransactionId);
                        }
                    }
                }
            }

            return retMsg;
        }

        private void BroadCast(RequestMessage reqMsg)
        {
            if (broadCastStrategy != null)
            {
                broadCastStrategy.BroadCast(reqMsg, onServiceRequests);
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Adds the request to queue.
        /// </summary>
        /// <param name="tid">The tid.</param>
        /// <param name="msg">The MSG.</param>
        protected virtual void AddRequestToQueue(Guid tid, RequestMessage msg)
        {
            lock (requests)
            {
                if (!requests.Contains(tid))
                {
                    requests.Add(tid, msg);
                }
                else
                {
                    requests[tid] = msg;
                }
            }
        }

        /// <summary>
        /// Adds the response to queue.
        /// </summary>
        /// <param name="tid">The tid.</param>
        /// <param name="msg">The MSG.</param>
        protected virtual void AddResponseToQueue(Guid tid, ResponseMessage msg)
        {
            lock (responses)
            {
                if (!responses.Contains(tid))
                {
                    responses.Add(tid, msg);
                }
                else
                {
                    responses[tid] = msg;
                }
            }
        }

        /// <summary>
        /// Gets the request from queue.
        /// </summary>
        /// <param name="tid">The tid.</param>
        /// <returns>The msg.</returns>
        protected virtual RequestMessage GetRequestFromQueue(Guid tid)
        {
            return (RequestMessage)GetData(requests, tid);
        }

        /// <summary>
        /// Gets the next request by service name from queue.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>The msg.</returns>
        protected virtual RequestMessage GetNextRequestByServiceNameFromQueue(string serviceName)
        {
            return (RequestMessage)GetData(requests, serviceName);
        }

        /// <summary>
        /// Gets the response from queue.
        /// </summary>
        /// <param name="tid">The tid.</param>
        /// <returns>The msg.</returns>
        protected virtual ResponseMessage GetResponseFromQueue(Guid tid)
        {
            return (ResponseMessage)GetData(responses, tid);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryServiceMQ"/> class.
        /// </summary>
        public MemoryServiceMQ()
        {
            SimpleBroadCastStrategy strategy = new SimpleBroadCastStrategy();
            strategy.OnLog += new LogHandler(WriteLog);
            this.broadCastStrategy = strategy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryServiceMQ"/> class.
        /// </summary>
        /// <param name="broadCastStrategy">The broad cast strategy.</param>
        public MemoryServiceMQ(IBroadCastStrategy broadCastStrategy)
        {
            this.broadCastStrategy = broadCastStrategy;
        }

        #endregion
    
        #region IServiceMQ Members

        /// <summary>
        /// Sends the request to queue.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns>The id of the msg.</returns>
        public Guid SendRequestToQueue(string serviceName, RequestMessage msg)
        {
            if (msg.TransactionId == default(Guid))
            {
                msg.TransactionId = Guid.NewGuid();
            }

            AddRequestToQueue(msg.TransactionId, msg);

            if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] AddRequestToQueue({1}):{0}", serviceName, msg.TransactionId));

            BroadCast(msg);

            return msg.TransactionId;
        }

        /// <summary>
        /// Sends the response to queue.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public void SendResponseToQueue(ResponseMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            AddResponseToQueue(msg.TransactionId, msg);

            if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] AddResponseToQueue({1}):{0}", msg.ServiceName, msg.TransactionId));
        }

        /// <summary>
        /// Receives the request from queue.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>The msg.</returns>
        public RequestMessage ReceiveRequestFromQueue(Guid transactionId)
        {
            RequestMessage msg = GetRequestFromQueue(transactionId);

            if (msg != null)
            {
                if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] GetRequestFromQueue({1}):{0}", msg.ServiceName, transactionId));
            }

            return msg;
        }

        /// <summary>
        /// Receives the next request from queue.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>The msg.</returns>
        public RequestMessage ReceiveNextRequestFromQueue(string serviceName)
        {
            RequestMessage msg = GetNextRequestByServiceNameFromQueue(serviceName);

            if (msg != null)
            {
                if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] GetNextRequestFromQueue({1}):{0}", serviceName, msg.TransactionId));
            }

            return msg;
        }

        /// <summary>
        /// Receieves the response from queue.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>The msg.</returns>
        public virtual ResponseMessage ReceieveResponseFromQueue(Guid transactionId)
        {
            ResponseMessage msg = GetResponseFromQueue(transactionId);

            if (msg != null)
            {
                if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] GetResponseFromQueue({1}):{0}", msg.ServiceName, transactionId));
            }

            return msg;
        }

        /// <summary>
        /// Subscribes the service request.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="handler">The handler.</param>
        public void SubscribeServiceRequest(string serviceName, Guid clientId, ServiceRequestNotifyHandler handler)
        {
            if (handler == null)
            {
                return;
            }

            lock (onServiceRequests)
            {
                if (!onServiceRequests.ContainsKey(serviceName))
                {
                    onServiceRequests.Add(serviceName, new Dictionary<Guid, ServiceRequestNotifyHandler>());
                }
            }
            onServiceRequests[serviceName].Add(clientId, handler);

            if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] Added new service reqMsg subscribing: {0}[{1}]", serviceName, clientId));
        }

        /// <summary>
        /// Cleans the expired messages.
        /// </summary>
        /// <param name="expiredRequests">The expired requests.</param>
        /// <param name="expiredResponses">The expired responses.</param>
        public virtual void CleanExpiredMessages(out RequestMessage[] expiredRequests, out ResponseMessage[] expiredResponses)
        {
            lock (requests)
            {
                List<RequestMessage> reqList = new List<RequestMessage>();
                foreach (object key in requests.Keys)
                {
                    RequestMessage reqMsg = (RequestMessage)requests[key];
                    if (reqMsg == null || reqMsg.Expiration < DateTime.Now)
                    {
                        reqList.Add(reqMsg);
                        requests.Remove(key);
                    }
                }
                expiredRequests = reqList.ToArray();
            }

            lock (responses)
            {
                List<ResponseMessage> resList = new List<ResponseMessage>();
                foreach (object key in responses.Keys)
                {
                    ResponseMessage resMsg = (ResponseMessage)responses[key];
                    if (resMsg == null || resMsg.Expiration < DateTime.Now)
                    {
                        resList.Add(resMsg);
                        responses.Remove(key);
                    }
                }
                expiredResponses = resList.ToArray();
            }
        }

        /// <summary>
        /// Sets the broad cast strategy.
        /// </summary>
        /// <param name="broadCastStrategy">The broad cast strategy.</param>
        public void SetBroadCastStrategy(IBroadCastStrategy broadCastStrategy)
        {
            this.broadCastStrategy = broadCastStrategy;
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

        #region ILogable Members

        /// <summary>
        /// OnLog event.
        /// </summary>
        public event LogHandler OnLog;

        #endregion
    }
}
