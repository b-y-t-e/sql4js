using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenObject : Is4jToken
    {
        public Is4jToken Parent { get; set; }

        public List<Is4jToken> Children { get; set; }

        public String Text { get; set; }

        public Boolean IsKey { get; set; }

        public bool IsCommited { get; set; }

        public S4JState State { get; set; }

        public S4JTokenObject()
        {
            Text = "";
            Children = new List<Is4jToken>();
        }

        public Dictionary<String, Object> GetResult()
        {
            Dictionary<String, Object> result = new Dictionary<string, object>();
            String lastKey = null;
            foreach (Is4jToken child in Children)
            {
                if (child.IsKey)
                {
                    lastKey = null;
                    if (child is S4JTokenFunction fun)
                    {
                        if (fun.IsEvaluated)
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        lastKey = child.ToJson();
                        if (lastKey != null)
                            result[lastKey] = null;
                    }
                }
                else if (lastKey != null)
                {
                    if (child is S4JTokenFunction fun)
                    {
                        if (fun.IsEvaluated)
                        {
                            Object val = fun.Result; // child.ToJson().DeserializeJson();
                            result[lastKey] = val;
                            //throw new NotImplementedException();
                        }
                    }
                    else if (child.State.IsValue)
                    {
                        Object val = child.ToJson().DeserializeJson();
                        result[lastKey] = val;
                    }
                }
            }
            return result;
        }

        public void AddChildToToken(Is4jToken Child)
        {
            this.Children.Add(Child);
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
            Builder.Append("{");
            Int32 i = 0;
            Boolean prevWasKey = true;
            foreach (var child in Children)
            {
                if (!prevWasKey) Builder.Append(",");
                child.BuildJson(Builder);
                if (child.IsKey) { prevWasKey = true; Builder.Append(":"); }
                else { prevWasKey = false; }
                i++;
            }
            Builder.Append("}");
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            BuildJson(builder);
            return builder.ToString();
        }
    }

    /*public class JsObjectKeyValue
    {
        public Object Key { get; set; }

        public Object Value { get; set; }

        public Boolean OnlyKey { get; set; }
    }*/
}
