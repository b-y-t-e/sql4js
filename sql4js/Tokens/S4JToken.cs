using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sql4js.Helpers;
using System.Collections;
using sql4js.Helpers.CoreHelpers;
using sql4js.Parser;

namespace sql4js.Tokens
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

        public S4JToken()
        {
        }

        //////////////////////////////////////////////////

        public virtual Dictionary<String, Object> GetParameters()
        {
            return null;
        }

        public virtual void AddChildToToken(S4JToken Child)
        {
            Children.Add(Child);
        }

        public virtual Int32 IndexOfChild(S4JToken OldChild)
        {
            Int32 childIndex = this.Children.IndexOf(OldChild);
            return childIndex;
        }

        public virtual bool ReplaceChild(S4JToken OldChild, IList<S4JToken> NewChilds)
        {
            Int32 childIndex = IndexOfChild(OldChild);
            return ReplaceChild(childIndex, NewChilds);
        }

        public virtual bool ReplaceChild(Int32 childIndex, IList<S4JToken> NewChilds)
        {
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
                txtVal.Commit();

            //CalculateIsSingleKey(this, lastChild);
            // CalculateIsSingleKey(this.Parent, this);
        }

        public virtual void OnPop()
        {
            // CalculateIsSingleKey(this.Parent, this);

            S4JToken lastChild = this.Children.LastOrDefault();
            if (lastChild is S4JTokenTextValue txtVal)
                txtVal.Commit();

            CalculateIsSingleKey(this, lastChild);
        }

        private void CalculateIsSingleKey(S4JToken Token, S4JToken Item)
        {
            if (Item == null)
                return;

            // ustalenie IsSingleKey = true
            // próba określenia czy token jest w obiekcie
            // oraz czy jest 'kluczem bez wartosci' 
            if (Token is S4JTokenObject ||
                Token is S4JTokenParameters)
            {
                S4JToken prevChild = null;
                foreach (S4JToken child in Token.Children)
                {
                    if (!child.IsObjectKey)
                    {
                        if (prevChild == null ||
                            prevChild.IsObjectKey == false)
                        {
                            child.MarkAsSingleObjectKey();
                        }

                        else if (prevChild != null &&
                                 prevChild.IsObjectKey == true)
                        {
                            child.MarkAsObjectValue();
                        }
                    }

                    prevChild = child;
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
            ////////////////////////////////////

            ////////////////////////////////////

            ////////////////////////////////////

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

        public S4JToken Clone()
        {
            S4JToken newToken = (S4JToken)this.MemberwiseClone();
            newToken.State = newToken.State?.Clone();
            newToken.Children = newToken.Children.Select(i => i.Clone()).ToList();
            // newToken.Parent = newToken.Parent?.Clone();
            return newToken;
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

    public class S4JFieldDescription
    {
        public String Name { get; set; }

        public S4JFieldType Type { get; set; }

        public Boolean IsRequired { get; set; }

        public static S4JFieldDescription Parse(String Name, String Type)
        {
            Type = (Type ?? "").Trim().ToLower();
            Name = (Name ?? "").Trim();
            if (string.IsNullOrEmpty(Name))
                throw new Exception("Field name should not be empty!");
            Boolean isRequired = Type.EndsWith("!");
            Type = Type.TrimEnd('!');
            S4JFieldType fieldType = (S4JFieldType)Enum.Parse(typeof(S4JFieldType), Type, true);
            return new S4JFieldDescription()
            {
                Name = Name,
                IsRequired = isRequired,
                Type = fieldType
            };
        }

        public Object Validate(Object Value)
        {
            if (Type == S4JFieldType.ANY)
                return Value;

            if (IsRequired && Value == null)
                throw new S4JNullParameterException("Parameter " + Name + " cannot be null");
            
            if (Value != null && Type == S4JFieldType.BOOL)
                if (!MyTypeHelper.IsBoolean(Value.GetType()))
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type boolean");

            if (Value != null && Type == S4JFieldType.DATETIME)
                if (!MyTypeHelper.IsDateTime(Value.GetType()))
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type datetime");

            if (Value != null && Type == S4JFieldType.FLOAT)
                if (!MyTypeHelper.IsNumeric(Value.GetType()))
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type float");

            if (Value != null && Type == S4JFieldType.INT)
                if (!MyTypeHelper.IsInteger(Value.GetType()))
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type integer");

            if (Value != null && Type == S4JFieldType.STRING)
                if (!MyTypeHelper.IsString(Value.GetType()))
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type string");

            if (Type == S4JFieldType.ARRAY)
                if (Value == null)
                {
                    return new List<Object>();
                }
                else if (!(Value is IList))
                {
                    //if (MyTypeHelper.IsClass(Value.GetType()) || Value is IDictionary<String, Object>)
                    //    return new List<Object>() { Value };
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type array");
                }

            if (Value != null && Type == S4JFieldType.OBJECT)
                if (!(MyTypeHelper.IsClass(Value.GetType()) || Value is IDictionary<String, Object>) || Value is IList)
                    throw new S4JInvalidParameterTypeException("Parameter " + Name + " should be of type object");

            return Value;
        }

        internal object ToJson()
        {
            return Type.ToString().ToLower();
        }
    }


    [Serializable]
    public class S4JNullParameterException : Exception
    {
        public S4JNullParameterException() { }
        public S4JNullParameterException(string message) : base(message) { }
        public S4JNullParameterException(string message, Exception inner) : base(message, inner) { }
        protected S4JNullParameterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class S4JInvalidParameterTypeException : Exception
    {
        public S4JInvalidParameterTypeException() { }
        public S4JInvalidParameterTypeException(string message) : base(message) { }
        public S4JInvalidParameterTypeException(string message, Exception inner) : base(message, inner) { }
        protected S4JInvalidParameterTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public enum S4JFieldType
    {
        ANY,
        STRING,
        //TIMESPAN,
        INT,
        BOOL,
        //DOUBLE,
        FLOAT,
        DATETIME,
        ARRAY,
        OBJECT
    }
}
