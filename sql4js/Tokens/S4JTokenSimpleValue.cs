using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenSimpleValue : S4JToken
    {
        public String Text { get; set; }

        public S4JTokenSimpleValue()
        {
            Text = "";
            IsKey = false;
            Children = new List<S4JToken>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            return null;
        }

        public override void AddChildToToken(S4JToken Child)
        {
            // Value = Child;
        }

        public override void AppendCharsToToken(IList<Char> Chars)
        {
            foreach (var Char in Chars)
            {
                if (this.Text.Length == 0 && System.Char.IsWhiteSpace(Char))
                    continue;
                this.Text += Char;
            }
        }

        public override void BuildJson(StringBuilder Builder)
        {
            Builder.Append(Text);
        }

        public override void CommitToken()
        {
            this.Text = this.Text.Trim();
            // this.ValueFromText = this.Text.DeserializeJson();
            base.CommitToken();
        }

    }
}
