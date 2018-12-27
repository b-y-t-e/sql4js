using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Functions
{
    public class TSqlFunction : S4JStateFunction
    {
        public TSqlFunction() :
            base("sql")
        {
            Priority = 0;
            BracketsDefinition = new TSqlBrackets();
            CommentDefinition = new TSqlComment();
        }
    }

    public class TSqlComment : S4JState
    {
        public TSqlComment()
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
                        Start = "--".ToCharArray(),
                        End = "\n".ToCharArray()
                    }
                };
        }
    }

    public class TSqlBrackets : S4JState
    {
        public TSqlBrackets()
        {
            IsValue = true;
            StateType = EStateType.FUNCTION_BRACKETS;
            AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.FUNCTION_BRACKETS,
                    EStateType.FUNCTION_COMMENT
                };
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
}
