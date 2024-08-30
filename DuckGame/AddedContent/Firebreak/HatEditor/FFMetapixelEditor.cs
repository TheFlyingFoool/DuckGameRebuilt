using DuckGame;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public partial class FeatherFashion
    {
        public class MetapixelEditor
        {
            public static Dictionary<byte, MetapixelInfo> MetapixelInfoMap;

            public static new void Initialize()
            {
                // don't ask.
                MetapixelInfoMap = typeof(Team.CustomHatMetadata)
                    .GetFields(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetCustomAttribute<Team.Metapixel>() is not null)
                    .Select(x =>
                    {
                        Team.Metapixel attribute = x.GetCustomAttribute<Team.Metapixel>();
                        bool dgr = x.GetCustomAttribute<DGRAttribute>() is not null;
                        bool synced = x.GetCustomAttribute<VanillaSyncedAttribute>() is not null;
                    
                        return new MetapixelInfo((byte)attribute.index, attribute.name, attribute.description, x.FieldType, dgr, synced);
                    })
                    .ToDictionary(x => x.Index, x => x);
            }
        }
    }
}