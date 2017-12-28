using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common;

namespace NBear.IoC.Service
{
    /// <summary>
    /// interface of BroadCastStrategy.
    /// </summary>
    public interface IBroadCastStrategy
    {
        /// <summary>
        /// Broads the cast.
        /// </summary>
        /// <param name="reqMsg">The req MSG.</param>
        /// <param name="onServiceRequests">The on service requests.</param>
        void BroadCast(RequestMessage reqMsg, Dictionary<string, Dictionary<Guid, ServiceRequestNotifyHandler>> onServiceRequests);
    }
}
