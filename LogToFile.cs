using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vrelk.Logging
{
    enum OutputLogMode : byte
    {
        APPEND = 0,
        OVERWRITE = 1
    }

    static class LogToFile
    {
        //declarations for general output
        private static TextWriter _msgOut;
        private static FileStream _msgOutStrm;
        private static StreamWriter _msgWriter = null;
        private static TextWriter _oldMsgOut; //this is the old output, before changing it to our new one
        private static bool _msgRedirected = false; //is the output currently redirected

        //declarations for error output
        private static TextWriter _errOut;
        private static FileStream _errOutStrm;
        private static StreamWriter _errWriter = null;
        private static TextWriter _oldErrOut; //this is the old output, before changing it to our new one
        private static bool _errRedirected = false; //is the output currently redirected

        /// <summary>
        /// Initialize the message logger
        /// </summary>
        /// <param name="logFileName">Output file name. The extension of '.log' will always be appended. Defaults to the current exe name (ex. app.exe_err.log).</param>
        /// <param name="writeMode">Default: Append</param>
        public static void CreateOutputLoggers(string logFileName = null, OutputLogMode writeMode = OutputLogMode.APPEND)
        {
            if (!_msgRedirected)
                _oldMsgOut = Console.Out;

            //if logFileName is the default, set it to <appname.exe>", we will append ".log" later
            if (logFileName == null)
                logFileName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            //make sure the filename isn't empty
            if (logFileName.Length > 0)
            {
                //check if the filename or path contain any invalid characters
                if (logFileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1)
                {
                    //append ".log" extension to the end
                    logFileName += ".log";

                    string fullPath = Path.GetFullPath(logFileName);
                    string fileName = Path.GetFileName(fullPath);
                    string pathName = Path.GetDirectoryName(fullPath);

                    //check if the directory exists, if not, create it
                    if (!Directory.Exists(pathName))
                    {
                        Directory.CreateDirectory(pathName);
                    }

                    if (writeMode == OutputLogMode.OVERWRITE)
                    {
                        if (File.Exists(fullPath))
                        {
                            //attempt to delete the file if it exists and overwrite is enabled
                            try
                            {
                                File.Delete(fullPath);
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Unable to delete previous logfile because: {e.Message}");
                            }
                        }

                        //creates a new log file to write to since we just deleted the previous one
                        _msgOutStrm = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
                        _msgWriter = new StreamWriter(_msgOutStrm);
                    }
                    else
                    {
                        //opens an existing file (or creates it) and appends to the end
                        _msgOutStrm = new FileStream(fullPath, FileMode.Append, FileAccess.Write);
                        _msgWriter = new StreamWriter(_msgOutStrm);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the error logger
        /// </summary>
        /// <param name="logFileName">Output file name. The extension of '.log' will always be appended. Defaults to the current exe name (ex. app.exe_err.log).</param>
        /// <param name="writeMode">Default: Append</param>
        public static void CreateErrorLogger(string logFileName = null, OutputLogMode writeMode = OutputLogMode.APPEND)
        {
            if (!_errRedirected)
                _oldErrOut = Console.Error;

            //if logFileName is the default, set it to <appname.exe>", we will append ".log" later
            if (logFileName == null)
                logFileName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "_err";

            //make sure the filename isn't empty
            if (logFileName.Length > 0)
            {
                //check if the filename or path contain any invalid characters
                if (logFileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1)
                {
                    //append ".log" extension to the end
                    logFileName += ".log";

                    string fullPath = Path.GetFullPath(logFileName);
                    string fileName = Path.GetFileName(fullPath);
                    string pathName = Path.GetDirectoryName(fullPath);

                    //check if the directory exists, if not, create it
                    if (!Directory.Exists(pathName))
                    {
                        Directory.CreateDirectory(pathName);
                    }

                    if (writeMode == OutputLogMode.OVERWRITE)
                    {
                        if (File.Exists(fullPath))
                        {
                            //attempt to delete the file if it exists and overwrite is enabled
                            try
                            {
                                File.Delete(fullPath);
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Unable to delete previous logfile because: {e.Message}");
                            }
                        }

                        //creates a new log file to write to since we just deleted the previous one
                        _errOutStrm = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
                        _errWriter = new StreamWriter(_errOutStrm);
                    }
                    else
                    {
                        //opens an existing file (or creates it) and appends to the end
                        _errOutStrm = new FileStream(fullPath, FileMode.Append, FileAccess.Write);
                        _errWriter = new StreamWriter(_errOutStrm);
                    }
                }
            }
        }

        /// <summary>
        /// Redirect all console output to file.
        /// </summary>
        /// <param name="redirect">Enable/Disable</param>
        public static void RedirectConsole(bool redirect)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");

            if (redirect)
            {
                if (!_msgRedirected) //not redirecting, lets start that
                {
                    Console.SetOut(_msgOut);
                    _msgRedirected = true;
                }
            }
            else
            {
                if (_msgRedirected) //redirecting, lets stop that
                {
                    Console.SetOut(_oldMsgOut);
                    _msgRedirected = false;
                }
            }
        }

        /// <summary>
        /// Redirect all console errors to file.
        /// </summary>
        /// <param name="redirect">Enable/Disable</param>
        public static void RedirectError(bool redirect)
        {
            if (_errWriter == null)
                throw new Exception("Error Logger uninitialized. Make sure to call CreateErrorLogger!");

            if (redirect)
            {
                if (!_errRedirected) //not redirecting, lets start that
                {
                    Console.SetOut(_errOut);
                    _errRedirected = true;
                }
            }
            else
            {
                if (_errRedirected) //redirecting, lets stop that
                {
                    Console.SetOut(_oldErrOut);
                    _errRedirected = false;
                }
            }
        }


        public static void Write(string msg)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _msgOut.Write(msg);
            _oldMsgOut.Write(msg);
        }
        public static void Write(string msg, params object[] list)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _msgOut.Write(msg, list);
            _oldMsgOut.Write(msg, list);
        }

        public static void WriteLine(string msg)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _msgOut.WriteLine(msg);
            _oldMsgOut.WriteLine(msg);
        }
        public static void WriteLine(string msg, params object[] list)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _msgOut.WriteLine(msg, list);
            _oldMsgOut.WriteLine(msg, list);
        }


        public static void EWrite(string msg)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _errOut.Write(msg);
            _oldErrOut.Write(msg);
        }
        public static void EWrite(string msg, params object[] list)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _errOut.Write(msg, list);
            _oldErrOut.Write(msg, list);
        }

        public static void EWriteLine(string msg)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _errOut.WriteLine(msg);
            _oldErrOut.WriteLine(msg);
        }
        public static void EWriteLine(string msg, params object[] list)
        {
            if (_msgWriter == null)
                throw new Exception("Message Logger uninitialized. Make sure to call CreateOutputLogger!");
            _errOut.WriteLine(msg, list);
            _oldErrOut.WriteLine(msg, list);
        }
    }
}
