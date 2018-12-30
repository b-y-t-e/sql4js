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

        private IList<Object> GetManyObjectsFromResult(Object value, Boolean AnaliseSubValues = true)
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
                if (AnaliseSubValues)
                {
                    foreach (Object subValue in (ICollection)value)
                        list.AddRange(GetManyObjectsFromResult(subValue, false));
                }
                else
                {
                    list.Add(value);
                }
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

        private List<Object> GetListOfSingleObjectsFromResult(Object value, Boolean AnalyseSubValues = true)
        {
            if (value == null)
                return null;

            List<Object> list = new List<object>();

            if (MyTypeHelper.IsPrimitive(value.GetType()))
                list.Add(value);

            else if (value is IDictionary<String, Object> dict)
            {
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

            return list;
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
