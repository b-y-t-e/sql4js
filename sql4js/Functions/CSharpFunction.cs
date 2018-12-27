﻿using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Functions
{
    public class CSharpFunction : S4JStateFunction
    {
        public CSharpFunction() :
            base("c")
        {
            Priority = 0;
            BracketsDefinition = new CSharpBrackets();
            CommentDefinition = new CSharpComment();
        }
    }

    public class CSharpComment : S4JState
    {
        public CSharpComment()
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
                        Start = "//".ToCharArray(),
                        End = "\n".ToCharArray()
                    }
                };
        }
    }

    public class CSharpBrackets : S4JState
    {
        public CSharpBrackets()
        {
            StateType = EStateType.FUNCTION_BRACKETS;
            AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.FUNCTION_BRACKETS
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
}
