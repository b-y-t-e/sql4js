using sql4js.Helpers;
using sql4js.Executor;
using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using sql4js.Tokens;
using DynLan;
using DynLan.Classes;
using sql4js.Helpers.CoreHelpers;
using sql4js.Database;

namespace sql4js.Functions
{
    public class DynLanFunction : S4JStateFunction
    {
        public DynLanFunction() :
            this("dynlan")
        {

        }

        public DynLanFunction(string aliasName) :
            base(aliasName)
        {
            Priority = 0;
            BracketsDefinition = new DynLanBrackets();
            CommentDefinition = new DynLanComment();
            QuotationDefinition = new DynLanQuotation();
            Evaluator = new DynLanEvaluator();
        }
    }

    public class DynLanComment : S4JState
    {
        public DynLanComment()
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
                        Start = "#".ToCharArray(),
                        End = "\n".ToCharArray()
                    }
                };
        }
    }

    public class DynLanQuotation : S4JState
    {
        public DynLanQuotation()
        {
            Priority = 2;
            StateType = EStateType.FUNCTION_QUOTATION;
            AllowedStatesNames = new List<EStateType?>()
            {

            };
            IsValue = true;
            IsQuotation = true;
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

    public class DynLanBrackets : S4JState
    {
        public DynLanBrackets()
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

    public class DynLanEvaluatorGlobals
    {
        public dynamic Globals { get; set; }

        public DynLanEvaluatorGlobals()
        {
            Globals = new ExpandoObject();
        }
    }

    public class DynLanEvaluator : IEvaluator
    {
        public async Task<Object> Evaluate(S4JExecutor Executor, S4JToken token, IDictionary<String, object> variables)
        {
            S4JTokenFunction function = token as S4JTokenFunction;
            StringBuilder code = new StringBuilder();

            DynLanEvaluatorGlobals globals = new DynLanEvaluatorGlobals();
            IDictionary<string, object> globaVariables = globals.Globals as IDictionary<string, object>;
            // var globalObject = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> keyAndVal in variables)
            {
                globaVariables[keyAndVal.Key/*.ToUpper()*/] = keyAndVal.Value;
                /*if (keyAndVal.Value == null)
                {
                    code.Append($"object {keyAndVal.Key} = {keyAndVal.Value.SerializeJson()};\n");
                }
                else
                {
                    code.Append($"var {keyAndVal.Key} = {keyAndVal.Value.SerializeJson()};\n");
                }*/
            }

            DynLanDbProxy dbProxy = new DynLanDbProxy();
            foreach (var source in Executor.Sources)
                dbProxy[source.Key] = new DbApi(source.Value);

            globaVariables["db"] = dbProxy;

            code.Append(function.ToJsonWithoutGate());

            // string finalCode = MyStringHelper.AddReturnStatement(code.ToString());

            Object result = new Compiler().
                Compile(code.ToString()).
                Eval(globaVariables);

            return result;
        }
    }

    public class DynLanDbProxy : Dictionary<String, Object>
    {

    }
}
