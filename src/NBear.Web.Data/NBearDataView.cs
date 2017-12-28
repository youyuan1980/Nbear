
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
using System.Reflection;
using System.Web.Compilation;
using System.Web.UI;
using NBear.Common;
using NBear.Data;

namespace NBear.Web.Data
{
	public class NBearDataView : System.Web.UI.DataSourceView
	{
        private NBearDataSource owner;

        internal NBearDataView(NBearDataSource owner, string name)
            : base(owner, name)
        {
			this.owner = owner;	
		}

		public override bool CanSort { get { return true; } }
		public override bool CanPage { get { return true; } }
		public override bool CanRetrieveTotalRowCount { get { return true; } }
		public override bool CanInsert { get { return true; } }
		public override bool CanUpdate { get { return true; } }
		public override bool CanDelete { get { return true; } }

		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            NBearDataSourceSelectingEventArgs selectingArgs = new NBearDataSourceSelectingEventArgs(arguments);
            owner.OnSelecting(selectingArgs);
            arguments = selectingArgs.SelectArguments == null ? DataSourceSelectArguments.Empty : selectingArgs.SelectArguments;
			arguments.RaiseUnsupportedCapabilitiesError(this);
			Gateway gateway = owner.Gateway;
			Type type = Util.GetType(this.owner.TypeName);

			string sort = owner.DefaultOrderByExpression;
			if (!string.IsNullOrEmpty(arguments.SortExpression))
            {
				string[] parts = arguments.SortExpression.Split(new char[] { ' ' }, 2);
				string field = "{" + parts[0] + "}";
				sort = field + (parts.Length >= 2 ? " " + parts[1] : " ASC");
			}

			int page = (arguments.MaximumRows > 0 ? (arguments.StartRowIndex / arguments.MaximumRows) + 1 : 1);
            WhereClip where = string.IsNullOrEmpty(owner.FilterExpression) ? WhereClip.All : new WhereClip(owner.FilterExpression);
            OrderByClip orderBy = string.IsNullOrEmpty(sort) ? OrderByClip.Default : new OrderByClip(sort);
            Array list;
            if (arguments.MaximumRows > 0)
            {
                object pageSelector = typeof(Gateway).GetMethod("GetPageSelector").MakeGenericMethod(type).Invoke(gateway, new object[] { where, orderBy, arguments.MaximumRows });
                list = (Array)typeof(PageSelector<>).MakeGenericType(type).GetMethod("FindPage").Invoke(pageSelector, new object[] { page });
            }
            else
            {
                list = (Array)GetGatewayMethodInfo("EntityType[] FindArray[EntityType](NBear.Common.WhereClip, NBear.Common.OrderByClip)").MakeGenericMethod(type).Invoke(gateway, new object[] { where, orderBy });
            }
            if (arguments.RetrieveTotalRowCount) arguments.TotalRowCount = (int)GetGatewayMethodInfo("Int32 Count[EntityType](NBear.Common.WhereClip)").MakeGenericMethod(type).Invoke(gateway, new object[] { where });
            NBearDataSourceSelectedEventArgs selectedArgs = new NBearDataSourceSelectedEventArgs(arguments, list);
            owner.OnSelected(selectedArgs);
            return selectedArgs.ResultEntities;
		}

		protected override int ExecuteInsert(IDictionary values)
        {
            Gateway gateway = owner.Gateway;
            Type type = Util.GetType(this.owner.TypeName);
            object entity = Activator.CreateInstance(type);

			foreach (string key in values.Keys)
            {
				PropertyInfo property = Util.DeepGetProperty(type, key);
				if (property.CanWrite)
                {
                    property.SetValue(entity, ChangeType(values[key], property.PropertyType), null);
				}
			}

            NBearDataSourceEventArgs savingArgs = new NBearDataSourceEventArgs(entity);
            owner.OnSaving(savingArgs);
            entity = savingArgs.Entity;

            if (entity == null)
            {
                return 0;
            }

            System.Data.Common.DbTransaction tran = null;
            try
            {
                tran = gateway.BeginTransaction();
                MethodInfo miSave = GetGatewayMethodInfo("Int32 Save[EntityType](EntityType, System.Data.Common.DbTransaction)");
                miSave.MakeGenericMethod(type).Invoke(gateway, new object[] { entity, tran });
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            finally
            {
                gateway.CloseTransaction(tran);
            }
            owner.OnSaved(new NBearDataSourceEventArgs(entity));

			this.OnDataSourceViewChanged(EventArgs.Empty);
			return 1;
		}

		protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            Gateway gateway = owner.Gateway;
            Type type = Util.GetType(this.owner.TypeName);
            string keyMember = null;
            Type keyType = null;
            object keyValue = null;
            PropertyInfo keyMemberProperty = null;
            List<string> keyNames = new List<string>();

            Entity entity = (Entity)Activator.CreateInstance(type);
            if (keys != null && keys.Count > 0)
            {
                IEnumerator en = keys.Keys.GetEnumerator();
                while (en.MoveNext())
                {
                    keyMember = en.Current.ToString();
                    keyNames.Add(keyMember);
                    keyMemberProperty = Util.DeepGetProperty(type, keyMember);
                    keyValue = ChangeType(keys[keyMember], keyMemberProperty.PropertyType);
                    keyMemberProperty.SetValue(entity, keyValue, null);
                }
            }
            else
            {
                EntityConfiguration ec = MetaDataManager.GetEntityConfiguration(this.owner.TypeName);
                foreach (PropertyConfiguration pc in ec.Properties)
                {
                    if (pc.IsPrimaryKey)
                    {
                        keyMember = pc.Name;
                        keyNames.Add(keyMember);
                        keyType = Util.GetType(pc.PropertyType);
			            keyValue = ChangeType(oldValues[keyMember], keyType);
                        keyMemberProperty = Util.DeepGetProperty(type, keyMember);
                        keyMemberProperty.SetValue(entity, keyValue, null);
                    }
                }
            }

			bool changed = false;
            entity.Attach();
			foreach (string key in values.Keys)
            {
				if (keyNames.Contains(key)) continue;
			    PropertyInfo property = Util.DeepGetProperty(type, key);
				if (this.owner.ConflictDetection == ConflictOptions.CompareAllValues)
                {
                    if (property != null && property.GetValue(entity, null) != ChangeType(oldValues[key], property.PropertyType))
                    {
                        throw new System.Data.DBConcurrencyException("The underlying data has changed.");
                    }
				}
				if (values[key] == oldValues[key]) continue;
                if (property != null && property.CanWrite)
                {
					changed = true;
                    property.SetValue(entity, ChangeType(values[key], property.PropertyType), null);
				}
			}

			if (changed)
            {
                NBearDataSourceEventArgs savingArgs = new NBearDataSourceEventArgs(entity);
                owner.OnSaving(savingArgs);
                entity = (Entity)savingArgs.Entity;

                if (entity == null)
                {
                    return 0;
                }

                System.Data.Common.DbTransaction tran = null;
                try
                {
                    tran = gateway.BeginTransaction();
                    MethodInfo miSave = GetGatewayMethodInfo("Int32 Save[EntityType](EntityType, System.Data.Common.DbTransaction)");
                    miSave.MakeGenericMethod(type).Invoke(gateway, new object[] { entity, tran });
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    gateway.CloseTransaction(tran);
                }
                owner.OnSaved(new NBearDataSourceEventArgs(entity));

				this.OnDataSourceViewChanged(EventArgs.Empty);
			}
			return (changed ? 1 : 0);
		}

        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
        {
            Gateway gateway = owner.Gateway;
            Type type = Util.GetType(this.owner.TypeName);

            string keyMember = null;
            Type keyType = null;
            object keyValue = null;
            PropertyInfo keyMemberProperty = null;
            List<string> keyNames = new List<string>();
            List<object> keyValues = new List<object>();

            if (keys != null && keys.Count > 0)
            {
                IEnumerator en = keys.Keys.GetEnumerator();
                while (en.MoveNext())
                {
                    keyMember = en.Current.ToString();
                    keyNames.Add(keyMember);
                    keyMemberProperty = Util.DeepGetProperty(type, keyMember);
                    keyValue = ChangeType(keys[keyMember], keyMemberProperty.PropertyType);
                    keyValues.Add(keyValue);
                }
            }
            else
            {
                EntityConfiguration ec = MetaDataManager.GetEntityConfiguration(this.owner.TypeName);
                foreach (PropertyConfiguration pc in ec.Properties)
                {
                    if (pc.IsPrimaryKey)
                    {
                        keyMember = pc.Name;
                        keyNames.Add(keyMember);
                        keyType = Util.GetType(pc.PropertyType);
                        keyValue = ChangeType(oldValues[keyMember], keyType);
                        keyValues.Add(keyValue);
                    }
                }
            }

            MethodInfo miFind = GetGatewayMethodInfo("EntityType Find[EntityType](System.Object[])");
            object entity = miFind.MakeGenericMethod(type).Invoke(gateway, new object[]{ keyValues.ToArray() });

            if (entity == null)
            {
                return 0;
            }

            //check DBConcurrency
            if (this.owner.ConflictDetection == ConflictOptions.CompareAllValues)
            {
                foreach (string key in oldValues.Keys)
                {
                    if (keyNames.Contains(key)) continue;
                    PropertyInfo property = Util.DeepGetProperty(type, key);
                    if (property != null && property.GetValue(entity, null) != ChangeType(oldValues[key], property.PropertyType))
                    {
                        throw new System.Data.DBConcurrencyException("The underlying data has changed.");
                    }
                }
            }

            NBearDataSourceEventArgs deletingArgs = new NBearDataSourceEventArgs(entity);
            owner.OnDeleting(deletingArgs);
            entity = deletingArgs.Entity;

            if (entity == null)
            {
                return 0;
            }

            System.Data.Common.DbTransaction tran = null;
            try
            {
                tran = gateway.BeginTransaction();

                MethodInfo miDelete = GetGatewayMethodInfo("Void Delete[EntityType](EntityType, System.Data.Common.DbTransaction)");
                miDelete.MakeGenericMethod(type).Invoke(gateway, new object[] { entity, tran });
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            finally
            {
                gateway.CloseTransaction(tran);
            }

            owner.OnDeleted(new NBearDataSourceEventArgs(entity));

            this.OnDataSourceViewChanged(EventArgs.Empty);
            return 1;
        }

        private static object ChangeType(object value, Type type)
        {
            if (value == null || value.GetType() == type)
            {
                return value;
            }

            if (type == typeof(Guid))
            {
                return new Guid(value.ToString());
            }
            else if (type == typeof(bool))
            {
                if (value.ToString().ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch
                {
                    return NBear.Common.SerializationManager.Deserialize(type, value.ToString());
                }
            }

        }

        private static MethodInfo GetGatewayMethodInfo(string signiture)
        {
            MethodInfo mi = null;
            foreach (MethodBase mb in typeof(Gateway).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (mb.ToString() == signiture)
                {
                    mi = (MethodInfo)mb;
                    break;
                }
            }
            return mi;
        }
	}
}
