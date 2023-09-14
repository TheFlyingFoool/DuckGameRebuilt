using System;

namespace DuckGame
{
    public static class DGRDevs
    {
        public static DGRebuiltDeveloper Collin = new()
        {
            DisplayName = "Collin",
            Color = new Color("#FED95F"),
            DevRole = DGRebuiltDeveloper.Role.Collin,
            DevItem = typeof(CollinGun),
            SteamID = 76561198114791325,
        };

        public static DGRebuiltDeveloper Dan = new()
        {
            DisplayName = "Dan",
            Color = new Color("#00FF00"),
            DevRole = DGRebuiltDeveloper.Role.LeadDeveloper,
            DevItem = typeof(DanGun),
            SteamID = 76561198124539558,
        };

        public static DGRebuiltDeveloper NiK0 = new()
        {
            DisplayName = "NiK0",
            Color = new Color("#00FFFF"),
            DevRole = DGRebuiltDeveloper.Role.LeadDeveloper,
            DevItem = typeof(NiK0Gun),
            SteamID = 76561198806685720,
        };

        public static DGRebuiltDeveloper Firebreak = new()
        {
            DisplayName = "Firebreak",
            Color = new Color("#FF5938"),
            DevRole = DGRebuiltDeveloper.Role.ActiveContributor,
            DevItem = typeof(FirebreakGun),
            SteamID = 76561198431449604,
        };

        public static DGRebuiltDeveloper Othello7 = new()
        {
            DisplayName = "othello7",
            Color = new Color("#A000FF"),
            DevRole = DGRebuiltDeveloper.Role.ActiveContributor,
            DevItem = typeof(othello7Gun),
            SteamID = 76561198797606383,
        };

        public static DGRebuiltDeveloper Hyeve = new()
        {
            DisplayName = "Hyeve",
            Color = new Color("#ED32A8"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(HyeveGun),
            SteamID = 76561198138278564,
        };

        public static DGRebuiltDeveloper Lutalli = new()
        {
            DisplayName = "Lutalli",
            Color = new Color("#9370DB"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(LutalliGun),
            SteamID = 76561198328210060,
        };

        public static DGRebuiltDeveloper Erik7302 = new()
        {
            DisplayName = "Erik7302",
            Color = new Color("#2B2121"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(PositronShooter),
            SteamID = 76561198158643960,
        };

        public static DGRebuiltDeveloper Klof44 = new()
        {
            DisplayName = "klof44",
            Color = new Color("#00FA9A"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(PositronShooter),
            SteamID = 76561198441121574,
        };

        public static DGRebuiltDeveloper Moro = new()
        {
            DisplayName = "Can't Sleep",
            Color = new Color("#FFFF00"),
            DevRole = DGRebuiltDeveloper.Role.MinorContributor,
            DevItem = typeof(PositronShooter),  
            SteamID = 76561198207428731,
        };

        public static DGRebuiltDeveloper Tater = new()
        {
            DisplayName = "Tater",
            Color = new Color("#FF9742"),
            DevRole = DGRebuiltDeveloper.Role.MinorContributor,
            DevItem = typeof(PositronShooter),
            SteamID = 76561198090678474,
        };

        public static DGRebuiltDeveloper Tmob03 = new()
        {
            DisplayName = "tmob03",
            Color = new Color("#1F5733"),
            DevRole = DGRebuiltDeveloper.Role.MinorContributor,
            DevItem = typeof(PositronShooter),
            SteamID = 76561198799204731,
        };

        public static DGRebuiltDeveloper TheKingOfCringe11 = new()
        {
            DisplayName = "TheKingOfCringe11",
            Color = new Color("#6ADE7E"),
            DevRole = DGRebuiltDeveloper.Role.MinorContributor,
            DevItem = typeof(PositronShooter),
            SteamID = 76561199015640200,
        };
        // sorted by importance
        public static readonly DGRebuiltDeveloper[] All =
        {
            Collin,
            Dan,
            NiK0,
            Firebreak,
            Othello7,
            Hyeve,
            Lutalli,
            Erik7302,
            Klof44,
            Moro,
            Tater,
            Tmob03,
            TheKingOfCringe11,
        };
        //NiK0 stuff
        public static readonly DGRebuiltDeveloper[] AllWithGuns =
        {
            Collin,
            Dan,
            NiK0,
            Firebreak,
            Othello7,
            Hyeve,
            Lutalli,
            Erik7302,
            Klof44,
        };
    }

    public struct DGRebuiltDeveloper
    {
        public string DisplayName;
        public Color Color;
        public string ColorTag => Color.ToDGColorString();
        public Role DevRole;
        public ulong SteamID;
        public Type DevItem;

        public enum Role
        {
            Collin,
            LeadDeveloper,
            ActiveContributor,
            HelpingHand,
            MinorContributor,
        }
    }
}