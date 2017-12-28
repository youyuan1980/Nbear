using System;
using System.Collections;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Core;
using NBear.IoC.Service.Services;

namespace NBear.IoC.Service.Facilities
{
    /// <summary>
    /// The facility is used to automatically subscribe msg reqyest to specified services.
    /// </summary>
    public class ServiceSubscribeMessageRequestFacility : AbstractFacility
    {
        #region Private Members

        private ArrayList waitList;
        private IServiceMQ mq;

        private bool CheckIfComponentImplementsIService(ComponentModel model)
        {
            return typeof(IService).IsAssignableFrom(model.Implementation);
        }

        /// <summary>
        /// For each new component registered,
        /// some components in the WaitingDependency
        /// state may have became valid, so we check them
        /// </summary>
        private void CheckWaitingList()
        {
            IHandler[] handlerArray1 = (IHandler[]) this.waitList.ToArray(typeof(IHandler));
            IHandler[] handlerArray2 = handlerArray1;
            for (int num1 = 0; num1 < handlerArray2.Length; num1++)
            {
                IHandler handler1 = handlerArray2[num1];
                if (handler1.CurrentState == HandlerState.Valid)
                {
                    this.Subscribe(handler1.ComponentModel.Name);
                    this.waitList.Remove(handler1);
                }
            }
        }

        private void Subscribe(string key)
        {
            IService obj = (IService)base.Kernel[key];
            MessageRequestCallbackHandler handler = new MessageRequestCallbackHandler(mq, obj);
            base.Kernel.AddComponentInstance("handler_" + obj.ClientId.ToString(), handler);
            mq.SubscribeServiceRequest(obj.ServiceName, obj.ClientId, new ServiceRequestNotifyHandler(handler.OnMessageRequestCallback));
        }

        private void OnComponentModelCreated(ComponentModel model)
        {
            bool flag1 = this.CheckIfComponentImplementsIService(model);
            model.ExtendedProperties["service subscribe"] = flag1;
        }

        private void OnComponentRegistered(string key, IHandler handler)
        {
            if ((bool)handler.ComponentModel.ExtendedProperties["service subscribe"])
            {
                if (handler.CurrentState == HandlerState.WaitingDependency)
                {
                    this.waitList.Add(handler);
                }
                else
                {
                    this.Subscribe(key);
                }
            }
            this.CheckWaitingList();
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSubscribeMessageRequestFacility"/> class.
        /// </summary>
        /// <param name="mq">The mq.</param>
        public ServiceSubscribeMessageRequestFacility(IServiceMQ mq)
        {
            this.waitList = new ArrayList();
            this.mq = mq;
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        protected override void Init()
        {
            base.Kernel.ComponentModelCreated += new ComponentModelDelegate(this.OnComponentModelCreated);
            base.Kernel.ComponentRegistered += new ComponentDataDelegate(this.OnComponentRegistered);
        }
    }
}
