using System;

namespace NBear.Common
{
    /// <summary>
    /// CouldNotLoadEntityConfigurationException
    /// </summary>
    public class CouldNotLoadEntityConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotLoadEntityConfigurationException"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        public CouldNotLoadEntityConfigurationException(Exception inner) : base("CouldNotLoadEntityConfigurationException", inner) { }
    }

    /// <summary>
    /// TypeIsNotASubClassOfEntityException
    /// </summary>
    public class TypeIsNotASubClassOfEntityException : Exception { }

    /// <summary>
    /// CouldNotFoundEntityConfigurationOfEntityException
    /// </summary>
    public class CouldNotFoundEntityConfigurationOfEntityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotFoundEntityConfigurationOfEntityException"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        public CouldNotFoundEntityConfigurationOfEntityException(string entityName) : base(entityName + " - Check your entity configuration file please.") { }
    }
}
