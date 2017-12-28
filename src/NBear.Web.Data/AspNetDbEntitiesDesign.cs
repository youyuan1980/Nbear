using System;
using System.Collections.Generic;
using System.Text;
using NBear.Common.Design;

namespace NBear.Web.Data.AspNetDbEntitiesDesign
{
    [ReadOnly]
    [MappingName("aspnet_Applications")]
    public interface Application : Entity
    {
        [SqlType("nvarchar(256)")]
        string ApplicationName { get; set; }
        [SqlType("nvarchar(256)")]
        string LoweredApplicationName { get; set; }
        [PrimaryKey]
        Guid ApplicationId { get; set; }
        [SqlType("nvarchar(256)")]
        string Description { get; set; }
    }

    [ReadOnly]
    [MappingName("aspnet_Membership")]
    public interface Membership : Entity
    {
        [FkReverseQuery(LazyLoad = true)]
        [MappingName("ApplicationId")]
        Application Application { get; set; }
        [PrimaryKey]
        Guid UserId { get; set; }
        [SqlType("nvarchar(128)")]
        string Password { get; set; }
        int PasswordFormat { get; set; }
        [SqlType("nvarchar(128)")]
        string PasswordSalt { get; set; }
        [SqlType("nvarchar(16)")]
        string MobilePIN { get; set; }
        [SqlType("nvarchar(256)")]
        string Email { get; set; }
        [SqlType("nvarchar(256)")]
        string LoweredEmail { get; set; }
        [SqlType("nvarchar(256)")]
        string PasswordQuestion { get; set; }
        [SqlType("nvarchar(128)")]
        string PasswordAnswer { get; set; }
        bool IsApproved { get; set; }
        bool IsLockedOut { get; set; }
        DateTime CreateDate { get; set; }
        DateTime LastLoginDate { get; set; }
        DateTime LastPasswordChangedDate { get; set; }
        DateTime LastLockoutDate { get; set; }
        int FailedPasswordAttemptCount { get; set; }
        DateTime FailedPasswordAttemptWindowStart { get; set; }
        int FailedPasswordAnswerAttemptCount { get; set; }
        DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        [SqlType("ntext")]
        string Comment { get; set; }
    }

    [ReadOnly]
    [MappingName("aspnet_Profile")]
    public interface Profile : Entity
    {
        [PrimaryKey]
        Guid UserId { get; set; }
        [SqlType("ntext")]
        string PropertyNames { get; set; }
        [SqlType("ntext")]
        string PropertyValuesString { get; set; }
        byte[] PropertyValuesBinary { get; set; }
        DateTime LastUpdatedDate { get; set; }
    }

    [ReadOnly]
    [MappingName("aspnet_Roles")]
    public interface Role : Entity
    {
        [FkReverseQuery(LazyLoad = true)]
        [MappingName("ApplicationId")]
        Application Application { get; set; }
        [PrimaryKey]
        Guid RoleId { get; set; }
        [SqlType("nvarchar(256)")]
        string RoleName { get; set; }
        [SqlType("nvarchar(256)")]
        string LoweredRoleName { get; set; }
        [SqlType("nvarchar(256)")]
        string Description { get; set; }
    }

    [ReadOnly]
    [MappingName("aspnet_Users")]
    public interface User : Entity
    {
        [FkReverseQuery(LazyLoad = true)]
        [MappingName("ApplicationId")]
        Application Application { get; set; }
        [PrimaryKey]
        Guid UserId { get; set; }
        [SqlType("nvarchar(256)")]
        string UserName { get; set; }
        [SqlType("nvarchar(256)")]
        string LoweredUserName { get; set; }
        [SqlType("nvarchar(16)")]
        string MobileAlias { get; set; }
        bool IsAnonymous { get; set; }
        DateTime LastActivityDate { get; set; }

        [PkQuery(LazyLoad = true)]
        Profile Profile { get; set; }
    }

    [ReadOnly]
    [MappingName("aspnet_UsersInRoles")]
    public interface UserInRole : Entity
    {
        [PrimaryKey]
        Guid UserId { get; set; }
        [PrimaryKey]
        Guid RoleId { get; set; }
    }

    [ReadOnly]
    [MappingName("vw_aspnet_MembershipUsers")]
    public interface vMembershipUser : Entity
    {
        Guid UserId { get; }
        int PasswordFormat { get; }
        [SqlType("nvarchar(16)")]
        string MobilePIN { get; }
        [SqlType("nvarchar(256)")]
        string Email { get; }
        [SqlType("nvarchar(256)")]
        string LoweredEmail { get; }
        [SqlType("nvarchar(256)")]
        string PasswordQuestion { get; }
        [SqlType("nvarchar(128)")]
        string PasswordAnswer { get; }
        bool IsApproved { get; }
        bool IsLockedOut { get; }
        DateTime CreateDate { get; }
        DateTime LastLoginDate { get; }
        DateTime LastPasswordChangedDate { get; }
        DateTime LastLockoutDate { get; }
        int FailedPasswordAttemptCount { get; }
        DateTime FailedPasswordAttemptWindowStart { get; }
        int FailedPasswordAnswerAttemptCount { get; }
        DateTime FailedPasswordAnswerAttemptWindowStart { get; }
        [SqlType("ntext")]
        string Comment { get; }
        Guid ApplicationId { get; }
        [SqlType("nvarchar(256)")]
        string UserName { get; }
        [SqlType("nvarchar(16)")]
        string MobileAlias { get; }
        bool IsAnonymous { get; }
        DateTime LastActivityDate { get; }
    }
}
