using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenRoot : S4JToken
    {
        public Dictionary<String, Object> Parameters { get; set; }

        public S4JTokenRoot()
        {
            Children = new List<S4JToken>();
            Parameters = new Dictionary<string, object>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            return Parameters;
        }

    }
}
