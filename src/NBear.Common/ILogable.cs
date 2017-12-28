using System;
using System.Collections.Generic;
using System.Text;

namespace NBear.Common
{
    /// <summary>
    /// A delegate used for log.
    /// </summary>
    /// <param name="logMsg">The msg to write to log.</param>
    public delegate void LogHandler(string logMsg);

    /// <summary>
    /// Mark a implementing class as loggable.
    /// </summary>
    public interface ILogable
    {
        /// <summary>
        /// OnLog event.
        /// </summary>
        event LogHandler OnLog;
    }
}
