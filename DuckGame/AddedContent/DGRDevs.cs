using System;

namespace DuckGame
{
    public static class DGRDevs
    {
        public static void DrawInfo(Vec2 position, DGRebuiltDeveloper dev)
        {
            string[] s = Extensions.SplitByLength(dev.Info);
        }
        public static DGRebuiltDeveloper Collin = new()
        {
            DisplayName = "Collin",
            Color = new Color("#FED95F"),
            DevRole = DGRebuiltDeveloper.Role.Mascot,
            DevItem = typeof(CollinGun),
            SteamID = 76561198114791325,
            Info = "Collin. This is Collin he is the Collin in the DGR Project he did all the Collin, Collin also did the Collin and by the way he did the Collin."
        };

        public static DGRebuiltDeveloper Dan = new()
        {
            DisplayName = "Dan",
            Color = new Color("#00FF00"),
            DevRole = DGRebuiltDeveloper.Role.LeadDeveloper,
            DevItem = typeof(DanGun),
            SteamID = 76561198124539558,
            Info = "YupDanielThatsMe. One of the two lead developers, Dan did all the major optimizations, QOL, ported DGR from XNA to FNA, made the AutoUpdater, many many rewrites, made DGR run native on Linux, made the Sprite Atlas system, made the Graphics Culling system, optimized networking, made the mod recompiler for DGR, and has guided the DGR team through, he did a lot ok?"
        };

        public static DGRebuiltDeveloper NiK0 = new()
        {
            DisplayName = "NiK0",
            Color = new Color("#00FFFF"),
            DevRole = DGRebuiltDeveloper.Role.LeadDeveloper,
            DevItem = typeof(NiK0Gun),
            SteamID = 76561198806685720,
            Info = "NiK0. One of the two lead developers, NiK0 made a lot of QOL additions and has fixed most DGR specific issues, he made almost every single DGR specific item and all the dev weapons, the VGM Music system, the ambient particle system, the weather system, the Heat Wave effect on lava, along with several optimizations to the code all around and guiding the DGR team"
        };

        public static DGRebuiltDeveloper Firebreak = new()
        {
            DisplayName = "Firebreak",
            Color = new Color("#FF5938"),
            DevRole = DGRebuiltDeveloper.Role.ActiveContributor,
            DevItem = typeof(FirebreakGun),
            SteamID = 76561198431449604,
            Info = "Firebreak. A major and active contributor Firebreak has made a lot of QOL changes, a full rework of the DevConsole, a lot of Editor improvements, the DGR Options backend, reworked the AutoUpdater, fixed a ton of issues, the Hat Editor Feather Fashion and many many minor additions"
        };

        public static DGRebuiltDeveloper Othello7 = new()
        {
            DisplayName = "othello7",
            Color = new Color("#A000FF"),
            DevRole = DGRebuiltDeveloper.Role.ActiveContributor,
            DevItem = typeof(othello7Gun),
            SteamID = 76561198797606383,
            Info = "othello7. Active contributor othello7 has helped testing and optimizing the game, he has helped us optimize DGR to its fullest now being able to run it on a raspberry pi 3. othello7 transcribed most DG songs into VGM massively reducing DGR's filesize"
        };

        public static DGRebuiltDeveloper Hyeve = new()
        {
            DisplayName = "Hyeve",
            Color = new Color("#ED32A8"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(HyeveGun),
            SteamID = 76561198138278564,
            Info = "Hyeve. A nice helping hand she made the polygon drawing system, fixed some shader issues, made the lava shader, made some DGR specific maps"
        };

        public static DGRebuiltDeveloper Lutalli = new()
        {
            DisplayName = "Lutalli",
            Color = new Color("#9370DB"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(LutalliGun),
            SteamID = 76561198328210060,
            Info = "Lutalli. A very active helping hand, Lutalli has fixed a lot of random DGR/vanilla issues, fixed some compatability issues with DGR and mods, enhanced the editor, and has generally made DGR be really polished"
        };

        public static DGRebuiltDeveloper Erik7302 = new()
        {
            DisplayName = "Erik7302",
            Color = new Color("#2B2121"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(PositronShooter),
            SteamID = 76561198158643960,
            Info = "Erik7302."
        };

        public static DGRebuiltDeveloper Klof44 = new()
        {
            DisplayName = "klof44",
            Color = new Color("#00FA9A"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(KlofGun),
            SteamID = 76561198441121574,
            Info = "klof44. A helping hand, klof44 has fixed some issues added minor QOL and most importantly implemented the Discord RPC with DGR"
        };

        public static DGRebuiltDeveloper Moro = new()
        {
            DisplayName = "Can't Sleep",
            Color = new Color("#FFFF00"),
            DevRole = DGRebuiltDeveloper.Role.Mascot,
            DevItem = typeof(MoroGun),  
            SteamID = 76561198207428731,
            Info = "Can't Sleep. A minor contributor Can't Sleep has made SFX for some DGR items and sprites/retextures, he made the lightning, made the SFX for many of the energy taped combos and made the sprite for the secret pillow item"
        };

        public static DGRebuiltDeveloper Tater = new()
        {
            DisplayName = "Tater",
            Color = new Color("#FF9742"),
            DevRole = DGRebuiltDeveloper.Role.HelpingHand,
            DevItem = typeof(PositronShooter),
            SteamID = 76561198090678474,
            Info = "Tater. A helping hand, Tater made the entirety of the uncapped FPS system, fixed some issues and managed part of the github making the README and build instructions"
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
            Tater,
            Hyeve,
            Lutalli,
            Erik7302,
            Klof44,
            Moro,
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
            Tater,
            Hyeve,
            Lutalli,
            Erik7302,
            Klof44,
            Moro,
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
        public string Info;

        public enum Role
        {
            Mascot,
            LeadDeveloper,
            ActiveContributor,
            HelpingHand,
            MinorContributor,
        }
    }
}