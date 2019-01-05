using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Else.HttpService.Helpers;

namespace sql4js.Parser
{
    public class S4JTokenRoot : S4JToken
    {
        public String Name { get; set; }

        public Dictionary<String, S4JFieldDescription> ParametersDefinitions { get; set; }

        public Dictionary<String, Object> Parameters { get; set; }

        //////////////////////////////////////////////////

        public S4JTokenRoot()
        {
            Children = new List<S4JToken>();
            Parameters = new Dictionary<string, object>();
            ParametersDefinitions = new Dictionary<string, S4JFieldDescription>();
        }

        public override void AddChildToToken(S4JToken Child)
        {
            base.AddChildToToken(Child);
        }

        public override Dictionary<String, Object> GetParameters()
        {
            return Parameters;
        }

        public override void BuildJson(StringBuilder Builder)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                Builder.Append(Name);

                Builder.Append("(");
                Int32 index = 0;
                if (Parameters != null)
                    foreach (var attr in ParametersDefinitions)
                    {
                        if (index > 0) Builder.Append(",");
                        if (attr.Value == null)
                        {
                            Builder.Append($"{attr.Key}");
                        }
                        else
                        {
                            Builder.Append($"{attr.Key}:{attr.Value.ToJson()}");
                        }
                        index++;
                    }
                Builder.Append(")");
            }

            base.BuildJson(Builder);
        }

        public override void Commit()
        {
            base.Commit();

            S4JTokenRoot root = this;
            /*if (root.Children.Any(c => c is S4JTokenParameters))
            {
                if (root.Children.Count < 3 ||
                    !(root.Children[0] is S4JTokenTextValue) ||
                    !(root.Children[1] is S4JTokenParameters))
                {
                    throw new InvalidParametersException();
                }

                S4JTokenTextValue name = root.Children[0] as S4JTokenTextValue;
                S4JTokenParameters parameters = root.Children[1] as S4JTokenParameters;

                root.Name = UniConvert.ToString(name.ToJson().ParseJsonOrText());

                //root.ReplaceChild(name, null);
                //root.ReplaceChild(parameters, null);
            }
            else*/
            
            {
                if (root.Children.Count > 1 && (root.Children.FirstOrDefault() is S4JTokenTextValue nameToken))
                {
                    root.Name = UniConvert.ToString(nameToken.ToJson().ParseJsonOrText());
                    root.ReplaceChild(nameToken, null);
                }

                if ((root.Children.FirstOrDefault() is S4JTokenParameters parametersToken))
                {
                    root.ParametersDefinitions = new Dictionary<string, S4JFieldDescription>();
                    root.Parameters = new Dictionary<string, object>();

                    string lastKey = null;
                    foreach (S4JToken child in parametersToken.Children)
                    {
                        Object val = child.ToJson().ParseJsonOrText();

                        if (child.IsObjectSingleKey)
                        {
                            lastKey = null;
                            root.ParametersDefinitions[UniConvert.ToString(val)] = null;
                            root.Parameters[UniConvert.ToString(val)] = null;
                        }
                        else if (child.IsObjectKey)
                        {
                            lastKey = null;
                            lastKey = UniConvert.ToString(val);
                            root.ParametersDefinitions[lastKey] = null;
                            root.Parameters[lastKey] = null;
                        }
                        else if (child.IsObjectValue)
                        {
                            root.ParametersDefinitions[lastKey] = S4JFieldDescription.Parse(lastKey, UniConvert.ToString(val));
                            root.Parameters[lastKey] = null;
                        }
                    }
                    root.ReplaceChild(parametersToken, null);
                }

            }
        }
    }
}
