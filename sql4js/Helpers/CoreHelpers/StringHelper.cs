using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Else.HttpService.Helpers.CoreHelpers
{
    public static class StringHelper
    {
        public static Boolean SequenceEqualInsensitive(
            IList<Char> Items1,
            IList<Char[]> ListOfItems2)
        {
            foreach (Char[] items2 in ListOfItems2)
                if (SequenceEqualInsensitive(Items1, items2))
                    return true;
            return false;
        }

        public static Boolean SequenceEqualInsensitive(
            this IList<Char> Items1,
            IList<Char> Items2)
        {
            if (Items1 != null && Items2 != null && Items1.Count == Items2.Count)
            {
                Int32 c = Items1.Count;
                for (Int32 i = 0; i < c; i++)
                    if (Char.ToLowerInvariant(Items1[i]) != Char.ToLowerInvariant(Items2[i]))
                        return false;
                return true;
            }
            else if (Items1 == null && Items2 == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static String FormatDate(DateTime DateTime, String Format)
        {
            String r = Format ?? "";
            r = r.Replace("mm", FormatDatePart(DateTime.Month));
            r = r.Replace("yyyy", DateTime.Year.ToString());
            r = r.Replace("dd", FormatDatePart(DateTime.Day));

            r = r.Replace("hh", FormatDatePart(DateTime.Hour));
            r = r.Replace("mi", FormatDatePart(DateTime.Minute));
            r = r.Replace("ss", FormatDatePart(DateTime.Second));
            r = r.Replace("ms", FormatDatePart(DateTime.Millisecond, 3));
            r = r.Replace("mmm", FormatDatePart(DateTime.Millisecond, 3));
            return r;
        }

        private static String FormatDatePart(Int32 Val, Int32 Count = 2)
        {
            var a = Val.ToString();
            if (Count == 2)
                return a.Length == 2 ? a : ("0" + a);
            while (a.Length < Count)
                a = "0" + a;
            return a;
        }

        public static String RemoveBracketsFromMethodCall(this String Text)
        {
            Text = (Text ?? "").Trim();
            if (Text.StartsWith("(")) Text = Text.Substring(1);
            if (Text.EndsWith(")")) Text = Text.Substring(0, Text.Length - 1);
            return Text;
        }

        //////////////////////////////

        /*public static IEnumerable<Char> Substring(
            List<Char> Chars,
            Int32 StartIndex,
            Int32? Length = null)
        {
            return Substring((IList<Char>)Chars, StartIndex, Length);
        }

        public static IEnumerable<Char> Substring(
            this IList<Char> Chars,
            Int32 StartIndex,
            Int32? Length = null)
        {
            Int32 max = Length == null ?
                Chars.Count :
                (StartIndex + Length > Chars.Count ? Chars.Count : StartIndex + Length.Value);

            //List<Char> outChars = new List<Char>();
            for (var i = StartIndex; i < max; i++)
                yield return Chars[i];
            //outChars.Add(Chars[i]);
            //return outChars;
        }*/

        public static int IndexOf(this List<char> haystack, List<char> needle, Int32 StartIndex = 0)
        {
            var len = needle.Count;
            var limit = haystack.Count - len;
            for (var i = StartIndex; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

        public static Int32 IndexOf2(this List<char> Text, List<char> TextPart, Int32 StartIndex = 0)
        {
            int result = -1;
            if (StartIndex < 0)
                StartIndex = 0;

            if (StartIndex >= Text.Count)
                return result;

            if (TextPart == null || TextPart.Count == 0)
                return result;

            var c1 = Text.Count;
            var c2 = TextPart.Count;

            for (var i = StartIndex; i < c1; i++)
            {
                var found = true;

                for (var j = 0; j < c2; j++)
                {
                    if (i + j >= c1)
                        return result;
                    
                    var txtCh = Text[i + j];
                    var partCh = TextPart[j];
                    if (txtCh != partCh)
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public static List<Char> Append(
            this List<Char> Chars1,
            List<Char> Chars2)
        {
            if (Chars2 != null)
                Chars1.AddRange(Chars2);
            return Chars1;
        }

        /* public static List<Char> Append(
             this List<Char> Chars1,
             String Chars2)
         {
             if (Chars2 != null)
                 Chars1.AddRange(Chars2.ToCharArray());
             return Chars1;
         }*/

        public static List<Char> RemoveFromString(
            this List<Char> Chars,
            Int32 StartIndex,
            Int32 Length)
        {
            var len = Length;
            var end = StartIndex + len;
            if (end >= Chars.Count)
            {
                len -= (end - Chars.Count);
                //d = Chars.Count;
            }


            /*for (var i = StartIndex; i < end; i++)
                Chars.RemoveAt(StartIndex);*/
            Chars.RemoveRange(StartIndex, len);
            return Chars;
        }

        public static List<Char> Substring(
            this List<Char> Chars,
            Int32 StartIndex,
            Int32? Length = null)
        {
            /*Int32 max = Length == null ?
                Chars.Count :
                (StartIndex + Length > Chars.Count ? Chars.Count : StartIndex + Length.Value);*/

            //List<Char> outChars = new List<Char>();

            if (Length != null)
            {
                var c = Chars.Count;
                for (var i = StartIndex + Length.Value; i < c; i++)
                    Chars.RemoveAt(Chars.Count - 1);
            }

            for (var i = 0; i < StartIndex; i++)
                Chars.RemoveAt(0);


            return Chars;

            //for (var i = StartIndex; i < max; i++)
            //   yield return Chars[i];
            //outChars.Add(Chars[i]);
            //return outChars;
        }

        public static IEnumerable<Char> Substring2(
            List<Char> Chars,
            Int32 StartIndex,
            Int32? Length = null)
        {
            return Substring2((IList<Char>)Chars, StartIndex, Length);
        }

        public static IEnumerable<Char> Substring2(
            this IList<Char> Chars,
            Int32 StartIndex,
            Int32? Length = null)
        {
            Int32 max = Length == null ?
                Chars.Count :
                (StartIndex + Length > Chars.Count ? Chars.Count : StartIndex + Length.Value);

            List<Char> outChars = new List<Char>();
            for (var i = StartIndex; i < max; i++)
                yield return Chars[i];
        }

        public static IEnumerable<Char> Replace2(
            this IEnumerable<Char> Chars,
            Char From,
            Char? To)
        {
            foreach (Char ch in Chars)
                if (ch == From)
                {
                    if (To != null)
                        yield return To.Value;
                }
                else
                    yield return ch;
        }

        public static String ToString2(
            this IEnumerable<Char> Chars)
        {
            StringBuilder str = new StringBuilder();
            foreach (Char ch in Chars)
                str.Append(ch);
            return str.ToString();
        }

        public static void Trim(
            this IList<Char> Chars,
            Char? trimchar = null)
        {
            for (var i = Chars.Count - 1; i >= 0; i--)
            {
                char ch = Chars[i];
                if (trimchar == null ? Char.IsWhiteSpace(ch) : trimchar == ch)
                    Chars.RemoveAt(i);
                else
                    break;
            }

            while (Chars.Count > 0)
            {
                char ch = Chars[0];
                if (trimchar == null ? Char.IsWhiteSpace(ch) : trimchar == ch)
                    Chars.RemoveAt(0);
                else
                    break;
            }
        }

        public static void TrimEnd(
            this IList<Char> Chars,
            Char? trimchar = null)
        {
            for (var i = Chars.Count - 1; i >= 0; i--)
            {
                char ch = Chars[i];
                if (trimchar == null ? Char.IsWhiteSpace(ch) : trimchar == ch)
                    Chars.RemoveAt(i);
                else
                    break;
            }
        }

        public static IEnumerable<Char> TrimStart(
            this IEnumerable<Char> Chars,
             Char[] Char)
        {
            foreach (var ch in Char)
                Chars = TrimStart(Chars, ch);
            return Chars;
        }

        public static IEnumerable<Char> TrimStart(
            this IEnumerable<Char> Chars,
            Char? trimchar = null)
        {
            Boolean areWhiteChars = true;
            foreach (char ch in Chars)
            {
                if (trimchar == null ? Char.IsWhiteSpace(ch) : trimchar == ch)
                {

                }
                else
                {
                    areWhiteChars = false;
                }

                if (!areWhiteChars)
                    yield return ch;
            }
        }

        public static Boolean StartsWith(
            this IList<Char> Chars,
            String txt,
            Int32 StartIndex = 0)
        {
            var txtArray = txt.ToCharArray();
            return StartsWith(Chars, txtArray, StartIndex);
        }

        public static Boolean StartsWith(
            this IList<Char> Chars,
            IList<Char> txtArray,
            Int32 StartIndex = 0)
        {
            var i = -1;
            //foreach (char ch in Chars)
            for (var j = StartIndex; j < Chars.Count; j++)
            //for( var i=0;i<txtArray.Length; i++)
            {
                var ch = Chars[j];
                i++;

                if (i >= txtArray.Count)
                    break;

                var txtCh = txtArray[i];

                if (txtCh != ch)
                    return false;

                //return ch == startChar;
            }
            return true;
        }

        public static Boolean StartsWith(
            this IEnumerable<Char> Chars,
            Char startChar)
        {
            foreach (char ch in Chars)
            {
                return ch == startChar;
            }
            return false;
        }

        public static Boolean IsNumber(
            IList<Char> Chars)
        {
            if (Chars.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var lChar in Chars)
                    if (!Char.IsNumber(lChar) && lChar != '.' && lChar != ',' && lChar != '-')
                        return false;
                return true;
            }
        }

        public static Boolean IsString(
            IList<Char> Chars,
            Char StringChar = '\'')
        {
            if (Chars.Count < 2)
            {
                return false;
            }
            else
            {
                if (Chars[0] == StringChar && Chars[Chars.Count - 1] == StringChar)
                {
                    for (int i = 1; i < Chars.Count - 1; i++)
                    {
                        Char? lPrevChar = i > 0 ? (Char?)Chars[i - 1] : null;
                        Char lChar = Chars[i];

                        if (lChar == StringChar)
                        {
                            if (lPrevChar != '\\')
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /*public static Boolean IsDateTime(
            IList<Char> Chars)
        {
            if (Chars.Count == 0)
            {
                return false;
            }
            else
            {
                if (Chars[0] == '#' && Chars[Chars.Count - 1] == '#')
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        */

        public static Char[] StrEquals(
            IList<Char> Source,
            IList<Char[]> ItemsToFind,
            Boolean Insensitive)
        {
            foreach (Char[] item in ItemsToFind)
                if (StrEquals(Source, item, Insensitive))
                    return item;
            return null;
        }

        public static Char[] StrEquals(
            IList<Char> Source,
            IList<Char[]> ItemsToFind,
            Int32 StartIndex,
            Boolean Insensitive)
        {
            foreach (Char[] item in ItemsToFind)
                if (StrEquals(Source, item, StartIndex, Insensitive))
                    return item;
            return null;
        }

        public static Boolean StrEquals(
            IList<Char> Source,
            IList<Char> ItemToFind,
            Boolean Insensitive)
        {
            if (Source == null || ItemToFind == null)
                return false;

            Int32 length = ItemToFind.Count;
            if (length > Source.Count)
                return false;

            for (int i = 0; i < length; i++)
                if (Insensitive)
                {
                    if (Char.ToLowerInvariant(Source[i]) != Char.ToLowerInvariant(ItemToFind[i]))
                        return false;
                }
                else
                {
                    if (Source[i] != ItemToFind[i])
                        return false;
                }

            return true;
        }

        public static Boolean StrEquals(
            IList<Char> Source,
            IList<Char> ItemToFind,
            Int32 StartIndex,
            Boolean Insensitive)
        {
            if (Source == null || ItemToFind == null)
                return false;

            else if (StartIndex + ItemToFind.Count > Source.Count)
                return false;

            else
            {
                Int32 length = StartIndex + ItemToFind.Count;
                Int32 j = 0;
                for (int i = StartIndex; i < length; i++)
                {
                    if (Insensitive)
                    {
                        if (Char.ToLowerInvariant(Source[i]) != Char.ToLowerInvariant(ItemToFind[j]))
                            return false;
                    }
                    else
                    {
                        if (Source[i] != ItemToFind[j])
                            return false;
                    }
                    j++;
                }
                return true;
            }
        }

        public static OnpOnpStringFindResult FirstNextIndex(
            IList<Char> Source,
            Int32 StartIndex,
            IList<Char[][]> ItemsToFind,
            Boolean Insensitive,
            Char StringChar = '\'')
        {
            if (ItemsToFind == null || ItemsToFind.Count == 0)
                return null;

            OnpOnpStringFindResult minResult = null;
            foreach (Char[][] items in ItemsToFind)
            {
                OnpOnpStringFindResult findResult = NextIndex(Source, items, StringChar, StartIndex, Insensitive);
                if (findResult != null)
                {
                    if (minResult == null || findResult.Index < minResult.Index)
                        minResult = findResult;
                }
            }

            return minResult;
        }

        public static OnpOnpStringFindResult NextIndex(
            IList<Char> Source,
            Char[][] ItemsToFind,
            Char StringChar,
            Int32 StartIndex,
            Boolean Insensitive)
        {
            Boolean isDigit = true;

            for (int i = StartIndex; i < Source.Count; i++)
            {
                Char cur = Source[i];

                if (!Char.IsDigit(cur) && cur != '.')
                    isDigit = false;

                if (cur == StringChar)
                {
                    int nextIndex = -1;
                    for (int j = i + 1; j < Source.Count; j++)
                    {
                        var lTmpChar = Source[j];
                        if (Source[j] == StringChar &&
                            Source[j - 1] != '\\')
                        {
                            nextIndex = j;
                            break;
                        }
                    }
                    if (nextIndex >= 0)
                    {
                        i = nextIndex;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    char[] foundToken = null;

                    if (i == 0)
                        foundToken = StringHelper.StrEquals(Source, ItemsToFind, Insensitive);
                    else
                        foundToken = StringHelper.StrEquals(Source, ItemsToFind, i, Insensitive);

                    if (foundToken != null)
                    {
                        // jeśli poprzedni znak to cyfra to szukamy dalej
                        if (!isDigit)
                        {
                            Char? prevChar = i > 0 ? (Char?)Source[i - 1] : null;
                            Char? nextChar = i + foundToken.Length < Source.Count ? (Char?)Source[i + foundToken.Length] : null;

                            if (!(prevChar != null && Char.IsLetterOrDigit(prevChar.Value) && Char.IsLetterOrDigit(foundToken[0])) &&
                                !(nextChar != null && Char.IsLetterOrDigit(nextChar.Value) && Char.IsLetterOrDigit(foundToken[foundToken.Length - 1])))
                            {
                                return new OnpOnpStringFindResult() { Index = i, Chars = foundToken };
                            }
                        }
                    }
                }
            }
            return null;
        }
    }

    public class OnpOnpStringFindResult
    {
        public char[] Chars;

        public Int32 Index;
    }
}
