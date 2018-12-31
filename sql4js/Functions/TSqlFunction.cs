using Database;
using ProZ.Base.Helpers;
using sql4js.Executor;
using sql4js.Parser;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace sql4js.Functions
{
    public class TSqlFunction : S4JStateFunction
    {
        public TSqlFunction(string ConnectionString) :
            this("sqlserver", ConnectionString)
        {

        }

        public TSqlFunction(string aliasName, string ConnectionString) :
            base(aliasName, ConnectionString)
        {
            Priority = 0;
            BracketsDefinition = new TSqlBrackets();
            CommentDefinition = new TSqlComment();
            QuotationDefinition = new TSqlQuotation();
            Evaluator = new TSqlEvaluator();
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

    public class TSqlQuotation : S4JState
    {
        public TSqlQuotation()
        {
            Priority = 2;
            StateType = EStateType.FUNCTION_QUOTATION;
            AllowedStatesNames = new List<EStateType?>()
            {

            };
            IsValue = true;
            IsQuotation = true;
            Gates = new List<S4JStateGate>()
                {
                    new S4JStateGate()
                    {
                        Start = "'".ToCharArray(),
                        End = "'".ToCharArray(),
                        Inner = "'".ToCharArray(),
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

    public class TSqlEvaluator : IEvaluator
    {
        public async Task<Object> Evaluate(S4JToken token, IDictionary<String, object> variables)
        {
            S4JTokenFunction functionToken = token as S4JTokenFunction;
            S4JStateFunction functionState = token.State as S4JStateFunction;

            MyQuery query = new MyQuery();

            foreach (KeyValuePair<string, object> keyAndVal in variables)
            {
                query.Append($"declare @{keyAndVal.Key} {TSqlHelper.GetSqlType(keyAndVal.Value?.GetType())};\n");

                if (keyAndVal.Value != null)
                {
                    query.Append($"set @{keyAndVal.Key} = {{0}};\n", keyAndVal.Value);
                }
            }

            query.Append(functionToken.ToJsonWithoutGate());

            using (SqlConnection con = new SqlConnection(functionState.Source))
            {
                var result = con.SelectItems(query.ToString());
                return result;
            }
        }
    }

    public static class TSqlHelper
    {
        public static string GetSqlType(Type csType)
        {
            if (csType != null)
            {
                if (MyTypeHelper.IsInteger(csType))
                    return "int";

                else if (MyTypeHelper.IsNumeric(csType))
                    return "real";

                else if (MyTypeHelper.IsDateTime(csType))
                    return "datetime";

                else if (MyTypeHelper.IsTimeSpan(csType))
                    return "datetime";
            }

            return "nvarchar(max)";
        }
    }
}
