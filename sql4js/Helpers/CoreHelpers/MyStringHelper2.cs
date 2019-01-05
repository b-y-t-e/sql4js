using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace sql4js.Helpers.CoreHelpers
{
    public static class MyStringHelper
    {
        private static readonly Char[] quotes = new[] { '\'', '"' };

        /*public static String MaxLength(this String Text, Int32 MaxLength)
        {
            Text = Text ?? "";
            return Text.Length > MaxLength ?
                Text.Substring(0, MaxLength) :
                Text;
        }*/

        public static String TrimStart(this String Text, String PrefixToRemove)
        {
            while (Text.StartsWith(PrefixToRemove))
                Text = Text.Substring(PrefixToRemove.Length);
            return Text;
        }

        public static Stream ToStream(this String Text, Encoding Enc = null)
        {
            if (Text == null)
                return null;

            if (Enc == null)
                Enc = Encoding.UTF8;

            return new MemoryStream(Enc.GetBytes(Text));
        }

        public static string ReplaceInsensitive(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static IList<ReplaceInfo> GetReplaceInfos(this string str, string oldValue, StringComparison comparison)
        {
            List<ReplaceInfo> result = new List<ReplaceInfo>();
            //StringBuilder sb = new StringBuilder();

            //int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                result.Add(new ReplaceInfo()
                {
                    Index = index,
                    Len = oldValue.Length
                });
                //sb.Append(str.Substring(previousIndex, index - previousIndex));
                //sb.Append(newValue);
                index += oldValue.Length;

                //previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            //sb.Append(str.Substring(previousIndex));

            return result.OrderByDescending(i => i.Index).ToArray(); // sb.ToString();
        }

        public static String MaxLength(this String Text, Int32 MaxLength)
        {
            Text = Text ?? "";
            return Text.Length > MaxLength ?
                Text.Substring(0, MaxLength) :
                Text;
        }

#if SILVERLIGHT
#else
        public static StringBuilder AppendWithNewLine(
            this StringBuilder scriptCode,
            String Script)
        {
            if (scriptCode.Length > 0 && !scriptCode.EndsWith(Environment.NewLine))
                scriptCode.Append(Environment.NewLine);
            scriptCode.Append(Script);
            return scriptCode;
        }
#endif

        public static bool StartsWithAny(this String txt, String[] Items)
        {
            if (Items == null || Items.Length == 0)
                return true;

            foreach (var item in Items)
                if (txt.StartsWith(item))
                    return true;

            return false;
        }

        public static bool EndsWith(this StringBuilder sb, string test)
        {
            return EndsWith(sb, test, StringComparison.CurrentCulture);
        }

        public static bool EndsWith(this StringBuilder sb, string test,
            StringComparison comparison)
        {
            if (sb.Length < test.Length)
                return false;

            string end = sb.ToString(sb.Length - test.Length, test.Length);
            return end.Equals(test, comparison);
        }

        public static String GetPrefix(this String Line)
        {
            StringBuilder prefix = new StringBuilder();
            foreach (Char ch in Line)
            {
                if (Char.IsWhiteSpace(ch))
                    prefix.Append(ch);
                else
                    break;
            }
            return prefix.ToString();
        }

        public static String GetFirstWord(this String Line)
        {
            StringBuilder prefix = new StringBuilder();
            foreach (Char ch in Line.TrimStart())
            {
                if (!Char.IsWhiteSpace(ch) && ch != '(' && ch != ')' && ch != ':' && ch != '[' && ch != ']' && ch != '{' && ch != '}')
                    prefix.Append(ch);
                else
                    break;
            }
            return prefix.ToString();
        }

        public static String PrefixToEveryLine(this String Text, String InsertPrefix, String InsertPostfix = "")
        {
            StringBuilder str = new StringBuilder();
            foreach (String line in GetLines(Text))
            {
                str.AppendFormat("{0}{1}{2}{3}", InsertPrefix, line, InsertPostfix, Environment.NewLine);
            }
            return str.ToString();
        }

        public static String AddReturnStatement(this String Text)
        {
            if (!Text.Contains("return"))
            {
                StringBuilder str = new StringBuilder();
                IList<String> lines = GetLines(Text.TrimEnd());
                Int32 index = -1;

                foreach (String line in lines)
                {
                    index++;
                    if (index == lines.Count - 1)
                    {
                        var lineWithoutStart = line.TrimStart();
                        var prefix = line.Substring(0, line.Length - lineWithoutStart.Length);
                        str.AppendFormat("{0}return {1}{2}", prefix, lineWithoutStart, Environment.NewLine);
                    }
                    else
                    {
                        str.AppendFormat("{0}{1}", line, Environment.NewLine);
                    }
                }
                return str.ToString();
            }
            else
            {
                return Text;
            }
        }

        public static String JoinString<T>(this IEnumerable<T> Elements, String Separator = ",")
        {
            StringBuilder lTxt = new StringBuilder();
            if (Elements != null)
                foreach (var lItem in Elements)
                {
                    if (lItem != null)
                    {
                        if (lTxt.Length > 0) lTxt.Append(Separator);
                        lTxt.Append(lItem);
                    }
                }
            return lTxt.ToString();
        }

        public static String JoinSql<T>(this IEnumerable<T> Elements, Boolean WrapWithComas, Boolean ToLower = false, String Separator = ",")
        {
            StringBuilder lTxt = new StringBuilder();
            if (Elements != null)
                foreach (var lItem in Elements)
                {
                    if (lTxt.Length > 0) lTxt.Append(Separator);

                    if (lItem != null)
                    {
                        if (WrapWithComas)
                        {
                            lTxt.
                                Append("'").
                                Append(
                                    ToLower ?
                                    Convert.ToString(lItem, CultureInfo.InvariantCulture).Replace("'", "''").ToLower() :
                                    Convert.ToString(lItem, CultureInfo.InvariantCulture).Replace("'", "''")).
                                Append("'");
                        }
                        else
                        {
                            lTxt.Append(lItem);
                        }
                    }
                    else
                    {
                        lTxt.Append("NULL");
                    }
                }
            return lTxt.ToString();
        }

        public static String Join<T>(this IEnumerable<T> Elements, String Separator)
        {
            StringBuilder lTxt = new StringBuilder();
            if (Elements != null)
                foreach (var lItem in Elements)
                {
                    if (lTxt.Length > 0) lTxt.Append(Separator);
                    lTxt.Append(lItem != null ? Convert.ToString(lItem, CultureInfo.InvariantCulture) : "");
                }
            return lTxt.ToString();
        }

        public static String Join(this IEnumerable<String> Texts, String Separator)
        {
            StringBuilder lTxt = new StringBuilder();
            if (Texts != null)
                foreach (var lItem in Texts)
                {
                    if (lTxt.Length > 0) lTxt.Append(Separator);
                    lTxt.Append(lItem ?? "");
                }
            return lTxt.ToString();
        }

        public static IEnumerable<String> OnlyNumbers(this IEnumerable<String> Texts)
        {
            Decimal lV = 0;
            if (Texts != null)
                foreach (var lText in Texts)
                {
                    if (!string.IsNullOrEmpty(lText))
                        if (Decimal.TryParse(lText.Trim(), out lV))
                            yield return lText;
                }
        }

        public static Boolean IsNumber(this String Text)
        {
            Text = (Text ?? "").Trim();
            if (Text.Length == 0)
                return false;
            foreach (Char ch in Text)
                if (!Char.IsNumber(ch))
                    return false;
            return true;
        }

        public static Boolean IsQuotedText(this String Text)
        {
            Text = (Text ?? "").Trim();
            if (Text.StartsWith("'") && Text.EndsWith("'"))
                return true;
            if (Text.StartsWith("\"") && Text.EndsWith("\""))
                return true;
            return false;
        }

        public static Object ParseJsonOrText(this String Text)
        {
            try
            {
                if (Text == null)
                    return null;

                if (Text == "null")
                    return null;

                if (MyStringHelper.IsNumber(Text.Trim()) ||
                    MyStringHelper.IsQuotedText(Text.Trim()))
                {
                    return Text.DeserializeJson();
                }
                else
                {
                    return Text.Trim();
                }
            }
            catch
            {
                throw;
            }
        }

        public static Boolean StartsWithNumber(this String Text)
        {
            Text = (Text ?? "").Trim();
            foreach (Char ch in Text)
                if (Char.IsNumber(ch))
                    return true;
                else
                    return false;
            return false;
        }

        public static Boolean IsWhiteString(this String Text)
        {
            return String.IsNullOrEmpty(Text) || Text.Trim().Length == 0;
        }

        public static String[] Split(this String Text, params String[] Separators)
        {
            return SplitWithDefault(Text, null, Separators);
        }

        public static String[] SplitWithDefault(this String Text, String ValueForEmpty, params String[] Separators)
        {
            var lR = (Text ?? "").Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            lR = lR ?? new String[0];
            if (lR.Length == 0 && ValueForEmpty != null) return new String[] { ValueForEmpty };
            return lR;
        }

        public static String[] GetLines(this String Text)
        {
            return Text.Split(
                new[] { Environment.NewLine, "\n", "\r" },
                StringSplitOptions.None);
        }

        public static Boolean EqualsNonsensitive(this String Str1, String Str2)
        {
            if (Str1 != null && Str2 == null) return false;
            else if (Str1 == null && Str2 != null) return false;
            else if (Str1 == null && Str2 == null) return true;
            else
            {
                return Str2.ToLower().Equals(Str1.ToLower());
            }
        }

        public static Char Get(this String Txt, Int32 Index)
        {
            if (Txt == null || Index < 0 || Index >= Txt.Length)
                return ' ';
            else
                return Txt[Index];
        }

        public static Char GetFromTail(this String Txt, Int32 Index)
        {
            var lInd = Txt.Length - 1 - Index;
            if (Txt == null || Index < 0 || Index >= Txt.Length)
                return ' ';
            else
                return Txt[lInd];
        }

        public static String Set(this String Txt, Int32 Index, Char Value)
        {
            if (Txt == null || Index < 0 || Index >= Txt.Length)
                return Txt ?? "";
            else
                return Txt.Remove(Index, 1).Insert(Index, Value.ToString());
        }

        public static String SetToTail(this String Txt, Int32 Index, Char Value)
        {
            var lInd = Txt.Length - 1 - Index;
            if (Txt == null || Index < 0 || Index >= Txt.Length)
                return Txt ?? "";
            else
                return Txt.Remove(lInd, 1).Insert(lInd, Value.ToString());
        }

        /// <summary>
        /// Split with Quotes
        /// </summary>
        public static List<String> SplitQ(this String Text, Char[] Separators, Boolean AddEmpty = true, Char[] Quotes = null)
        {
            if (Quotes == null)
                Quotes = quotes;

            List<String> items = new List<String>();
            StringBuilder currentItem = new StringBuilder();
            Boolean insideQuote = false;

            if (Text == null)
                return items;

            Char[] text = Text.ToCharArray();
            for (var i = 0; i < text.Length; i++)
            {
                Char ch = text[i];

                if (Quotes.Contains(ch))
                {
                    if (!insideQuote)
                    {
                        insideQuote = true;
                    }
                    else
                    {
                        insideQuote = false;
                    }
                    currentItem.Append(ch);
                }
                else
                {
                    if (insideQuote)
                    {
                        currentItem.Append(ch);
                    }
                    else
                    {
                        if (Separators.Contains(ch))
                        {
                            if (currentItem.Length > 0 || AddEmpty)
                            {
                                items.Add(currentItem.ToString());
                                currentItem.Clear();
                            }
                        }
                        else
                        {
                            currentItem.Append(ch);
                        }
                    }
                }
            }

            if (currentItem.Length > 0)
            {
                items.Add(currentItem.ToString());
                currentItem.Clear();
            }

            return items;
        }

        /// <summary>
        /// Get text without quotes
        /// </summary>
        public static String GetTextWithoutQuotes(this String Text, Char[] Quotes = null)
        {
            if (Quotes == null)
                Quotes = quotes;

            StringBuilder outText = new StringBuilder();
            Boolean insideQuote = false;

            if (Text == null)
                return null;

            Char[] text = Text.ToCharArray();
            for (var i = 0; i < text.Length; i++)
            {
                Char ch = text[i];

                if (Quotes.Contains(ch))
                {
                    if (!insideQuote)
                    {
                        insideQuote = true;
                    }
                    else
                    {
                        insideQuote = false;
                    }
                }
                else
                {
                    if (!insideQuote)
                    {
                        outText.Append(ch);
                    }
                }
            }

            return outText.ToString();
        }
    }

    public class ReplaceInfo
    {
        public Int32 Index;

        public Int32 Len;
    }
}
