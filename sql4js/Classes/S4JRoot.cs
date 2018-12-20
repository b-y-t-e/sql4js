using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JRoot : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public Is4jToken Value { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public S4JRoot()
        {

        }

        public void AddChildToToken(Is4jToken Child)
        {
            Value = Child;
        }
        
        public void AppendCharsToToken(IList<Char> Chars)
        {

        }

        public void CommitToken()
        {
            IsCommited = true;
        }

        public void BuildJson(StringBuilder Builder)
        {
            Value.BuildJson(Builder);
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }
    }
}
