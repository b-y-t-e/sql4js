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
            AvailableFunctions.Add(new CSharpFunction("c#")
            {

            });

            AvailableFunctions.Add(new DynLanFunction("dynlan")
            {

            });
        }
    }
}
