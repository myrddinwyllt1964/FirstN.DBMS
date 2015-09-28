using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CodePlusTimeSavers_CE
{
     
    
        public class DbConnector
        {
             
            public static SqlConnection ConnectionOpen()
            {
                SqlConnection conn;
                
                try
                {
                    conn = new SqlConnection("Data Source=CPWIN2012SRV;Initial Catalog=H2MBRSATDb_dev;Persist Security Info=True;User ID=H2MBRSAT;Password=Dashboard!123!");
                }
                catch (Exception exc1)
                {
                    throw new Exception("Unable to connect to database.");
                }
                return conn;
            }


        }
    }

