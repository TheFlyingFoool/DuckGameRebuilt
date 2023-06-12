using System.Text.RegularExpressions;

namespace DuckGame
{
    public static class BiDirectionalWordBoundaryMovement
    {
        public static string GetNextWord(
            string text,
            int currentIndex,
            HorizontalDirection direction)
        {
            (int StartIndex, int EndIndex) wordBoundaryRange = GetNextRange(text, currentIndex, direction);
            string nextWord = text.Substring(wordBoundaryRange.StartIndex,
                wordBoundaryRange.EndIndex - wordBoundaryRange.StartIndex);

            return nextWord;
        }

        public static (int StartIndex, int EndIndex) GetNextRange(
            string text,
            int currentIndex,
            HorizontalDirection direction)
        {
            // this fucking regex took 3 days to make
            Match match = direction == HorizontalDirection.Left
                ? Regex.Match(text.Substring(0, currentIndex), @"( {2,}|(?:\w+|[^\w\s]+) ?)$", RegexOptions.RightToLeft)
                : Regex.Match(text.Substring(currentIndex), @"^( {2,}| ?(?:\w+|[^\w\s]+))");

            int nextIndex = match.Success
                ? match.Index + (direction == HorizontalDirection.Left 
                    ? 0 
                    : currentIndex)
                : 0;

            int startIndex = nextIndex;
            int endIndex = nextIndex + match.Value.Length;
            return (startIndex, endIndex);
        }
    }

    public enum HorizontalDirection
    {
        Right,
        Left,
    }
}