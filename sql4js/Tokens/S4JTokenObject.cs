﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Else.HttpService.Helpers;

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
                if (child.IsObjectSingleKey)
                {
                    lastKey = null;
                    if (child is S4JTokenFunction fun)
                    {
                        if (fun.IsEvaluated)
                        {
                            /*lastKey = UniConvert.ToString(fun.Result);
                            if (lastKey != null)
                                result[lastKey] = null;*/
                        }
                    }
                    else if (child is S4JTokenObjectContent obj)
                    {
                        foreach (var keyAndVal in obj.GetParameters())
                        {
                            result[keyAndVal.Key] = keyAndVal.Value;
                        }
                    }
                    else
                    {
                        /*lastKey = UniConvert.ToString(child.ToJson().ParseJsonOrText());
                        if (lastKey != null)
                            result[lastKey] = null;*/
                    }
                }
                else if (child.IsObjectKey)
                {
                    lastKey = null;
                    if (child is S4JTokenFunction fun)
                    {
                        if (fun.IsEvaluated)
                        {
                            lastKey = UniConvert.ToString(fun.Result);
                            if (lastKey != null)
                                result[lastKey] = null;
                        }
                    }
                    else
                    {
                        lastKey = UniConvert.ToString(child.ToJson().ParseJsonOrText());
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
            Builder.Append("{");
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
