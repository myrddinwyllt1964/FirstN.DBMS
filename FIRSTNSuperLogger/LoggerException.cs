using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace SuperLogger
{
    public class LoggerException
    {
        private static string ExceptionDetails(Exception p1)
        {
            return "";
        }

        public static void ExceptionOccured(Exception parmExc, params Object[] parmBlackBox)
        {
            var sbMsg = new StringBuilder();
            sbMsg.AppendLine(parmExc.Message);
            sbMsg.AppendLine(ExceptionDetails(parmExc));
            Exception excInner = parmExc.InnerException;
            int intInnerCounter = 0;
            while (excInner != null)
            {
                intInnerCounter++;
                sbMsg.AppendLine("InnerException #" + intInnerCounter);
                sbMsg.AppendLine(parmExc.Message);
                sbMsg.AppendLine(ExceptionDetails(parmExc));
                excInner = excInner.InnerException;
            }
            Logger.LogRaw(sbMsg.ToString());
            if (parmBlackBox.Length > 0)
            {
                Logger.LogRaw(ExceptionOccuredBlackBox(parmBlackBox));
            }

        } // end ExceptionOccured

        private static string ExceptionOccuredBlackBox(params Object[] parmBlackBox)
        {
            var sbBlackBox = new StringBuilder();
            sbBlackBox.AppendLine("Blackbox");

            foreach (object o in parmBlackBox)
            {
                bool bolObjectTypeDetected = false;
                if (o == null)
                {
                    bolObjectTypeDetected = true;
                    sbBlackBox.AppendLine("BlackObject null");
                }

                if (o is SqlCommand)
                {
                    bolObjectTypeDetected = true;
                    sbBlackBox.Append(LoggerDb.DBMSSprocDetails((SqlCommand)o));
                }
                if (!bolObjectTypeDetected)
                {
                    sbBlackBox.AppendLine("BlackBox object getType()=" + o.GetType());
                    sbBlackBox.AppendLine();
                }
            } // end foreach
            return sbBlackBox.ToString();
        }

    }
}
