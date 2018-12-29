using Microsoft.CodeAnalysis.CSharp.Scripting;
using ProZ.Base.Helpers;
using sql4js.Parser;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sql4js.Executor
{
    public class S4JExecutor
    {
        public S4JParser Parser { get; set; }

        public Object Result { get; private set; }

        public S4JExecutor(S4JParser Parser)
        {
            this.Parser = Parser;
        }

        async public Task<S4JToken> Execute(String Text)
        {
            S4JToken tree = Parser.Parse(Text);
            await Evaluate(tree);
            return tree;
        }

        async private Task Evaluate(S4JToken token)
        {
            if (token == null)
                return;

            if (token.State.StateType == EStateType.FUNCTION)
            {
                S4JTokenFunction function = token as S4JTokenFunction;
                object result = await function.Evaluator?.Evaluate(token);

                function.IsEvaluated = true;
                function.Result = result;

                if (function.Parent is S4JTokenObject &&
                    token.IsSingleKey)
                {
                    IList<S4JToken> tokens = ConvertToTokens(
                        GetSingleObjectFromResult(result)).ToArray();

                    function.Parent.ReplaceChild(
                        function,
                        tokens);
                }
                else
                {
                    IList<S4JToken> tokens = ConvertToTokens(
                        GetSingleAndFirstValueFromResult(result)).ToArray();

                    if (function.IsKey)
                    {
                        String text = JsonSerializer.SerializeJson(result);
                        function.Children.Clear();
                        function.Children.AddRange(tokens);
                    }
                    else
                    {
                        String text = JsonSerializer.SerializeJson(result);
                        function.Children.Clear();
                        function.Children.AddRange(tokens);
                    }
                }
            }
            else
            {
                for (var i = 0; i < token.Children.Count; i++)
                {
                    S4JToken child = token.Children[i];
                    await Evaluate(child);
                }
            }
        }

        private IEnumerable<S4JToken> ConvertToTokens(IDictionary<String, Object> Dictionary)
        {
            if (Dictionary == null)
                yield break;

            yield return new S4JTokenObjectContent()
            {
                Text = Dictionary.SerializeJsonNoBrackets(),
                //IsKey = true,
                IsSingleKey = true,
                IsCommited = true,
                State = new S4JState() { StateType = EStateType.S4J_OBJECT_CONTENT, IsValue = true, IsSimpleValue = true }
            };
        }

        private IEnumerable<S4JToken> ConvertToTokens(Object Value)
        {
            if (Value == null)
                yield break;

            yield return new S4JTokenSimpleValue()
            {
                Text = Value.SerializeJson(),
                IsSingleKey = true,
                IsCommited = true,
                State = new S4JState() { StateType = EStateType.S4J_SIMPLE_VALUE, IsValue = true, IsSimpleValue = true }
            };
        }

        private IDictionary<String, Object> GetSingleObjectFromResult(Object value)
        {
            if (value == null)
                return null;

            if (MyTypeHelper.IsPrimitive(value.GetType()))
                return new Dictionary<string, object>
                {
                    { "value", value }
                };

            if (value is IDictionary<String, Object>)
            {
                return (IDictionary<String, Object>)value;
            }

            if (value is ICollection)
            {
                foreach (Object subValue in (ICollection)value)
                    return GetSingleObjectFromResult(subValue);
            }

            return null;
        }

        private Object GetSingleAndFirstValueFromResult(Object value)
        {
            if (value == null)
                return null;

            if (MyTypeHelper.IsPrimitive(value.GetType()))
                return value;

            if (value is IDictionary<String, Object> dict)
            {
                return dict.Count > 0 ? dict.FirstOrDefault().Value : null;
            }

            if (value is ICollection)
            {
                foreach (Object subValue in (ICollection)value)
                    return GetSingleAndFirstValueFromResult(subValue);
            }

            return null;
        }
    }

    public interface IEvaluator
    {
        Task<Object> Evaluate(S4JToken node);
    }

    public class ExecutionTree
    {
        public S4JToken Root;

        public ExecutionTreeNode RootExecutionNode;

        public void Build(S4JToken Root)
        {

        }
    }

    public class ExecutionTreeNode
    {
        public S4JToken Node;

        public Dictionary<String, Object> Variables;

        public List<S4JToken> Dependencies;

        public ExecutionTreeNode()
        {
            Variables = new Dictionary<string, object>();
            Dependencies = new List<S4JToken>();
        }
    }
}
