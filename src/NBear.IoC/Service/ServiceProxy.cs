using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using NBear.Common;
using Castle.Windsor;

namespace NBear.IoC.Service
{
    internal sealed class ServiceProxy : ILogable
    {
        private int maxTryNum;
        private IServiceMQ mq;

        public int MaxTryNum
        {
            get
            {
                return maxTryNum;
            }
            set
            {
                maxTryNum = value;
            }
        }

        public ServiceProxy(IServiceMQ mq, int maxTryNum)
        {
            this.mq = mq;
            this.maxTryNum = maxTryNum;
        }

        private ResponseMessage Run(string serviceName, RequestMessage msg)
        {
            ResponseMessage retMsg = null;

            if (OnLog != null) OnLog(string.Format("[" + DateTime.Now.ToString() + "] Run reqMsg for {0} to service mq. RequestMsg:{1} ", serviceName, SerializationManager.Serialize(msg)));
            Guid tid = mq.SendRequestToQueue(serviceName, msg);
            for (int i = 0; i < maxTryNum; i++)
            {
                retMsg = mq.ReceieveResponseFromQueue(tid);
                if (retMsg == null)
                {
                    if (OnLog != null) OnLog("Try...... " + (i + 1));
                    Thread.Sleep(i * 10);
                }
                else
                {
                    break;
                }
            }

            if (retMsg != null)
            {
                if (OnLog != null) OnLog("Result:" + serviceName + "-->" + SerializationManager.Serialize(retMsg));
            }
            else
            {
                //delete the reqMsg message from mq if no service host can process it
                mq.ReceiveRequestFromQueue(msg.TransactionId);
            }

            return retMsg;
        }

        public ResponseMessage CallMethod(string serviceName, RequestMessage msg)
        {
            if (OnLog != null) OnLog("[" + DateTime.Now.ToString() + "] Receive reqMsg for service:" + serviceName + "-->" + SerializationManager.Serialize(msg));

            long t1 = System.Environment.TickCount;
            ResponseMessage retMsg = Run(serviceName, msg);
            long t2 = System.Environment.TickCount - t1;
            if (OnLog != null) OnLog("Spent time:(" + t2.ToString() + ")ms ");

            return retMsg;
        }

        #region ILogable Members

        public event LogHandler OnLog;

        #endregion
    }
}
