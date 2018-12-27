using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JParser
    {
        public List<S4JStateFunction> Functions { get; set; }

        public S4JParser()
        {
            Functions = new List<S4JStateFunction>();
        }

        public Is4jToken Parse(String Text)
        {
            IList<char> chars = Text.Trim().ToCharArray();

            S4JStateBag stateBag = new S4JStateBag();
            if (Functions != null)
                foreach (var function in Functions)
                {
                    stateBag.Add(function);
                    stateBag.Add(function.BracketsDefinition);
                    stateBag.Add(function.CommentDefinition);
                    stateBag.Add(function.QuotationDefinition);
                }

            S4JTokenStack valueStack = new S4JTokenStack();
            S4JTokenRoot rootVal = new S4JTokenRoot()
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
                        if (stackEvent.Chars != null)
                        {
                            currentVal.AppendCharsToToken(stackEvent.Chars);
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

                            if (!stackEvent.State.IsComment)
                                prevVal.AddChildToToken(newToken);
                        }

                        if (stackEvent.Chars != null)
                        {
                            Is4jToken currentVal = valueStack.Peek();
                            currentVal.AppendCharsToToken(stackEvent.Chars);
                        }
                    }
                }
            }

            while (valueStack.Count > 0)
            {
                Is4jToken currentVal = valueStack.Peek();
                if (currentVal != null)
                    currentVal.CommitToken();
                valueStack.Pop();
            }


            return rootVal.Children.Single() as Is4jToken;
        }

        private static IEnumerable<S4JStateStackEvent> Analyse(IList<char> code, int index, S4JStateBag StateBag, S4JTokenStack stateStack) // S4JStateStack stateStack)
        {
            // sprawdzamy zakończenie stanu
            Is4jToken prevTokenNonValue = stateStack.PeekNonValue();
            Is4jToken prevToken = stateStack.Peek();
            if (prevTokenNonValue != null)
            {
                IList<char> end = prevTokenNonValue?.State?.Gate?.End;
                if (S4JParserHelper.Is(code, index, end))
                {
                    if (prevToken.State.IsSimpleValue)
                    {
                        yield return new S4JStateStackEvent()
                        {
                            NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + (end == null ? 0 : (end.Count - 1)) + 1),
                            State = prevToken?.State,
                            Popped = true,
                            Chars = null
                        };
                    }

                    if (prevToken != prevTokenNonValue ||
                        !prevToken.State.IsSimpleValue)
                    {
                        yield return new S4JStateStackEvent()
                        {
                            NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + (end == null ? 0 : (end.Count - 1)) + 1),
                            State = stateStack.Peek()?.State,
                            Popped = true,
                            Chars = end
                        };
                        yield break;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }

            prevToken = stateStack.Peek();
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
                                    Chars = null
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
                                    Chars = new[] { code[index] }
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
                                    Chars = null
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
                            NewIndex =
                                state.IsCollection ?
                                    S4JParserHelper.SkipWhiteSpaces(code, index + (matchedGate?.Start == null ? 0 : (matchedGate.Start.Count - 1)) + 1) :
                                    index + (matchedGate?.Start == null ? 0 : (matchedGate.Start.Count - 1)) + 1, 
                            State = newState,
                            Pushed = true,
                            Chars = matchedGate?.Start ?? new[] { code[index] }
                        };

                        yield break;
                    }
                }
            }

            yield return new S4JStateStackEvent()
            {
                NewIndex = null,
                State = stateStack.Peek()?.State,
                Chars = new[] { code[index] }
            };
        }
    }

}
