using System.Text.RegularExpressions;

namespace DuckGame
{
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

        // match right to left
        private static readonly Regex s_rtlMatchRegex = new(@"( {2,}|(?:\w+|[^\w\s]+) ?)$", RegexOptions.RightToLeft | RegexOptions.Compiled);
        
        // match left to right
        private static readonly Regex s_ltrMatchRegex = new(@"^( {2,}| ?(?:\w+|[^\w\s]+))", RegexOptions.Compiled);

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
    }

    public enum HorizontalDirection
    {
        Right,
        Left,
    }
}