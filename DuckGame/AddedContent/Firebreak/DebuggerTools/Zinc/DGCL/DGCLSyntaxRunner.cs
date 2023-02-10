#nullable enable
using System;

namespace DuckGame;

public static partial class DGCommandLanguage
{
    [DevConsoleCommand]
    public static DGCLRunResult Run(string str)
    {
        var tokens = Tokenize(str);

        int codeBlockDepth = 0;
        

        return new DGCLRunResult();
    }

    public record struct DGCLRunResult
    {
        public bool Success;
        public Error ExecutionError;
        
        public DGCLRunResult(Error error)
        {
            Success = false;
            ExecutionError = error;
        }
        
        public DGCLRunResult()
        {
            Success = true;
            ExecutionError = default;
        }

        public record struct Error(string Message, IntVec2 FaliurePoint);
    }
}