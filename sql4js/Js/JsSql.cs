﻿using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class JsSql : Is4jToken
    {
        // public Object Value { get; set; }

        public Is4jToken Parent { get; set; }

        public String Text { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public JsSql()
        {
            Text = "";
        }

        public void AddChildToToken(Is4jToken Child)
        {
            throw new NotImplementedException();
            // Value = Child;
        }

        public void AppendCharToToken(Char Char)
        {
            if (this.Text.Length == 0 && System.Char.IsWhiteSpace(Char))
                return;
            this.Text += Char;
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
