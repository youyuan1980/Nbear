using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common;
using System.Data;
using System.Data.Common;

namespace NBear.Data
{
    public class DalBase<T, S> where T : Entity, new()
    {
        #region 保存信息
        public static void Save(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Gateway gateway = Gateway.Default;
            gateway.Save<T>(data);
        }

        public static void Save(T data, DbTransaction trans)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Gateway gateway = Gateway.Default;
            gateway.Save<T>(data, trans);
        }
        #endregion

        #region 删除信息
        public static void Remove(S ID)
        {
            Gateway gateway = Gateway.Default;
            gateway.Delete<T>(ID);
        }
        #endregion

        #region 获取单个信息
        public static T GetT_ById(S ID)
        {
            Gateway gateway = Gateway.Default;
            return gateway.Find<T>(ID);
        }
        #endregion

        #region 查找列表
        public static DataSet FindList(string fields, string tablename, string keyId, string expression, int PageSize, int PageIndex, out int RowCount)
        {
            return FindList(fields, tablename, keyId, string.Empty, expression, PageSize, PageIndex, out RowCount);
        }

        public static DataSet FindList(string fields, string tablename, string keyId, string order, string expression, int PageSize, int PageIndex, out int RowCount)
        {
            string SelectSql = string.Format("SELECT {0} from {1}", fields, tablename);
            if (expression != null && expression != string.Empty)
            {
                SelectSql = SelectSql + " WHERE " + expression;
            }
            SelectSql += order;
            Gateway gateway = Gateway.Default;
            IPageSplit PageSplit = gateway.Db.GetPageSplit(SelectSql, keyId, null);
            PageSplit.PageSize = PageSize;
            RowCount = PageSplit.GetRowCount();
            DataSet ds = PageSplit.GetPage(PageIndex);
            return ds;
        }

        public static DataSet FindList(string fields, string tablename, string expression)
        {
            return FindList(fields, tablename, string.Empty, expression);
        }


        public static DataSet FindList(string fields, string tablename, string order, string expression)
        {
            string SelectSql = string.Format("SELECT {0} from {1}", fields, tablename);
            if (expression != null && expression != string.Empty)
            {
                SelectSql = SelectSql + " WHERE " + expression;
            }
            SelectSql += order;
            Gateway gateway = Gateway.Default;
            return gateway.Db.ExecuteDataSet(CommandType.Text, SelectSql);
        }
        #endregion
    }
}
