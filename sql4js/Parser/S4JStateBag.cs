using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace sql4js.Parser
{
    public class S4JStateBag : IEnumerable<S4JState> //: List<S4JState>
    {
        public S4JState RootState;

        public S4JState ValueState;

        ////////////////////////////////////////

        private List<S4JState> items;

        private Dictionary<EStateType, List<S4JState>> dict;

        ////////////////////////////////////////

        public S4JStateBag()
        {
            items = new List<S4JState>();
            dict = new Dictionary<EStateType, List<S4JState>>();

            RootState = AddBase(new S4JState()
            {
                Priority = -1000,
                StateType = EStateType.S4J,
                AllowedStateTypes = new[]
                {
                    EStateType.S4J_COMMENT,
                    EStateType.S4J_QUOTATION,
                    EStateType.S4J_OBJECT,
                    EStateType.S4J_ARRAY,
                    EStateType.S4J_VALUE_DELIMITER,
                    EStateType.S4J_COMA,
                    EStateType.S4J_TEXT_VALUE,
                    EStateType.FUNCTION,
                    EStateType.S4J_PARAMETERS,
                    EStateType.S4J_TAG
                },
                Gates = new List<S4JStateGate>()
                {

                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = -999,
                StateType = EStateType.S4J_COMMENT,
                AllowedStateTypes = new[]
                {
                    EStateType.S4J_COMMENT
                },
                IsComment = true,
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
                }
            });

            ////////////////////////////////

            /*AddBase( new S4JState()
            {
                StateType = EStateType.SQL,
                IsValue = true,
                AllowedStatesNames = new [] 
                {
                    EStateType.SQL_COMMENT
                },
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "{{".ToCharArray(),
                        End = "}}".ToCharArray(),
                        Inner = "\\".ToCharArray()
                    }
                }
            });*/

            ////////////////////////////////

            //AddBase(new S4JState()
            //{
            //    StateType = EStateType.SQL_COMMENT,
            //    AllowedStatesNames = new [] 
            //    {
            //        EStateType.SQL_COMMENT
            //    },
            //    IsComment = true,
            //    Gates = new List<S4JStateGate>()
            //    {
            //        new S4JStateGate()
            //        {
            //            Start = "/*".ToCharArray(),
            //            End = "*/".ToCharArray()
            //        },
            //        new S4JStateGate()
            //        {
            //            Start = "--".ToCharArray(),
            //            End = "\n".ToCharArray()
            //        }
            //    }
            //});

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = -998,
                StateType = EStateType.S4J_QUOTATION,
                IsValue = true,
                IsQuotation = true,
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "'".ToCharArray(),
                        End = "'".ToCharArray(),
                        Inner = "\\".ToCharArray()
                    },
                    new S4JStateGate()
                    {
                        Start = "\"".ToCharArray(),
                        End = "\"".ToCharArray(),
                        Inner = "\\".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = 1000,
                StateType = EStateType.S4J_ARRAY,
                IsCollection = true,
                AllowedStateTypes = new[]
                {
                    EStateType.S4J_COMMENT,
                    EStateType.S4J_QUOTATION,
                    EStateType.S4J_COMA,
                    EStateType.S4J_OBJECT,
                    EStateType.S4J_ARRAY,
                    EStateType.S4J_TEXT_VALUE,
                    EStateType.FUNCTION,
                    EStateType.S4J_TAG
                },
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "[".ToCharArray(),
                        End = "]".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = 2000,
                StateType = EStateType.S4J_OBJECT,
                IsCollection = true,
                AllowedStateTypes = new[]
                {
                    EStateType.S4J_COMMENT,
                    EStateType.S4J_QUOTATION,
                    EStateType.S4J_OBJECT,
                    EStateType.S4J_ARRAY,
                    EStateType.S4J_VALUE_DELIMITER,
                    EStateType.S4J_COMA,
                    EStateType.S4J_TEXT_VALUE,
                    EStateType.FUNCTION,
                    EStateType.S4J_TAG
                },
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "{".ToCharArray(),
                        End = "}".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = 2500,
                StateType = EStateType.S4J_PARAMETERS,
                IsCollection = true,
                AllowedStateTypes = new[]
                {
                    EStateType.S4J_COMMENT,
                    EStateType.S4J_QUOTATION,
                    EStateType.S4J_OBJECT,
                    EStateType.S4J_ARRAY,
                    EStateType.S4J_VALUE_DELIMITER,
                    EStateType.S4J_COMA,
                    EStateType.S4J_TEXT_VALUE,
                    EStateType.S4J_TAG
                },
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "(".ToCharArray(),
                        End = ")".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = 2600,
                StateType = EStateType.S4J_TAG,
                IsCollection = true,
                AllowedStateTypes = new[]
                {
                    //EStateType.S4J_COMMENT,
                    //EStateType.S4J_QUOTATION,
                    //EStateType.S4J_OBJECT,
                    //EStateType.S4J_ARRAY,
                    EStateType.S4J_VALUE_DELIMITER,
                    //EStateType.S4J_COMA,
                    EStateType.S4J_TEXT_VALUE
                },
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "#".ToCharArray(),
                        End = " ".ToCharArray()
                    },
                    new S4JStateGate()
                    {
                        Start = "#".ToCharArray(),
                        End = "\n".ToCharArray()
                    },
                    new S4JStateGate()
                    {
                        Start = "#".ToCharArray(),
                        End = "\r".ToCharArray()
                    },
                    new S4JStateGate()
                    {
                        Start = "(#".ToCharArray(),
                        End = ")".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = 3000,
                StateType = EStateType.S4J_VALUE_DELIMITER,
                AllowedStateTypes = new[]
                {
                    EStateType.ANY
                },
                IsDelimiter = true,
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = ":".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            AddBase(new S4JState()
            {
                Priority = 4000,
                StateType = EStateType.S4J_COMA,
                AllowedStateTypes = new[]
                {
                    EStateType.ANY
                },
                IsComa = true,
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = ",".ToCharArray()
                    }
                }
            });

            ////////////////////////////////

            this.ValueState = AddBase(new S4JState()
            {
                Priority = 5000,
                StateType = EStateType.S4J_TEXT_VALUE,
                AllowedStateTypes = new[]
                {
                    EStateType.ANY
                },
                IsValue = true,
                IsSimpleValue = true,
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        End = ",".ToCharArray()
                    },
                    new S4JStateGate()
                    {
                        End = ":".ToCharArray()
                    }
                }
            });

            CorrectItems();
            CorrectOrderOfItems();
        }

        public IEnumerable<S4JState> GetStates(S4JState State)
        {
            if (State?.AllowedStateTypes == null)
                return items;

            return State.AllowedStates; // StateTypes.Select(i => dict[i]).OrderBy(i => i.Priority);

            // return this.items.Where((s, i) => StateTypes.Contains(s.StateType));
        }

        public void Add(params S4JState[] States)
        {
            foreach (S4JState state in States)
            {
                AddBase(state);
                Correct(state);
            }
            CorrectDependent(States);
            CorrectOrderOfItems();
        }

        S4JState AddBase(S4JState state)
        {
            this.items.Add(state);

            if (!this.dict.ContainsKey(state.StateType))
                this.dict[state.StateType] = new List<S4JState>();

            this.dict[state.StateType].Add(state);
            return state;
        }

        void CorrectItems()
        {
            foreach (S4JState state in items)
                Correct(state);
        }

        void CorrectDependent(IEnumerable<S4JState> States)
        {
            foreach (EStateType stateType in States.Select(s => s.StateType))
                foreach (S4JState state in items)
                    if (state.AllowedStateTypes.Contains(stateType))
                        Correct(state);
        }

        void Correct(S4JState State)
        {
            State.AllowedStates = State.AllowedStateTypes == null ? null :
                (State.AllowedStateTypes.
                    Where(i => dict.ContainsKey(i)).
                    SelectMany(i => dict[i]).
                    OrderBy(i => i.Priority).
                    ToArray());
        }

        void CorrectOrderOfItems()
        {
            this.items = this.items.
                OrderBy(i => i.Priority).
                ToList();
        }

        public IEnumerator<S4JState> GetEnumerator()
        {
            return (IEnumerator<S4JState>)items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)items.GetEnumerator();
        }
    }
}
