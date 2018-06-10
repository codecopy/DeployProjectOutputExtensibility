using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.AutoDeploy.Helper
{
    public class EnvironmentHelper
    {
        /// <summary>
        /// The event log source
        /// </summary>
        private const string eventLogSource = "AutoDeployExtensibility";

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        public static void WriteMessage(string message, EventLogEntryType level = EventLogEntryType.Information)
        {
            EventLog.WriteEntry(eventLogSource, message, level);
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void WriteError(string message)
        {
            WriteMessage(message, EventLogEntryType.Error);
        }

        /// <summary>
        /// Writes the warn.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void WriteWarn(string message)
        {
            WriteMessage(message, EventLogEntryType.Warning);
        }
    }
}
