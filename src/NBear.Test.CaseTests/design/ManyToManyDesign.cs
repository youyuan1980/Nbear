using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common.Design;

namespace NBear.Test.CaseTests.design.ManyToManyDesign
{
    [OutputNamespace("ManyToManyImpl")]
    [MappingName("mtm_User")]
    public interface User : Entity
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }

        [ManyToManyQuery(typeof(UserGroup))]
        Group[] Groups { get; set; }
    }

    [OutputNamespace("ManyToManyImpl")]
    [MappingName("mtm_Group")]
    public interface Group : Entity
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }

        [ManyToManyQuery(typeof(UserGroup))]
        User[] Users { get; set; }
    }

    [Relation]
    [OutputNamespace("ManyToManyImpl")]
    [MappingName("mtm_UserGroup")]
    public interface UserGroup : Entity
    {
        [RelationKey(typeof(User))]
        int UserID { get; set; }
        [RelationKey(typeof(Group))]
        int GroupID { get; set; }
    }
}
