﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
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
using sql4js.Database;

namespace sql4js.Functions
{
    public class CSharpFunction : S4JStateFunction
    {
        public CSharpFunction() :
            this("c#")
        {

        }

        public CSharpFunction(string aliasName) :
            base(aliasName)
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
            AllowedStateTypes = new [] 
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

    public class CSharpBrackets : S4JState
    {
        public CSharpBrackets()
        {
            Priority = 3;
            StateType = EStateType.FUNCTION_BRACKETS;
            AllowedStateTypes = new [] 
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

    public class CSharpEvaluatorGlobals
    {
        public dynamic Globals { get; set; }

        public CSharpEvaluatorGlobals()
        {
            Globals = new ExpandoObject();
        }
    }

    public class CSharpEvaluator : IEvaluator
    {
        public async Task<Object> Evaluate(S4JExecutor Executor, S4JToken token, IDictionary<String, object> variables)
        {
            S4JTokenFunction function = token as S4JTokenFunction;
            StringBuilder code = new StringBuilder();

            CSharpEvaluatorGlobals globals = new CSharpEvaluatorGlobals();
            IDictionary<string, object> globaVariables = globals.Globals as IDictionary<string, object>;
            // var globalObject = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> keyAndVal in variables)
            {
                globaVariables[keyAndVal.Key] = keyAndVal.Value;
                code.Append("var ").Append(keyAndVal.Key).Append(" = ").Append("Globals.").Append(keyAndVal.Key).Append(";");
            }

            dynamic dbProxy = new ExpandoObject();
            foreach (var source in Executor.Sources)
                (dbProxy as IDictionary<string, object>)[source.Key] = new DbApi(source.Value);
            globaVariables["db"] = dbProxy;
            code.Append("var ").Append("db").Append(" = ").Append("Globals.").Append("db").Append(";");

            code.Append(function.ToJsonWithoutGate());

            var refs = new List<MetadataReference>{
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location)};

            var imports = ScriptOptions.Default.
                WithImports(
                    "System",
                    "System.Text",
                    "System.Linq",
                    "System.Collections",
                    "System.Collections.Generic").
                WithReferences(refs);

            object result = await CSharpScript.EvaluateAsync(
                code.ToString(),
                imports,
                globals);

            return result;
        }
    }
}
