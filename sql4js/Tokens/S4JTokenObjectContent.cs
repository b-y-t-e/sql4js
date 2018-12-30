using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenObjectContent : S4JToken
    {
        public String Text { get; set; }

        public Object Value { get; set; }

        public S4JTokenObjectContent()
        {
            Text = "";
            IsObjectKey = false;
            Children = new List<S4JToken>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            if( Value is IDictionary<string, object> dict)
            {
                Dictionary<String, Object> variables = new Dictionary<string, object>();
                foreach (var item in dict)
                    variables[item.Key] = item.Value;
                return variables;
            }

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

        public override void Commit()
        {
            this.Text = this.Text.Trim();
            // this.ValueFromText = this.Text.DeserializeJson();
            base.Commit();
        }

    }
}
