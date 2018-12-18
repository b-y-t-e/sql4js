using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public interface Is4jToken
    {
        Is4jToken Parent { get; set; }

        S4JState State { get; set; }

        bool IsKey { get; set; }

        bool IsCommited { get; set; }

        void AddChildToToken(Is4jToken Child);

        void AppendCharToToken(Char Char);

        void CommitToken();

        void BuildJson(StringBuilder Builder);

        string ToJson();
    }

    public class S4JTokenStack : List<Is4jToken>
    {
        public void Push(Is4jToken Token)
        {
            this.Add(Token);
        }

        public void Pop()
        {
            this.RemoveAt(this.Count - 1);
        }

        public Is4jToken Peek()
        {
            return this.LastOrDefault();
        }

        public Is4jToken PeekNonValue()
        {
            return this.
                LastOrDefault(t => !t.State.IsSimpleValue && !t.State.IsComment);
        }
    }
}
