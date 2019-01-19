using sql4js.Functions;
using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Executor
{
    public class S4JExecutorForTests : S4JExecutor
    {
        public S4JExecutorForTests() :
            base(new S4JParserForTests())
        {
            Sources.Register("sql", "Data Source=.;uid=dba;pwd=xxxxxxxxxxxx;initial catalog=dynjson;");
        }
    }
}
