using sql4js.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JDefaultParser : S4JParser
    {
        public S4JDefaultParser()
        {
            Functions.Add(new CSharpFunction());
            Functions.Add(new TSqlFunction());
        }
    }
}
