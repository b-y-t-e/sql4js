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

        public override void AppendCharsToToken(IList<Char> Chars)
        {
            S4JToken lastChild = this.Children.LastOrDefault();
            if (!(lastChild is S4JTokenTextValue))
            {
                lastChild = new S4JTokenTextValue();
                this.Children.Add(lastChild);
            }
            lastChild.AppendCharsToToken(Chars);
        }
        
    }
}
