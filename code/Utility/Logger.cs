using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tortoise
{
    class Logger
    {
        public delegate void OnLogLineAddedCallbackDelegate(string line);
        private static Logger? logger;
        private string logsDirectory = "TortoiseBotLogs";
        private string logFileName = "TortoiseBotLog";
        private string logFileFullPath = "";
        private Mutex mutex = new Mutex();
        private OnLogLineAddedCallbackDelegate? _onLogLineAdded;

        private Logger()
        {
            CreateLogFile();
        }

        private void CreateLogFile()
        {
            if(!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
            string fileNameDate = GetFormattedDateTime().Replace(":", "_");
            fileNameDate = fileNameDate.Replace(":", "_");
            fileNameDate = fileNameDate.Replace("|", "_");
            fileNameDate = fileNameDate.Replace("/", "_");

            logFileFullPath = $"{logsDirectory}\\{logFileName} {fileNameDate}.txt";
            if (!File.Exists(logFileFullPath))
            {
                File.CreateText(logFileFullPath).Close();
            }
        }

        private string FormatTime_Seconds(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }

        private string FormatTime_Day(DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy");
        }

        private string GetFormattedDateTime()
        {
            DateTime dateTime = DateTime.Now;
            return $"{FormatTime_Day(dateTime)} | {FormatTime_Seconds(dateTime)}";
        }

        private void priv_LogWithDate(string prefix, string logLine)
        {
            priv_LogOut(prefix, $"{GetFormattedDateTime()} | {logLine}");
        }

        private void priv_LogOut(string prefix, string logLine)
        {
            string logLineOut = $"{prefix} | {logLine}";
            mutex.WaitOne();
            File.AppendAllText(logFileFullPath, $"{logLineOut}{Environment.NewLine}");
            _onLogLineAdded?.Invoke(logLineOut);
            mutex.ReleaseMutex();
        }

        public static void Initialize()
        {
            logger = new Logger();
        }

        public static void WriteLine_Critical(string logLine)
        {
            logger?.priv_LogWithDate("C", logLine);
        }

        public static void WriteLine_Error(string logLine)
        {
            logger?.priv_LogWithDate("E", logLine);
        }

        public static void WriteLine_Warning(string logLine)
        {
            logger?.priv_LogWithDate("W", logLine);
        }

        public static void WriteLine_Info(string logLine)
        {
            logger?.priv_LogWithDate("I", logLine);
        }

        public static void WriteLine_Verbose(string logLine)
        {
#if DEBUG
            logger?.priv_LogWithDate("V", logLine);
#endif
        }

        public static void WriteLine_Debug(string logLine)
        {
#if DEBUG
            logger?.priv_LogWithDate("D", logLine);
#endif
        }

        public static void AddOnLogLineCallbackAdded(OnLogLineAddedCallbackDelegate onLogLineAdded)
        {
            if(logger is not null)
            {
                logger.priv_AddDelegate(onLogLineAdded);
            }
        }

        private void priv_AddDelegate(OnLogLineAddedCallbackDelegate onLogLineAdded)
        {
            mutex.WaitOne();
            _onLogLineAdded += onLogLineAdded;
            mutex.ReleaseMutex();
        }

        public static void RemoveOnLogLineCallbackAdded(OnLogLineAddedCallbackDelegate onLogLineAdded)
        {
            if (logger is not null)
            {
                logger.priv_RemoveDelegate(onLogLineAdded);
            }
        }

        private void priv_RemoveDelegate(OnLogLineAddedCallbackDelegate onLogLineAdded)
        {
            mutex.WaitOne();
            _onLogLineAdded -= onLogLineAdded;
            mutex.ReleaseMutex();
        }
    }
}
