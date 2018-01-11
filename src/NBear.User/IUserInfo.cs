using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBear.User
{
    public interface IUserInfo
    {
        string UserID { get; set; }
        string UserName { get; set; }
        List<string> Roles { get; set; }
        string UserPwd { get; set; }
    }
}
