using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenArray : S4JToken
    {
        public S4JTokenArray()
        {
            Children = new List<S4JToken>();
        }

        public override void BuildJson(StringBuilder Builder)
        {
            Builder.Append("[");
            Int32 i = 0;
            foreach (var child in Children)
            {
                if (i > 0) Builder.Append(",");
                child.BuildJson(Builder);
                i++;
            }
            Builder.Append("]");
        }
    }
}
