using System.Text;

namespace DuckGame;

public static partial class DGCommandLanguage
{
    [DevConsoleCommand]
    public static string Highlight(string str)
    {
        var tokens = DGCommandLanguage.Tokenize(str);
        StringBuilder stringBuilder = new();
        string? lastColor = null;

        foreach (SyntaxToken token in tokens)
        {
            string color = token.TokenType.GetColor();
            if (token.TokenType != TokenType.Whitespace && color != lastColor)
            {
                stringBuilder.Append(color);
                lastColor = color;
            }

            if (token.Id == TokenId.NewLine)
                lastColor = null;

            stringBuilder.Append(token.Token);
        }

        return stringBuilder.ToString();
    }
}