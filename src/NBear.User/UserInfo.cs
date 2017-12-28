using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBear.User
{
    public class UserInfo
    {
        private string _UserID;
        private string _UserName;
        private string[] _Roles;
        private string _UserPwd;

        public string UserPwd {
            get { return _UserPwd; }
            set { _UserPwd = value; }
        }

        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public string[] Roles
        {
            get { return _Roles; }
            set { _Roles = value; }
        }
    }
}
