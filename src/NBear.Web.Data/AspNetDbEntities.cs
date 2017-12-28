// IMPORTANT NOTICE: 
// You should never modify classes in this file manully. 
// To attach additional functions to entity classes, you should write partial classes in separate files.

using System;
using System.Xml.Serialization;
using NBear.Common;

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class ApplicationArrayList : EntityArrayList<Application> { }

    [Serializable]
    public partial class Application : Entity
    {
        protected static EntityConfiguration _ApplicationEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_ApplicationEntityConfiguration == null) _ApplicationEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.Application");
            return _ApplicationEntityConfiguration;
        }

        protected string _ApplicationName;
        protected string _LoweredApplicationName;
        protected Guid _ApplicationId;
        protected string _Description;

        public string ApplicationName
        {
            get { return _ApplicationName; }
            set { OnPropertyChanged("ApplicationName", _ApplicationName, value); _ApplicationName = value; }
        }

        public string LoweredApplicationName
        {
            get { return _LoweredApplicationName; }
            set { OnPropertyChanged("LoweredApplicationName", _LoweredApplicationName, value); _LoweredApplicationName = value; }
        }

        public Guid ApplicationId
        {
            get { return _ApplicationId; }
            set { OnPropertyChanged("ApplicationId", _ApplicationId, value); _ApplicationId = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { OnPropertyChanged("Description", _Description, value); _Description = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _ApplicationName, _LoweredApplicationName, _ApplicationId, _Description };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _ApplicationName = reader.GetString(0);
            if (!reader.IsDBNull(1)) _LoweredApplicationName = reader.GetString(1);
            if (!reader.IsDBNull(2)) _ApplicationId = GetGuid(reader, 2);
            if (!reader.IsDBNull(3)) _Description = reader.GetString(3);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _ApplicationName = (string)row[0];
            if (!row.IsNull(1)) _LoweredApplicationName = (string)row[1];
            if (!row.IsNull(2)) _ApplicationId = (Guid)GetGuid(row, 2);
            if (!row.IsNull(3)) _Description = (string)row[3];
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.Application left, global::NBear.Web.Data.AspNetDbEntities.Application right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.Application left, global::NBear.Web.Data.AspNetDbEntities.Application right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.Application)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.Application)obj).isAttached && this.ApplicationId == ((global::NBear.Web.Data.AspNetDbEntities.Application)obj).ApplicationId;
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem ApplicationName = new PropertyItem("ApplicationName");
            public static PropertyItem LoweredApplicationName = new PropertyItem("LoweredApplicationName");
            public static PropertyItem ApplicationId = new PropertyItem("ApplicationId");
            public static PropertyItem Description = new PropertyItem("Description");
        }

        #endregion
    }
}

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class MembershipArrayList : EntityArrayList<Membership> { }

    [Serializable]
    public partial class Membership : Entity
    {
        protected static EntityConfiguration _MembershipEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_MembershipEntityConfiguration == null) _MembershipEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.Membership");
            return _MembershipEntityConfiguration;
        }

        protected global::NBear.Web.Data.AspNetDbEntities.Application _Application;
        protected Guid? _Application_ApplicationId;
        protected Guid _UserId;
        protected string _Password;
        protected int _PasswordFormat;
        protected string _PasswordSalt;
        protected string _MobilePIN;
        protected string _Email;
        protected string _LoweredEmail;
        protected string _PasswordQuestion;
        protected string _PasswordAnswer;
        protected bool _IsApproved;
        protected bool _IsLockedOut;
        protected DateTime _CreateDate;
        protected DateTime _LastLoginDate;
        protected DateTime _LastPasswordChangedDate;
        protected DateTime _LastLockoutDate;
        protected int _FailedPasswordAttemptCount;
        protected DateTime _FailedPasswordAttemptWindowStart;
        protected int _FailedPasswordAnswerAttemptCount;
        protected DateTime _FailedPasswordAnswerAttemptWindowStart;
        protected string _Comment;

        public global::NBear.Web.Data.AspNetDbEntities.Application Application
        {
            get
            {
                if (!IsQueryPropertyLoaded("Application")) { _Application = (global::NBear.Web.Data.AspNetDbEntities.Application)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Application), "Application", this); }
                return _Application;
            }
            set { OnQueryOnePropertyChanged("Application", Application, value); _Application = value; if (value == null) { OnPropertyChanged("Application", _Application_ApplicationId, null); _Application_ApplicationId = null; } else { OnPropertyChanged("Application", _Application_ApplicationId, value.ApplicationId); _Application_ApplicationId = value.ApplicationId; } }
        }

        public Guid UserId
        {
            get { return _UserId; }
            set { OnPropertyChanged("UserId", _UserId, value); _UserId = value; }
        }

        public string Password
        {
            get { return _Password; }
            set { OnPropertyChanged("Password", _Password, value); _Password = value; }
        }

        public int PasswordFormat
        {
            get { return _PasswordFormat; }
            set { OnPropertyChanged("PasswordFormat", _PasswordFormat, value); _PasswordFormat = value; }
        }

        public string PasswordSalt
        {
            get { return _PasswordSalt; }
            set { OnPropertyChanged("PasswordSalt", _PasswordSalt, value); _PasswordSalt = value; }
        }

        public string MobilePIN
        {
            get { return _MobilePIN; }
            set { OnPropertyChanged("MobilePIN", _MobilePIN, value); _MobilePIN = value; }
        }

        public string Email
        {
            get { return _Email; }
            set { OnPropertyChanged("Email", _Email, value); _Email = value; }
        }

        public string LoweredEmail
        {
            get { return _LoweredEmail; }
            set { OnPropertyChanged("LoweredEmail", _LoweredEmail, value); _LoweredEmail = value; }
        }

        public string PasswordQuestion
        {
            get { return _PasswordQuestion; }
            set { OnPropertyChanged("PasswordQuestion", _PasswordQuestion, value); _PasswordQuestion = value; }
        }

        public string PasswordAnswer
        {
            get { return _PasswordAnswer; }
            set { OnPropertyChanged("PasswordAnswer", _PasswordAnswer, value); _PasswordAnswer = value; }
        }

        public bool IsApproved
        {
            get { return _IsApproved; }
            set { OnPropertyChanged("IsApproved", _IsApproved, value); _IsApproved = value; }
        }

        public bool IsLockedOut
        {
            get { return _IsLockedOut; }
            set { OnPropertyChanged("IsLockedOut", _IsLockedOut, value); _IsLockedOut = value; }
        }

        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { OnPropertyChanged("CreateDate", _CreateDate, value); _CreateDate = value; }
        }

        public DateTime LastLoginDate
        {
            get { return _LastLoginDate; }
            set { OnPropertyChanged("LastLoginDate", _LastLoginDate, value); _LastLoginDate = value; }
        }

        public DateTime LastPasswordChangedDate
        {
            get { return _LastPasswordChangedDate; }
            set { OnPropertyChanged("LastPasswordChangedDate", _LastPasswordChangedDate, value); _LastPasswordChangedDate = value; }
        }

        public DateTime LastLockoutDate
        {
            get { return _LastLockoutDate; }
            set { OnPropertyChanged("LastLockoutDate", _LastLockoutDate, value); _LastLockoutDate = value; }
        }

        public int FailedPasswordAttemptCount
        {
            get { return _FailedPasswordAttemptCount; }
            set { OnPropertyChanged("FailedPasswordAttemptCount", _FailedPasswordAttemptCount, value); _FailedPasswordAttemptCount = value; }
        }

        public DateTime FailedPasswordAttemptWindowStart
        {
            get { return _FailedPasswordAttemptWindowStart; }
            set { OnPropertyChanged("FailedPasswordAttemptWindowStart", _FailedPasswordAttemptWindowStart, value); _FailedPasswordAttemptWindowStart = value; }
        }

        public int FailedPasswordAnswerAttemptCount
        {
            get { return _FailedPasswordAnswerAttemptCount; }
            set { OnPropertyChanged("FailedPasswordAnswerAttemptCount", _FailedPasswordAnswerAttemptCount, value); _FailedPasswordAnswerAttemptCount = value; }
        }

        public DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get { return _FailedPasswordAnswerAttemptWindowStart; }
            set { OnPropertyChanged("FailedPasswordAnswerAttemptWindowStart", _FailedPasswordAnswerAttemptWindowStart, value); _FailedPasswordAnswerAttemptWindowStart = value; }
        }

        public string Comment
        {
            get { return _Comment; }
            set { OnPropertyChanged("Comment", _Comment, value); _Comment = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
            if (includeLazyLoadQueries || (!MetaDataManager.IsLazyLoad("NBear.Web.Data.AspNetDbEntities.Membership", "Application"))) { _Application = (global::NBear.Web.Data.AspNetDbEntities.Application)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Application), "Application", this); }
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _Application_ApplicationId, _UserId, _Password, _PasswordFormat, _PasswordSalt, _MobilePIN, _Email, _LoweredEmail, _PasswordQuestion, _PasswordAnswer, _IsApproved, _IsLockedOut, _CreateDate, _LastLoginDate, _LastPasswordChangedDate, _LastLockoutDate, _FailedPasswordAttemptCount, _FailedPasswordAttemptWindowStart, _FailedPasswordAnswerAttemptCount, _FailedPasswordAnswerAttemptWindowStart, _Comment };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _Application_ApplicationId = GetGuid(reader, 0);
            if (!reader.IsDBNull(1)) _UserId = GetGuid(reader, 1);
            if (!reader.IsDBNull(2)) _Password = reader.GetString(2);
            if (!reader.IsDBNull(3)) _PasswordFormat = reader.GetInt32(3);
            if (!reader.IsDBNull(4)) _PasswordSalt = reader.GetString(4);
            if (!reader.IsDBNull(5)) _MobilePIN = reader.GetString(5);
            if (!reader.IsDBNull(6)) _Email = reader.GetString(6);
            if (!reader.IsDBNull(7)) _LoweredEmail = reader.GetString(7);
            if (!reader.IsDBNull(8)) _PasswordQuestion = reader.GetString(8);
            if (!reader.IsDBNull(9)) _PasswordAnswer = reader.GetString(9);
            if (!reader.IsDBNull(10)) _IsApproved = reader.GetBoolean(10);
            if (!reader.IsDBNull(11)) _IsLockedOut = reader.GetBoolean(11);
            if (!reader.IsDBNull(12)) _CreateDate = reader.GetDateTime(12);
            if (!reader.IsDBNull(13)) _LastLoginDate = reader.GetDateTime(13);
            if (!reader.IsDBNull(14)) _LastPasswordChangedDate = reader.GetDateTime(14);
            if (!reader.IsDBNull(15)) _LastLockoutDate = reader.GetDateTime(15);
            if (!reader.IsDBNull(16)) _FailedPasswordAttemptCount = reader.GetInt32(16);
            if (!reader.IsDBNull(17)) _FailedPasswordAttemptWindowStart = reader.GetDateTime(17);
            if (!reader.IsDBNull(18)) _FailedPasswordAnswerAttemptCount = reader.GetInt32(18);
            if (!reader.IsDBNull(19)) _FailedPasswordAnswerAttemptWindowStart = reader.GetDateTime(19);
            if (!reader.IsDBNull(20)) _Comment = reader.GetString(20);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _Application_ApplicationId = (System.Guid)GetGuid(row, 0);
            if (!row.IsNull(1)) _UserId = (Guid)GetGuid(row, 1);
            if (!row.IsNull(2)) _Password = (string)row[2];
            if (!row.IsNull(3)) _PasswordFormat = (int)row[3];
            if (!row.IsNull(4)) _PasswordSalt = (string)row[4];
            if (!row.IsNull(5)) _MobilePIN = (string)row[5];
            if (!row.IsNull(6)) _Email = (string)row[6];
            if (!row.IsNull(7)) _LoweredEmail = (string)row[7];
            if (!row.IsNull(8)) _PasswordQuestion = (string)row[8];
            if (!row.IsNull(9)) _PasswordAnswer = (string)row[9];
            if (!row.IsNull(10)) _IsApproved = (bool)row[10];
            if (!row.IsNull(11)) _IsLockedOut = (bool)row[11];
            if (!row.IsNull(12)) _CreateDate = (DateTime)row[12];
            if (!row.IsNull(13)) _LastLoginDate = (DateTime)row[13];
            if (!row.IsNull(14)) _LastPasswordChangedDate = (DateTime)row[14];
            if (!row.IsNull(15)) _LastLockoutDate = (DateTime)row[15];
            if (!row.IsNull(16)) _FailedPasswordAttemptCount = (int)row[16];
            if (!row.IsNull(17)) _FailedPasswordAttemptWindowStart = (DateTime)row[17];
            if (!row.IsNull(18)) _FailedPasswordAnswerAttemptCount = (int)row[18];
            if (!row.IsNull(19)) _FailedPasswordAnswerAttemptWindowStart = (DateTime)row[19];
            if (!row.IsNull(20)) _Comment = (string)row[20];
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.Membership left, global::NBear.Web.Data.AspNetDbEntities.Membership right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.Membership left, global::NBear.Web.Data.AspNetDbEntities.Membership right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.Membership)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.Membership)obj).isAttached && this.UserId == ((global::NBear.Web.Data.AspNetDbEntities.Membership)obj).UserId;
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem ApplicationID = new PropertyItem("Application");
            public static PropertyItem UserId = new PropertyItem("UserId");
            public static PropertyItem Password = new PropertyItem("Password");
            public static PropertyItem PasswordFormat = new PropertyItem("PasswordFormat");
            public static PropertyItem PasswordSalt = new PropertyItem("PasswordSalt");
            public static PropertyItem MobilePIN = new PropertyItem("MobilePIN");
            public static PropertyItem Email = new PropertyItem("Email");
            public static PropertyItem LoweredEmail = new PropertyItem("LoweredEmail");
            public static PropertyItem PasswordQuestion = new PropertyItem("PasswordQuestion");
            public static PropertyItem PasswordAnswer = new PropertyItem("PasswordAnswer");
            public static PropertyItem IsApproved = new PropertyItem("IsApproved");
            public static PropertyItem IsLockedOut = new PropertyItem("IsLockedOut");
            public static PropertyItem CreateDate = new PropertyItem("CreateDate");
            public static PropertyItem LastLoginDate = new PropertyItem("LastLoginDate");
            public static PropertyItem LastPasswordChangedDate = new PropertyItem("LastPasswordChangedDate");
            public static PropertyItem LastLockoutDate = new PropertyItem("LastLockoutDate");
            public static PropertyItem FailedPasswordAttemptCount = new PropertyItem("FailedPasswordAttemptCount");
            public static PropertyItem FailedPasswordAttemptWindowStart = new PropertyItem("FailedPasswordAttemptWindowStart");
            public static PropertyItem FailedPasswordAnswerAttemptCount = new PropertyItem("FailedPasswordAnswerAttemptCount");
            public static PropertyItem FailedPasswordAnswerAttemptWindowStart = new PropertyItem("FailedPasswordAnswerAttemptWindowStart");
            public static PropertyItem Comment = new PropertyItem("Comment");
        }

        #endregion
    }
}

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class ProfileArrayList : EntityArrayList<Profile> { }

    [Serializable]
    public partial class Profile : Entity
    {
        protected static EntityConfiguration _ProfileEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_ProfileEntityConfiguration == null) _ProfileEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.Profile");
            return _ProfileEntityConfiguration;
        }

        protected Guid _UserId;
        protected string _PropertyNames;
        protected string _PropertyValuesString;
        protected byte[] _PropertyValuesBinary;
        protected DateTime _LastUpdatedDate;

        public Guid UserId
        {
            get { return _UserId; }
            set { OnPropertyChanged("UserId", _UserId, value); _UserId = value; }
        }

        public string PropertyNames
        {
            get { return _PropertyNames; }
            set { OnPropertyChanged("PropertyNames", _PropertyNames, value); _PropertyNames = value; }
        }

        public string PropertyValuesString
        {
            get { return _PropertyValuesString; }
            set { OnPropertyChanged("PropertyValuesString", _PropertyValuesString, value); _PropertyValuesString = value; }
        }

        public byte[] PropertyValuesBinary
        {
            get { return _PropertyValuesBinary; }
            set { OnPropertyChanged("PropertyValuesBinary", _PropertyValuesBinary, value); _PropertyValuesBinary = value; }
        }

        public DateTime LastUpdatedDate
        {
            get { return _LastUpdatedDate; }
            set { OnPropertyChanged("LastUpdatedDate", _LastUpdatedDate, value); _LastUpdatedDate = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _UserId, _PropertyNames, _PropertyValuesString, _PropertyValuesBinary, _LastUpdatedDate };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _UserId = GetGuid(reader, 0);
            if (!reader.IsDBNull(1)) _PropertyNames = reader.GetString(1);
            if (!reader.IsDBNull(2)) _PropertyValuesString = reader.GetString(2);
            if (!reader.IsDBNull(3)) _PropertyValuesBinary = (byte[])reader.GetValue(3);
            if (!reader.IsDBNull(4)) _LastUpdatedDate = reader.GetDateTime(4);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _UserId = (Guid)GetGuid(row, 0);
            if (!row.IsNull(1)) _PropertyNames = (string)row[1];
            if (!row.IsNull(2)) _PropertyValuesString = (string)row[2];
            if (!row.IsNull(3)) _PropertyValuesBinary = (byte[])row[3];
            if (!row.IsNull(4)) _LastUpdatedDate = (DateTime)row[4];
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.Profile left, global::NBear.Web.Data.AspNetDbEntities.Profile right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.Profile left, global::NBear.Web.Data.AspNetDbEntities.Profile right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.Profile)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.Profile)obj).isAttached && this.UserId == ((global::NBear.Web.Data.AspNetDbEntities.Profile)obj).UserId;
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem UserId = new PropertyItem("UserId");
            public static PropertyItem PropertyNames = new PropertyItem("PropertyNames");
            public static PropertyItem PropertyValuesString = new PropertyItem("PropertyValuesString");
            public static PropertyItem PropertyValuesBinary = new PropertyItem("PropertyValuesBinary");
            public static PropertyItem LastUpdatedDate = new PropertyItem("LastUpdatedDate");
        }

        #endregion
    }
}

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class RoleArrayList : EntityArrayList<Role> { }

    [Serializable]
    public partial class Role : Entity
    {
        protected static EntityConfiguration _RoleEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_RoleEntityConfiguration == null) _RoleEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.Role");
            return _RoleEntityConfiguration;
        }

        protected global::NBear.Web.Data.AspNetDbEntities.Application _Application;
        protected Guid? _Application_ApplicationId;
        protected Guid _RoleId;
        protected string _RoleName;
        protected string _LoweredRoleName;
        protected string _Description;

        public global::NBear.Web.Data.AspNetDbEntities.Application Application
        {
            get
            {
                if (!IsQueryPropertyLoaded("Application")) { _Application = (global::NBear.Web.Data.AspNetDbEntities.Application)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Application), "Application", this); }
                return _Application;
            }
            set { OnQueryOnePropertyChanged("Application", Application, value); _Application = value; if (value == null) { OnPropertyChanged("Application", _Application_ApplicationId, null); _Application_ApplicationId = null; } else { OnPropertyChanged("Application", _Application_ApplicationId, value.ApplicationId); _Application_ApplicationId = value.ApplicationId; } }
        }

        public Guid RoleId
        {
            get { return _RoleId; }
            set { OnPropertyChanged("RoleId", _RoleId, value); _RoleId = value; }
        }

        public string RoleName
        {
            get { return _RoleName; }
            set { OnPropertyChanged("RoleName", _RoleName, value); _RoleName = value; }
        }

        public string LoweredRoleName
        {
            get { return _LoweredRoleName; }
            set { OnPropertyChanged("LoweredRoleName", _LoweredRoleName, value); _LoweredRoleName = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { OnPropertyChanged("Description", _Description, value); _Description = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
            if (includeLazyLoadQueries || (!MetaDataManager.IsLazyLoad("NBear.Web.Data.AspNetDbEntities.Role", "Application"))) { _Application = (global::NBear.Web.Data.AspNetDbEntities.Application)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Application), "Application", this); }
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _Application_ApplicationId, _RoleId, _RoleName, _LoweredRoleName, _Description };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _Application_ApplicationId = GetGuid(reader, 0);
            if (!reader.IsDBNull(1)) _RoleId = GetGuid(reader, 1);
            if (!reader.IsDBNull(2)) _RoleName = reader.GetString(2);
            if (!reader.IsDBNull(3)) _LoweredRoleName = reader.GetString(3);
            if (!reader.IsDBNull(4)) _Description = reader.GetString(4);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _Application_ApplicationId = (System.Guid)GetGuid(row, 0);
            if (!row.IsNull(1)) _RoleId = (Guid)GetGuid(row, 1);
            if (!row.IsNull(2)) _RoleName = (string)row[2];
            if (!row.IsNull(3)) _LoweredRoleName = (string)row[3];
            if (!row.IsNull(4)) _Description = (string)row[4];
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.Role left, global::NBear.Web.Data.AspNetDbEntities.Role right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.Role left, global::NBear.Web.Data.AspNetDbEntities.Role right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.Role)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.Role)obj).isAttached && this.RoleId == ((global::NBear.Web.Data.AspNetDbEntities.Role)obj).RoleId;
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem ApplicationID = new PropertyItem("Application");
            public static PropertyItem RoleId = new PropertyItem("RoleId");
            public static PropertyItem RoleName = new PropertyItem("RoleName");
            public static PropertyItem LoweredRoleName = new PropertyItem("LoweredRoleName");
            public static PropertyItem Description = new PropertyItem("Description");
        }

        #endregion
    }
}

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class UserArrayList : EntityArrayList<User> { }

    [Serializable]
    public partial class User : Entity
    {
        protected static EntityConfiguration _UserEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_UserEntityConfiguration == null) _UserEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.User");
            return _UserEntityConfiguration;
        }

        protected global::NBear.Web.Data.AspNetDbEntities.Application _Application;
        protected Guid? _Application_ApplicationId;
        protected Guid _UserId;
        protected string _UserName;
        protected string _LoweredUserName;
        protected string _MobileAlias;
        protected bool _IsAnonymous;
        protected DateTime _LastActivityDate;
        protected global::NBear.Web.Data.AspNetDbEntities.Profile _Profile;

        public global::NBear.Web.Data.AspNetDbEntities.Application Application
        {
            get
            {
                if (!IsQueryPropertyLoaded("Application")) { _Application = (global::NBear.Web.Data.AspNetDbEntities.Application)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Application), "Application", this); }
                return _Application;
            }
            set { OnQueryOnePropertyChanged("Application", Application, value); _Application = value; if (value == null) { OnPropertyChanged("Application", _Application_ApplicationId, null); _Application_ApplicationId = null; } else { OnPropertyChanged("Application", _Application_ApplicationId, value.ApplicationId); _Application_ApplicationId = value.ApplicationId; } }
        }

        public Guid UserId
        {
            get { return _UserId; }
            set { OnPropertyChanged("UserId", _UserId, value); _UserId = value; }
        }

        public string UserName
        {
            get { return _UserName; }
            set { OnPropertyChanged("UserName", _UserName, value); _UserName = value; }
        }

        public string LoweredUserName
        {
            get { return _LoweredUserName; }
            set { OnPropertyChanged("LoweredUserName", _LoweredUserName, value); _LoweredUserName = value; }
        }

        public string MobileAlias
        {
            get { return _MobileAlias; }
            set { OnPropertyChanged("MobileAlias", _MobileAlias, value); _MobileAlias = value; }
        }

        public bool IsAnonymous
        {
            get { return _IsAnonymous; }
            set { OnPropertyChanged("IsAnonymous", _IsAnonymous, value); _IsAnonymous = value; }
        }

        public DateTime LastActivityDate
        {
            get { return _LastActivityDate; }
            set { OnPropertyChanged("LastActivityDate", _LastActivityDate, value); _LastActivityDate = value; }
        }

        public global::NBear.Web.Data.AspNetDbEntities.Profile Profile
        {
            get
            {
                if (!IsQueryPropertyLoaded("Profile")) { _Profile = (global::NBear.Web.Data.AspNetDbEntities.Profile)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Profile), "Profile", this); }
                return _Profile;
            }
            set { OnQueryOnePropertyChanged("Profile", Profile, value); _Profile = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
            if (includeLazyLoadQueries || (!MetaDataManager.IsLazyLoad("NBear.Web.Data.AspNetDbEntities.User", "Application"))) { _Application = (global::NBear.Web.Data.AspNetDbEntities.Application)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Application), "Application", this); }
            if (includeLazyLoadQueries || (!MetaDataManager.IsLazyLoad("NBear.Web.Data.AspNetDbEntities.User", "Profile"))) { _Profile = (global::NBear.Web.Data.AspNetDbEntities.Profile)QueryOne(typeof(global::NBear.Web.Data.AspNetDbEntities.Profile), "Profile", this); }
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _Application_ApplicationId, _UserId, _UserName, _LoweredUserName, _MobileAlias, _IsAnonymous, _LastActivityDate };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _Application_ApplicationId = GetGuid(reader, 0);
            if (!reader.IsDBNull(1)) _UserId = GetGuid(reader, 1);
            if (!reader.IsDBNull(2)) _UserName = reader.GetString(2);
            if (!reader.IsDBNull(3)) _LoweredUserName = reader.GetString(3);
            if (!reader.IsDBNull(4)) _MobileAlias = reader.GetString(4);
            if (!reader.IsDBNull(5)) _IsAnonymous = reader.GetBoolean(5);
            if (!reader.IsDBNull(6)) _LastActivityDate = reader.GetDateTime(6);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _Application_ApplicationId = (System.Guid)GetGuid(row, 0);
            if (!row.IsNull(1)) _UserId = (Guid)GetGuid(row, 1);
            if (!row.IsNull(2)) _UserName = (string)row[2];
            if (!row.IsNull(3)) _LoweredUserName = (string)row[3];
            if (!row.IsNull(4)) _MobileAlias = (string)row[4];
            if (!row.IsNull(5)) _IsAnonymous = (bool)row[5];
            if (!row.IsNull(6)) _LastActivityDate = (DateTime)row[6];
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.User left, global::NBear.Web.Data.AspNetDbEntities.User right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.User left, global::NBear.Web.Data.AspNetDbEntities.User right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.User)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.User)obj).isAttached && this.UserId == ((global::NBear.Web.Data.AspNetDbEntities.User)obj).UserId;
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem ApplicationID = new PropertyItem("Application");
            public static PropertyItem UserId = new PropertyItem("UserId");
            public static PropertyItem UserName = new PropertyItem("UserName");
            public static PropertyItem LoweredUserName = new PropertyItem("LoweredUserName");
            public static PropertyItem MobileAlias = new PropertyItem("MobileAlias");
            public static PropertyItem IsAnonymous = new PropertyItem("IsAnonymous");
            public static PropertyItem LastActivityDate = new PropertyItem("LastActivityDate");
        }

        #endregion
    }
}

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class UserInRoleArrayList : EntityArrayList<UserInRole> { }

    [Serializable]
    public partial class UserInRole : Entity
    {
        protected static EntityConfiguration _UserInRoleEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_UserInRoleEntityConfiguration == null) _UserInRoleEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.UserInRole");
            return _UserInRoleEntityConfiguration;
        }

        protected Guid _UserId;
        protected Guid _RoleId;

        public Guid UserId
        {
            get { return _UserId; }
            set { OnPropertyChanged("UserId", _UserId, value); _UserId = value; }
        }

        public Guid RoleId
        {
            get { return _RoleId; }
            set { OnPropertyChanged("RoleId", _RoleId, value); _RoleId = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _UserId, _RoleId };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _UserId = GetGuid(reader, 0);
            if (!reader.IsDBNull(1)) _RoleId = GetGuid(reader, 1);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _UserId = (Guid)GetGuid(row, 0);
            if (!row.IsNull(1)) _RoleId = (Guid)GetGuid(row, 1);
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.UserInRole left, global::NBear.Web.Data.AspNetDbEntities.UserInRole right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.UserInRole left, global::NBear.Web.Data.AspNetDbEntities.UserInRole right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.UserInRole)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.UserInRole)obj).isAttached && this.UserId == ((global::NBear.Web.Data.AspNetDbEntities.UserInRole)obj).UserId && this.RoleId == ((global::NBear.Web.Data.AspNetDbEntities.UserInRole)obj).RoleId;
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem UserId = new PropertyItem("UserId");
            public static PropertyItem RoleId = new PropertyItem("RoleId");
        }

        #endregion
    }
}

namespace NBear.Web.Data.AspNetDbEntities
{
    [Serializable]
    public partial class vMembershipUserArrayList : EntityArrayList<vMembershipUser> { }

    [Serializable]
    public partial class vMembershipUser : Entity
    {
        protected static EntityConfiguration _vMembershipUserEntityConfiguration;

        public override EntityConfiguration GetEntityConfiguration()
        {
            if (_vMembershipUserEntityConfiguration == null) _vMembershipUserEntityConfiguration = MetaDataManager.GetEntityConfiguration("NBear.Web.Data.AspNetDbEntities.vMembershipUser");
            return _vMembershipUserEntityConfiguration;
        }

        protected Guid _UserId;
        protected int _PasswordFormat;
        protected string _MobilePIN;
        protected string _Email;
        protected string _LoweredEmail;
        protected string _PasswordQuestion;
        protected string _PasswordAnswer;
        protected bool _IsApproved;
        protected bool _IsLockedOut;
        protected DateTime _CreateDate;
        protected DateTime _LastLoginDate;
        protected DateTime _LastPasswordChangedDate;
        protected DateTime _LastLockoutDate;
        protected int _FailedPasswordAttemptCount;
        protected DateTime _FailedPasswordAttemptWindowStart;
        protected int _FailedPasswordAnswerAttemptCount;
        protected DateTime _FailedPasswordAnswerAttemptWindowStart;
        protected string _Comment;
        protected Guid _ApplicationId;
        protected string _UserName;
        protected string _MobileAlias;
        protected bool _IsAnonymous;
        protected DateTime _LastActivityDate;

        public Guid UserId
        {
            get { return _UserId; }
            set { OnPropertyChanged("UserId", _UserId, value); _UserId = value; }
        }

        public int PasswordFormat
        {
            get { return _PasswordFormat; }
            set { OnPropertyChanged("PasswordFormat", _PasswordFormat, value); _PasswordFormat = value; }
        }

        public string MobilePIN
        {
            get { return _MobilePIN; }
            set { OnPropertyChanged("MobilePIN", _MobilePIN, value); _MobilePIN = value; }
        }

        public string Email
        {
            get { return _Email; }
            set { OnPropertyChanged("Email", _Email, value); _Email = value; }
        }

        public string LoweredEmail
        {
            get { return _LoweredEmail; }
            set { OnPropertyChanged("LoweredEmail", _LoweredEmail, value); _LoweredEmail = value; }
        }

        public string PasswordQuestion
        {
            get { return _PasswordQuestion; }
            set { OnPropertyChanged("PasswordQuestion", _PasswordQuestion, value); _PasswordQuestion = value; }
        }

        public string PasswordAnswer
        {
            get { return _PasswordAnswer; }
            set { OnPropertyChanged("PasswordAnswer", _PasswordAnswer, value); _PasswordAnswer = value; }
        }

        public bool IsApproved
        {
            get { return _IsApproved; }
            set { OnPropertyChanged("IsApproved", _IsApproved, value); _IsApproved = value; }
        }

        public bool IsLockedOut
        {
            get { return _IsLockedOut; }
            set { OnPropertyChanged("IsLockedOut", _IsLockedOut, value); _IsLockedOut = value; }
        }

        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { OnPropertyChanged("CreateDate", _CreateDate, value); _CreateDate = value; }
        }

        public DateTime LastLoginDate
        {
            get { return _LastLoginDate; }
            set { OnPropertyChanged("LastLoginDate", _LastLoginDate, value); _LastLoginDate = value; }
        }

        public DateTime LastPasswordChangedDate
        {
            get { return _LastPasswordChangedDate; }
            set { OnPropertyChanged("LastPasswordChangedDate", _LastPasswordChangedDate, value); _LastPasswordChangedDate = value; }
        }

        public DateTime LastLockoutDate
        {
            get { return _LastLockoutDate; }
            set { OnPropertyChanged("LastLockoutDate", _LastLockoutDate, value); _LastLockoutDate = value; }
        }

        public int FailedPasswordAttemptCount
        {
            get { return _FailedPasswordAttemptCount; }
            set { OnPropertyChanged("FailedPasswordAttemptCount", _FailedPasswordAttemptCount, value); _FailedPasswordAttemptCount = value; }
        }

        public DateTime FailedPasswordAttemptWindowStart
        {
            get { return _FailedPasswordAttemptWindowStart; }
            set { OnPropertyChanged("FailedPasswordAttemptWindowStart", _FailedPasswordAttemptWindowStart, value); _FailedPasswordAttemptWindowStart = value; }
        }

        public int FailedPasswordAnswerAttemptCount
        {
            get { return _FailedPasswordAnswerAttemptCount; }
            set { OnPropertyChanged("FailedPasswordAnswerAttemptCount", _FailedPasswordAnswerAttemptCount, value); _FailedPasswordAnswerAttemptCount = value; }
        }

        public DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get { return _FailedPasswordAnswerAttemptWindowStart; }
            set { OnPropertyChanged("FailedPasswordAnswerAttemptWindowStart", _FailedPasswordAnswerAttemptWindowStart, value); _FailedPasswordAnswerAttemptWindowStart = value; }
        }

        public string Comment
        {
            get { return _Comment; }
            set { OnPropertyChanged("Comment", _Comment, value); _Comment = value; }
        }

        public Guid ApplicationId
        {
            get { return _ApplicationId; }
            set { OnPropertyChanged("ApplicationId", _ApplicationId, value); _ApplicationId = value; }
        }

        public string UserName
        {
            get { return _UserName; }
            set { OnPropertyChanged("UserName", _UserName, value); _UserName = value; }
        }

        public string MobileAlias
        {
            get { return _MobileAlias; }
            set { OnPropertyChanged("MobileAlias", _MobileAlias, value); _MobileAlias = value; }
        }

        public bool IsAnonymous
        {
            get { return _IsAnonymous; }
            set { OnPropertyChanged("IsAnonymous", _IsAnonymous, value); _IsAnonymous = value; }
        }

        public DateTime LastActivityDate
        {
            get { return _LastActivityDate; }
            set { OnPropertyChanged("LastActivityDate", _LastActivityDate, value); _LastActivityDate = value; }
        }

        #region Get & Set PropertyValues

        public override void ReloadQueries(bool includeLazyLoadQueries)
        {
        }

        public override object[] GetPropertyValues()
        {
            return new object[] { _UserId, _PasswordFormat, _MobilePIN, _Email, _LoweredEmail, _PasswordQuestion, _PasswordAnswer, _IsApproved, _IsLockedOut, _CreateDate, _LastLoginDate, _LastPasswordChangedDate, _LastLockoutDate, _FailedPasswordAttemptCount, _FailedPasswordAttemptWindowStart, _FailedPasswordAnswerAttemptCount, _FailedPasswordAnswerAttemptWindowStart, _Comment, _ApplicationId, _UserName, _MobileAlias, _IsAnonymous, _LastActivityDate };
        }

        public override void SetPropertyValues(System.Data.IDataReader reader)
        {
            if (!reader.IsDBNull(0)) _UserId = GetGuid(reader, 0);
            if (!reader.IsDBNull(1)) _PasswordFormat = reader.GetInt32(1);
            if (!reader.IsDBNull(2)) _MobilePIN = reader.GetString(2);
            if (!reader.IsDBNull(3)) _Email = reader.GetString(3);
            if (!reader.IsDBNull(4)) _LoweredEmail = reader.GetString(4);
            if (!reader.IsDBNull(5)) _PasswordQuestion = reader.GetString(5);
            if (!reader.IsDBNull(6)) _PasswordAnswer = reader.GetString(6);
            if (!reader.IsDBNull(7)) _IsApproved = reader.GetBoolean(7);
            if (!reader.IsDBNull(8)) _IsLockedOut = reader.GetBoolean(8);
            if (!reader.IsDBNull(9)) _CreateDate = reader.GetDateTime(9);
            if (!reader.IsDBNull(10)) _LastLoginDate = reader.GetDateTime(10);
            if (!reader.IsDBNull(11)) _LastPasswordChangedDate = reader.GetDateTime(11);
            if (!reader.IsDBNull(12)) _LastLockoutDate = reader.GetDateTime(12);
            if (!reader.IsDBNull(13)) _FailedPasswordAttemptCount = reader.GetInt32(13);
            if (!reader.IsDBNull(14)) _FailedPasswordAttemptWindowStart = reader.GetDateTime(14);
            if (!reader.IsDBNull(15)) _FailedPasswordAnswerAttemptCount = reader.GetInt32(15);
            if (!reader.IsDBNull(16)) _FailedPasswordAnswerAttemptWindowStart = reader.GetDateTime(16);
            if (!reader.IsDBNull(17)) _Comment = reader.GetString(17);
            if (!reader.IsDBNull(18)) _ApplicationId = GetGuid(reader, 18);
            if (!reader.IsDBNull(19)) _UserName = reader.GetString(19);
            if (!reader.IsDBNull(20)) _MobileAlias = reader.GetString(20);
            if (!reader.IsDBNull(21)) _IsAnonymous = reader.GetBoolean(21);
            if (!reader.IsDBNull(22)) _LastActivityDate = reader.GetDateTime(22);
            ReloadQueries(false);
        }

        public override void SetPropertyValues(System.Data.DataRow row)
        {
            if (!row.IsNull(0)) _UserId = (Guid)GetGuid(row, 0);
            if (!row.IsNull(1)) _PasswordFormat = (int)row[1];
            if (!row.IsNull(2)) _MobilePIN = (string)row[2];
            if (!row.IsNull(3)) _Email = (string)row[3];
            if (!row.IsNull(4)) _LoweredEmail = (string)row[4];
            if (!row.IsNull(5)) _PasswordQuestion = (string)row[5];
            if (!row.IsNull(6)) _PasswordAnswer = (string)row[6];
            if (!row.IsNull(7)) _IsApproved = (bool)row[7];
            if (!row.IsNull(8)) _IsLockedOut = (bool)row[8];
            if (!row.IsNull(9)) _CreateDate = (DateTime)row[9];
            if (!row.IsNull(10)) _LastLoginDate = (DateTime)row[10];
            if (!row.IsNull(11)) _LastPasswordChangedDate = (DateTime)row[11];
            if (!row.IsNull(12)) _LastLockoutDate = (DateTime)row[12];
            if (!row.IsNull(13)) _FailedPasswordAttemptCount = (int)row[13];
            if (!row.IsNull(14)) _FailedPasswordAttemptWindowStart = (DateTime)row[14];
            if (!row.IsNull(15)) _FailedPasswordAnswerAttemptCount = (int)row[15];
            if (!row.IsNull(16)) _FailedPasswordAnswerAttemptWindowStart = (DateTime)row[16];
            if (!row.IsNull(17)) _Comment = (string)row[17];
            if (!row.IsNull(18)) _ApplicationId = (Guid)GetGuid(row, 18);
            if (!row.IsNull(19)) _UserName = (string)row[19];
            if (!row.IsNull(20)) _MobileAlias = (string)row[20];
            if (!row.IsNull(21)) _IsAnonymous = (bool)row[21];
            if (!row.IsNull(22)) _LastActivityDate = (DateTime)row[22];
            ReloadQueries(false);
        }

        #endregion

        #region Equals

        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(global::NBear.Web.Data.AspNetDbEntities.vMembershipUser left, global::NBear.Web.Data.AspNetDbEntities.vMembershipUser right) { return ((object)left) != null ? left.Equals(right) : ((object)right) == null ? true : false; }

        public static bool operator !=(global::NBear.Web.Data.AspNetDbEntities.vMembershipUser left, global::NBear.Web.Data.AspNetDbEntities.vMembershipUser right) { return ((object)left) != null ? !left.Equals(right) : ((object)right) == null ? false : true; }

        public override bool Equals(object obj)
        {
            return obj == null || (!(obj is global::NBear.Web.Data.AspNetDbEntities.vMembershipUser)) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((global::NBear.Web.Data.AspNetDbEntities.vMembershipUser)obj).isAttached && base.Equals(obj);
        }

        #endregion

        #region QueryCode

        public abstract class _
        {
            private _() { }

            public static PropertyItem UserId = new PropertyItem("UserId");
            public static PropertyItem PasswordFormat = new PropertyItem("PasswordFormat");
            public static PropertyItem MobilePIN = new PropertyItem("MobilePIN");
            public static PropertyItem Email = new PropertyItem("Email");
            public static PropertyItem LoweredEmail = new PropertyItem("LoweredEmail");
            public static PropertyItem PasswordQuestion = new PropertyItem("PasswordQuestion");
            public static PropertyItem PasswordAnswer = new PropertyItem("PasswordAnswer");
            public static PropertyItem IsApproved = new PropertyItem("IsApproved");
            public static PropertyItem IsLockedOut = new PropertyItem("IsLockedOut");
            public static PropertyItem CreateDate = new PropertyItem("CreateDate");
            public static PropertyItem LastLoginDate = new PropertyItem("LastLoginDate");
            public static PropertyItem LastPasswordChangedDate = new PropertyItem("LastPasswordChangedDate");
            public static PropertyItem LastLockoutDate = new PropertyItem("LastLockoutDate");
            public static PropertyItem FailedPasswordAttemptCount = new PropertyItem("FailedPasswordAttemptCount");
            public static PropertyItem FailedPasswordAttemptWindowStart = new PropertyItem("FailedPasswordAttemptWindowStart");
            public static PropertyItem FailedPasswordAnswerAttemptCount = new PropertyItem("FailedPasswordAnswerAttemptCount");
            public static PropertyItem FailedPasswordAnswerAttemptWindowStart = new PropertyItem("FailedPasswordAnswerAttemptWindowStart");
            public static PropertyItem Comment = new PropertyItem("Comment");
            public static PropertyItem ApplicationId = new PropertyItem("ApplicationId");
            public static PropertyItem UserName = new PropertyItem("UserName");
            public static PropertyItem MobileAlias = new PropertyItem("MobileAlias");
            public static PropertyItem IsAnonymous = new PropertyItem("IsAnonymous");
            public static PropertyItem LastActivityDate = new PropertyItem("LastActivityDate");
        }

        #endregion
    }
}

#region Extended Code

namespace NBear.Web.Data.AspNetDbEntities
{
    internal abstract class AspNetDb
    {
        private AspNetDb()
        {
        }

        static AspNetDb()
        {
            try
            {
                //load entities config from embed resource
                System.IO.Stream stream = typeof(AspNetDb).Assembly.GetManifestResourceStream("NBear.Web.Data.AspNetDbEntitiesConfig.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(EntityConfiguration[]));
                EntityConfiguration[] objs = serializer.Deserialize(stream) as EntityConfiguration[];
                MetaDataManager.AddEntityConfigurations(objs);
                MetaDataManager.ParseNonRelatedEntities();
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }

        internal static void InitAspNetDbEntities()
        {
            //null is OK, init details done in static constructor
        }
    }

    public partial class Application : Entity
    {
        static Application()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }

    public partial class Membership : Entity
    {
        static Membership()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }

    public partial class Profile : Entity
    {
        static Profile()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }

    public partial class Role : Entity
    {
        static Role()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }

    public partial class User : Entity
    {
        static User()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }

    public partial class UserInRole : Entity
    {
        static UserInRole()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }

    public partial class vMembershipUser : Entity
    {
        static vMembershipUser()
        {
            AspNetDb.InitAspNetDbEntities();
        }
    }
}

#endregion