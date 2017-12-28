using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common;
using NBear.IoC.Service;

namespace NBear.MVP
{
    /// <summary>
    /// Interface of all presenters
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        void BindView(object view);
        /// <summary>
        /// Binds the model.
        /// </summary>
        /// <param name="model">The model.</param>
        void BindModel(object model);
        /// <summary>
        /// Gets the type of view.
        /// </summary>
        /// <value>The type of view.</value>
        Type TypeOfView { get; }
        /// <summary>
        /// Gets the type of model.
        /// </summary>
        /// <value>The type of model.</value>
        Type TypeOfModel { get; }
    }

    /// <summary>
    /// The extended interface of all presenters
    /// </summary>
    public interface IPresenter2
    {
        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        void BindView(object view);
        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        void BindModels(object[] models);
        /// <summary>
        /// Gets the type of view.
        /// </summary>
        /// <value>The type of view.</value>
        Type TypeOfView { get; }
        /// <summary>
        /// Gets the types of model.
        /// </summary>
        /// <value>The types of model.</value>
        Type[] TypeOfModels { get; }
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    public abstract class Presenter<ViewType> : IPresenter2
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;ViewType&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null;
            }
        }

        #region IPresenter2 Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        public void BindModels(object[] models)
        {
            //null is ok
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] TypeOfModels
        {
            get
            {
                return new Type[0];
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    /// <typeparam name="IModelType"></typeparam>
    public abstract class Presenter<ViewType, IModelType> : IPresenter
        //where IModelType : IServiceInterface
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// The model
        /// </summary>
        protected IModelType model;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;IViewType, IModelType&gt;"/> is initialized.
        /// Only when both view and model are binded, a presenter is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null && model != null;
            }
        }

        #region IPresenter Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void BindModel(object model)
        {
            Check.Require(model != null, "model could not be null.");
            Check.Require(typeof(IModelType).IsAssignableFrom(model.GetType()), "model's type does not match Presenter's model type.");
            this.model = (IModelType)model;
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type TypeOfModel
        {
            get
            {
                return typeof(IModelType);
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    /// <typeparam name="IModelType1"></typeparam>
    /// <typeparam name="IModelType2"></typeparam>
    public abstract class Presenter<ViewType, IModelType1, IModelType2> : IPresenter2
        //where IModelType1 : IServiceInterface
        //where IModelType2 : IServiceInterface
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// The model 1
        /// </summary>
        protected IModelType1 model1;

        /// <summary>
        /// The model 2
        /// </summary>
        protected IModelType2 model2;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;ViewType, IModelType1, IModelType2&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null && model1 != null && model2 != null;
            }
        }

        #region IPresenter2 Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        public void BindModels(object[] models)
        {
            Check.Require(models != null && models.Length == 2, "models' length length  must be 2.");
            Check.Require(typeof(IModelType1).IsAssignableFrom(models[0].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType2).IsAssignableFrom(models[1].GetType()), "model's type does not match Presenter's model type.");
            this.model1 = (IModelType1)models[0];
            this.model2 = (IModelType2)models[1];
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] TypeOfModels
        {
            get
            {
                return new Type[] { typeof(IModelType1), typeof(IModelType2) };
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    /// <typeparam name="IModelType1"></typeparam>
    /// <typeparam name="IModelType2"></typeparam>
    /// <typeparam name="IModelType3"></typeparam>
    public abstract class Presenter<ViewType, IModelType1, IModelType2, IModelType3> : IPresenter2
        //where IModelType1 : IServiceInterface
        //where IModelType2 : IServiceInterface
        //where IModelType3 : IServiceInterface
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// The model 1
        /// </summary>
        protected IModelType1 model1;

        /// <summary>
        /// The model 2
        /// </summary>
        protected IModelType2 model2;

        /// <summary>
        /// The model 3
        /// </summary>
        protected IModelType3 model3;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;ViewType, IModelType1, IModelType2, IModelType3&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null && model1 != null && model2 != null && model3 != null;
            }
        }

        #region IPresenter2 Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        public void BindModels(object[] models)
        {
            Check.Require(models != null && models.Length == 3, "models' length length  must be 3.");
            Check.Require(typeof(IModelType1).IsAssignableFrom(models[0].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType2).IsAssignableFrom(models[1].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType3).IsAssignableFrom(models[2].GetType()), "model's type does not match Presenter's model type.");
            this.model1 = (IModelType1)models[0];
            this.model2 = (IModelType2)models[1];
            this.model3 = (IModelType3)models[2];
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] TypeOfModels
        {
            get
            {
                return new Type[] { typeof(IModelType1), typeof(IModelType2), typeof(IModelType3) };
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    /// <typeparam name="IModelType1"></typeparam>
    /// <typeparam name="IModelType2"></typeparam>
    /// <typeparam name="IModelType3"></typeparam>
    /// <typeparam name="IModelType4"></typeparam>
    public abstract class Presenter<ViewType, IModelType1, IModelType2, IModelType3, IModelType4> : IPresenter2
        //where IModelType1 : IServiceInterface
        //where IModelType2 : IServiceInterface
        //where IModelType3 : IServiceInterface
        //where IModelType4 : IServiceInterface
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// The model 1
        /// </summary>
        protected IModelType1 model1;

        /// <summary>
        /// The model 2
        /// </summary>
        protected IModelType2 model2;

        /// <summary>
        /// The model 3
        /// </summary>
        protected IModelType3 model3;

        /// <summary>
        /// The model 4
        /// </summary>
        protected IModelType4 model4;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;ViewType, IModelType1, IModelType2, IModelType3, IModelType4&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null && model1 != null && model2 != null && model3 != null && model4 != null;
            }
        }

        #region IPresenter2 Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        public void BindModels(object[] models)
        {
            Check.Require(models != null && models.Length == 4, "models' length length  must be 4.");
            Check.Require(typeof(IModelType1).IsAssignableFrom(models[0].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType2).IsAssignableFrom(models[1].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType3).IsAssignableFrom(models[2].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType4).IsAssignableFrom(models[3].GetType()), "model's type does not match Presenter's model type.");
            this.model1 = (IModelType1)models[0];
            this.model2 = (IModelType2)models[1];
            this.model3 = (IModelType3)models[2];
            this.model4 = (IModelType4)models[3];
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] TypeOfModels
        {
            get
            {
                return new Type[] { typeof(IModelType1), typeof(IModelType2), typeof(IModelType3), typeof(IModelType4) };
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    /// <typeparam name="IModelType1"></typeparam>
    /// <typeparam name="IModelType2"></typeparam>
    /// <typeparam name="IModelType3"></typeparam>
    /// <typeparam name="IModelType4"></typeparam>
    /// <typeparam name="IModelType5"></typeparam>
    public abstract class Presenter<ViewType, IModelType1, IModelType2, IModelType3, IModelType4, IModelType5> : IPresenter2
        //where IModelType1 : IServiceInterface
        //where IModelType2 : IServiceInterface
        //where IModelType3 : IServiceInterface
        //where IModelType4 : IServiceInterface
        //where IModelType5 : IServiceInterface
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// The model 1
        /// </summary>
        protected IModelType1 model1;

        /// <summary>
        /// The model 2
        /// </summary>
        protected IModelType2 model2;

        /// <summary>
        /// The model 3
        /// </summary>
        protected IModelType3 model3;

        /// <summary>
        /// The model 4
        /// </summary>
        protected IModelType4 model4;

        /// <summary>
        /// The model 5
        /// </summary>
        protected IModelType5 model5;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;ViewType, IModelType1, IModelType2, IModelType3, IModelType4, IModelType5&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null && model1 != null && model2 != null && model3 != null && model4 != null && model5 != null;
            }
        }

        #region IPresenter2 Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        public void BindModels(object[] models)
        {
            Check.Require(models != null && models.Length == 5, "models' length length  must be 5.");
            Check.Require(typeof(IModelType1).IsAssignableFrom(models[0].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType2).IsAssignableFrom(models[1].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType3).IsAssignableFrom(models[2].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType4).IsAssignableFrom(models[3].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType5).IsAssignableFrom(models[4].GetType()), "model's type does not match Presenter's model type.");
            this.model1 = (IModelType1)models[0];
            this.model2 = (IModelType2)models[1];
            this.model3 = (IModelType3)models[2];
            this.model4 = (IModelType4)models[3];
            this.model5 = (IModelType5)models[4];
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] TypeOfModels
        {
            get
            {
                return new Type[] { typeof(IModelType1), typeof(IModelType2), typeof(IModelType3), typeof(IModelType4), typeof(IModelType5) };
            }
        }

        #endregion
    }

    /// <summary>
    /// Base class of all presenters
    /// </summary>
    /// <typeparam name="ViewType"></typeparam>
    /// <typeparam name="IModelType1"></typeparam>
    /// <typeparam name="IModelType2"></typeparam>
    /// <typeparam name="IModelType3"></typeparam>
    /// <typeparam name="IModelType4"></typeparam>
    /// <typeparam name="IModelType5"></typeparam>
    /// <typeparam name="IModelType6"></typeparam>
    public abstract class Presenter<ViewType, IModelType1, IModelType2, IModelType3, IModelType4, IModelType5, IModelType6> : IPresenter2
        //where IModelType1 : IServiceInterface
        //where IModelType2 : IServiceInterface
        //where IModelType3 : IServiceInterface
        //where IModelType4 : IServiceInterface
        //where IModelType5 : IServiceInterface
        //where IModelType6 : IServiceInterface
    {
        /// <summary>
        /// The view
        /// </summary>
        protected ViewType view;

        /// <summary>
        /// The model 1
        /// </summary>
        protected IModelType1 model1;

        /// <summary>
        /// The model 2
        /// </summary>
        protected IModelType2 model2;

        /// <summary>
        /// The model 3
        /// </summary>
        protected IModelType3 model3;

        /// <summary>
        /// The model 4
        /// </summary>
        protected IModelType4 model4;

        /// <summary>
        /// The model 5
        /// </summary>
        protected IModelType5 model5;

        /// <summary>
        /// The model 6
        /// </summary>
        protected IModelType5 model6;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Presenter&lt;ViewType, IModelType1, IModelType2, IModelType3, IModelType4, IModelType5, IModelType6&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized
        {
            get
            {
                return view != null && model1 != null && model2 != null && model3 != null && model4 != null && model5 != null && model6 != null;
            }
        }

        #region IPresenter2 Members

        /// <summary>
        /// Binds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void BindView(object view)
        {
            Check.Require(view != null, "view could not be null.");
            Check.Require(typeof(ViewType).IsAssignableFrom(view.GetType()), "view's type does not match Presenter's view type.");
            this.view = (ViewType)view;
        }

        /// <summary>
        /// Binds the models.
        /// </summary>
        /// <param name="model">The models.</param>
        public void BindModels(object[] models)
        {
            Check.Require(models != null && models.Length == 6, "models' length length  must be 6.");
            Check.Require(typeof(IModelType1).IsAssignableFrom(models[0].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType2).IsAssignableFrom(models[1].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType3).IsAssignableFrom(models[2].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType4).IsAssignableFrom(models[3].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType5).IsAssignableFrom(models[4].GetType()), "model's type does not match Presenter's model type.");
            Check.Require(typeof(IModelType6).IsAssignableFrom(models[5].GetType()), "model's type does not match Presenter's model type.");
            this.model1 = (IModelType1)models[0];
            this.model2 = (IModelType2)models[1];
            this.model3 = (IModelType3)models[2];
            this.model4 = (IModelType4)models[3];
            this.model5 = (IModelType5)models[4];
            this.model6 = (IModelType5)models[5];
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>The type of the view.</value>
        public Type TypeOfView
        {
            get
            {
                return typeof(ViewType);
            }
        }

        /// <summary>
        /// Gets the types of the model.
        /// </summary>
        /// <value>The types of the model.</value>
        public Type[] TypeOfModels
        {
            get
            {
                return new Type[] { typeof(IModelType1), typeof(IModelType2), typeof(IModelType3), typeof(IModelType4), typeof(IModelType5), typeof(IModelType6) };
            }
        }

        #endregion
    }
}
