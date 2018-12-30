using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Else.HttpService.Helpers;

namespace sql4js.Parser
{
    public class S4JTokenParameters : S4JToken
    {
        public S4JTokenParameters()
        {
            Children = new List<S4JToken>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            Dictionary<String, Object> result = new Dictionary<string, object>();
            String lastKey = null;
            foreach (S4JToken child in Children)
            {
                if (child.IsObjectKey)
                {
                    lastKey = null;
                    /*if (child is S4JTokenFunction fun)
                    {
                        if (fun.IsEvaluated)
                        {
                            lastKey = UniConvert.ToString(fun.Result);
                            if (lastKey != null)
                                result[lastKey] = null;
                        }
                    }
                    else*/
                    {
                        lastKey = UniConvert.ToString(child.ToJson().ParseJsonOrText());
                        if (lastKey != null)
                            result[lastKey] = null;
                    }
                }
                else if (lastKey != null)
                {
                    /*if (child is S4JTokenFunction fun)
                    {
                        if (fun.IsEvaluated)
                        {
                            Object val = fun.Result; // child.ToJson().DeserializeJson();
                            result[lastKey] = val;
                            //throw new NotImplementedException();
                        }
                    }
                    else*/
                    if (child.State.IsValue)
                    {
                        try
                        {
                            Object val = child.ToJson().ParseJsonOrText();
                            result[lastKey] = val;
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            return result;
        }

        public override void BuildJson(StringBuilder Builder)
        {
            Builder.Append("(");
            Int32 i = 0;
            Boolean prevWasKey = true;
            foreach (S4JToken child in Children)
            {
                if (!prevWasKey)
                {
                    Builder.Append(",");
                }

                if (child.IsObjectKey)
                {
                    child.BuildJson(Builder);
                    prevWasKey = true;
                    Builder.Append(":");
                }
                else
                {
                    child.BuildJson(Builder);
                    prevWasKey = false;
                }

                i++;
            }
            Builder.Append(")");
        }

        public override void Commit()
        {
            base.Commit();
        }

        public override void OnPop()
        {
            base.OnPop();

            // if (Parent is S4JTokenRoot root)
            {
                var root = Parent;

                root.Attributes = new Dictionary<string, object>();

                string lastKey = null;
                foreach (S4JToken child in this.Children)
                {
                    Object val = child.ToJson().ParseJsonOrText();

                    if (child.IsObjectSingleKey)
                    {
                        lastKey = null;
                        root.Attributes[UniConvert.ToString(val)] = null;
                    }
                    else if (child.IsObjectKey)
                    {
                        lastKey = null;
                        lastKey = UniConvert.ToString(val);
                        root.Attributes[lastKey] = null;
                    }
                    else if (child.IsObjectValue)
                    {
                        root.Attributes[lastKey] = val;
                    }
                }
            }

            Parent.ReplaceChild(this, null);
        }
    }


    [Serializable]
    public class InvalidParametersException : Exception
    {
        public InvalidParametersException() { }
        public InvalidParametersException(string message) : base(message) { }
        public InvalidParametersException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParametersException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /*public class JsObjectKeyValue
    {
        public Object Key { get; set; }

        public Object Value { get; set; }

        public Boolean OnlyKey { get; set; }
    }*/
}
