using sql4js.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JParserForTests : S4JDefaultParser
    {
        public S4JParserForTests()
        {
            AvailableFunctions.Add(new TSqlFunction("sql", "Data Source=.;uid=dba;pwd=;initial catalog=dynjson;")
            {

            });
        }
    }
}
