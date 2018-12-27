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

        async private Task Evaluate(Is4jToken node)
        {
            if (node == null)
                return;

            if (node.State.StateType == EStateType.FUNCTION)
            {
                S4JFunction function = node as S4JFunction;
                string code = function.ToJsonWithoutGate();
                object result = await CSharpScript.EvaluateAsync(code);
                // node.
                String text = JsonSerializer.SerializeJson(result);
                function.Children.Clear();
                function.Children.Add(new S4JTextValue() { Text = text });
            }
            else
            {
                foreach (Is4jToken child in node.Children)
                {
                    await Evaluate(child);
                }
            }
        }
    }

}
