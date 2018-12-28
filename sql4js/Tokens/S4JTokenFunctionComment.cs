using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenFunctionComment : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public List<Is4jToken> Children { get; set; }

        public String Text { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public S4JTokenFunctionComment()
        {
            Text = "";
            Children = new List<Is4jToken>();
        }

        public Dictionary<String, Object> GetResult()
        {
            return null;
        }

        public void AddChildToToken(Is4jToken Child)
        {

        }

        public void AppendCharsToToken(IList<Char> Chars)
        {
            foreach (var Char in Chars)
            {
                this.Text += Char;
            }
        }

        public void CommitToken()
        {
            this.Text = this.Text.Trim();
            IsCommited = true;
        }

        public void BuildJson(StringBuilder Builder)
        {
            foreach (var child in Children)
                child.BuildJson(Builder);
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }
    }
}
