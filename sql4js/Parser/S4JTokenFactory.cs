using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JTokenFactory
    {
        public Is4jToken To_token(S4JState State)
        {
            Is4jToken result = null;

            if (State != null)
            {
                if (State.StateType == EStateType.S4J)
                    result = new JsRoot();

                if (State.StateType == EStateType.S4J_ARRAY)
                    result = new JsArray();

                if (State.StateType == EStateType.S4J_COMMENT)
                    result = new JsComment();

                if (State.StateType == EStateType.S4J_OBJECT)
                    result = new JsObject();

                if (State.StateType == EStateType.S4J_QUOTATION)
                    result = new JsValue();

                //if (State.StateType == EStateType.S4J_SEPARATOR)
                //    result = new JsArray();

                if (State.StateType == EStateType.S4J_VALUE)
                    result = new JsValue();

                //if (State.StateType == EStateType.S4J_VALUE_DELIMITER)
                //    result = new JsArray();

                if (State.StateType == EStateType.SQL)
                    result = new JsSql();

                if (State.StateType == EStateType.SQL_COMMENT)
                    result = new JsSqlComment();
            }

            if (result != null)
                result.State = State;

            return result;
        }
    }
}
