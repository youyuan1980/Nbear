using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common.Design;

namespace NBear.Test.CaseTests.design.ManyToManyDesign2
{
    [BatchUpdate(10)]
    [MappingName("mtm2_Role")]
    [OutputNamespace("ManyToManyImpl2")]
    public interface Role : Entity
    {
        string Name
        {
            get;
            set;
        }

        int Describe
        {
            get;
            set;
        }
        [PrimaryKey]
        Guid ID
        {
            get;
            set;
        }
        //[PrimaryKey]
        [IndexProperty]
        int FID
        {
            get;
        }
        [ManyToManyQuery(typeof(UserRoles), Contained = false, LazyLoad = false)]
        User[] Users
        {
            get;
            set;
        }
    }

    [BatchUpdate(3)]
    [MappingName("mtm2_User")]
    [OutputNamespace("ManyToManyImpl2")]
    public interface User : Entity
    {
        [PrimaryKey]
        Guid ID
        {
            get;
            set;
        }
        //[PrimaryKey]
        int FID
        {
            get;
        }
        [ManyToManyQuery(typeof(UserRoles), Contained = false)]
        Role[] Roles
        {
            get;
            set;
        }
    }

    [Relation]
    [MappingName("mtm2_UserRole")]
    [OutputNamespace("ManyToManyImpl2")]
    public interface UserRoles : NBear.Common.Design.Entity
    {
        [RelationKey(typeof(Role))]
        Guid RoleID
        {
            get;
            set;
        }
        [RelationKey(typeof(User))]
        Guid UserID
        {
            get;
            set;
        }
    }
}
