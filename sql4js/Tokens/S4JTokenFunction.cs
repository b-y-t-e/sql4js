using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sql4js.Executor;

namespace sql4js.Tokens
{
    public class S4JTokenFunction : S4JToken
    {
        public Object Result { get; set; }

        public IEvaluator Evaluator { get; set; }

        public Boolean IsEvaluated { get; set; }

        ////////////////////////////////////////////

        public S4JTokenFunction()
        {
            Children = new List<S4JToken>();
        }

        public override Dictionary<String, Object> GetParameters()
        {
            return null;
            // throw new NotImplementedException();
        }

        public override void BuildJson(StringBuilder Builder)
        {
            if (IsEvaluated)
            {
                foreach (var child in Children)
                    child.BuildJson(Builder);
            }
            else
            {
                base.BuildJson(Builder);
            }
        }
    }
}
