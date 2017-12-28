using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common;
using System.Data.Common;

namespace NBear.Data
{
    public class DalBase<T, S> where T : Entity,new()
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
    }
}
