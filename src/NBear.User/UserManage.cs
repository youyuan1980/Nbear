using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using NBear.Data;
namespace NBear.User
{
    public class UserManage
    {

        public static UserInfo Get_ById(string UserId) {

            UserInfo data = null;
            Gateway gateway = Gateway.Default;
            string sql = "select * from t_user where userid = '"+UserId+"'";
            DataSet dt = gateway.Db.ExecuteDataSet(CommandType.Text, sql);
            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                data = new UserInfo();
                DataRow row = dt.Tables[0].Rows[i];
                data.UserID = Convert.ToString(row["userid"]);
                data.UserName = Convert.ToString(row["username"]);
                data.UserPwd = Convert.ToString(row["userpwd"]);

                sql = "select roleid from t_userrole where userid = '"+UserId+"'";
                DataSet roledt = gateway.Db.ExecuteDataSet(CommandType.Text, sql);
                List<string> list = new List<string>();
                for (int j = 0; j < roledt.Tables[0].Rows.Count; j++)
                {
                    list.Add(Convert.ToString(roledt.Tables[0].Rows[j]["roleid"]));
                }

                data.Roles = list.ToArray();
            }
            return data;
        }

        public static string GetUserRoles(string UserId)
        {
            string rolesstr = string.Empty;
            string sql = "select t.userid,b.rolename from t_user t inner join t_userrole a on a.userid=t.userid ";
            sql += " inner join t_roles b on b.roleid=a.roleid where t.userid='" + UserId + "'";
            Gateway gateway = Gateway.Default;
            DataSet dt = gateway.Db.ExecuteDataSet(CommandType.Text, sql);
            if (dt.Tables.Count == 1)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        rolesstr += Convert.ToString(dt.Tables[0].Rows[i]["rolename"]);
                    }
                    else
                    {
                        rolesstr += "、" + Convert.ToString(dt.Tables[0].Rows[i]["rolename"]);
                    }
                }
            }
            return rolesstr;
        }

        public static bool CheckUserID(string USERID)
        {
            Gateway gateway = Gateway.Default;
            int flag = Convert.ToInt32(gateway.Db.ExecuteScalar(CommandType.Text, "select count(*) from t_user where userid='" + USERID + "'"));
            if (flag > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region 验证登陆
        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="UserPassWord"></param>
        /// <returns></returns>
        public static UserInfo ValidateUser(string UserId, string UserPassWord)
        {
            List<string> list = new List<string>();
            UserInfo data = Get_ById(UserId);
            if (data != null)
            {
                string pwd = getMd5Hash(UserPassWord);
                if (data.UserPwd == pwd)
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 返回左侧树形菜单信息
        public static string GetMenuJson(string UserID)
        {
            string json = "[";
            Gateway gateway = Gateway.Default;
            string sql = "select id,name,pid,isparent,url from t_menu t ";
            sql += " where roleid is null or roleid in (select roleid from t_userrole where userid='" + UserID + "') ";
            sql += " order by menuorder ";
            DataSet dt = gateway.Db.ExecuteDataSet(CommandType.Text, sql);
            if (dt.Tables.Count == 1)
            {
                for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
                {
                    if (i > 0)
                    {
                        json += ",";
                    }
                    string id = Convert.ToString(dt.Tables[0].Rows[i]["id"]);
                    string pid = Convert.ToString(dt.Tables[0].Rows[i]["pid"]);
                    string name = Convert.ToString(dt.Tables[0].Rows[i]["name"]);
                    string isparent = string.Empty;
                    if (Convert.ToString(dt.Tables[0].Rows[i]["isparent"]) == "1")
                    {
                        isparent = ",isParent:true";
                    }
                    string url = string.Empty;
                    url = ",url:\"" + Convert.ToString(dt.Tables[0].Rows[i]["url"]) + "\",target:\"main\"";
                    json += "{\"id\":\"" + id + "\",\"pId\":\"" + pid + "\",open:true,\"name\":\"" + name + "\"" + isparent + url + "}";
                }
            }
            json += "]";
            return json;
        }
        #endregion

        #region 获取MD5码
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion
    }
}
