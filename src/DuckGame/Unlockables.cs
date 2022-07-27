// Decompiled with JetBrains decompiler
// Type: DuckGame.Unlockables
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Unlockables
    {
        //private static bool wawa = false;
        private static List<Unlockable> _unlocks = new List<Unlockable>();
        private static HashSet<Unlockable> _pendingUnlocks = new HashSet<Unlockable>();

        public static List<Unlockable> lockedItems => Unlockables._unlocks.Where<Unlockable>(x => !x.CheckCondition() && x.allowHints && x.description != null && x.description != "").ToList<Unlockable>();

        public static void Initialize()
        {
            Unlockables._unlocks.Add(new UnlockableHats(false, "hatpack1", new List<Team>()
      {
        Teams.GetTeam("BAWB"),
        Teams.GetTeam("Frank"),
        Teams.GetTeam("Meeee")
      }, () => Unlocks.IsUnlocked("HATTY1"), "Hat Pack 1", "Check out these nifty cool hats."));
            Unlockables._unlocks.Add(new UnlockableHats(false, "hatpack2", new List<Team>()
      {
        Teams.GetTeam("Pulpy"),
        Teams.GetTeam("Joey"),
        Teams.GetTeam("Cowboys")
      }, () => Unlocks.IsUnlocked("HATTY2"), "Hat Pack 2", "More cool hats! WOW!"));
            bool futz = false;
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "gamerduck", () => Global.data.timesSpawned > (futz ? 0 : 99), "Duck Gamer", "Spawn 100 times.", "play100"));
            Unlockables._unlocks.Add(new UnlockableHat("chancyHat", Teams.GetTeam("Chancy"), () => Unlocks.IsUnlocked("ULTIMATE"), "Chancy", "Get platinum on all challenges", "chancy"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "ritual", () => Global.data.timesSpawned > (futz ? 1 : 999), "Ritual", "Spawn 1000 times.", "play1000"));
            Unlockables._unlocks.Add(new UnlockableHat("skully", Teams.GetTeam("SKULLY"), () => Global.data.kills > (futz ? 3 : 999), "SKULLY", "Kill 1000 Ducks", "kill1000"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "endurance", () => Global.data.longestMatchPlayed > (futz ? 5 : 49), "Endurance", "Play through a 50 point match", "endurance"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "outgoing", () => Global.data.onlineWins > (futz ? 0 : 9), "Outgoing", "Win 10 online matches", "online10"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "basement", () => Unlocks.IsUnlocked("BASEMENTKEY"), "Basement Dweller", "Unlock the basement", "basement"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "poweruser", () => Global.data.customMapPlayCount.Count > (futz ? 0 : 9), "Power User", "Play on 10 different custom maps", "editor"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "drawbreaker", () => Global.data.drawsPlayed > (futz ? 0 : 9), "Draw Breaker", "Break 10 draws", "drawbreaker"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "hotstuff", () => (double)Profiles.MostTimeOnFire() > (futz ? 2.0 : 899.0), "Hot Stuff", "Spend 15 minutes on fire with any one profile", "fire"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "myboy", () => Profiles.experienceProfile != null && Profiles.experienceProfile.numLittleMen > 0, "That's My Boy", "Raise a little man.", "myboy"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "jukebox", () => Profiles.experienceProfile != null && Profiles.experienceProfile.numLittleMen > 7, "Jukebox Hero", "Raise eight little men.", "jukebox"));
            Unlockables._unlocks.Add(new UnlockableAchievement(false, "kingme", () => Profiles.experienceProfile != null && Profiles.experienceProfile.xp >= DuckNetwork.GetLevel(999).xpRequired, "King Me", "Level up all the way.", "kingme"));
            Unlockables._unlocks.Add(new UnlockableHat("ballz", Teams.GetTeam("BALLZ"), () => Global.data.ducksCrushed > (futz ? 0 : 49), "BALLZ", "Crush 50 Ducks", "crate"));
            Unlockables._unlocks.Add(new UnlockableHat("hearts", Teams.GetTeam("Hearts"), () => Global.data.matchesPlayed > (futz ? 0 : 49), "<3", "Finish 50 whole matches.", "finish50"));
            Unlockables._unlocks.Add(new UnlockableHat("swackHat", Teams.GetTeam("SWACK"), () => Global.data.matchesPlayed > 0, "SWACK", "Play through a match"));
            Unlockables._unlocks.Add(new UnlockableHat("BRODUCK", Teams.GetTeam("BRODUCK"), () => Global.data.strafeDistance > (futz ? 0.25f : 10f), "BRODUCK", "Strafe 10 Kilometers"));
            Unlockables._unlocks.Add(new UnlockableHat("astropal", Teams.GetTeam("astropal"), () => Global.data.jetFuelUsed > (futz ? 5f : 200f), "ASTROPAL", "Burn 200 gallons of rocket fuel."));
            Unlockables._unlocks.Add(new UnlockableHat("eggpal", Teams.GetTeam("eggpal"), () => Global.data.winsAsSwack > (futz ? 0 : 4), "EGGPAL", "Win 5 rounds as SWACK"));
            Unlockables._unlocks.Add(new UnlockableHat("brad", Teams.GetTeam("brad"), () => Global.data.disarms > (futz ? 0 : 99), "BRAD DUNGEON", "Disarm 100 ducks."));
            Unlockables._unlocks.Add(new UnlockableHat("brick", Teams.GetTeam("BRICK"), () => Global.data.laserBulletsFired > (futz ? 0 : 149), "BRICK", "Fire 150 laser bullets."));
            Unlockables._unlocks.Add(new UnlockableHat("ducks", Teams.GetTeam("DUCKS"), () => Global.data.quacks > 999, "DUCK", "Quack 1000 times."));
            Unlockables._unlocks.Add(new UnlockableHat("funnyman", Teams.GetTeam("FUNNYMAN"), () => Global.data.hatsStolen > 100, "FUNNYMAN", "Wear 100 different faces."));
            Unlockables._unlocks.Add(new UnlockableHat("wizards", Teams.GetTeam("Wizards"), () => Global.data.angleShots > 25, "Wizard", "Make 25 angle trick shots."));
            Unlockables._unlocks.Add(new UnlockableHat("wolves", Teams.GetTeam("wolfy"), () => Global.data.playedDuringFullMoon, "Wolves by DORD", "Play at 12 AM on a Full Moon."));
            Unlockables._unlocks.Add(new UnlockableHat("masterHat", Teams.GetTeam("masters"), () => Global.data.nettedDuckTossKills > 25, "BackSlash", "Toss kill 25 netted ducks."));
            Unlockables._unlocks.Add(new UnlockableHat("clamHat", Teams.GetTeam("clams"), () => Global.data.secondsUnderwater > 600, "CLAMS", "Spend 10 minutes under water."));
            Unlockables._unlocks.Add(new UnlockableHat("wafflesHat", Teams.GetTeam("waffles"), () => Global.data.playedInTheMorning, "WAFFLES", "Play Duck Game at 9:00AM."));
            Unlockables._unlocks.Add(new UnlockableHats("toeboynearlsHats", new List<Team>()
      {
        Teams.GetTeam("toeboys"),
        Teams.GetTeam("bigearls")
      }, () => Global.data.presentsOpened > 100, "TOEBOY N BIGEARL", "Open 100 Presents"));
            Unlockables._unlocks.Add(new UnlockableHat("katanaHat", Teams.GetTeam("zeros"), () => Global.data.swordKills > 25, "KATANA ZERO", "Get 25 Sword Kills"));
            Unlockables._unlocks.Add(new UnlockableHat("johnnygreyHat", Teams.GetTeam("johnnygrey"), () => Global.data.typedJohnny, "JOHNNYGREY", "Type 'johnnygrey' into the command console."));
            Unlockables._unlocks.Add(new UnlockableHat("diplomatsHat", Teams.GetTeam("diplomats"), () => (int)Global.data.onlineMatches >= 50, "DIPLOMATS", "Finish 50 online matches."));
            Unlockables._unlocks.Add(new UnlockableHat("b52sHat", Teams.GetTeam("B52s"), () => (int)Global.data.winsAsHair >= 10, "B52s", "Win 10 Matches With Great Hair."));
            Unlockables._unlocks.Add(new UnlockableHats(false, "swimhats", new List<Team>()
      {
        Teams.GetTeam("kerchiefs"),
        Teams.GetTeam("postals"),
        Teams.GetTeam("wahhs")
      }, () => Global.data.bootedSinceSwitchHatPatch > 0 && Global.data.matchesPlayed > 0, "HATS FOR YOU", "Thanks for playing Duck Game!"));
            Unlockables._unlocks.Add(new UnlockableHat("ufosHat", Teams.GetTeam("uufos"), () => Global.data.timeJetpackedAsRagdoll >= 1200, "UUFOS", "Spend 20 seconds ragdoll jetpacking."));
            string str1 = "CYCLOPS";
            if (Teams.GetTeam(str1) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str1.ToLowerInvariant(), Teams.GetTeam(str1), () => Global.boughtHats.Contains("CYCLOPS"), str1, Teams.GetTeam(str1).description));
            string str2 = "MOTHERS";
            if (Teams.GetTeam(str2) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str2.ToLowerInvariant(), Teams.GetTeam(str2), () => Global.boughtHats.Contains("MOTHERS"), str2, Teams.GetTeam(str2).description));
            string str3 = "BIG ROBO";
            if (Teams.GetTeam(str3) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str3.ToLowerInvariant(), Teams.GetTeam(str3), () => Global.boughtHats.Contains("BIG ROBO"), str3, Teams.GetTeam(str3).description));
            string str4 = "TINCAN";
            if (Teams.GetTeam(str4) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str4.ToLowerInvariant(), Teams.GetTeam(str4), () => Global.boughtHats.Contains("TINCAN"), str4, Teams.GetTeam(str4).description));
            string str5 = "WELDERS";
            if (Teams.GetTeam(str5) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str5.ToLowerInvariant(), Teams.GetTeam(str5), () => Global.boughtHats.Contains("WELDERS"), str5, Teams.GetTeam(str5).description));
            string str6 = "PONYCAP";
            if (Teams.GetTeam(str6) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str6.ToLowerInvariant(), Teams.GetTeam(str6), () => Global.boughtHats.Contains("PONYCAP"), str6, Teams.GetTeam(str6).description));
            string str7 = "TRICORNE";
            if (Teams.GetTeam(str7) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str7.ToLowerInvariant(), Teams.GetTeam(str7), () => Global.boughtHats.Contains("TRICORNE"), str7, Teams.GetTeam(str7).description));
            string str8 = "TWINTAIL";
            if (Teams.GetTeam(str8) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str8.ToLowerInvariant(), Teams.GetTeam(str8), () => Global.boughtHats.Contains("TWINTAIL"), str8, Teams.GetTeam(str8).description));
            string str9 = "MAJESTY";
            if (Teams.GetTeam(str9) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str9.ToLowerInvariant(), Teams.GetTeam(str9), () => Global.boughtHats.Contains("MAJESTY"), str9, "Max out your level (holy crap!!)"));
            string str10 = "MOONWALK";
            if (Teams.GetTeam(str10) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str10.ToLowerInvariant(), Teams.GetTeam(str10), () => Global.boughtHats.Contains("MOONWALK"), str10, "Raise 8 little men."));
            string str11 = "HIGHFIVES";
            if (Teams.GetTeam(str11) != null)
                Unlockables._unlocks.Add(new UnlockableHat(false, str11.ToLowerInvariant(), Teams.GetTeam(str11), () => Global.boughtHats.Contains("HIGHFIVES"), str11, Teams.GetTeam(str11).description));
            Unlockables._unlocks.Add(new UnlockableHat(false, "devtimes", Teams.GetTeam("CAPTAIN"), () => (Profile.CalculateLocalFlippers() & 16U) > 0U, "UR THE BEST", "Thank you for playing Duck Game <3"));
            Unlockables._unlocks.Add(new UnlockableHat("eyebob", Teams.GetTeam("eyebob"), () => Global.data.giantLaserKills > 24, "CHARGE SHOT", "Get 25 Kills With the Giant Death Laser"));
            foreach (Unlockable unlock in Unlockables._unlocks)
            {
                unlock.Initialize();
                if (unlock.CheckCondition())
                    unlock.DoUnlock();
            }
            if (MonoMain.GetLocalTime().Hour == 9)
                Global.data.playedInTheMorning = true;
            if (!MonoMain.FullMoon)
                return;
            Global.data.playedDuringFullMoon = true;
        }

        public static bool HasPendingUnlocks()
        {
            if (Profiles.experienceProfile != null)
            {
                if (Profiles.experienceProfile.numLittleMen >= 7 && !Global.boughtHats.Contains("MOONWALK"))
                    Global.boughtHats.Add("MOONWALK");
                if (Profiles.experienceProfile.xp >= DuckNetwork.GetLevel(999).xpRequired && !Global.boughtHats.Contains("MAJESTY"))
                    Global.boughtHats.Add("MAJESTY");
            }
            Unlockables._pendingUnlocks.Clear();
            foreach (Unlockable unlock in Unlockables._unlocks)
            {
                if (unlock.locked)
                {
                    if (unlock.CheckCondition())
                    {
                        if (unlock.showScreen)
                            Unlockables._pendingUnlocks.Add(unlock);
                        else
                            unlock.DoUnlock();
                    }
                    else
                        unlock.DoLock();
                }
            }
            Steam.StoreStats();
            return Unlockables._pendingUnlocks.Count > 0;
        }

        public static void CheckAchievements()
        {
            foreach (Unlockable unlock in Unlockables._unlocks)
            {
                if (unlock.locked && !unlock.showScreen)
                {
                    if (unlock.CheckCondition())
                        unlock.DoUnlock();
                    else
                        unlock.DoLock();
                }
            }
        }

        public static Unlockable GetUnlock(string identifier) => Unlockables._unlocks.FirstOrDefault<Unlockable>(x => x.id == identifier);

        public static HashSet<Unlockable> GetPendingUnlocks() => Unlockables._pendingUnlocks;

        public static void UnlockAll()
        {
            foreach (Unlockable unlock in Unlockables._unlocks)
                unlock.DoUnlock();
        }
    }
}
