using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Data.SqlClient;


namespace CodePlustTimeSavers_CE
{
    public class DbDataInsert
    {
        public static string DbInsert(SqlConnection pdbconn, string pSproc, params object[] parmInput)
        {
            pdbconn.Open();
            var command = new SqlCommand(pSproc, pdbconn) { CommandType = CommandType.StoredProcedure };
            for (int i = 0; i < parmInput.Length; i+=2)
            {
                var paramType = parmInput[i].GetType().Name;
                switch (paramType)
                {
                    case "String":
                        command.Parameters.Add(parmInput[i + 1].ToString(), SqlDbType.VarChar).Value = parmInput[i];
                        break;
                        
                    case "Int32":
                        command.Parameters.Add(parmInput[i + 1].ToString(), SqlDbType.Int).Value = parmInput[i];
                        break;
                }
                
            }
            command.ExecuteNonQuery();    
            return "";
        }
    }
}
