using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace CodePlusTimeSavers_CE
{
    public class EnumsStructs
    {
        public struct CodeRunnerOptions
        {
            public int RepeatCount;
            public int TimeOutCodeExecutionTime;
            public List<Exception> ExceptionsToIgnore;
        }

        public delegate void CodeCompleted(Exception parmExc, ref bool parmTryAgain, params object[] p2);

        public enum DbFetchType
        {
            DataTable,
            DataReader,
            Entity,
            Scalar
        }

        internal struct DbTask
        {
            public object[] ParamsIn;
            public string SprocName;
            public string Sql;
            public DbArrivedRow TargetRow;
            public DbArrivedDataTable TargetDataTable;
            public DbActionQueryComplete TargetExec;
        }

        public delegate void DbArrivedRow(DataRow parmRow, Exception parmExc);

        public delegate void DbArrivedDataTable(DataTable parmData, Exception parmExc);

        public delegate T DbArrivedOneValue<T>(T parmData, Exception parmExc);

        public delegate void DbArrivedReader(IDataReader parmData, Exception parmExc);

        public delegate void DbActionQueryComplete(int parmResult, Exception parmExc);

        [Flags]
        public enum DbTalkOptions
        {
            Transactional = 1,
            AnyOrderAllowed = 2
        }
    }
}
