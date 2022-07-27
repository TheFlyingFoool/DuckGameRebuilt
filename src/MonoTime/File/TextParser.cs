// Decompiled with JetBrains decompiler
// Type: DuckGame.TextParser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class TextParser
    {
        private static HashSet<char> _wordSeparators = new HashSet<char>()
    {
      ' ',
      '\n',
      '\r',
      '\t',
      ';',
      '{',
      '}',
      '.',
      '(',
      ')',
      ',',
      '<',
      '>'
    };

        public static string ReadNextWord(string s, ref int index, char? stop = null)
        {
            if (index < s.Length && stop.HasValue && s[index] == stop.Value)
                return null;
            while (index < s.Length && TextParser._wordSeparators.Contains(s[index]))
            {
                if (stop.HasValue && s[index] == stop.Value)
                    return null;
                ++index;
            }
            string str = "";
            while (index < s.Length && !TextParser._wordSeparators.Contains(s[index]) && (!stop.HasValue || s[index] != stop.Value))
            {
                str += s[index].ToString();
                ++index;
            }
            return str;
        }

        public static string ReadNextWordBetween(char between, string s, ref int index)
        {
            while (index < s.Length && s[index] != between)
                ++index;
            ++index;
            string str = "";
            while (index < s.Length && s[index] != between)
            {
                str += s[index].ToString();
                ++index;
            }
            ++index;
            return str;
        }

        public static void SkipWord(string s, ref int index) => TextParser.ReadNextWord(s, ref index);

        public static string ReverseReadWord(string s, ref int index)
        {
            --index;
            while (index >= 0 && TextParser._wordSeparators.Contains(s[index]))
                --index;
            string str = "";
            while (index >= 0 && !TextParser._wordSeparators.Contains(s[index]))
            {
                str = str.Insert(0, s[index].ToString() ?? "");
                --index;
            }
            ++index;
            return str;
        }

        public static void StepBackWord(string s, ref int index) => TextParser.ReverseReadWord(s, ref index);

        public static char ReadNextCharacter(string s, ref int index)
        {
            while (index < s.Length && TextParser._wordSeparators.Contains(s[index]))
                ++index;
            return index < s.Length ? s[index] : ' ';
        }

        public static string ReadNextBrace(string s, ref int index, bool ignoreSemi = false)
        {
            ++index;
            while (index < s.Length && (s[index] == ' ' || s[index] == '\n' || s[index] == '\r' || s[index] == '\t'))
                ++index;
            while (index < s.Length && s[index] != '}' && s[index] != '{' && (ignoreSemi || s[index] != ';'))
                ++index;
            return index < s.Length ? s[index].ToString() ?? "" : "}";
        }

        public static string ReadNext(char c, string s, ref int index)
        {
            while (index < s.Length && (s[index] == ' ' || s[index] == '\n' || s[index] == '\r' || s[index] == '\t'))
                ++index;
            while (index < s.Length && s[index] != c)
                ++index;
            if (index >= s.Length)
                return null;
            ++index;
            return s[index - 1].ToString() ?? "";
        }

        public static string ReadTo(char c, string s, ref int index)
        {
            int num = s.IndexOf(c, index);
            if (num < 0)
                return s.Substring(index, s.Length - index);
            string str = s.Substring(index, num - index);
            index = num;
            return str;
        }

        public static string ReverseReadNext(char c, string s, ref int index)
        {
            --index;
            while (index >= 0 && (s[index] == ' ' || s[index] == '\n' || s[index] == '\r' || s[index] == '\t'))
                --index;
            while (index >= 0 && s[index] != c)
                --index;
            return s[index].ToString() ?? "";
        }
    }
}
