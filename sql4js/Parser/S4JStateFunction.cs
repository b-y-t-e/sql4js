using sql4js.Executor;
using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JStateFunction : S4JState
    {
        public String Name { get; set; }
        
        public S4JState BracketsDefinition { get; set; }

        public S4JState CommentDefinition { get; set; }

        public S4JState QuotationDefinition { get; set; }

        public IEvaluator Evaluator { get; set; }

        public S4JStateFunction(String Name)
        {
            this.Name = Name;
            this.IsValue = true;
            this.IsFunction = true;
            this.Priority = 0;
            this.StateType = EStateType.FUNCTION;
            this.AllowedStatesNames = new List<EStateType?>()
                {
                    EStateType.FUNCTION_COMMENT,
                    EStateType.FUNCTION_BRACKETS,
                };
            this.Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = (Name + "(").ToCharArray(),
                        End = ")".ToCharArray()
                    }
                };
        }
    }
}
