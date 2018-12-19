using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public static class S4JParser
    {
        public static Is4jToken Parse(String Text)
        {
            IList<char> chars = Text.Trim().ToCharArray();

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
                            }
                        }

                        if (stackEvent.State.StateType == EStateType.S4J_COMA)
                        {
                            if (valueStack.Peek() != null)
                            {
                                valueStack.Peek().CommitToken();
                            }
                        }

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
            // sprawdzamy zakończenie stanu
            Is4jToken prevTokenNonValue = stateStack.PeekNonValue();
            if (prevTokenNonValue != null)
            {
                IList<char> end = prevTokenNonValue?.State?.Gate?.End;
                if (S4JParserHelper.Is(code, index, end))
                {
                    yield return new S4JStateStackEvent()
                    {
                        NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + (end == null ? 0 : (end.Count - 1)) + 1),
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
                        if (!state.IsComment)
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
                        }
                        else
                        {

                        }

                        S4JState newState = state.Clone();
                        newState.Gate = matchedGate;
                        yield return new S4JStateStackEvent()
                        {
                            // NewIndex = null,
                            NewIndex = index + (matchedGate?.End == null ? 0 : (matchedGate.End.Count - 1)) + 1, //S4JParserHelper.SkipWhiteSpaces(code, index + (matchedGate?.End == null ? 0 : (matchedGate.End.Count - 1)) + 1),
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
