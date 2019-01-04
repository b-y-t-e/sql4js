using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Else.HttpService.Helpers;

namespace sql4js.Parser
{
    public class S4JTokenRoot : S4JToken
    {
        public S4JTokenRoot()
        {
            Children = new List<S4JToken>();
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
            if (root.Children.Count > 1)
            {
                if (!(root.Children[0] is S4JTokenTextValue))
                {
                    throw new InvalidParametersException();
                }

                S4JTokenTextValue name = root.Children[0] as S4JTokenTextValue;
                root.Name = UniConvert.ToString(name.ToJson().ParseJsonOrText());
                root.ReplaceChild(name, null);
            }
        }
    }
}
