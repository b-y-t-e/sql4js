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

        private List<S4JState> items;

        public S4JStateBag()
        {
            items = new List<S4JState>();

            RootState = new S4JState()
            {
                Priority = -1000,
                StateType = EStateType.S4J,
                AllowedStatesNames = new List<EStateType?>()
                {
                    null
                },
                Gates = new List<S4JStateGate>()
                {

                }
            };
            this.items.Add(RootState);

            ////////////////////////////////

            S4JState sS4jComment = new S4JState()
            {
                Priority = -999,
                StateType = EStateType.S4J_COMMENT,
                AllowedStatesNames = new List<EStateType?>()
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
            };
            this.items.Add(sS4jComment);

            ////////////////////////////////

            /*S4JState sS4jSqlExpression = new S4JState()
            {
                StateType = EStateType.SQL,
                IsValue = true,
                AllowedStatesNames = new List<EStateType?>()
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
            };
            this.items.Add(sS4jSqlExpression);*/

            ////////////////////////////////

            //S4JState sSqlComment = new S4JState()
            //{
            //    StateType = EStateType.SQL_COMMENT,
            //    AllowedStatesNames = new List<EStateType?>()
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
            //};
            //this.items.Add(sSqlComment);

            ////////////////////////////////

            S4JState sS4jQuotation = new S4JState()
            {
                Priority = -998,
                StateType = EStateType.S4J_QUOTATION,
                IsValue = true,
                AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.S4J_QUOTATION
                },
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "'".ToCharArray(),
                        End = "'".ToCharArray(),
                        Inner = "\\".ToCharArray()
                    }
                }
            };
            this.items.Add(sS4jQuotation);

            ////////////////////////////////

            this.items.Add(new S4JState()
            {
                Priority = 1000,
                StateType = EStateType.S4J_ARRAY,
                IsCollection = true,
                AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.S4J_COMMENT,
                    EStateType.S4J_QUOTATION,
                    EStateType.S4J_COMA,
                    EStateType.S4J_OBJECT,
                    EStateType.S4J_ARRAY,
                    EStateType.S4J_SIMPLE_VALUE,
                    EStateType.FUNCTION
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

            this.items.Add(new S4JState()
            {
                Priority = 1001,
                StateType = EStateType.S4J_OBJECT,
                IsCollection = true,
                AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.S4J_COMMENT,
                    EStateType.S4J_QUOTATION,
                    EStateType.S4J_OBJECT,
                    EStateType.S4J_ARRAY,
                    EStateType.S4J_VALUE_DELIMITER,
                    EStateType.S4J_COMA,
                    EStateType.S4J_SIMPLE_VALUE,
                    EStateType.FUNCTION
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

            S4JState sS4jDelimiter = new S4JState()
            {
                Priority = 1002,
                StateType = EStateType.S4J_VALUE_DELIMITER,
                AllowedStatesNames = new List<EStateType?>()
                {
                    null
                },
                IsDelimiter = true,
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = ":".ToCharArray()
                    }
                }
            };
            this.items.Add(sS4jDelimiter);

            ////////////////////////////////

            S4JState sS4jSeparator = new S4JState()
            {
                Priority = 1003,
                StateType = EStateType.S4J_COMA,
                AllowedStatesNames = new List<EStateType?>()
                {
                    null
                },
                IsComa = true,
                Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = ",".ToCharArray()
                    }
                }
            };
            this.items.Add(sS4jSeparator);

            ////////////////////////////////

            S4JState sS4jValue = new S4JState()
            {
                Priority = 1004,
                StateType = EStateType.S4J_SIMPLE_VALUE,
                AllowedStatesNames = new List<EStateType?>()
                {
                    null
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
            };
            this.items.Add(sS4jValue);
            this.ValueState = sS4jValue;
        }

        public IEnumerator<S4JState> GetEnumerator()
        {
            return (IEnumerator<S4JState>)items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)items.GetEnumerator();
        }
     
        public void Add(S4JState State)
        {
            if (State == null)
                return;

            this.items.Add(State);
            this.items = this.items.OrderBy(i => i.Priority).ToList();
        }
    }
}
