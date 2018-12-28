using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sql4js.Executor;

namespace sql4js.Parser
{
    public class S4JTokenFunction : S4JToken
    {
        public Object Result { get; set; }

        public IEvaluator Evaluator { get; set; }

        public Boolean IsEvaluated { get; set; }

        ////////////////////////////////////////////

        public S4JTokenFunction()
        {
            Children = new List<S4JToken>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            return null;
            // throw new NotImplementedException();
        }


        public override void AppendCharsToToken(IList<Char> Chars)
        {
            /*foreach (var Char in Chars)
            {
                if (this.Text.Length == 0 && System.Char.IsWhiteSpace(Char))
                    continue;
                this.Text += Char;
            }*/

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
