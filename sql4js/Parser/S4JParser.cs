using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public static class S4JParser
    {
        public static Is4jToken Parse(String Text)
        {
            IList<char> chars = Text.ToCharArray();

            S4JStateBag stateBag = new S4JStateBag();

            S4JTokenStack valueStack = new S4JTokenStack();
            JsRoot rootVal = new JsRoot()
            {
                State = stateBag.RootState
            };
            valueStack.Push(rootVal);

            for (int i = 0; i < chars.Count; i++)
            {
                foreach (S4JStateStackEvent stackEvent in Analyse(chars, i, stateBag, valueStack))
                {
                    if (stackEvent.NewIndex != null)
                        i = stackEvent.NewIndex.Value - 1;

                    // zdjęcie ze stosy
                    if (stackEvent.Popped)
                    {
                        Is4jToken currentVal = valueStack.PeekNonValue();
                        if (stackEvent.Char != null)
                        {
                            currentVal.AppendCharToToken(stackEvent.Char.Value);
                        }

                        // zatwierdzenie tokena
                        currentVal = valueStack.Peek();
                        if (currentVal != null)
                        {
                            currentVal.CommitToken();
                        }

                        valueStack.Pop();
                    }

                    else
                    {
                        if (stackEvent.State.StateType == EStateType.S4J_VALUE_DELIMITER)
                        {
                            if (valueStack.Peek() != null)
                            {
                                valueStack.Peek().IsKey = true;
                                valueStack.Peek().CommitToken();
                                /*if (stackEvent.Char != null)
                                {
                                    Is4jToken currentVal = valueStack.Peek();
                                    currentVal.AppendCharToToken(stackEvent.Char.Value);
                                }*/
                            }
                        }

                        if (stackEvent.State.StateType == EStateType.S4J_COMA)
                        {
                            if (valueStack.Peek() != null)
                            {
                                valueStack.Peek().CommitToken();
                                /*if (stackEvent.Char != null)
                                {
                                    Is4jToken currentVal = valueStack.Peek();
                                    currentVal.AppendCharToToken(stackEvent.Char.Value);
                                }*/
                            }
                        }


                        // np. 'a : 123' (znalezienie znaku ':')
                        /*if (stackEvent.State.StateType == EStateType.S4J_VALUE_DELIMITER)
                        {
                            if (valueStack.Peek()?.State.IsValue == false)
                            {
                                Is4jToken prevVal = valueStack.Peek();
                                valueStack.Push(new JsValue()
                                {
                                    State = stackEvent.State
                                });
                                prevVal.AddChildToToken(valueStack.Peek());
                            }

                            if (valueStack.Peek() is JsValue val)
                            {
                                val.IsKey = true;

                                if (stackEvent.Char != null)
                                {
                                    Is4jToken currentVal = valueStack.Peek();
                                    currentVal.AppendCharToToken(stackEvent.Char.Value);
                                }
                            }
                        }

                        // np. '{a : 1, b : 2}' (znalezienie znaku ',')
                        if (stackEvent.State.StateType == EStateType.S4J_COMA)
                        {
                            if (valueStack.Peek()?.State.IsValue == false)
                            {
                                Is4jToken prevVal = valueStack.Peek();
                                valueStack.Push(new JsValue()
                                {
                                    State = stackEvent.State
                                });
                                prevVal.AddChildToToken(valueStack.Peek());
                            }

                            {
                                Is4jToken prevVal = valueStack.Peek();
                                valueStack.Push(new JsValue()
                                {
                                    State = stackEvent.State
                                });
                                prevVal.AddChildToToken(valueStack.Peek());
                            }
                        }
                        
                        */

                        if (stackEvent.Pushed)
                        {
                            Is4jToken prevVal = valueStack.Peek();
                            Is4jToken newToken = new S4JTokenFactory().To_token(stackEvent.State);
                            valueStack.Push(newToken);

                            newToken.Parent = prevVal;
                            prevVal.AddChildToToken(newToken);
                        }

                        if (stackEvent.Char != null)
                        {
                            Is4jToken currentVal = valueStack.Peek();
                            currentVal.AppendCharToToken(stackEvent.Char.Value);
                        }
                    }
                }
            }

            return rootVal.Value as Is4jToken;
        }

        private static IEnumerable<S4JStateStackEvent> Analyse(IList<char> code, int index, S4JStateBag StateBag, S4JTokenStack stateStack) // S4JStateStack stateStack)
        {
            /*char ch = code[index];
            ch = ch;

            if(ch == '}')
            {

            }*/

            // sprawdzamy zakończenie stanu
            Is4jToken prevTokenNonValue = stateStack.PeekNonValue();
            if (prevTokenNonValue != null)
            {
                if (S4JParserHelper.Is(code, index, prevTokenNonValue?.State?.Gate?.End))
                {
                    yield return new S4JStateStackEvent()
                    {
                        NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + 1),
                        State = stateStack.Peek()?.State,
                        Pushed = false,
                        Popped = true,
                        Char = code[index]
                    };
                    yield break;
                }
            }

            Is4jToken prevToken = stateStack.Peek();
            foreach (S4JState state in StateBag)
            {
                // sprawdzamy rozpoczecie stanu
                if (prevToken == null ||
                    prevToken.State.IsAllowed(state))
                {
                    Boolean isAllowed = false;
                    S4JStateGate matchedGate = null;

                    // IsSimpleValue -> bedzie zawsze sprawdzany na końcu
                    if (state.IsSimpleValue)
                    {
                        isAllowed = true;
                    }
                    else
                    {
                        // pobszukiwanie rozpoczecia stanu
                        foreach (S4JStateGate gate in state.Gates)
                        {
                            if (S4JParserHelper.Is(code, index, gate.Start))
                            {
                                matchedGate = gate.Clone();
                                isAllowed = true;
                                break;
                            }
                        }
                    }

                    if (isAllowed)
                    {
                        if (state.IsComa || state.IsDelimiter)
                        {
                            yield return new S4JStateStackEvent()
                            {
                                NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + 1),
                                State = state,
                                Char = null
                            };

                            yield break;
                        }

                        // jeśli poprzedni stan to 'prosta wartość' 
                        // i aktualny to też 'prosta wartość'
                        // to chcemy dodać znak do aktualnego stanu na stosie
                        if (prevToken.State.IsSimpleValue &&
                            !prevToken.IsCommited &&
                            state.IsSimpleValue)
                        {
                            yield return new S4JStateStackEvent()
                            {
                                NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + 1),
                                State = stateStack.Peek()?.State,
                                Char = code[index]
                            };

                            yield break;
                        }

                        // jeśli poprzedni stan to 'prosta wartość' 
                        // i aktualny to nie 'prosta wartość'
                        // to sciagamy aktualny stan ze stosu 
                        // i dodajemy nowy stany na stos
                        if (prevToken.State.IsSimpleValue &&
                            (prevToken.IsCommited || !state.IsSimpleValue))
                        {
                            yield return new S4JStateStackEvent()
                            {
                                NewIndex = null,
                                State = prevToken?.State,
                                Popped = true,
                                Char = null
                            };
                        }

                        S4JState newState = state.Clone();
                        newState.Gate = matchedGate;
                        yield return new S4JStateStackEvent()
                        {
                            NewIndex = null,
                            // NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + 1),
                            State = newState,
                            Pushed = true,
                            Char = code[index]
                        };

                        yield break;
                    }
                }
            }

            yield return new S4JStateStackEvent()
            {
                NewIndex = null,
                State = stateStack.Peek()?.State,
                Char = code[index]
            };
        }

        public static S4JResult Parse2(String Text)
        {
            throw new NotSupportedException();

            if (Text == null)
                return null;

            S4JStateBag stateBag = new S4JStateBag();


            S4JResult result = new S4JResult();

            StringBuilder buffer = new StringBuilder();
            StringBuilder simplifiedBuffer = new StringBuilder();
            StringBuilder expressionBuffer = new StringBuilder();

            char[] chars = Text.ToCharArray();
            /*S4JStateStack stateStack = stateBag.BuildStack(chars);

            return result;*/

            string expresionTestVariable = "@EXP@";

            IList<char> s4jExpression = "\"".ToCharArray();
            IList<char> s4jInnerExpression = "\\\"".ToCharArray();
            bool insideS4jExpression = false;

            IList<char> jsQuotations = "'".ToCharArray();
            IList<char> jsInnerQuotations = "\\'".ToCharArray();
            bool insideJsQuotation = false;

            IList<char> s4jCommentStart = "/*".ToCharArray();
            IList<char> s4jCommentEnd = "*/".ToCharArray();
            bool insideS4jComment = false;

            // StringBuilder buffer = new StringBuilder();
            // StringBuilder simplifiedBuffer = new StringBuilder();
            // StringBuilder expressionBuffer = new StringBuilder();
            // char[] chars = Text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                bool s4jexpressionStarted = false;
                bool s4jexpressionEnded = false;

                if (!insideS4jExpression)
                {
                    if (Is(chars, i, jsQuotations) &&
                        !Is(chars, i, jsInnerQuotations))
                    {
                        insideJsQuotation = !insideJsQuotation;
                    }
                }

                if (!insideJsQuotation)
                {
                    if (Is(chars, i, s4jExpression) &&
                        !Is(chars, i, s4jInnerExpression))
                    {
                        insideS4jExpression = !insideS4jExpression;
                        if (insideS4jExpression)
                        {
                            s4jexpressionStarted = true;
                        }
                        else
                        {
                            s4jexpressionEnded = true;
                        }
                    }
                }

                if (s4jexpressionEnded)
                {
                    simplifiedBuffer.Append(expresionTestVariable);
                }

                if (insideS4jExpression)
                {
                    if (!s4jexpressionStarted)
                    {
                        expressionBuffer.Append(ch);
                    }
                }
                else
                {
                    if (!s4jexpressionEnded)
                    {
                        buffer.Append(ch);
                        simplifiedBuffer.Append(ch);
                    }
                }
            }

            result.SimplifiedScriptAsText = simplifiedBuffer.ToString();

            return result;
        }

        public static bool Is(IList<char> chars, int index, IList<char> toFindChars)
        {
            bool result = false;
            var j = toFindChars.Count - 1;
            var i = index;
            if (chars.Count > 0)
            {
                for (; ; i--, j--)
                {
                    if (j < 0)
                    {
                        result = true;
                        break;
                    }

                    if (i < 0)
                    {
                        break;
                    }

                    char ch = chars[i];
                    char toFindCh = toFindChars[j];
                    if (ch != toFindCh)
                    {
                        break;
                    }
                }
            }
            return result;
        }
    }

    public class S4JResult
    {
        public Object Value { get; set; }

        public Dictionary<String, S4JExpression> Expressions { get; set; }

        public String ScriptAsText { get; set; }

        public String SimplifiedScriptAsText { get; set; }

        public S4JResult()
        {
            Expressions = new Dictionary<string, S4JExpression>();
        }

        public String GetJson()
        {
            return JsonSerializer.SerializeJson(this.Value);
        }
    }

    public class S4JList : List<Object>
    {

    }

    public class S4JExpression
    {

    }

    public class S4JObject : Dictionary<Object, Object>
    {

    }
}
