using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NBear.Tools.EntityDesignToEntity
{
    [Serializable]
    public class EntityDesignToEntityConfiguration
    {
        public string CompileMode = "Debug";
        public string InputDllName = string.Empty;
        public string OutputNamespace = string.Empty;
        public string OutputLanguage = "C#";
        public string EntityCodePath = string.Empty;
        public string EntityConfigPath = string.Empty;

        public SqlSyncConfig SqlSync = new SqlSyncConfig();
    }

    [Serializable]
    public class SqlSyncConfig
    {
        [XmlAttribute("enable")]
        public bool Enable = false;
        public string SqlServerFolder = string.Empty;
        public string ServerName = ".";
        public string UserID = "sa";
        public string Password = string.Empty;
        public string DatabaseName = string.Empty;
    }
}
