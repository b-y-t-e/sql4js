using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using sql4js.Executor;
using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sql4js.Functions
{
    public class CSharpFunction : S4JStateFunction
    {
        public CSharpFunction() :
            base("c#")
        {
            Priority = 0;
            BracketsDefinition = new CSharpBrackets();
            CommentDefinition = new CSharpComment();
            QuotationDefinition = new CSharpQuotation();
            Evaluator = new CSharpEvaluator();
        }
    }

    public class CSharpComment : S4JState
    {
        public CSharpComment()
        {
            Priority = 1;
            StateType = EStateType.FUNCTION_COMMENT;
            AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.FUNCTION_COMMENT
                };
            IsComment = true;
            Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "/*".ToCharArray(),
                        End = "*/".ToCharArray()
                    },
                    new S4JStateGate()
                    {
                        Start = "//".ToCharArray(),
                        End = "\n".ToCharArray()
                    }
                };
        }
    }

    public class CSharpQuotation : S4JState
    {
        public CSharpQuotation()
        {
            Priority = 2;
            StateType = EStateType.FUNCTION_QUOTATION;
            AllowedStatesNames = new List<EStateType?>()
            {

            };
            IsValue = true;
            Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "\"".ToCharArray(),
                        End = "\"".ToCharArray(),
                        Inner = "\\".ToCharArray(),
                    },
                    new S4JStateGate()
                    {
                        Start = "'".ToCharArray(),
                        End = "'".ToCharArray(),
                        Inner = "\\".ToCharArray(),
                    }
                };
        }
    }

    public class CSharpBrackets : S4JState
    {
        public CSharpBrackets()
        {
            Priority = 3;
            StateType = EStateType.FUNCTION_BRACKETS;
            AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.FUNCTION_BRACKETS,
                    EStateType.FUNCTION_COMMENT
                };
            IsValue = true;
            Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "(".ToCharArray(),
                        End = ")".ToCharArray()
                    }
                };
        }
    }

    public class CSharpEvaluator : IEvaluator
    {
        public async Task<Object> Evaluate(S4JToken token)
        {
            S4JToken parentToken = token;
            Dictionary<String, object> values = new Dictionary<string, object>();
            while (parentToken != null)
            {
                var parentResult = parentToken.GetParameters();
                if (parentResult != null)
                {
                    foreach (var keyAndVal in parentResult)
                    {
                        if (!values.ContainsKey(keyAndVal.Key))
                        {
                            values[keyAndVal.Key] = keyAndVal.Value;
                        }
                    }
                }
                parentToken = parentToken.Parent;
            }

            S4JTokenFunction function = token as S4JTokenFunction;
            string code = function.ToJsonWithoutGate();

            foreach (var keyAndVal in values)
            {
                if (keyAndVal.Value == null)
                {
                    code = $"object {keyAndVal.Key} = {keyAndVal.Value.SerializeJson()};" + Environment.NewLine + code;
                }
                else
                {
                    code = $"var {keyAndVal.Key} = {keyAndVal.Value.SerializeJson()};" + Environment.NewLine + code;
                }
            }

            var imports = ScriptOptions.Default.WithImports("System", "System.Text", "System.Collections.Generic");

            object result = await CSharpScript.EvaluateAsync(
                code,
                imports);

            function.Result = result;
            //String text = JsonSerializer.SerializeJson(result);
            //function.Children.Clear();
            //function.Children.Add(new S4JTextValue() { Text = text });
            return result;
        }
    }
}
