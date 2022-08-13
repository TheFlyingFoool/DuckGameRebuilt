using System;
using System.Collections.Generic;

namespace DuckGame;

public static class ConfigTest
{
    // [AutoConfigField(ShortName = "test1")]
    public static List<Vec2[]> ListOfArrayOfVec2 = new()
    {
        new[] { new Vec2(2, 5), new Vec2(7, 13) },
        new[] { new Vec2(65, 23), new Vec2(82, 9) },
        new[] { new Vec2(12, 77), new Vec2(19, 84) },
    };

    // [AutoConfigField(ShortName = "test2")]
    public static DateTime NOW = DateTime.Now;

    // [AutoConfigField(ShortName = "test3")]
    public static sbyte this_byte_is_signed;
}