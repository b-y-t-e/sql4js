using Microsoft.CodeAnalysis.CSharp.Scripting;
using sql4js.Helpers;
using sql4js.Parser;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using sql4js.Helpers.CoreHelpers;
using sql4js.Tokens;
using sql4js.Classes;

namespace sql4js.Executor
{
    public class S4JExecutor
    {
        public S4JParser Parser { get; private set; }

        public Sources Sources { get; private set; }

        public Object Result { get; private set; }

        public S4JExecutor(S4JParser Parser)
        {
            this.Parser = Parser;
            this.Sources = new Sources();
        }

        async public Task<S4JToken> ExecuteWithJsonParameters(String MethodDefinitionAsJson, params String[] ParametersAsJson)
        {
            Object[] parameters = null;

            if (ParametersAsJson != null)
            {
                parameters = ParametersAsJson.
                    Select(p => JsonToDynamicDeserializer.Deserialize(p)).
                    ToArray();
            }

            return await ExecuteWithParameters(MethodDefinitionAsJson, parameters);
        }

        async public Task<S4JToken> ExecuteWithParameters(String MethodDefinitionAsJson, params Object[] Parameters)
        {
            S4JToken methodDefinition = Parser.Parse(MethodDefinitionAsJson);
            return await ExecuteWithParameters(methodDefinition, Parameters);
        }

        async public Task<S4JToken> ExecuteWithParameters(S4JToken MethodDefinition, params Object[] Parameters)
        {
            if (MethodDefinition is S4JTokenRoot root)
            {
                if (Parameters != null)
                {
                    Int32 index = 0;
                    foreach (var key in root.Parameters.Keys.ToArray())
                    {
                        object parameterValue = null;
                        if (index < Parameters.Length)
                            parameterValue = Parameters[index];

                        S4JFieldDescription fieldDescription = null;
                        root.ParametersDefinitions.TryGetValue(key, out fieldDescription);

                        if (fieldDescription != null)
                            fieldDescription.Validate(parameterValue);

                        if (index < Parameters.Length)
                            root.Parameters[key] = Parameters[index];
                        index++;
                    }
                }
            }

            await Evaluate(MethodDefinition);

            if (MethodDefinition is S4JTokenRoot)
                return MethodDefinition.Children.LastOrDefault();

            return MethodDefinition;
        }

        async private Task Evaluate(S4JToken token)
        {
            if (token == null)
                return;

            if (token.State.StateType == EStateType.FUNCTION)
            {
                IDictionary<String, object> variables = GetExecutiongVariables(token);
                S4JTokenFunction function = token as S4JTokenFunction;
                object result = await function.Evaluator?.Evaluate(this, token, variables);

                function.IsEvaluated = true;
                function.Result = result;

                if (function.Parent is S4JTokenObject objectToken &&
                    token.IsObjectSingleKey)
                {
                    if (objectToken.Parent is S4JTokenArray)
                    {
                        if (objectToken.Children.Count == 1)
                        {
                            Int32 indexOfFun = objectToken.IndexOfChild(function);

                            IList<S4JToken> tokensFromResult = ConvertToToken(
                                GetManyObjectsFromResult(result)).ToArray();

                            objectToken.Parent.ReplaceChild(
                                objectToken,
                                tokensFromResult);
                        }
                        else
                        {
                            Int32 indexOfFun = objectToken.IndexOfChild(function);

                            IList<S4JToken> tokensFromResult = ConvertToManyTokens(
                                GetManyObjectsFromResult(result)).ToArray();

                            List<S4JToken> newTokens = new List<S4JToken>();
                            foreach (S4JToken tokenFromResult in tokensFromResult)
                            {
                                S4JToken newObjectToken = objectToken.Clone();

                                newObjectToken.ReplaceChild(
                                    indexOfFun,
                                    new[] { tokenFromResult });

                                newTokens.Add(newObjectToken);
                            }

                            objectToken.Parent.ReplaceChild(
                                objectToken,
                                newTokens);
                        }
                    }
                    else
                    {
                        IList<S4JToken> tokens = ConvertToTokens(
                            GetSingleObjectFromResult(result)).ToArray();

                        objectToken.ReplaceChild(
                            function,
                            tokens);
                    }
                }
                else if (function.Parent is S4JTokenArray)
                {
                    IList<S4JToken> tokens = ConvertToToken(
                        GetListOfSingleObjectsFromResult(result)).ToArray();

                    function.Parent.ReplaceChild(
                        function,
                        tokens);
                }
                else
                {
                    IList<S4JToken> tokens = ConvertToTokens(
                        GetSingleAndFirstValueFromResult(result)).ToArray();

                    String text = JsonSerializer.SerializeJson(result);
                    function.Children.Clear();
                    function.Children.AddRange(tokens);
                }
            }
            else
            {
                var children = token.Children.ToArray();
                for (var i = 0; i < children.Length; i++)
                {
                    S4JToken child = children[i];
                    await Evaluate(child);
                }
            }
        }

        private IDictionary<String, object> GetExecutiongVariables(S4JToken token)
        {
            Dictionary<String, object> variables = new Dictionary<string, object>();
            {
                S4JToken parentToken = token;
                while (parentToken != null)
                {
                    Dictionary<string, object> parentParameters = parentToken.GetParameters();
                    if (parentParameters != null)
                    {
                        foreach (KeyValuePair<string, object> keyAndVal in parentParameters)
                        {
                            if (!variables.ContainsKey(keyAndVal.Key))
                            {
                                variables[keyAndVal.Key] = keyAndVal.Value;
                            }
                        }
                    }
                    parentToken = parentToken.Parent;
                }
            }
            return variables;
        }

        private IEnumerable<S4JToken> ConvertToTokens(IDictionary<String, Object> Dictionary)
        {
            if (Dictionary == null)
                yield break;

            yield return new S4JTokenObjectContent()
            {
                Value = Dictionary,
                Text = Dictionary.SerializeJsonNoBrackets(),
                //IsKey = true,
                IsObjectSingleKey = true,
                IsCommited = true,
                State = new S4JState() { StateType = EStateType.S4J_OBJECT_CONTENT, IsValue = true, IsSimpleValue = true }
            };
        }

        private IEnumerable<S4JToken> ConvertToToken(IList<Object> List)
        {
            if (List == null || List.Count == 0)
                yield break;

            yield return new S4JTokenObjectContent()
            {
                Value = List,
                Text = List.SerializeJsonNoBrackets(),
                //IsKey = true,
                IsObjectSingleKey = true,
                IsCommited = true,
                State = new S4JState() { StateType = EStateType.S4J_OBJECT_CONTENT, IsValue = true, IsSimpleValue = true }
            };
        }

        private IEnumerable<S4JToken> ConvertToManyTokens(IList<Object> List)
        {
            if (List == null)
                yield break;

            foreach (Object item in List)
                yield return new S4JTokenObjectContent()
                {
                    Value = item,
                    Text = item.SerializeJsonNoBrackets(),
                    //IsKey = true,
                    IsObjectSingleKey = true,
                    IsCommited = true,
                    State = new S4JState() { StateType = EStateType.S4J_OBJECT_CONTENT, IsValue = true, IsSimpleValue = true }
                };
        }

        private IEnumerable<S4JToken> ConvertToTokens(Object Value)
        {
            //if (Value == null)
            //    yield break;

            yield return new S4JTokenTextValue()
            {
                Text = Value.SerializeJson(),
                IsObjectSingleKey = true,
                IsCommited = true,
                State = new S4JState() { StateType = EStateType.S4J_TEXT_VALUE, IsValue = true, IsSimpleValue = true }
            };
        }

        private IList<Object> GetManyObjectsFromResult(Object value, Boolean AnalyseSubValues = true)
        {
            if (value == null)
                return null;

            List<Object> list = new List<object>();

            if (MyTypeHelper.IsPrimitive(value.GetType()))
                list.Add(value);

            else if (value is IDictionary<String, Object>)
            {
                list.Add(value);
            }

            else if (value is ICollection)
            {
                if (AnalyseSubValues)
                {
                    foreach (Object subValue in (ICollection)value)
                        list.AddRange(GetManyObjectsFromResult(subValue, false));
                }
                else
                {
                    list.Add(value);
                }
            }

            else if (value.GetType().IsClass)
            {
                list.Add(value);
            }

            return list;
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

            else if (value is IDictionary<String, Object>)
            {
                return (IDictionary<String, Object>)value;
            }

            else if (value is ICollection)
            {
                foreach (Object subValue in (ICollection)value)
                    return GetSingleObjectFromResult(subValue);
            }

            else if (value.GetType().IsClass)
            {
                return ReflectionHelper.ToDictionary(value);
            }

            return null;
        }

        private List<Object> GetListOfSingleObjectsFromResult(Object value, Boolean AnalyseSubValues = true)
        {
            if (value == null)
                return null;

            List<Object> list = new List<object>();

            if (MyTypeHelper.IsPrimitive(value.GetType()))
                list.Add(value);

            else if (value is IDictionary<String, Object> dict)
            {
                if (dict.Count > 0)
                    list.Add(dict.First().Value);
            }

            else if (value is ICollection)
            {
                if (AnalyseSubValues)
                {
                    foreach (Object subValue in (ICollection)value)
                        list.AddRange(GetListOfSingleObjectsFromResult(subValue, false));
                }
                else
                {
                    list.Add(value);
                }
            }

            else if (value.GetType().IsClass)
            {
                var dictForValue = ReflectionHelper.ToDictionary(value);
                if (dictForValue.Count > 0)
                    list.Add(dictForValue.First().Value);
            }

            return list;
        }

        private Object GetSingleAndFirstValueFromResult(Object value)
        {
            if (value == null)
                return null;

            if (MyTypeHelper.IsPrimitive(value.GetType()))
                return value;

            else if (value is IDictionary<String, Object> dict)
            {
                return dict.Count > 0 ? dict.FirstOrDefault().Value : null;
            }

            else if (value is ICollection)
            {
                foreach (Object subValue in (ICollection)value)
                    return GetSingleAndFirstValueFromResult(subValue);
            }

            else if (value.GetType().IsClass)
            {
                var dictForValue = ReflectionHelper.ToDictionary(value);
                return dictForValue.Count > 0 ? dictForValue.FirstOrDefault().Value : null;
            }

            return null;
        }
    }

    public interface IEvaluator
    {
        Task<Object> Evaluate(S4JExecutor Executor, S4JToken node, IDictionary<String, object> variables);
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
