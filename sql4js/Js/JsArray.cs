using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class JsArray : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public String Text { get; set; }

        public List<Is4jToken> Array { get; set; }

        public S4JState State { get; set; }

        public JsArray()
        {
            Text = "";
            Array = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {
            this.Array.Add(Child);
        }

        public void AppendCharsToToken(IList<Char> Chars)
        {
            //this.Text += Char;
        }

        public void CommitToken()
        {
            IsCommited = true;
        }

        public void BuildJson(StringBuilder Builder)
        {
            Builder.Append("[");
            Int32 i = 0;
            foreach (var child in Array)
            {
                if (i > 0) Builder.Append(",");
                child.BuildJson(Builder);
                i++;
            }
            Builder.Append("]");
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }
    }
}
