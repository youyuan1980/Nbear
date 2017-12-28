using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel;
using NBear.Common;
using NBear.IoC.Service;

namespace NBear.MVP
{
    /// <summary>
    /// Presenter Factory
    /// </summary>
    public sealed class PresenterFactory
    {
        private ServiceFactory container;

        private PresenterFactory()
        {
            container = ServiceFactory.Create();
        }

        private static PresenterFactory singleton = null;

        /// <summary>
        /// Creates this singleton instance.
        /// </summary>
        /// <returns></returns>
        public static PresenterFactory Create()
        {
            if (singleton == null)
            {
                singleton = new PresenterFactory();
            }
            return singleton;
        }

        /// <summary>
        /// Gets the presenter.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public IPresenterType GetPresenter<IPresenterType>(object view)
        {
            Check.Require(view != null, "view could not be null.");
            if (container.ServiceContainer.Kernel.HasComponent(typeof(IPresenterType)))
            {
                IPresenterType _presenter = (IPresenterType)container.ServiceContainer.Kernel[typeof(IPresenterType)];
                if (typeof(IPresenter).IsAssignableFrom(_presenter.GetType()))
                {
                    IPresenter presenter = (IPresenter)_presenter;
                    object model = container.GetType().GetMethod("GetService").MakeGenericMethod(presenter.TypeOfModel).Invoke(container, null); ;
                    presenter.BindView(view);
                    presenter.BindModel(model);
                    return _presenter;
                }
                else if (typeof(IPresenter2).IsAssignableFrom(_presenter.GetType()))
                {
                    IPresenter2 presenter = (IPresenter2)_presenter;
                    object[] models = new object[presenter.TypeOfModels.Length];
                    for (int i = 0; i < models.Length; i++)
                    {
                        models[i] = container.GetType().GetMethod("GetService").MakeGenericMethod(presenter.TypeOfModels[i]).Invoke(container, null); ;
                    }
                    presenter.BindView(view);
                    presenter.BindModels(models);
                    return _presenter;
                }
            }

            return default(IPresenterType);
        }
    }
}
