#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DuckGame;

public static class DGCommandLanguage
{
    public static class FontColors
    {
        public const string PINK   = "|255,097,136|";
        public const string CYAN   = "|089,222,222|";
        public const string GREY   = "|147,146,147|";
        public const string LIME   = "|169,220,118|";
        public const string PURPLE = "|171,157,242|";
        public const string ORANGE = "|245,151,098|";
        public const string WHITE  = "|252,252,250|";
        public const string YELLOW = "|255,216,102|";
    }

    [DevConsoleCommand]
    public static string Highlight(string str)
    {
        var tokens = Tokenize(str);
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

    public static List<SyntaxToken> Tokenize(string str)
    {
        string[] matches = _regex.Matches(str).Cast<Match>().Select(x => x.Value).ToArray();
        List<SyntaxToken> tokens = new();
        StringBuilder? longToken = null;

        string? longTokenMatcher = null;

        for (int i = 0; i < matches.Length; i++)
        {
            string token = matches[i];

            longToken?.Append(token);

            if (token is "\"" or "'" && (i == 0 || matches[i - 1] != "\\"))
            {
                if (longToken is null) // start string
                {
                    longToken = new();
                    longToken.Append(token);
                    longTokenMatcher = token;
                }
                else if (token == longTokenMatcher) // end string
                {
                    tokens.Add(new SyntaxToken(longToken.ToString(), TokenType.String, TokenId.String));
                    longToken = null;
                    longTokenMatcher = null;
                }
            }
            else if (longToken is null) // not part of a string
            {
                TokenType type = TokenType.Unknown;
                TokenId id = TokenId.Unknown;
                if (Tokens.TryFirst(x => Regex.IsMatch(token, x.Token), out var first))
                    (_, type, id) = first;

                tokens.Add(new SyntaxToken(token, type, id));
            }
        }

        if (longToken is not null)
            tokens.Add(new SyntaxToken(longToken.ToString(), TokenType.String, TokenId.String));

        return tokens;
    }

    public static readonly SyntaxToken[] Tokens =
    {
        new(@"\/\/.*", TokenType.Comments, TokenId.SingleLineComment),
        new(@"\/\*.*?\*\/", TokenType.Comments, TokenId.MultiLineComment),

        new(@"\d*\.\.\d*", TokenType.Number, TokenId.Range),
        new(@"[\d_]+(?:\.[\d_]+)?|\.[\d_]+", TokenType.Number, TokenId.Number),
        
        new (@"^true$", TokenType.Boolean, TokenId.BooleanTrue),
        new (@"^false$", TokenType.Boolean, TokenId.BooleanFalse),

        new(" ", TokenType.Whitespace, TokenId.Space),
        new("\n", TokenType.Whitespace, TokenId.NewLine),
        new(@"\s", TokenType.Whitespace, TokenId.Unknown),

        new("^rep$", TokenType.Keyword, TokenId.RepeatLoop),
        new("^for$", TokenType.Keyword, TokenId.ForLoop),
        new("^in$", TokenType.Keyword, TokenId.In),
        new("^while$", TokenType.Keyword, TokenId.WhileLoop),
        new("^if$", TokenType.Keyword, TokenId.IfStatement),
        new("^else$", TokenType.Keyword, TokenId.ElseStatement),
        new("^break$", TokenType.Keyword, TokenId.Break),
        new("^def$", TokenType.Keyword, TokenId.DefineFunction),
        new("^var$", TokenType.Keyword, TokenId.DefineVariable),
        new("^new$", TokenType.Keyword, TokenId.Instantiate),
        new("^get$", TokenType.Keyword, TokenId.Acquire),

        new(@"\+=", TokenType.Operator, TokenId.CompoundingAdd),
        new(@"\-=", TokenType.Operator, TokenId.CompoundingSubtract),
        new(@"\/=", TokenType.Operator, TokenId.CompoundingDivide),
        new(@"\*=", TokenType.Operator, TokenId.CompoundingMultiply),
        new(@"\+\+", TokenType.Operator, TokenId.Increment),
        new(@"\-\-", TokenType.Operator, TokenId.Decrement),
        new(@"\+", TokenType.Operator, TokenId.Add),
        new(@"\-", TokenType.Operator, TokenId.Subtract),
        new(@"\/", TokenType.Operator, TokenId.Divide),
        new(@"\*", TokenType.Operator, TokenId.Multiply),
        new(@"&&", TokenType.Operator, TokenId.LogicalAnd),
        new(@"\|\|", TokenType.Operator, TokenId.LogicalOr),
        new(@">=", TokenType.Operator, TokenId.GreaterThanOrEqualTo),
        new(@"<=", TokenType.Operator, TokenId.LessThanOrEqualTo),
        new(@">", TokenType.Operator, TokenId.GreaterThan),
        new(@"<", TokenType.Operator, TokenId.LessThan),
        new(@"==", TokenType.Operator, TokenId.EqualTo),
        new(@"!=", TokenType.Operator, TokenId.NotEqualTo),
        new(@"=", TokenType.Operator, TokenId.AssignValue),
        new(@"!", TokenType.Operator, TokenId.NegateBoolean),

        new(@"@@\w+", TokenType.CommandMeta, TokenId.ReferenceStaticInstance),
        new(@"@\w+", TokenType.CommandMeta, TokenId.ReferenceInstance),

        new(@"\{", TokenType.Punctuation, TokenId.OpenCodeBlock),
        new(@"\}", TokenType.Punctuation, TokenId.CloseCodeBlock),
        new(@"\(", TokenType.Punctuation, TokenId.OpenFunctionArguments),
        new(@"\)", TokenType.Punctuation, TokenId.CloseFunctionArguments),
        new(@"\[", TokenType.Punctuation, TokenId.OpenCollectionIndexing),
        new(@"\]", TokenType.Punctuation, TokenId.CloseCollectionIndexing),
        new(@"\.", TokenType.Punctuation, TokenId.AccessInstanceMembers),
        new(@",", TokenType.Punctuation, TokenId.SeparateParameters),
        new(@";", TokenType.Punctuation, TokenId.SeparateCommands),
    };

    private static readonly Regex _regex =
        new($@"{string.Join("|", Tokens.Where(x => x.TokenType is not TokenType.Keyword).Select(x => x.Token))}|\s+|\w+|\W",
            RegexOptions.Compiled);

    public static string GetColor(this TokenType type)
    {
        return type switch
        {
            TokenType.Keyword => FontColors.PINK,
            TokenType.Operator => FontColors.PINK,
            TokenType.Punctuation => FontColors.GREY,
            TokenType.CommandMeta => FontColors.LIME,
            TokenType.Number or TokenType.Boolean => FontColors.PURPLE,
            TokenType.String => FontColors.YELLOW,
            TokenType.Comments => FontColors.CYAN,
            TokenType.Unknown or TokenType.Whitespace => FontColors.WHITE,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}