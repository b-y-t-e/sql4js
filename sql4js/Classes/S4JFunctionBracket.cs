﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JFunctionBracket : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public List<Is4jToken> Children { get; set; }
        
        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public S4JFunctionBracket()
        {
            Children = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {
            Children.Add(Child);
        }

        public void AppendCharsToToken(IList<Char> Chars)
        {
            Is4jToken lastChild = this.Children.LastOrDefault();
            if (!(lastChild is S4JTextValue))
            {
                lastChild = new S4JTextValue();
                this.Children.Add(lastChild);
            }
            lastChild.AppendCharsToToken(Chars);
        }

        public void CommitToken()
        {
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
