using SyslogLogging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrelk.Logging
{
    /// <summary>
    /// Logging of console apps using file and syslog
    /// </summary>
    static class LogToSyslog
    {
        private static LoggingModule _logger = null;
        private static string _appName;

        /// <summary>
        /// Creates a new syslog logger
        /// </summary>
        /// <param name="ip">IPv4 address of the syslog host</param>
        /// <param name="port">Port of syslog server</param>
        /// <param name="appName">Name of the app to show in syslog</param>
        /// <param name="consoleEnable">Display logs in the console</param>
        /// <param name="fileLog">Output logs to file. Default: disabled</param>
        /// <param name="logFileName">Output file name (if enabled). The extension of '.log' will always be appended. Defaults to the current exe name (ex. app.exe.log).</param>
        /// <param name="overwrite">If a log file with the same name exists, should we overwrite it.</param>
        public static void CreateLogger(string ip, int port, string appName, bool consoleEnable = true, FileLoggingMode fileLog = FileLoggingMode.Disabled, string logFileName = null, bool overwrite = true)
        {
            //set some defaults for displaying logs in console
            _logger.ConsoleEnable = consoleEnable;
            _logger.EnableColors = true;

            //make sure this is a valid IPv4 address & create the logger if it is
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"))
                _logger = new LoggingModule(ip, port);
            else
                throw new Exception("Invalid IPv4 address");

            //hold app name to filename character requirements
            if (appName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) == -1)
                _appName = appName;
            else
                throw new Exception("App name includes invalid characters.");

            if (fileLog != FileLoggingMode.Disabled)
            {
                //if logFileName is the default, set it to <appname.exe>.log
                if (logFileName == null)
                    logFileName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

                //make sure the filename isn't empty
                if (logFileName.Length > 0)
                {
                    //check if the filename or path contain any invalid characters
                    if (logFileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1)
                    {
                        string fullPath = Path.GetFullPath(logFileName);
                        string fileName = Path.GetFileName(fullPath);
                        string pathName = Path.GetDirectoryName(fullPath);

                        //check if the directory exists, if not, create it
                        if (!Directory.Exists(pathName))
                        {
                            Directory.CreateDirectory(pathName);
                        }
                        if (overwrite)
                        {
                            if (File.Exists(fullPath))
                            {
                                //attempt to delete the file if it exists and overwrite is enabled
                                try {
                                    File.Delete(fullPath);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception($"Unable to delete previous logfile because: {e.Message}");
                                }
                            }
                        }
                        _logger.FileLogging = fileLog;
                        _logger.LogFilename = fullPath;
                    }
                    else
                    {
                        throw new Exception("Logfile name or path contains invalid characters.");
                    }
                }
                else
                {
                    throw new Exception("Log filename is empty.");
                }
            }
        }

        /// <summary>
        /// Returns if calls to the logger are also shown in console or not.
        /// </summary>
        /// <returns></returns>
        public static bool DisplayConsole()
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            return _logger.ConsoleEnable;
        }

        /// <summary>
        /// Enables/Disables displaying logs in the console.
        /// </summary>
        /// <param name="disp"></param>
        public static void DisplayConsole(bool disp)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.ConsoleEnable = disp;
        }

        /// <summary>
        /// Log message with separate error string. (Severity: Emergency)
        /// </summary>
        /// <param name="msg">ex. Your custom message.</param>
        /// <param name="error">ex. A detailed error string.</param>
        public static void LogEmergency(string msg, string error)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Emergency(String.Format("APP: {0}.\nMSG: {1}\nERROR: {2}", _appName, msg, error));
        }

        /// <summary>
        /// Log message. (Severity: Emergency)
        /// </summary>
        /// <param name="msg"></param>
        public static void LogEmergency(string msg)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Emergency(String.Format("APP: {0}.\nMSG: {1}", _appName, msg));
        }

        /// <summary>
        /// Log message with separate error string. (Severity: Error)
        /// </summary>
        /// <param name="msg">ex. Your custom message.</param>
        /// <param name="error">ex. A detailed error string.</param>
        public static void LogError(string msg, string error)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Error(String.Format("APP: {0}.\nMSG: {1}\nERROR: {2}", _appName, msg, error));
        }

        /// <summary>
        /// Log message. (Severity: Error)
        /// </summary>
        /// <param name="msg"></param>
        public static void LogError(string msg)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Error(String.Format("APP: {0}.\nMSG: {1}", _appName, msg));
        }

        /// <summary>
        /// Log message with separate error string. (Severity: Alert)
        /// </summary>
        /// <param name="msg">ex. Your custom message.</param>
        /// <param name="error">ex. A detailed error string.</param>
        public static void LogAlert(string msg, string error)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Alert(String.Format("APP: {0}.\nMSG: {1}\nALERT: {2}", _appName, msg, error));
        }

        /// <summary>
        /// Log message. (Severity: Alert)
        /// </summary>
        /// <param name="msg"></param>
        public static void LogAlert(string msg)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Alert(String.Format("APP: {0}.\nMSG: {1}", _appName, msg));
        }

        /// <summary>
        /// Log message. (Severity: Info)
        /// </summary>
        /// <param name="msg"></param>
        public static void LogInfo(string msg)
        {
            //throw exception if the logger isn't initialized
            if (_logger == null)
                throw new Exception("Logger not initialized. Make sure to call CreateLogger at the start of your program!");

            _logger.Info(String.Format("APP: {0}.\nMSG: {1}", _appName, msg));
        }
    }
}
