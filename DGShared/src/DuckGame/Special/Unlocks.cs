// Decompiled with JetBrains decompiler
// Type: DuckGame.Unlocks
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Unlocks
    {
        public static int bronzeTotalTickets;
        public static int silverTotalTickets;
        public static int goldTotalTickets;
        public static int platinumTotalTickets;
        private static List<UnlockData> _unlocks = new List<UnlockData>();
        private static List<UnlockData> _allUnlocks = new List<UnlockData>();
        public static Dictionary<string, byte> modifierToByte = new Dictionary<string, byte>();

        public static void CalculateTreeValues()
        {
            ArcadeLevel arcadeLevel = new ArcadeLevel(Content.GetLevelID("arcade"));
            arcadeLevel.InitializeMachines();
            int numChallenges = 0;
            foreach (ArcadeMachine challenge in arcadeLevel._challenges)
                numChallenges += challenge.data.challenges.Count;
            numChallenges = numChallenges + Challenges.GetAllChancyChallenges().Count;
            int bronzeValue = numChallenges * Challenges.valueBronze;
            int silverValue = numChallenges * Challenges.valueSilver;
            int goldValue = numChallenges * Challenges.valueGold;
            int platinumValue = numChallenges * Challenges.valuePlatinum;
            int bronzeTotal = bronzeValue;
            int silverTotal = bronzeValue + silverValue;
            int goldTotal = bronzeValue + silverValue + goldValue;
            int platinumTotal = bronzeValue + silverValue + goldValue + platinumValue;
            bronzeTotalTickets = bronzeTotal;
            silverTotalTickets = silverTotal;
            goldTotalTickets = goldTotal;
            platinumTotalTickets = platinumTotal;
            int numCheap = 0;
            int numNormal = 0;
            int numHigh = 0;
            int numRidiculous = 0;
            foreach (UnlockData unlock in GetUnlocks(UnlockType.Any))
            {
                if (unlock.priceTier == UnlockPrice.Cheap)
                    ++numCheap;
                else if (unlock.priceTier == UnlockPrice.Normal)
                    ++numNormal;
                else if (unlock.priceTier == UnlockPrice.High)
                    ++numHigh;
                else if (unlock.priceTier == UnlockPrice.Ridiculous)
                    ++numRidiculous;
            }
            int cheapTickets = (int)Math.Round(goldTotal * 0.1f);
            int normalTickets = (int)Math.Round(goldTotal * 0.3f);
            int highTickets = (int)Math.Round(goldTotal * 0.4f);
            int ridiculousTickets = (int)Math.Round(goldTotal * 0.20f);

            int costPerCheap = (int)Math.Round(cheapTickets / (float)numCheap);
            int costPerNormal = (int)Math.Round(normalTickets / (float)numNormal);
            int costPerHigh = (int)Math.Round(highTickets / (float)numHigh);
            int costPerRidiculous = (int)Math.Round(ridiculousTickets / (float)numRidiculous);
            while (costPerCheap * numCheap + costPerNormal * numNormal + costPerHigh * numHigh + costPerRidiculous * numRidiculous > goldTotal)
                --costPerRidiculous;
            while (costPerCheap * numCheap + costPerNormal * numNormal + costPerHigh * numHigh + costPerRidiculous * numRidiculous < goldTotal)
                ++costPerRidiculous;
            foreach (UnlockData unlock in GetUnlocks(UnlockType.Any))
            {
                if (unlock.priceTier == UnlockPrice.Cheap)
                    unlock.cost = costPerCheap;
                else if (unlock.priceTier == UnlockPrice.Normal)
                    unlock.cost = costPerNormal;
                else if (unlock.priceTier == UnlockPrice.High)
                    unlock.cost = costPerHigh;
                else if (unlock.priceTier == UnlockPrice.Ridiculous)
                    unlock.cost = costPerRidiculous;
                else if (unlock.priceTier == UnlockPrice.Chancy)
                    unlock.cost = platinumValue;
            }
        }

        public static bool IsUnlocked(string unlock, Profile pro = null)
        {
            if (FireDebug.Debugging)
            {
                return true;
            }
            foreach (Profile profile in Profiles.all)
            {
                if ((pro == null || profile == pro) && profile.unlocks.Contains(unlock))
                    return true;
            }
            return false;
        }

        public static List<UnlockData> unlocks => new List<UnlockData>(_unlocks);

        public static List<UnlockData> allUnlocks => new List<UnlockData>(_allUnlocks);

        public static List<UnlockData> GetTreeLayer(int layer)
        {
            if (layer == 0)
                return new List<UnlockData>(_unlocks);
            int num = 0;
            List<UnlockData> collection = new List<UnlockData>();
            List<UnlockData> unlockDataList = new List<UnlockData>(_unlocks);
            for (int index = 0; index < unlockDataList.Count; ++index)
            {
                if (unlockDataList[index].children.Count > 0)
                {
                    foreach (UnlockData child in unlockDataList[index].children)
                    {
                        if (!collection.Contains(child))
                            collection.Add(child);
                    }
                }
                if (index == unlockDataList.Count - 1)
                {
                    if (num == layer - 1)
                        return collection;
                    unlockDataList = new List<UnlockData>(collection);
                    collection.Clear();
                    ++num;
                    index = -1;
                }
            }
            return null;
        }

        public static List<UnlockData> GetUnlocks(UnlockType type)
        {
            if (type == UnlockType.Any)
                return new List<UnlockData>(_allUnlocks);
            List<UnlockData> unlocks = new List<UnlockData>();
            foreach (UnlockData allUnlock in _allUnlocks)
            {
                if (allUnlock.type == type)
                    unlocks.Add(allUnlock);
            }
            return unlocks;
        }

        public static UnlockData GetUnlock(string id)
        {
            foreach (UnlockData allUnlock in _allUnlocks)
            {
                if (allUnlock.id == id)
                    return allUnlock;
            }
            return null;
        }

        public static void Initialize()
        {
            UnlockData child1 = new UnlockData()
            {
                name = "Chancy's Key",
                id = "BASEMENTKEY",
                longDescription = "The key to the basement, where some tricky machines are!",
                type = UnlockType.Special,
                cost = 180,
                description = "You'll need this for the basement.",
                icon = 24,
                priceTier = UnlockPrice.Ridiculous,
                layer = 3
            };
            _allUnlocks.Add(child1);
            UnlockData unlockData1 = new UnlockData()
            {
                name = "Moon Gravity",
                id = "MOOGRAV",
                type = UnlockType.Modifier,
                cost = 15,
                description = "Ain't it time|CONCERNED| you got higher?",
                longDescription = "Gravity is greatly reduced. Ducks jump higher and throw things further.",
                icon = 3,
                priceTier = UnlockPrice.Cheap
            };
            _unlocks.Add(unlockData1);
            _allUnlocks.Add(unlockData1);
            UnlockData child2 = new UnlockData()
            {
                name = "Start With Helmet",
                id = "HELMY",
                longDescription = "Ducks start the round with a helmet.",
                type = UnlockType.Modifier,
                cost = 20,
                description = "|CONCERNED|You're tired of being crushed?",
                icon = 11,
                priceTier = UnlockPrice.Cheap
            };
            unlockData1.AddChild(child2);
            _allUnlocks.Add(child2);
            UnlockData child3 = new UnlockData()
            {
                name = "Exploding Props",
                id = "EXPLODEYCRATES",
                longDescription = "Props, such as rocks and crates, will explode when shot.",
                type = UnlockType.Modifier,
                cost = 30,
                description = "Watch  |CONCERNED|where you shoot, OK?.",
                icon = 6,
                priceTier = UnlockPrice.Normal
            };
            unlockData1.AddChild(child3);
            _allUnlocks.Add(child3);
            UnlockData child4 = new UnlockData()
            {
                name = "Ammo, Infinite",
                shortName = "Infinite Ammo",
                id = "INFAMMO",
                longDescription = "Guns will spawn as golden guns, and will not run out of ammo.",
                type = UnlockType.Modifier,
                cost = 35,
                description = "Just shoot. Don't even aim.",
                icon = 13,
                priceTier = UnlockPrice.High
            };
            child3.AddChild(child4);
            _allUnlocks.Add(child4);
            UnlockData child5 = new UnlockData()
            {
                name = "Empty Guns Explode",
                id = "GUNEXPL",
                longDescription = "If you press fire after your gun is empty, it will explode in your hands and kill you.",
                type = UnlockType.Modifier,
                cost = 40,
                description = "Hey, |CONCERNED|watch your ammo count.",
                icon = 17,
                priceTier = UnlockPrice.Normal
            };
            child1.AddChild(child5);
            _allUnlocks.Add(child5);
            UnlockData child6 = new UnlockData()
            {
                name = "Hat Pack 2",
                id = "HATTY2",
                type = UnlockType.Hat,
                cost = 40,
                description = "More hats!",
                icon = 15,
                priceTier = UnlockPrice.High
            };
            child1.AddChild(child6);
            _allUnlocks.Add(child6);
            UnlockData unlockData2 = new UnlockData()
            {
                name = "Hat Pack 1",
                id = "HATTY1",
                type = UnlockType.Hat,
                cost = 20,
                description = "Some hats.",
                icon = 14,
                priceTier = UnlockPrice.Normal
            };
            _unlocks.Add(unlockData2);
            _allUnlocks.Add(unlockData2);
            UnlockData child7 = new UnlockData()
            {
                name = "Presents for Winners",
                id = "WINPRES",
                longDescription = "The winners of every round get a present for the next round.",
                type = UnlockType.Modifier,
                cost = 25,
                description = "It's probably just a rock.",
                icon = 12,
                priceTier = UnlockPrice.Normal
            };
            unlockData2.AddChild(child7);
            _allUnlocks.Add(child7);
            UnlockData child8 = new UnlockData()
            {
                name = "Start With Shoes",
                id = "SHOESTAR",
                longDescription = "Ducks start the round with shoes.",
                type = UnlockType.Modifier,
                cost = 20,
                description = "The most stylin' unlock around.",
                icon = 5,
                priceTier = UnlockPrice.Cheap
            };
            unlockData2.AddChild(child8);
            _allUnlocks.Add(child8);
            UnlockData child9 = new UnlockData()
            {
                name = "QWOP Mode",
                id = "QWOPPY",
                longDescription = "Alternate left and right triggers to move. If you screw up, you fall over.",
                type = UnlockType.Modifier,
                cost = 35,
                description = "Practically impossible.",
                icon = 8,
                priceTier = UnlockPrice.High,
                onlineEnabled = false
            };
            child7.AddChild(child9);
            _allUnlocks.Add(child9);
            child4.AddChild(child1);
            child9.AddChild(child1);
            UnlockData child10 = new UnlockData()
            {
                name = "Start With Jetpack",
                id = "JETTY",
                longDescription = "Ducks start the round with a jetpack.",
                type = UnlockType.Modifier,
                cost = 50,
                description = "You love it.",
                icon = 1,
                priceTier = UnlockPrice.High
            };
            child1.AddChild(child10);
            _allUnlocks.Add(child10);
            UnlockData child11 = new UnlockData()
            {
                name = "Live Grenade On Death",
                shortName = "Grenade On Death",
                id = "CORPSEBLOW",
                longDescription = "When killed, a duck will drop a live grenade.",
                type = UnlockType.Modifier,
                cost = 45,
                description = "Makes death deadly. |CONCERNED|err...",
                icon = 7,
                priceTier = UnlockPrice.Normal
            };
            child1.AddChild(child11);
            _allUnlocks.Add(child11);
            UnlockData child12 = new UnlockData()
            {
                name = "Ultimate Champion",
                id = "ULTIMATE",
                longDescription = "A nifty hat, very expensive. Only the very best can afford this hat!",
                type = UnlockType.Hat,
                cost = 180,
                description = "A hat |RED|only|WHITE| for the very best!",
                icon = 19,
                priceTier = UnlockPrice.Chancy
            };
            child11.AddChild(child12);
            child5.AddChild(child12);
            child6.AddChild(child12);
            child10.AddChild(child12);
            _allUnlocks.Add(child12);
            byte num = 0;
            foreach (UnlockData allUnlock in _allUnlocks)
            {
                if (allUnlock.type == UnlockType.Modifier)
                {
                    modifierToByte[allUnlock.id] = num;
                    ++num;
                }
            }
        }
    }
}
