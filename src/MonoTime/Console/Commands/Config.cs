using System;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static object Config(string id, string newValue = null)
    {
        switch (id)
        {
            case "%SAVE":
                AutoConfigHandler.SaveAll(false);
                return "|DGBLUE|Saved all config fields";
            case "%LOAD":
                AutoConfigHandler.LoadAll();
                return "|DGBLUE|Reloaded all config fields";
            case "%LIST":
                return AutoConfigFieldAttribute.All
                    .Select(x => $"{x.MemberInfo.Name}: |DGBLUE|{x.MemberInfo.GetValue(null)}|PREV|");
        }

        var all = AutoConfigFieldAttribute.All;

        if (!all.TryFirst(x => (x.Attribute.ShortName ?? x.Attribute.Id ?? x.MemberInfo.Name).CaselessEquals(id), out var field))
            throw new Exception($"No configuration field found with ID: {id}");

        object val = field!.MemberInfo.GetValue(null);
        
        if (newValue is null)
            return val;

        object newVal = FireSerializer.Deserialize(field.MemberInfo.FieldType, newValue);

        field!.MemberInfo.SetValue(null, newVal);
        return $"|DGBLUE|Modified value of field [|PINK|{field.MemberInfo.Name}|DGBLUE|] from [|GREEN|{val}|DGBLUE|] to [|GREEN|{newVal}|DGBLUE|]";
    }
}