
//Teddy - http://nbear.org - NBear.Web.Data.NBearDataSource Control.
//Modified based on Paul Wilson's WilsonORMDataSource, 
//So, leave Paul's credit lines below as he wants.

//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using NBear.Common;

namespace NBear.Web.Data
{
    /// <summary>
    /// NBear DataSource Event Arguments
    /// </summary>
    public class NBearDataSourceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NBearDataSourceEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public NBearDataSourceEventArgs(object entity)
        {
            this.entity = entity;
        }

        private object entity;

        /// <summary>
        /// The entity being operated.
        /// </summary>
        /// <value>The entity.</value>
        public object Entity
        {
            get { return this.entity; }
        }
    }

    /// <summary>
    /// NBear DataSource Selecting Event Arguments
    /// </summary>
    public class NBearDataSourceSelectingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NBearDataSourceSelectingEventArgs"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public NBearDataSourceSelectingEventArgs(DataSourceSelectArguments arguments)
        {
            this.arguments = arguments;
        }

        private DataSourceSelectArguments arguments;

        /// <summary>
        /// Gets or sets the select arguments.
        /// </summary>
        /// <value>The select arguments.</value>
        public DataSourceSelectArguments SelectArguments
        {
            get { return this.arguments; }
            set { this.arguments = value; }
        }    
    }

    public class NBearDataSourceSelectedEventArgs : NBearDataSourceSelectingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NBearDataSourceSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="resultEntities">The result entities.</param>
        public NBearDataSourceSelectedEventArgs(DataSourceSelectArguments arguments, Array resultEntities) : base(arguments)
        {
            this.resultEntities = resultEntities;
        }

        private Array resultEntities;

        /// <summary>
        /// Gets or sets the result entities.
        /// </summary>
        /// <value>The result entities.</value>
        public Array ResultEntities
        {
            get { return this.resultEntities; }
            set { this.resultEntities = value; }
        }
    }

    /// <summary>
    /// NBear Data Source
    /// </summary>
	public class NBearDataSource : System.Web.UI.DataSourceControl
	{
		private ConflictOptions conflicts = ConflictOptions.OverwriteChanges;
		private NBearDataView dataView;

        private T GetViewState<T>(string key)
        {
            return ViewState[key] == null ? default(T) : (T)ViewState[key];
        }

        static private Dictionary<string, NBear.Data.Gateway> gateways = new Dictionary<string, NBear.Data.Gateway>();

        internal NBear.Data.Gateway Gateway
        {
            get
            {
                string connStrName = GetViewState<string>("connStrName");
                if (string.IsNullOrEmpty(connStrName))
                {
                    return NBear.Data.Gateway.Default;
                }

                if (!gateways.ContainsKey(connStrName))
                {
                    lock (gateways)
                    {
                        NBear.Data.Gateway gw = new NBear.Data.Gateway(connStrName);
                        gateways.Add(connStrName, gw);
                    }
                }
                return gateways[connStrName];
            }
        }

		[Category("Data"), DefaultValue(""), Description("The value of name attribute of the connectionString configuration line.")]
		public string ConnectionStringName
        {
            get { return GetViewState<string>("connStrName"); }
			set {
                if ((!string.IsNullOrEmpty(value)) && (!value.Equals(GetViewState<string>("connStrName"))))
                {
                    ViewState["connStrName"] = value;
					this.RaiseDataSourceChangedEvent(EventArgs.Empty);
				}
			}
		}

		[Category("Data"), DefaultValue(""), Description("The entity type used for this control.  An example would be 'Entities.BusObject'.")]
		public string TypeName
        {
            get { return GetViewState<string>("typeName"); }
			set {
                if ((!string.IsNullOrEmpty(value)) && (!value.Equals(GetViewState<string>("typeName"))))
                {
					ViewState["typeName"] = value;
					this.RaiseDataSourceChangedEvent(EventArgs.Empty);
				}
			}
		}

        [Category("Data"), DefaultValue(""), Description("Optional sql where-clause used in data retrieval.")]
        public string FilterExpression
        {
            get { return GetViewState<string>("filter"); }
            set
            {
                if ((!string.IsNullOrEmpty(value)) && (!value.Equals(GetViewState<string>("filter"))))
                {
                    ViewState["filter"] = value;
                    this.RaiseDataSourceChangedEvent(EventArgs.Empty);
                }
            }
        }

        [Category("Data"), DefaultValue(""), Description("Optional sql order by-clause used in data retrieval as default order by condition.")]
        public string DefaultOrderByExpression
        {
            get { return GetViewState<string>("defaultOrderBy"); }
            set
            {
                if ((!string.IsNullOrEmpty(value)) && (!value.Equals(GetViewState<string>("defaultOrderBy"))))
                {
                    ViewState["defaultOrderBy"] = value;
                    this.RaiseDataSourceChangedEvent(EventArgs.Empty);
                }
            }
        }

        [Category("Data"), DefaultValue(ConflictOptions.OverwriteChanges), Description("Specifies how data conflicts are resolved.")]
		public ConflictOptions ConflictDetection
        {
			get { return this.conflicts; }
			set {
				if (!value.Equals(this.conflicts)) {
					this.conflicts = value;
					this.RaiseDataSourceChangedEvent(EventArgs.Empty);
				}
			}
		}

		static private readonly string[] viewNames = { "DefaultView" };

        public NBearDataSource()
        {
			this.dataView = new NBearDataView(this, viewNames[0]);
		}

		protected override DataSourceView GetView(string viewName)
        {
			return this.dataView;
		}

		protected override ICollection GetViewNames()
        {
			return viewNames;
		}

        public void Filter(WhereClip where)
        {
            if (where == null || where == WhereClip.All)
            {
                FilterExpression = null;
            }
            else
            {
                string whereStr = where.ToString();
                if (where.ParamValues != null)
                {
                    foreach (object p in where.ParamValues)
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("(" + "@" + @"[\w\d_]+)");
                        if (p != null && p is string)
                        {
                            whereStr = r.Replace(whereStr, Util.FormatParamVal(p.ToString().Replace("@", "\007")), 1);
                        }
                        else
                        {
                            whereStr = r.Replace(whereStr, Util.FormatParamVal(p), 1);
                        }
                    }
                }
                whereStr = whereStr.Replace("\007", "@");
                FilterExpression = whereStr;
            }
        }

        public void OrderBy(OrderByClip orderBy)
        {
            DefaultOrderByExpression = (orderBy == null ? null : orderBy.ToString());
        }

        #region Events

        /// <summary>
        /// The saving evet.
        /// </summary>
        public event EventHandler<NBearDataSourceEventArgs> Saving;

        /// <summary>
        /// Raises the <see cref="E:Saving"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NBear.Web.Data.NBearDataSourceEventArgs"/> instance containing the event data.</param>
        internal void OnSaving(NBearDataSourceEventArgs args)
        {
            if (Saving != null)
            {
                Saving(this, args);
            }
        }

        /// <summary>
        /// The saved event.
        /// </summary>
        public event EventHandler<NBearDataSourceEventArgs> Saved;

        /// <summary>
        /// Raises the <see cref="E:Saved"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NBear.Web.Data.NBearDataSourceEventArgs"/> instance containing the event data.</param>
        internal void OnSaved(NBearDataSourceEventArgs args)
        {
            if (Saved != null)
            {
                Saved(this, args);
            }
        }

        /// <summary>
        /// The selecting evet.
        /// </summary>
        public event EventHandler<NBearDataSourceSelectingEventArgs> Selecting;

        /// <summary>
        /// Raises the <see cref="E:Selecting"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NBear.Web.Data.NBearDataSourceEventArgs"/> instance containing the event data.</param>
        internal void OnSelecting(NBearDataSourceSelectingEventArgs args)
        {
            if (Selecting != null)
            {
                Selecting(this, args);
            }
        }

        /// <summary>
        /// The Selected event.
        /// </summary>
        public event EventHandler<NBearDataSourceSelectedEventArgs> Selected;

        /// <summary>
        /// Raises the <see cref="E:Selected"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NBear.Web.Data.NBearDataSourceEventArgs"/> instance containing the event data.</param>
        internal void OnSelected(NBearDataSourceSelectedEventArgs args)
        {
            if (Selected != null)
            {
                Selected(this, args);
            }
        }

        /// <summary>
        /// The Deleting evet.
        /// </summary>
        public event EventHandler<NBearDataSourceEventArgs> Deleting;

        /// <summary>
        /// Raises the <see cref="E:Deleting"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NBear.Web.Data.NBearDataSourceEventArgs"/> instance containing the event data.</param>
        internal void OnDeleting(NBearDataSourceEventArgs args)
        {
            if (Deleting != null)
            {
                Deleting(this, args);
            }
        }

        /// <summary>
        /// The Deleted event.
        /// </summary>
        public event EventHandler<NBearDataSourceEventArgs> Deleted;

        /// <summary>
        /// Raises the <see cref="E:Deleted"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NBear.Web.Data.NBearDataSourceEventArgs"/> instance containing the event data.</param>
        internal void OnDeleted(NBearDataSourceEventArgs args)
        {
            if (Deleted != null)
            {
                Deleted(this, args);
            }
        }

        #endregion
    }
}
