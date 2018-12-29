using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenQuotation : S4JToken
    {
        public S4JTokenQuotation()
        {
            IsObjectKey = false;
            Children = new List<S4JToken>();
        }

        public override void BuildJson(StringBuilder Builder)
        {
            //Builder.Append("'");
            base.BuildJson(Builder);
            //Builder.Append("'");
        }
        /*public override void AddChildToToken(S4JToken Child)
        {
            // Value = Child;
        }*/

    }
}
