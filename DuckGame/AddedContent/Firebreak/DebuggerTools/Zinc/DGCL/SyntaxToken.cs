#nullable enable
namespace DuckGame;

public record struct SyntaxToken(string Token, TokenType TokenType, TokenId Id);