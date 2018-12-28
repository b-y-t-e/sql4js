using Microsoft.CodeAnalysis.CSharp.Scripting;
using sql4js.Parser;
using System;
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

                /*if (function.Parent is S4JTokenObject)
                {

                }
                else*/
                {
                    String text = JsonSerializer.SerializeJson(result);
                    function.Children.Clear();
                    function.Children.Add(new S4JTokenTextValue() { Text = text });
                }
            }
            else
            {
                foreach (S4JToken child in token.Children)
                {
                    await Evaluate(child);
                }
            }
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
