using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenFunctionQuotation : S4JToken
    {
        public S4JTokenFunctionQuotation()
        {
            Children = new List<S4JToken>();
        }

    }
}
