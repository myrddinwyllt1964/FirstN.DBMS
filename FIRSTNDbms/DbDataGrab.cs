using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using CodePlusTimeSavers_CE;
using SuperLogger;


namespace CodePlusTimeSavers_CE
{
    public class DbDataGrab
    {
        public static bool log = false;
        private static void AaaConfig()
        {
            // TODOL Read Config file for Debug Options
        }

        public static void CallSql(EnumsStructs.DbArrivedDataTable parmDbArrived, string parmConnString, string parmSql)
        {
            DataGet(delegate(object o, Exception e)
            {
                if (o == null && log)
                {
                    LoggerDb.LogEmptyResults(parmSql);
                }
                var dtblTemp = (DataTable) o;
                parmDbArrived(dtblTemp, null);
            }, EnumsStructs.DbFetchType.DataTable, parmConnString, parmSql);
        }

        public static void CallSql(EnumsStructs.DbArrivedRow parmDbArrived, string parmConnString, string parmSql)
        {
            DataGet(delegate(object o, Exception e)
            {
                var dtblTemp = (DataTable) o;
                foreach (DataRow drw in dtblTemp.Rows)
                {
                    parmDbArrived(drw, null);
                }
            },
                EnumsStructs.DbFetchType.DataTable, parmConnString, parmSql);
        }

        public static void CallSproc(EnumsStructs.DbArrivedDataTable parmDbArrived, string parmConnString,
            string parmSprocName,
            params object[] parmInput)
        {
            DataGet(delegate(Object o, Exception e) { parmDbArrived((DataTable) o, e); },
                EnumsStructs.DbFetchType.DataTable,
                parmConnString, parmSprocName, parmInput);
            
            Debug.WriteLine(string.Format("sproc name is: {0}", parmSprocName));
            //return Void(parmSprocName);
        }

        public static void CallSproc(EnumsStructs.DbArrivedRow parmDbArrived, string parmConnString,
            string parmSprocName,
            params object[] parmInput)
        {
            DataGet(delegate(object o, Exception e)
            {
                var dtblTemp = (DataTable) o;
                if (o == null)
                {
                    return;
                }
                int i=1;
                //int j=1;
                foreach (DataRow drw in dtblTemp.Rows)
                {
                    
                    //Debug.WriteLine(dtblTemp.Rows[i][j].ToString());
                    parmDbArrived(drw , null);
                    //i=i+1;
                    
                }
            },
                EnumsStructs.DbFetchType.DataTable, parmConnString, parmSprocName, parmInput);
            Debug.WriteLine("SprocName= " + parmSprocName);
        }

        internal delegate void DataFetch(object parmData, Exception parmExc);

        internal static void DataGet(DbDataGrab.DataFetch pDataArrived, EnumsStructs.DbFetchType parmDbFetchType,
            string pConnectionString,
            string pSql, params object[] parmInput)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader rdr = null;
            DataTable dtbl = null;
            AaaConfig();
            try
            {
                conn = DbConnector.ConnectionOpen();
                if (conn == null && log)
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
                //var str_cmd = CommandType.StoredProcedure.ToString();
                DbUtil.ParameterSetup(ref cmd, parmInput);
                //LoggerDb.DBMSSproc(conn, cmd);
                object rtrnObject = null;
                switch (parmDbFetchType)
                {
                    case EnumsStructs.DbFetchType.DataTable:
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        if (rdr.FieldCount > 0) //edited RG

                        {
                            dtbl = new DataTable();
                            dtbl.Load(rdr);
                            rtrnObject = dtbl;
                        }
                        else
                        {
                            LoggerDb.DBMSEmptyResults(null, conn, cmd);
                        }
                        break;
                    case EnumsStructs.DbFetchType.DataReader:
                        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        rtrnObject = rdr;
                        break;
                    case EnumsStructs.DbFetchType.Entity:
                        // TODO: Entity Support
                        rtrnObject = null;
                        break;
                    case EnumsStructs.DbFetchType.Scalar:
                        rtrnObject = cmd.ExecuteScalar();
                        break;
                    default:
                        rtrnObject = null;
                        break;
                }
                if (parmDbFetchType == EnumsStructs.DbFetchType.DataTable)
                {
                    if (rtrnObject == null || ((DataTable) rtrnObject).Rows.Count == 0)
                    {
                        LoggerDb.DBMSSproc(conn, cmd);
                        pDataArrived(null, new DataException("Zero Records Returned"));
                        return;
                    }
                }
                pDataArrived(rtrnObject, null);
            } // end try
            /*
            catch (Exception exc1)
            {
                LoggerException.ExceptionOccured(exc1, conn, cmd, rdr, dtbl);
                pDataArrived(null, exc1);
            }
            */
            finally
            {
                if (conn != null) conn.Dispose();
                if (cmd != null) cmd.Dispose();
            }
        } // end DataGet



        public static T ScalarSql<T>(string parmConnString, string parmSql)
        {
            // TODO: test nullable ref types including No Value scenarios
            // TODO: throw exception on Nulls tie into overall exception strategy
            Object objReturn = null;
            DataGet(delegate(Object o, Exception e) { objReturn = o; }, EnumsStructs.DbFetchType.Scalar, parmConnString,
                parmSql);
            return (T) objReturn;
        }

        public static T ScalarSproc<T>(string parmConnString, string parmSprocName, params object[] parmInput)
        {
            // TODO: test nullable ref types including No Value scenarios
            // TODO: throw exception on Nulls tie into overall exception strategy
            Object objReturn = null;
            DataGet(delegate(Object o, Exception e) { objReturn = o; }, EnumsStructs.DbFetchType.Scalar, parmConnString,
                parmSprocName, parmInput);
            return (T) objReturn;
        }
    }
}