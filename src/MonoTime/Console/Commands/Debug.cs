using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    private static int? _lineIndex = null;
    private static ProgressValue _progress = new();
    private static int _nextUpdateFrame = 0;
    
    [DevConsoleCommand(Description = "If this command still exists after release im gonna eat my shoes")]
    public static object Debug(int i)
    {
        switch (i)
        {
            case 0:
            {
                _nextUpdateFrame = 0;
                _progress = new ProgressValue(0, 0.03, 0, 20);
                // DevConsole.Log($"[{_progress.GenerateBar()}] {_progress.NormalizedValue}%");
                DevConsole.Log(_progress.NormalizedValue);
                _lineIndex = DevConsole.core.lines.Count;
                break;
            }
            case 1:
            {
                var p = new ProgressValue(35, 1, 0, 100);
                DevConsole.Log(p);
                break;
            }
        }

        return null;
    }

    [DrawingContext]
    public static void DcTestLineUpdate()
    {
        if (_lineIndex is null || _lineIndex >= DevConsole.core.lines.Count || _lineIndex < 0 || _nextUpdateFrame++ >= 30) 
            return;

        _progress++;
        _progress = ~_progress;

        string highlightColor = _progress.NormalizedValue switch
        {
            >= 1.0f => "|DGGREEN|",
            >= 0.5f => "|DGYELLOW|",
            _ => "|DGRED|",
        };
        
        DevConsole.core.lines.ElementAt(_lineIndex.Value).line = 
            $"{Regex.Replace($"[{_progress.GenerateBar()}]", @"#+", @"|DGGREEN|$&|WHITE|")}" +
            $"{highlightColor} {Math.Round(_progress.NormalizedValue * 100, 2)}%";
        
        _nextUpdateFrame = 0;

        if (!_progress.Completed)
            _lineIndex = null;
    }
}