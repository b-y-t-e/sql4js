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

        public S4JToken Parse(String Text)
        {
            IList<char> chars = Text.Trim().ToCharArray();

            S4JStateBag stateBag = new S4JStateBag();
            if (Functions != null)
            {
                foreach (S4JStateFunction function in Functions)
                {
                    stateBag.Add(function);
                    stateBag.Add(function.BracketsDefinition);
                    stateBag.Add(function.CommentDefinition);
                    stateBag.Add(function.QuotationDefinition);
                }
            }

            S4JTokenStack valueStack = new S4JTokenStack();
            S4JTokenRoot rootVal = new S4JTokenRoot()
            {
                State = stateBag.RootState
            };
            valueStack.Push(rootVal);

            Int32 startIndex = S4JParserHelper.SkipWhiteSpaces(chars, 0) ?? Int32.MaxValue;

            for (int i = startIndex; i < chars.Count; i++)
            {
                foreach (S4JStateStackEvent stackEvent in Analyse(chars, i, stateBag, valueStack))
                {
                    if (stackEvent.NewIndex != null)
                        i = stackEvent.NewIndex.Value - 1;

                    // zdjęcie ze stosu
                    if (stackEvent.Popped)
                    {
                        S4JToken currentVal = valueStack.Peek(); // PeekNonValue();
                        if (stackEvent.Chars != null)
                        {
                            currentVal.AppendCharsToToken(stackEvent.Chars);
                        }

                        // zatwierdzenie tokena
                        currentVal = valueStack.Peek();
                        if (currentVal != null)
                        {
                            currentVal.Commit();
                        }

                        valueStack.Pop();
                    }

                    else
                    {
                        if (stackEvent.State.StateType == EStateType.S4J_VALUE_DELIMITER)
                        {
                            if (valueStack.Peek() != null)
                            {
                                valueStack.Peek().MarkLastChildAsObjectKey();
                                valueStack.Peek().Commit();
                            }
                        }

                        else if (stackEvent.State.StateType == EStateType.S4J_COMA)
                        {
                            if (valueStack.Peek() != null)
                            {
                                valueStack.Peek().Commit();
                            }
                        }

                        else
                        {

                            if (stackEvent.Pushed &&
                                stackEvent.State.IsSimpleValue == false)
                            {
                                S4JToken prevVal = valueStack.Peek();
                                S4JToken newToken = new S4JTokenFactory().To_token(stackEvent.State);

                                valueStack.Push(newToken);

                                newToken.Parent = prevVal;

                                if (!stackEvent.State.IsComment)
                                    prevVal.AddChildToToken(newToken);
                            }

                            if (stackEvent.Chars != null)
                            {
                                if (stackEvent.State.IsSimpleValue)
                                {
                                }

                                S4JToken currentVal = valueStack.Peek();
                                currentVal.AppendCharsToToken(stackEvent.Chars);
                            }

                        }
                    }
                }
            }

            while (valueStack.Count > 0)
            {
                S4JToken currentVal = valueStack.Peek();
                if (currentVal != null)
                    currentVal.Commit();
                valueStack.Pop();
            }


            return rootVal.Children.Single() as S4JToken;
        }

        private static IEnumerable<S4JStateStackEvent> Analyse(IList<char> code, int index, S4JStateBag StateBag, S4JTokenStack stateStack) // S4JStateStack stateStack)
        {
            // sprawdzamy zakończenie stanu
            // S4JToken prevTokenNonValue = stateStack.PeekNonValue();
            S4JToken prevToken = stateStack.Peek();
            if (GetStateEnd(code, index, StateBag, prevToken) != null)
            {
                Int32 nextIndex = index + (prevToken.State.Gate.End == null ? 0 : (prevToken.State.Gate.End.Count - 1)) + 1;

                yield return new S4JStateStackEvent()
                {
                    NewIndex = S4JParserHelper.SkipWhiteSpaces(code, nextIndex),
                    State = prevToken.State,
                    Popped = true,
                    // Chars = end
                };

                yield break;
            }
            
            prevToken = stateStack.Peek();
            S4JState state = GetStateBegin(code, index, StateBag, prevToken);
            if (state != null)
            {
                Int32 nextIndex = index + (state.Gate?.Start == null ? 0 : (state.Gate.Start.Count - 1)) + 1;

                yield return new S4JStateStackEvent()
                {
                    // NewIndex = null,
                    NewIndex = state.IsQuotation ?
                            nextIndex:
                            // state.IsCollection ?
                            S4JParserHelper.SkipWhiteSpaces(code, nextIndex),
                    //   index + (matchedGate?.Start == null ? 0 : (matchedGate.Start.Count - 1)) + 1,
                    State = state,
                    Pushed = true,
                    // Chars = matchedGate?.Start ?? new[] { code[index] }
                };
            }
            else
            {
                Int32? newIndex = null;

                // pominiecie białych znaków do nastepnego 'stanu'
                // tylko jesli nie nie jestesmy w cytacie
                if (!prevToken.State.IsQuotation &&
                    !prevToken.State.IsComment)
                {
                    newIndex = S4JParserHelper.SkipWhiteSpaces(code, index + 1);
                    if (newIndex != null)
                    {
                        S4JState stateBegin = GetStateBegin(code, newIndex.Value, StateBag, prevToken);
                        S4JState stateEnd = GetStateEnd(code, newIndex.Value, StateBag, prevToken);

                        if (stateBegin != null || stateEnd != null)
                        {
                            newIndex = newIndex;
                        }
                        else
                        {
                            newIndex = null;
                        }
                    }
                    else
                    {
                        newIndex = Int32.MaxValue;
                    }
                }

                yield return new S4JStateStackEvent()
                {
                    NewIndex = newIndex,
                    State = stateStack.Peek()?.State,
                    Chars = new[] { code[index] }
                };
            }
        }

        private static S4JState GetStateEnd(IList<char> code, Int32 index, S4JStateBag StateBag, S4JToken prevToken)
        {
            if (prevToken == null)
                return null;

            IList<char> end = prevToken?.State?.Gate?.End;  // prevTokenNonValue?.State?.Gate?.End;
            if (S4JParserHelper.Is(code, index, end))
            {
                return prevToken?.State;
                /*yield return new S4JStateStackEvent()
                {
                    NewIndex = S4JParserHelper.SkipWhiteSpaces(code, index + (end == null ? 0 : (end.Count - 1)) + 1),
                    State = stateStack.Peek()?.State,
                    Popped = true,
                    // Chars = end
                };
                */
            }

            return null;
        }

        private static S4JState GetStateBegin(IList<char> code, Int32 index, S4JStateBag StateBag, S4JToken prevToken)
        {
            if (prevToken == null)
                return null;

            foreach (S4JState state in StateBag)
            {
                // sprawdzamy rozpoczecie stanu
                if (prevToken == null ||
                    prevToken.State.IsAllowed(state))
                {

                    Boolean isAllowed = false;
                    S4JStateGate matchedGate = null;

                    // IsSimpleValue -> bedzie zawsze sprawdzany na końcu
                    /*if (state.IsSimpleValue)
                    {
                        isAllowed = true;
                    }
                    else*/
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
                        // if (!state.IsComment)
                        {
                            S4JState newState = state.Clone();
                            newState.Gate = matchedGate;
                            return newState;
                        }
                        break;
                        /*if (!state.IsComment)
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

                        }*/

                        /* S4JState newState = state.Clone();
                         newState.Gate = matchedGate;
                         yield return new S4JStateStackEvent()
                         {
                             // NewIndex = null,
                             NewIndex =
                                     // state.IsCollection ?
                                     S4JParserHelper.SkipWhiteSpaces(code, index + (matchedGate?.Start == null ? 0 : (matchedGate.Start.Count - 1)) + 1),
                             //   index + (matchedGate?.Start == null ? 0 : (matchedGate.Start.Count - 1)) + 1,
                             State = newState,
                             Pushed = true,
                             // Chars = matchedGate?.Start ?? new[] { code[index] }
                         };

                         yield break;*/
                    }
                }
            }
            return null;
        }
    }

}
