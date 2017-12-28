//using System;
//using System.Data;
//using System.Collections.Generic;
//using NBear.Common;

//namespace NBear.Test.UnitTests.temp.EntitySampleTemp1_impl
//{
//    [Serializable]
//    public partial class UserGroup : Entity
//    {
//        protected int _ID;
//        protected string _Name;
//        protected string _Comment;
//        protected User[] _Users;
//        protected User _FirstUser;

//        public override void ReloadQueries(bool includeLazyLoadQueries)
//        {
//        }

//        public UserGroup() : base()
//        {
//            _FirstUser = (User)QueryOne(typeof(User), "FirstUser", null, null, this);
//        }

//        public int ID
//        {
//            get { return _ID; }
//            set { OnPropertyChanged("ID", _ID, value); _ID = value; }
//        }

//        public string Name
//        {
//            get { return _Name; }
//            set { OnPropertyChanged("Name", _Name, value); _Name = value; }
//        }

//        public string Comment
//        {
//            get { return _Comment; }
//            set { OnPropertyChanged("Commnet", _Comment, value); _Comment = value; }
//        }

//        public User[] Users
//        {
//            get 
//            {
//                if (_Users == null) _Users = (User[])Query(typeof(User), "Users", null, null, this);
//                return _Users;
//            }
//        }

//        public User FirstUser
//        {
//            get { return _FirstUser; }
//        }

//        #region Get & Set PropertyValues

//        public override object[] GetPropertyValues()
//        {
//            return new object[] { _ID, _Name, _Comment };
//        }

//        public override void SetPropertyValues(IDataReader reader)
//        {
//            _ID = reader.GetInt32(0);
//            _Name = reader.GetString(1);
//            _Comment = reader.GetString(2);
//        }

//        public override void SetPropertyValues(DataRow row)
//        {
//            _ID = (int)row[0];
//            _Name = (string)row[1];
//            _Comment = (string)row[2];
//        }

//        #endregion

//        #region QueryCode

//        public abstract class _
//        {
//            private _() { }

//            public static PropertyItem ID = new PropertyItem("ID");
//            public static PropertyItem Name = new PropertyItem("Name");
//            public static PropertyItem Commnet = new PropertyItem("Commnet");
//        }

//        #endregion
//    }

//    public partial class User : Entity
//    {
//        protected int _ID;
//        protected string _Name;

//        public override void ReloadQueries(bool includeLazyLoadQueries)
//        {
//        }

//        public int ID
//        {
//            get { return _ID; }
//            set { OnPropertyChanged("ID", _ID, value); _ID = value; }
//        }

//        public string Name
//        {
//            get { return _Name; }
//            set { OnPropertyChanged("Name", _Name, value); _Name = value; }
//        }

//        #region Get & Set PropertyValues

//        public override object[] GetPropertyValues()
//        {
//            return new object[] { _ID, _Name };
//        }

//        public override void SetPropertyValues(IDataReader reader)
//        {
//            _ID = reader.GetInt32(0);
//            _Name = reader.GetString(1);
//        }

//        public override void SetPropertyValues(DataRow row)
//        {
//            _ID = (int)row[0];
//            _Name = (string)row[1];
//        }

//        #endregion

//        #region QueryCode

//        public abstract class _
//        {
//            private _() { }

//            public static PropertyItem ID = new PropertyItem("ID");
//            public static PropertyItem Name = new PropertyItem("Name");
//        }

//        #endregion
//    }

//    public partial class GhostUser : User
//    {
//    }

//    public partial class LocalUser : User
//    {
//        protected string _LoginID;
//        protected string _Password;

//        public string LoginID
//        {
//            get { return _LoginID; }
//            set { OnPropertyChanged("LoginID", _LoginID, value); _LoginID = value; }
//        }

//        public string Password
//        {
//            get { return _Password; }
//            set { OnPropertyChanged("Password", _Password, value); _Password = value; }
//        }

//        #region Get & Set PropertyValues

//        public override object[] GetPropertyValues()
//        {
//            return new object[] { _ID, _Name, _LoginID, _Password };
//        }

//        public override void SetPropertyValues(IDataReader reader)
//        {
//            _ID = reader.GetInt32(0);
//            _Name = reader.GetString(1);
//            _LoginID = reader.GetString(2);
//            _Password = reader.GetString(3);
//        }

//        public override void SetPropertyValues(DataRow row)
//        {
//            _ID = (int)row[0];
//            _Name = (string)row[1];
//            _LoginID = (string)row[2];
//            _Password = (string)row[3];
//        }

//        #endregion

//        #region QueryCode

//        public new abstract class _
//        {
//            private _() { }

//            public static PropertyItem ID = new PropertyItem("ID");
//            public static PropertyItem Name = new PropertyItem("Name");
//            public static PropertyItem LoginID = new PropertyItem("LoginID");
//            public static PropertyItem Password = new PropertyItem("Password");
//        }

//        #endregion
//    }

//    public partial class AgentUser : User
//    {
//        protected string _LoginID;

//        public string LoginID
//        {
//            get { return _LoginID; }
//            set { OnPropertyChanged("LoginID", _LoginID, value); _LoginID = value; }
//        }

//        #region Get & Set PropertyValues

//        public override object[] GetPropertyValues()
//        {
//            return new object[] { _ID, _Name, _LoginID };
//        }

//        public override void SetPropertyValues(IDataReader reader)
//        {
//            _ID = reader.GetInt32(0);
//            _Name = reader.GetString(1);
//            _LoginID = reader.GetString(2);
//        }

//        public override void SetPropertyValues(DataRow row)
//        {
//            _ID = (int)row[0];
//            _Name = (string)row[1];
//            _LoginID = (string)row[2];
//        }

//        #endregion

//        #region QueryCode

//        public new abstract class _
//        {
//            private _() { }

//            public static PropertyItem ID = new PropertyItem("ID");
//            public static PropertyItem Name = new PropertyItem("Name");
//            public static PropertyItem LoginID = new PropertyItem("LoginID");
//        }

//        #endregion
//    }
//}