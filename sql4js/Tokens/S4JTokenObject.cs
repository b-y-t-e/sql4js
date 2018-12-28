using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenObject : S4JToken
    {
        public S4JTokenObject()
        {
            Children = new List<S4JToken>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            Dictionary<String, Object> result = new Dictionary<string, object>();
            String lastKey = null;
            foreach (S4JToken child in Children)
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

        public override void BuildJson(StringBuilder Builder)
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
    }

    /*public class JsObjectKeyValue
    {
        public Object Key { get; set; }

        public Object Value { get; set; }

        public Boolean OnlyKey { get; set; }
    }*/
}
