using System;
using System.Collections.Generic;
using System.Text;

using NBear.Common.Design;
using NBear.Test.CaseTests.shared;

namespace NBear.Test.CaseTests.design
{
    public interface ITemp { }

    [Comment("This is a user. 这是一个User。")]
    [ImplementInterface("NBear.Test.CaseTests.design.ITemp")]
    [BatchUpdate(10)]
    public interface User : Entity
    {
        [CompoundUnit, SqlType("ntext")]
        UserName Name
        {
            get;
            set;
        }

        [IndexProperty(true)]
        [SqlType("int", DefaultValue="1")]
        UserStatus Status
        {
            get;
            set;
        }

        [Comment("This is \nuser's ID.")]
        [PrimaryKey]
        [MappingName("UserID")]
        Guid ID
        {
            get;
            set;
        }

        [PkQuery]
        UserProfile Profile
        {
            get;
            set;
        }

        [ManyToManyQuery(typeof(UserGroup), AdditionalWhere="{Weight} >= 0")]
        [SerializationIgnore]
        Group[] Groups
        {
            get;
            set;
        }

        [FkReverseQuery]
        [MappingName("TeamID")]
        Team Team
        {
            get;
            set;
        }
    }

    [BatchUpdate(10)]
    public interface Group : Entity
    {
        [PrimaryKey]
        [MappingName("GroupID")]
        Guid ID
        {
            get;
            set;
        }

        [SqlType("nvarchar(50)", DefaultValue="'default group name'")]
        string Name
        {
            get;
            set;
        }

        bool IsPublic
        {
            get;
            set;
        }

        [ManyToManyQuery(typeof(UserGroup), Contained=true)]
        User[] Users { get; set; }
    }

    [BatchUpdate(10)]
    public interface AgentUser : User
    {
        string LoginName
        {
            get;
            set;
        }

        [ManyToManyQuery(typeof(AgentUserDomain), Contained=true)]
        Domain[] Domains
        {
            get;
            set;
        }
    }

    [AutoPreLoad]
    [BatchUpdate(10)]
    public interface LocalUser : AgentUser
    {
        [SerializationIgnore]
        [NotNull]
        string Password
        {
            get;
            set;
        }
    }

    public interface UserProfile : Entity
    {
        [PrimaryKey]
        Guid UserID
        {
            get;
            set;
        }

        string ContentXml
        {
            get;
            set;
        }
    }

    [Relation]
    [BatchUpdate(10)]
    public interface UserGroup : Entity
    {
        [RelationKey(typeof(User))]
        Guid UserID
        {
            get;
            set;
        }

        [RelationKey(typeof(Group))]
        Guid GroupID
        {
            get;
            set;
        }

        int Weight
        {
            get;
            set;
        }
    }

    [Relation]
    public interface AgentUserDomain : Entity
    {
        [RelationKey(typeof(AgentUser))]
        [MappingName("UserID")]
        Guid AgentUserID
        {
            get;
            set;
        }

        [RelationKey(typeof(Domain))]
        Guid DomainID
        {
            get;
            set;
        }
    }

    public interface Domain : Entity
    {
        [PrimaryKey]
        Guid ID
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        string Desc
        {
            get;
            set;
        }
    }

    public interface Team : Entity
    {
        [PrimaryKey]
        Guid ID
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        [FkQuery("Team", Contained=true)]
        User[] Users { get; set; }
    }

    public interface MasterParent : Entity
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }
    }

    public interface Master : MasterParent
    {
        string OtherData { get; set; }

        [FkQuery("MasterID", Contained=true, AdditionalWhere="NOT {Name} IS NULL")]
        Detail[] Details { get; set; }

        int? IntProperty
        {
            get;
            set;
        }

        decimal? DecimalProperty
        {
            get;
            set;
        }

        Guid? GuidProperty
        {
            get;
            set;
        }
    }

    public interface Detail : Entity
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }

        [FriendKey(typeof(Master))]
        int MasterID { get; set; }
    }

    public interface CategoryInfo : Entity
    {
        [PrimaryKey]
        [SqlType("varchar(10)")]
        [MappingName("CategoryId")]
        string Id { get; set; }

        [SqlType("varchar(80)")]
        string Name { get; set; }
    }

    public interface ProductInfo : Entity
    {
        [PrimaryKey]
        [SqlType("varchar(10)")]
        [MappingName("ProductId")]
        string Id { get; set; }

        [FriendKey(typeof(CategoryInfo))]
        [IndexProperty]
        [NotNull]
        [SqlType("varchar(10)")]
        string CategoryId { get; set; }
    }

    public interface TreeEntity<EntityType>
        where EntityType : Entity
    {
        [FkReverseQuery(LazyLoad = true)]
        [MappingName("ParentID")]
        [SerializationIgnore]
        EntityType Parent
        {
            get;
            set;
        }

        [FkQuery("Parent", Contained=true, LazyLoad = true)]
        EntityType[] Childs
        {
            get;
            set;
        }
    }

    public interface Category : Entity, TreeEntity<Category>
    {
        [PrimaryKey]
        long ID { get; }

        string Name { get; set; }
    }

    public interface cms_Articles : Entity
    {
        [PrimaryKey]
        int Id { get; }
        //[FriendKey(typeof(cms_Channels))]
        int ChannelId { get; set; }
        [SqlType("nvarchar(64)")]
        string Editor { get; set; }
        [SqlType("nvarchar(64)")]
        string Author { get; set; }
        [SqlType("nvarchar(256)")]
        string Source { get; set; }
        [SqlType("nvarchar(256)")]
        string Picture { get; set; }
        [SqlType("nvarchar(256)")]
        string Title { get; set; }
        [SqlType("ntext")]
        string Body { get; set; }
        DateTime UpdateTime { get; set; }
        DateTime CreateTime { get; set; }

        [PkQuery(LazyLoad = true)]
        cms_ArticleStatistics Statistics { get; set; }

        [CustomQuery("{Id} = @ChannelId", LazyLoad = true)]
        cms_Channels Channel { get; set; }
    }

    public interface cms_ArticleStatistics : cms_Statistics
    {
        [PkReverseQuery(LazyLoad = true)]
        cms_Articles Article { get; set; }
    }

    public interface cms_Statistics : Entity
    {
        [PrimaryKey]
        int ItemId { get; set; }
        int Day { get; set; }
        int DayClick { get; set; }
        int Week { get; set; }
        int WeekClick { get; set; }
        int Month { get; set; }
        int MonthClick { get; set; }
        int Year { get; set; }
        int YearClick { get; set; }
        int TotalClick { get; set; }
    }

    public interface cms_Channels : Entity
    {
        [PrimaryKey]
        int Id { get; }
        //[FriendKey(typeof(cms_Channels))]
        int ParentId { get; set; }
        int OrderNum { get; set; }
        int Depth { get; set; }
        [SqlType("nvarchar(64)")]
        string Dir { get; set; }
        [SqlType("nvarchar(128)")]
        string Title { get; set; }
        // 父频道
        //[FkReverseQuery(LazyLoad = true), MappingName("ParentId")]
        [CustomQuery("{Id} = @ParentId", LazyLoad = true)]
        cms_Channels Parent { get; set; }
        // 子频道
        [CustomQuery("{ParentId} = @Id", LazyLoad = true), SerializationIgnore]
        cms_Channels[] Childs { get; set; }
        // 文章
        [FkQuery("Channel", OrderBy = "{Id} DESC", Contained = false, LazyLoad = true)]
        cms_Articles[] Articles { get; set; }
    }

    public interface nb_PageParts : Entity
    {
        [PrimaryKey]
        int Id { get; }
        [SqlType("nvarchar(64)")]
        string Title { get; set; }
        [SqlType("ntext")]
        string Body { get; set; }
    }

    public interface m_User : Entity
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }

        [ManyToManyQuery(typeof(m_UserGroup))]
        m_Group[] Groups { get; set; }
    }

    public interface m_Group : Entity
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }

        [ManyToManyQuery(typeof(m_UserGroup), Contained = true)]
        m_User[] Users { get; set; }
    }

    [Relation]
    [MappingName("m_UserInGroups")]
    public interface m_UserGroup : Entity
    {
        [RelationKey(typeof(m_User))]
        int UserID { get; set; }
        [RelationKey(typeof(m_Group))]
        int GroupID { get; set; }
    }

    public interface home_Sorts : Entity
    {
        [PrimaryKey]
        int Id { get; }
        int OrderNum { get; set; }
        [SqlType("nvarchar(32)")]
        string Title { get; set; }
    }

    public interface home_PostSorts : home_Sorts
    {
        int ItemAmount { get; set; }
    }

    public interface Orders : Entity
    {
        [PrimaryKey]
        int ID { get;  }
        string Name { get; set; }
        [FkQuery("Order", Contained = true, LazyLoad = true)]
        OrderItem[] OrderItem { get; set; }
    }

    [OutputNamespace("Test.Entities")]
    //[AdditionalSqlScript("123", PreCleanSql="456")]
    //[AdditionalSqlScript("456", PreCleanSql="789")]
    public interface OrderItem : Entity
    {
        [PrimaryKey]
        int ID { get;  }
        string Name { get; set; }
        [FkReverseQuery(LazyLoad = true)]
        Orders Order { get; set;}
    }

    public interface SampleContract1
    {
        [PrimaryKey]
        int ID { get; }
        string Name { get; set; }
        [FkQuery("Parent", Contained=true)]
        SampleEntityWithContract[] Childs { get; set; }
        [FkReverseQuery]
        SampleEntityWithContract Parent { get; set; }
    }

    public interface SampleContract2
    {
        string Address { get; set; }
    }

    public interface SampleEntityWithContract : Entity, SampleContract1, SampleContract2
    {
    }
}
