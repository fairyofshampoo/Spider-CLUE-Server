using System;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace GameService.Utilities
{
    public class LoggerManager
    {
        public ILog Logger { get; set; }


        public LoggerManager(Type type)
        {
            Logger = LogManager.GetLogger(type);
        }

        public ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        public void LogInfo(string message)
        {
            Logger.Info(message);
        }

        public void LogError(string message, Exception ex)
        {
            Logger.Error(message, ex);
        }

        public void LogError(Exception ex)
        {
            Logger.Error(ex);
        }
        public void LogFatal(Exception ex)
        {
            Logger.Fatal(ex);
        }

        public void LogWarn(Exception ex)
        {
            Logger.Warn(ex);
        }

        public void LogDebug(Exception ex)
        {
            Logger.Debug(ex);
        }

    }
}
