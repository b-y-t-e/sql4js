using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JFunction : Is4jToken
    {
        // public Object Value { get; set; }

        public Is4jToken Parent { get; set; }

        public List<Is4jToken> Children { get; set; }

        public String Text { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public S4JFunction()
        {
            Text = "";
            Children = new List<Is4jToken>();
        }

        public void AddChildToToken(Is4jToken Child)
        {
            Children.Add(Child);
        }

        public void AppendCharsToToken(IList<Char> Chars)
        {
            /*foreach (var Char in Chars)
            {
                if (this.Text.Length == 0 && System.Char.IsWhiteSpace(Char))
                    continue;
                this.Text += Char;
            }*/

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
            this.Text = this.Text;
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

        public string ToJsonWithoutGate()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            if (State?.Gate != null)
            {
                if (builder.ToString().StartsWith(new string(State.Gate.Start.ToArray())))
                {
                    builder.Remove(0, State.Gate.Start.Count);
                }
                if (builder.ToString().EndsWith(new string(State.Gate.End.ToArray())))
                {
                    builder.Remove(builder.Length - State.Gate.End.Count, State.Gate.End.Count);
                }
            }
            return builder.ToString();
        }
    }
}
