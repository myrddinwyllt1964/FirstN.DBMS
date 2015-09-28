using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace SuperLogger
{
    public class LoggerDb
    {
        private static bool _showDBMSQuery = false;
        private static bool _showDBMSEmptyResults = true;
        private static bool _showDBMSTestModeNoExecute = true;

        private static void AaaConfig()
        {
            _showDBMSQuery = true;
            _showDBMSEmptyResults = false;
            _showDBMSTestModeNoExecute = false;            
        }

        public static void DBMSEmptyResults(Exception parmExc, SqlConnection parmConn, SqlCommand parmCmd)
        {
            AaaConfig();
            if (!_showDBMSEmptyResults) return;
            Logger.LogRaw("Empty Results for " + DBMSSprocDetails(parmCmd));
        }

        public static string DBMSSprocDetails(SqlCommand parmCmd)
        {
            AaaConfig();
            var sbMsg = new StringBuilder();
            if (parmCmd == null) return "no DBMS details SqlCommand=null";
            string strPrefixSproc = parmCmd.CommandType == CommandType.StoredProcedure ? "EXEC " : "";
            sbMsg.AppendLine(strPrefixSproc + parmCmd.CommandText);
            int intParamCounter = 0;
            foreach (SqlParameter p in parmCmd.Parameters)
            {
                string strParmDisplay = string.Format("{0}='{1}'", p.ParameterName, p.Value);
                if (intParamCounter > 0)
                    strParmDisplay = "," + strParmDisplay;
                sbMsg.AppendLine(strParmDisplay);
                intParamCounter++;
            }
            return sbMsg.ToString();
        } // end DBMSSprocDetails


        public static void DBMSSproc(SqlConnection parmConn, SqlCommand parmCmd)
        {
            AaaConfig();
            if (!_showDBMSQuery) return;
            Logger.LogRaw(DBMSSprocDetails(parmCmd));
        } // end SprocDetails

        public static void DBMSSql(Guid g, string parmSql)
        {
            AaaConfig();
            if (!_showDBMSQuery) return;
            Logger.LogRaw(parmSql);
        }

        public static bool DBMSTestMode()
        {
            AaaConfig();
            return _showDBMSTestModeNoExecute;
        }

        public static void LogEmptyResults(string parmSql)
        {
            AaaConfig();
            Logger.LogRaw("Empty Results for SQL=" + parmSql);
        }
    }
}
