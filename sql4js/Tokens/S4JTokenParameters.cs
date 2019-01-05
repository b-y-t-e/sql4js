using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sql4js.Helpers;

namespace sql4js.Tokens
{
    public class S4JTokenParameters : S4JToken
    {
        public S4JTokenParameters()
        {
            Children = new List<S4JToken>();
        }
    }



}
