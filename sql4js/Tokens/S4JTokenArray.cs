using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenArray : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public String Text { get; set; }

        public List<Is4jToken> Children { get; set; }

        public S4JState State { get; set; }

        public S4JTokenArray()
        {
            Text = "";
            Children = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {
            this.Children.Add(Child);
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
            foreach (var child in Children)
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
