using System;
using System.Collections.Generic;
using System.Text;

using NBear.Common;
using NBear.IoC.Service;

namespace NBear.IoC.Service
{
    /// <summary>
    /// The ServiceRequestNotifyHandler
    /// </summary>
    /// <param name="header">the msg request handler.</param>
    public delegate void ServiceRequestNotifyHandler(RequestMessage header);

    /// <summary>
    /// interface of service mq.
    /// </summary>
    public interface IServiceMQ : ILogable
    {
        /// <summary>
        /// Sends the request to queue.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="msg">The MSG.</param>
        /// <returns>The id of the msg.</returns>
        Guid SendRequestToQueue(string serviceName, RequestMessage msg);
        /// <summary>
        /// Sends the response to queue.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        void SendResponseToQueue(ResponseMessage msg);
        /// <summary>
        /// Receives the request from queue.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>The msg.</returns>
        RequestMessage ReceiveRequestFromQueue(Guid transactionId);
        /// <summary>
        /// Receieves the response from queue.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns>The msg.</returns>
        ResponseMessage ReceieveResponseFromQueue(Guid transactionId);
        /// <summary>
        /// Subscribes the service request.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="handler">The handler.</param>
        void SubscribeServiceRequest(string serviceName, Guid clientId, ServiceRequestNotifyHandler handler);
        /// <summary>
        /// Cleans the expired messages.
        /// </summary>
        /// <param name="expiredRequests">The expired requests.</param>
        /// <param name="expiredResponses">The expired responses.</param>
        void CleanExpiredMessages(out RequestMessage[] expiredRequests, out ResponseMessage[] expiredResponses);
        /// <summary>
        /// Sets the broad cast strategy.
        /// </summary>
        /// <param name="broadCastStrategy">The broad cast strategy.</param>
        void SetBroadCastStrategy(IBroadCastStrategy broadCastStrategy);
    }
}
