using sql4js.Functions;
using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Executor
{
    public class S4JExecutorForTests : S4JDefaultExecutor
    {
        public S4JExecutorForTests() 
        {
            Sources.Register("sql", "Data Source=.;uid=dba;pwd=sql;initial catalog=dynjson;");
        }
    }
}
