using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace NBear.Data
{
    /// <summary>
    /// cache config section
    /// </summary>
    public class CacheConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The default entity cache expire seconds
        /// </summary>
        public const int DEFAULT_EXPIRE_SECONDS = 60;

        private bool enable = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CacheConfigurationSection"/> is enable.
        /// </summary>
        /// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("enable")]
        public bool Enable
        {
            get { return this["enable"] == null ? enable : (bool)this["enable"]; }
            set { this["enable"] = value; }
        }

        /// <summary>
        /// Gets or sets the caching tables.
        /// </summary>
        /// <value>The caching tables.</value>
        [ConfigurationProperty("cachingTables", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection CachingTables
        {
            get
            {
                return (KeyValueConfigurationCollection)this["cachingTables"];
            }
            set
            {
                this["cachingTables"] = value;
            }
        }
    }
}
