using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "If this command still exists after release im gonna eat my shoes")]
    public static object Debug(int i)
    {
        const string code = "for var item in @@profiles.active {\n    @@chat.send(\"Hey, \" + @item.name);\n}";
        switch (i)
        {
            case 0:
                return code;
            case 1:
                return DGCommandLanguage.Highlight(code);
        }

        return null;
    }
}