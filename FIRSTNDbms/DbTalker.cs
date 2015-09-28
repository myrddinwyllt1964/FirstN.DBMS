using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using SuperLogger;


namespace CodePlusTimeSavers_CE
{
    public class DbTalker
    {
        private readonly string _connStr;
        private readonly List<EnumsStructs.DbTask> _dbTasks = new List<EnumsStructs.DbTask>();

        private static void AaaConfig()
        {

        } 

        
        public DbTalker(string parmConnName)
        {
            _connStr = parmConnName;
        }


        public static void ExecuteSql(EnumsStructs.DbActionQueryComplete p1, string parmConnString, string parmSql)
        {
            Exec(delegate(object o, Exception e) { p1((int)o, e); },
                false, parmConnString, parmSql);
        }

        public static void ExecuteSproc(EnumsStructs.DbActionQueryComplete p1, string parmConnString, string parmSprocName,
            params object[] parmInput)
        {
            Exec(delegate(object o, Exception e) { p1((int)o, e); },
              false, parmConnString, parmSprocName, parmInput);
        }

        public void TaskAddQuery(string parmSql, EnumsStructs.DbActionQueryComplete parmArrived)
        {
            var dbt = new EnumsStructs.DbTask();
            dbt.Sql = parmSql;
            dbt.TargetExec = parmArrived;
            _dbTasks.Add(dbt);
        }

        public void TaskExecuteAll(EnumsStructs.DbTalkOptions p1)
        {
            // TODO: support DbTalkOptions
            foreach (EnumsStructs.DbTask dbt in _dbTasks)
            {
                Exec(delegate(object o, Exception e) { dbt.TargetExec((int)o, e); },false,_connStr, dbt.Sql);
            }
            ;
            //if(p1.HasFlags(DbTalkOptions.Async))
        }


       internal static void Exec(DbDataGrab.DataFetch pDataArrived, bool pAsync, string pConnectionString,string pSql, params object[] parmInput)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rdr = null;
            DataTable dtbl = null;
            AaaConfig();
           try
           {
               conn = DbConnector.ConnectionOpen();
               if (conn == null)
               {
                   pDataArrived(null, new Exception("Connection could not be opened => " + pConnectionString));
                   return;
               }
               conn.Open();
               cmd = new SqlCommand
               {
                   Connection = conn,
                   CommandText = pSql,
                   CommandType = Hacks.IsSql(pSql) ? CommandType.Text : CommandType.StoredProcedure
               };
               if (cmd.CommandType == CommandType.StoredProcedure && !cmd.CommandText.StartsWith("["))
                   cmd.CommandText = "[" + cmd.CommandText + "]";
               DbUtil.ParameterSetup(ref cmd, parmInput);
               LoggerDb.DBMSSproc(conn, cmd);
               object rtrnObject = null;
               if (LoggerDb.DBMSTestMode())
               {
                   rtrnObject = -1;
               }
               else
               {
                   rtrnObject = cmd.ExecuteNonQuery();
               }
           }
           catch (Exception exc1)
           {
               LoggerException.ExceptionOccured(exc1, conn, cmd, rdr, dtbl);
               pDataArrived(null, exc1);
           }
           finally
           {
               if (conn != null) conn.Dispose();
               if (cmd != null) cmd.Dispose();
           }
        }

    }
}
