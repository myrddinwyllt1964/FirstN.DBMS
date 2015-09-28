using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;


namespace SuperLogger
{
    public class Logger
    {
        private const string LogToDiskDirectory = @"C:\data\LogsBoltLightning";
        private const string LogToDiskFilePrefix = @"H2MBRSAT";
        private const char IndentCharacter = '\t';
        private const int IndentSpaces = 5;

        private static bool _showOutputWindow = false;       //reset to false RG

        private static bool _logToDisk = false;         //reset to false RG

        private static string _logToDiskFileName = String.Empty;

        private static Dictionary<Guid, string> _ApplicationLogs = null;

        private static void AaaConfig()
        {
            // all _show members will be set here from a Config file
        }

        public static void AppEvent_AppStarted(string parmAppName)
        {
            if (!_logToDisk) return;
            _logToDiskFileName = LogFileNameGenerateUnique(parmAppName, LogToDiskDirectory, LogToDiskFilePrefix);
            LogInfo(parmAppName + " started: " + DateTime.Now.ToLongTimeString(), 0);
        }

        public static void AppEvent_AppFinished(string parmAppName)
        {
            LogInfo(parmAppName + " ended: " + DateTime.Now.ToLongTimeString(), 0);
        }


        /*
                public static Guid AppEventStarted(string parmAppName)
                {
                    // if test prevents recursion
                    // TODO provide ThreadLocking
                    string strLogFile = String.Empty;
                    if (_ApplicationLogs.ContainsValue(parmAppName))
                    {
                
                    }
                    strLogFile = LogFileNameGenerateUnique(LogToDiskDirectory, parmAppName + "_" + LogToDiskFilePrefix);
                    var guidApp = new Guid();
                    _ApplicationLogs.Add(guidApp,strLogFile);
                    return new Guid();
                }
         */




        private static string LogFileNameGenerateUnique(string parmAppName, string parmDiskDirectory,
            string parmFilePrefix)
        {
            // AaaConfig(); causes recursion problem
            if (_logToDiskFileName == String.Empty)
            {
                string strFileName = parmDiskDirectory + @"\" + parmFilePrefix + "_" + parmAppName + "_" +
                                     DateTime.Now.ToLongDateString() + "_" +
                                     DateTime.Now.ToLongTimeString().Replace(":", "_");

                _logToDiskFileName = strFileName.Replace(",", "_");
                _logToDiskFileName = _logToDiskFileName.Replace(" PM", "_PM");
                _logToDiskFileName = _logToDiskFileName.Replace(" AM", "_AM");
                _logToDiskFileName = _logToDiskFileName + ".log";
                //_logToDiskFileName = strFileName.Replace(":", "_");
                using (File.Create(_logToDiskFileName))
                {
                }
                // TODO: Handle collisions if App is created at exact same millisecond
            }
            return _logToDiskFileName;
        }

        public static void LogInfo(string p1, int parmIndentationLevel)
        {
            LogInternalWriteLine(p1, parmIndentationLevel);
        }

        public static void LogRaw(string p1)
        {
            LogInternalWriteLine(p1,0);
        }


        private static void LogInternalWriteLine(string p1, int parmIndentationLevel)
        {
            AaaConfig();
            string msg = new string(IndentCharacter, IndentSpaces * parmIndentationLevel) + p1;
            if (_showOutputWindow) Debug.WriteLine(msg);
            if (!_logToDisk)
            {
                Debug.WriteLine(msg);
                return;
            }
            //http://stackoverflow.com/questions/411752/best-way-to-repeat-a-character-in-c-sharp
            string strLogFileName = _logToDiskFileName;
            if (_logToDiskFileName == String.Empty)
            {
                Debug.WriteLine(
                    "Cannot create Application log. Programmer needs to Call Logger.AppEvent_AppStarted()...");
                Debug.WriteLine(msg);
                return;
            }
            // http://stackoverflow.com/questions/8545880/the-most-condensed-shortest-way-to-append-a-new-line-to-a-file
            bool bolWriteSucceeded = false;
            int intRetryCount = 0;
            while (!bolWriteSucceeded)
            {
                try
                {
                    File.AppendText(msg + Environment.NewLine); // @"\n\r" did not work
                    bolWriteSucceeded = true;
                }
                catch (System.IO.IOException exc1)
                {
                    LogInfo("LogEntry RetryCount=" + intRetryCount + exc1.Message, 0);
                    intRetryCount++;
                    Thread.Sleep(1);
                }
            }

        } // end LogInternalWrite
    } // end class Logger
} // end namespace