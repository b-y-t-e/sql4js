using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public abstract class S4JToken
    {
        public S4JToken Parent { get; set; }

        public List<S4JToken> Children { get; set; }

        public S4JState State { get; set; }

        //////////////////////////////////////////////////

        public bool IsKey { get; set; }

        public bool IsSingleKey { get; set; }

        public bool IsCommited { get; set; }

        //////////////////////////////////////////////////

        public virtual Dictionary<String, Object> GetParameters()
        {
            return null;
        }

        public virtual void AddChildToToken(S4JToken Child)
        {
            Children.Add(Child);
        }

        public virtual void AppendCharsToToken(IList<Char> Chars)
        {

        }

        public virtual bool ReplaceChild(S4JToken OldChild, IList<S4JToken> NewChilds)
        {
            Int32 childIndex = this.Children.IndexOf(OldChild);
            if (childIndex < 0)
                return false;

            this.Children.RemoveAt(childIndex);
            foreach (S4JToken newChild in NewChilds)
            {
                this.Children.Insert(childIndex, newChild);
                childIndex++;
            }

            return true;
        }

        public virtual void CommitToken()
        {
            IsCommited = true;

            // próba określenia czy token jest w obiekcie
            // oraz czy jest 'kluczem bez wartosci' 
            if (Parent is S4JTokenObject)
            {
                if (!this.IsKey && !(this is S4JTokenComment))
                {
                    int indexInParent = Parent.Children.IndexOf(this);
                    S4JToken prevChild = null;
                    for (var i = indexInParent - 1; i >= 0; i--)
                    {
                        S4JToken child = Parent.Children[i];
                        if (child is S4JTokenComment)
                            continue;
                        prevChild = child;
                        break;
                    }
                    if (prevChild == null || prevChild.IsKey == false)
                    {
                        this.IsSingleKey = true;
                    }
                }
            }
        }

        public virtual void BuildJson(StringBuilder Builder)
        {
            foreach (var child in Children)
                child.BuildJson(Builder);
        }

        public virtual string ToJson()
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

    public class S4JTokenStack : List<S4JToken>
    {
        public void Push(S4JToken Token)
        {
            this.Add(Token);
        }

        public void Pop()
        {
            this.RemoveAt(this.Count - 1);
        }

        public S4JToken Peek()
        {
            return this.LastOrDefault();
        }

        public S4JToken PeekNonValue()
        {
            return this.
                LastOrDefault(t => !t.State.IsSimpleValue); // && !t.State.IsComment);
        }
    }
}
