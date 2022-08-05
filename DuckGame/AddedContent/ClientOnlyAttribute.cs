using System;

namespace DuckGame
{
   
    /// <summary>
    /// Marks the class using the attribute as ClientOnly. This
    /// means that if there is a player present in the lobby
    /// without this specific duck game client, then this class
    /// will cease it's functionality to maintain compatibility.
    /// It will however continue working as normal if every person
    /// in the lobby does have the client.
    /// <br /> <br />
    /// Applicable on types:<br />
    /// <list type="bullet">
    ///     <item> Implementations of <see cref="AmmoType"/> </item>
    ///     <item> Implementations of <see cref="Thing"/> </item>
    ///     <item> Implementations of <see cref="DestroyType"/> </item>
    ///     <item> Implementations of <see cref="DeathCrateSetting"/> </item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ClientOnlyAttribute : Attribute { }
}
