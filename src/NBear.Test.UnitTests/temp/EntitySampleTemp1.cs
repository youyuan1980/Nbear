using System;
using NBear.Common.Design;

namespace NBear.Test.UnitTests.temp.EntitySampleTemp1
{
    public interface PrivilegeOwner : Entity
    {
        [PrimaryKey]
        [SqlType("int")]
        int ID { get; set; }

        [SqlType("nvarchar(50)")]
        string Name { get; set; }
    }

    public interface UserGroup : Entity, PrivilegeOwner
    {
        [SqlType("nvarchar(255)")]
        string Comment { get; set; }
    }

    public interface User : Entity, PrivilegeOwner
    {
    }

    public interface GhostUser : User
    {
    }

    public interface LocalUser : User
    {
        [SqlType("nvarchar(50)")]
        string LoginID { get; set; }

        [SqlType("nvarchar(50)")]
        string Password { get; set; }
    }

    public interface AgentUser : User
    {
        [SqlType("nvarchar(50)")]
        string LoginID { get; set; }
    }
}