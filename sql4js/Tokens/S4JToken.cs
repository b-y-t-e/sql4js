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

        public bool IsObjectKey { get; set; }

        public bool IsObjectValue { get; set; }

        public bool IsObjectSingleKey { get; set; }

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

        public virtual bool ReplaceChild(S4JToken OldChild, IList<S4JToken> NewChilds)
        {
            Int32 childIndex = this.Children.IndexOf(OldChild);
            if (childIndex < 0)
                return false;

            this.Children.RemoveAt(childIndex);

            if (NewChilds != null)
                foreach (S4JToken newChild in NewChilds)
                {
                    this.Children.Insert(childIndex, newChild);
                    childIndex++;
                }

            return true;
        }

        public virtual void AppendCharsToToken(IList<Char> Chars)
        {
            S4JToken lastChild = this.Children.LastOrDefault();
            if (!(lastChild is S4JTokenTextValue) || lastChild.IsCommited)
            {
                lastChild = new S4JTokenTextValue();
                this.Children.Add(lastChild);
            }
            lastChild.AppendCharsToToken(Chars);
        }

        public virtual void MarkLastChildAsObjectValue()
        {
            S4JToken lastChild = this.Children.LastOrDefault();
            if (lastChild == null)
                return;

            lastChild.MarkAsObjectValue();
        }

        public virtual void MarkAsObjectValue()
        {
            this.IsObjectValue = true;
            this.IsObjectSingleKey = false;
            this.IsObjectKey = false;
        }

        public virtual void MarkLastChildAsObjectKey()
        {
            S4JToken lastChild = this.Children.LastOrDefault();
            if (lastChild == null)
                return;

            lastChild.MarkAsObjectKey();
        }

        public virtual void MarkAsObjectKey()
        {
            this.IsObjectKey = true;
            this.IsObjectSingleKey = false;
        }

        public virtual void MarkAsSingleObjectKey()
        {
            this.IsObjectKey = false;
            this.IsObjectSingleKey = true;
        }

        public virtual void Commit()
        {
            // IsCommited = true;

            S4JToken lastChild = this.Children.LastOrDefault();
            if (lastChild is S4JTokenTextValue txtVal)
            {
                txtVal.Commit();
            }

            // ustalenie IsSingleKey = true
            // próba określenia czy token jest w obiekcie
            // oraz czy jest 'kluczem bez wartosci' 
            if (Parent is S4JTokenObject)
            {
                if (!this.IsObjectKey && !(this is S4JTokenComment))
                {
                    S4JToken prevChild = null;

                    int indexInParent = Parent.Children.IndexOf(this);
                    for (var i = indexInParent - 1; i >= 0; i--)
                    {
                        S4JToken child = Parent.Children[i];
                        if (child is S4JTokenComment)
                            continue;

                        prevChild = child;
                        break;
                    }

                    if (prevChild == null ||
                        prevChild.IsObjectKey == false)
                    {
                        this.MarkAsSingleObjectKey();
                    }

                    else if (prevChild != null &&
                             prevChild.IsObjectKey == true)
                    {
                        this.MarkAsObjectValue();
                    }
                }
            }
        }

        public virtual string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }

        public virtual void BuildJson(StringBuilder Builder)
        {
            if (State.Gate != null)
                foreach (var ch in State.Gate.Start)
                    Builder.Append(ch);
            else if (State.Gates.Count > 0)
                foreach (var ch in State.Gates[0].Start)
                    Builder.Append(ch);

            ////////////////////////////////////

            foreach (var child in Children)
                child.BuildJson(Builder);

            ////////////////////////////////////

            if (State.Gate != null)
                foreach (var ch in State.Gate.End)
                    Builder.Append(ch);
            else if (State.Gates.Count > 0)
                foreach (var ch in State.Gates[0].End)
                    Builder.Append(ch);
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

        /*public S4JToken PeekNonValue()
        {
            return this.
                LastOrDefault(t => !t.State.IsValue); // && !t.State.IsComment);
        }*/
    }
}
