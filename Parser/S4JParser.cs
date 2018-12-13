using System;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Parser
{
    public class S4JParser
    {
        public S4JParser()
        {
        }

        public static S4JResult Parse(String Text)
        {
            if (Text == null)
                return null;

            S4JResult result = new S4JResult();

            string expresionTestVariable = "@EXP@";

            IList<char> s4jExpression = "\"".ToCharArray();
            IList<char> s4jInnerExpression = "\\\"".ToCharArray();
            bool insides4jExpression = false;

            IList<char> jsQuotations = "'".ToCharArray();
            IList<char> jsInnerQuotations = "\\'".ToCharArray();
            bool insideJsQuotation = false;

            StringBuilder buffer = new StringBuilder();
            StringBuilder simplifiedBuffer = new StringBuilder();
            StringBuilder expressionBuffer = new StringBuilder();
            char[] chars = Text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                bool s4jexpressionStarted = false;
                bool s4jexpressionEnded = false;

                if (!insides4jExpression)
                {
                    if (Is(chars, i, jsQuotations) &&
                        !Is(chars, i, jsInnerQuotations))
                    {
                        insideJsQuotation = !insideJsQuotation;
                    }
                }

                if (!insideJsQuotation)
                {
                    if (Is(chars, i, s4jExpression) &&
                        !Is(chars, i, s4jInnerExpression))
                    {
                        insides4jExpression = !insides4jExpression;
                        if (insides4jExpression)
                        {
                            s4jexpressionStarted = true;
                        }
                        else
                        {
                            s4jexpressionEnded = true;
                        }
                    }
                }

                if (s4jexpressionEnded)
                {
                    simplifiedBuffer.Append(expresionTestVariable);
                }

                if (insides4jExpression)
                {
                    if (!s4jexpressionStarted)
                    {
                        expressionBuffer.Append(ch);
                    }
                }
                else
                {
                    if (!s4jexpressionEnded)
                    {
                        buffer.Append(ch);
                        simplifiedBuffer.Append(ch);
                    }
                }
            }

            result.SimplifiedScriptAsText = simplifiedBuffer.ToString();

            return result;
        }

        public static bool Is(IList<char> chars, int index, IList<char> toFindChars)
        {
            bool result = false;
            var j = toFindChars.Count - 1;
            var i = index;
            if (chars.Count > 0)
            {
                for (; ; i--, j--)
                {
                    if (j < 0)
                    {
                        result = true;
                        break;
                    }

                    if (i < 0)
                    {
                        break;
                    }

                    char ch = chars[i];
                    char toFindCh = toFindChars[j];
                    if (ch != toFindCh)
                    {
                        break;
                    }
                }
            }
            return result;
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
