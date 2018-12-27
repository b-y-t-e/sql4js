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

        public S4JExecutor(S4JParser Parser)
        {
            this.Parser = Parser;
        }

        async public Task<Is4jToken> Execute(String Text)
        {
            Is4jToken tree = Parser.Parse(Text);
            await Evaluate(tree);
            return tree;
        }

        async private Task Evaluate(Is4jToken token)
        {
            if (token == null)
                return;

            if (token.State.StateType == EStateType.FUNCTION)
            {
                S4JTokenFunction function = token as S4JTokenFunction;
                object result = await function.Evaluator?.Evaluate(token);

                function.Result = result;
                String text = JsonSerializer.SerializeJson(result);
                function.Children.Clear();
                function.Children.Add(new S4JTokenTextValue() { Text = text });
            }
            else
            {
                foreach (Is4jToken child in token.Children)
                {
                    await Evaluate(child);
                }
            }
        }
    }

    public interface IEvaluator
    {
        Task<Object> Evaluate(Is4jToken node);
    }

    public class ExecutionTree
    {
        public Is4jToken Root;

        public ExecutionTreeNode RootExecutionNode;

        public void Build(Is4jToken Root)
        {

        }
    }

    public class ExecutionTreeNode
    {
        public Is4jToken Node;

        public Dictionary<String, Object> Variables;

        public List<Is4jToken> Dependencies;

        public ExecutionTreeNode()
        {
            Variables = new Dictionary<string, object>();
            Dependencies = new List<Is4jToken>();
        }
    }
}
