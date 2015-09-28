using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace CodePlusTimeSavers_CE
{
    class DbUtil
    {
        public static void ParameterSetup(ref SqlCommand parmCmd, params object[] parmInput)
        {
            bool parametersExist = true;
            if (parmInput == null) parametersExist = false;
            else if (parmInput.Length == 0) parametersExist = false;
            if (parametersExist)
            {
                for (int countparams = 0; countparams < parmInput.Length; countparams += 2)
                {
                    SqlParameter param = parmCmd.CreateParameter();
                    param.ParameterName = parmInput[countparams].ToString();
                    param.Direction = ParameterDirection.Input;
                    param.Value = parmInput[countparams + 1];
                    parmCmd.Parameters.Add(param);
                }
            }
        }


    } // end class
} // end namespace
