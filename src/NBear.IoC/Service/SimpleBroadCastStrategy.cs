using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using NBear.Common;

namespace NBear.IoC.Service
{
    /// <summary>
    /// SimpleBroadCastStrategy for service request.
    /// </summary>
    public class SimpleBroadCastStrategy : IBroadCastStrategy, ILogable
    {
        #region Private Members

        private Dictionary<Guid, ServiceRequestNotifyHandler> RemoveNullHandlers(Dictionary<Guid, ServiceRequestNotifyHandler> handlers)
        {
            Dictionary<Guid, ServiceRequestNotifyHandler> newHandlers = new Dictionary<Guid, ServiceRequestNotifyHandler>();
            foreach (Guid key in handlers.Keys)
            {
                if (handlers[key] != null)
                {
                    newHandlers.Add(key, handlers[key]);
                }
            }
            return newHandlers;
        }
        
        #endregion

        #region Protected Members

        /// <summary>
        /// Does the broad cast.
        /// </summary>
        /// <param name="reqMsg">The req MSG.</param>
        /// <param name="handlers">The handlers.</param>
        /// <returns>Is needed to clean null handlers</returns>
        protected virtual bool DoBroadCast(RequestMessage reqMsg, Dictionary<Guid, ServiceRequestNotifyHandler> handlers)
        {
            bool needCleanHandlers = false;

            List<Guid> clientIdList = new List<Guid>(handlers.Keys);
            Random random = new Random();
            int start = random.Next(handlers.Count);
            for (int i = 0; i < handlers.Count; i++)
            {
                Guid tempClientId = clientIdList[(i + start) % handlers.Count];
                ServiceRequestNotifyHandler tempHandler = handlers[tempClientId];
                if (tempHandler != null)
                {
                    try
                    {
                        IService service = ((Services.MessageRequestCallbackHandler)tempHandler.Target).Service;
                        if (OnLog != null) OnLog("[" + DateTime.Now.ToString() + "] Notify service host: " + reqMsg.ServiceName + "[" + tempClientId.ToString() + "]");
                        tempHandler(reqMsg);

                        //if calling ok, skip other subscribers, easily exit loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (OnLog != null) OnLog("[" + DateTime.Now.ToString() + "] Service host: " + reqMsg.ServiceName + "[" + tempClientId.ToString() + "] shutdown! Reason: " + ex.ToString());
                        handlers[tempClientId] = null;
                        needCleanHandlers = true;
                    }
                }
                else
                {
                    needCleanHandlers = true;
                }
            }

            return needCleanHandlers;
        }

        #endregion

        #region IBroadCastStrategy Members

        /// <summary>
        /// Broads the cast.
        /// </summary>
        /// <param name="reqMsg">The req MSG.</param>
        /// <param name="onServiceRequests">The on service requests.</param>
        public void BroadCast(RequestMessage reqMsg, Dictionary<string, Dictionary<Guid, ServiceRequestNotifyHandler>> onServiceRequests)
        {
            if (reqMsg == null)
            {
                return;
            }

            if (onServiceRequests != null && onServiceRequests.ContainsKey(reqMsg.ServiceName))
            {
                Dictionary<Guid, ServiceRequestNotifyHandler> handlers = onServiceRequests[reqMsg.ServiceName];

                bool needCleanHandlers = DoBroadCast(reqMsg, handlers);

                if (needCleanHandlers)
                {
                    onServiceRequests[reqMsg.ServiceName] = RemoveNullHandlers(handlers);
                }
            }
            else
            {
                if (OnLog != null) OnLog("have not any subscriber!");
            }
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
