﻿using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTextValue : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public List<Is4jToken> Children { get; set; }

        public String Text { get; set; }
        
        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public S4JTextValue()
        {
            Text = "";
            IsKey = false;
            Children = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {

        }

        public void AppendCharsToToken(IList<Char> Chars)
        {
            foreach (var Char in Chars)
            {
                if (this.Text.Length == 0 && System.Char.IsWhiteSpace(Char))
                    continue;
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
            Builder.Append(Text);
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }
    }
}
