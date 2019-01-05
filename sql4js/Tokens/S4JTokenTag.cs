using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sql4js.Helpers;

namespace sql4js.Tokens
{
    public class S4JTokenTag : S4JToken
    {
        public S4JTokenTag()
        {
            Children = new List<S4JToken>();
        }
    }



}
