using System.Text.RegularExpressions;

namespace DuckGame
{
    // looking back, i have no fucking clue how i wrote
    // any of this, nor does it make any sense whatsoever
    // ~ firebreak
    public static class WordBoundary
    {
        public static string GetNextWord(
            string text,
            int caretIndex,
            HorizontalDirection direction = HorizontalDirection.Right)
        {
            IntRange wordRange = GetNextRange(text, caretIndex, direction);
            string nextWord = text.Substring(wordRange.Start, wordRange.End - wordRange.Start);

            return nextWord;
        }

        // stand ctrl word jumping
        private static readonly Regex s_rtlMatchRegex = new(@"( {2,}|(?:\w+|[^\w\s]+) ?)$", RegexOptions.RightToLeft | RegexOptions.Compiled);
        private static readonly Regex s_ltrMatchRegex = new(@"^( {2,}| ?(?:\w+|[^\w\s]+))", RegexOptions.Compiled);

        // hard boundary word jumping
        private static readonly Regex s_rtlMatchRegex_h = new(@"([^\s]+)$", RegexOptions.RightToLeft | RegexOptions.Compiled);
        private static readonly Regex s_ltrMatchRegex_h = new(@"^([^\s]+)", RegexOptions.Compiled);

        public static IntRange GetNextRange(
            string text,
            int caretIndex,
            HorizontalDirection direction = HorizontalDirection.Right)
        {
            Match match = direction == HorizontalDirection.Left
                ? s_rtlMatchRegex.Match(text.Substring(0, caretIndex))
                : s_ltrMatchRegex.Match(text.Substring(caretIndex));

            int nextIndex = match.Success
                ? match.Index + (direction == HorizontalDirection.Left 
                    ? 0
                    : caretIndex)
                : 0;

            int startIndex = nextIndex;
            int endIndex = nextIndex + match.Value.Length;
            
            return new IntRange(startIndex, endIndex);
        }

        public static IntRange GetNextRange_Hard(
            string text,
            int caretIndex,
            HorizontalDirection direction = HorizontalDirection.Right)
        {
            Match match = direction == HorizontalDirection.Left
                ? s_rtlMatchRegex_h.Match(text.Substring(0, caretIndex))
                : s_ltrMatchRegex_h.Match(text.Substring(caretIndex));

            int nextIndex = match.Success
                ? match.Index + (direction == HorizontalDirection.Left 
                    ? 0
                    : caretIndex)
                : 0;

            int startIndex = nextIndex;
            int endIndex = nextIndex + match.Value.Length;
            
            return new IntRange(startIndex, endIndex);
        }
    }

    public enum HorizontalDirection
    {
        Right,
        Left,
    }
}