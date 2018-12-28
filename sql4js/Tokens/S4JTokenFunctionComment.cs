using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenFunctionComment : S4JToken
    {
        public String Text { get; set; }

        public S4JTokenFunctionComment()
        {
            Text = "";
            Children = new List<S4JToken>();
        }

        public override void AddChildToToken(S4JToken Child)
        {

        }

        public override void AppendCharsToToken(IList<Char> Chars)
        {
            foreach (var Char in Chars)
            {
                this.Text += Char;
            }
        }

        public override void CommitToken()
        {
            this.Text = this.Text.Trim();
            base.CommitToken();
        }

    }
}
