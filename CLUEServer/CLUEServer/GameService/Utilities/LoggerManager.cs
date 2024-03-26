using System;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace GameService.Utilities
{
    /// <summary>
    /// Manager for logging operations using log4net.
    /// </summary>
    public class LoggerManager
    {
        /// <summary>
        /// Gets or sets the logger instance.
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the LoggerManager class.
        /// </summary>
        /// <param name="type">The type for which the logger is created.</param>
        public LoggerManager(Type type)
        {
            Logger = LogManager.GetLogger(type);
        }

        /// <summary>
        /// Gets a logger instance for a specified type.
        /// </summary>
        /// <param name="type">The type for which to get the logger.</param>
        /// <returns>The logger instance.</returns>
        public ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        public void LogInfo(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Logs an error message with an associated exception.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="ex">The associated exception.</param>
        public void LogError(string message, Exception ex)
        {
            Logger.Error(message, ex);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="ex">The error exception to log.</param>
        public void LogError(Exception ex)
        {
            Logger.Error(ex);
        }

        /// <summary>
        /// Logs a fatal error message.
        /// </summary>
        /// <param name="ex">The fatal error exception to log.</param>
        public void LogFatal(Exception ex)
        {
            Logger.Fatal(ex);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="ex">The warning exception to log.</param>
        public void LogWarn(Exception ex)
        {
            Logger.Warn(ex);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="ex">The debug exception to log.</param>
        public void LogDebug(Exception ex)
        {
            Logger.Debug(ex);
        }
    }
}