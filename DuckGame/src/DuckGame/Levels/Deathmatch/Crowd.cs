// Decompiled with JetBrains decompiler
// Type: DuckGame.Crowd
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Crowd : Thing
    {
        private static CrowdCore _core = new CrowdCore();
        public static int crowdSeed = 0;
        private static HelperChairDistanceSorter helperChairDistanceSorter = new HelperChairDistanceSorter();
        private static Dictionary<Profile, FanNum> fanList = new Dictionary<Profile, FanNum>();
        private static int extraFans;

        public static CrowdCore core
        {
            get => _core;
            set => _core = value;
        }

        public static Mood mood
        {
            get => _core._mood;
            set => _core._newMood = value;
        }

        public static List<List<CrowdDuck>> _members => _core._members;

        public static int fansUsed
        {
            get => _core.fansUsed;
            set => _core.fansUsed = value;
        }

        public Crowd()
          : base()
        {
        }

        public BitBuffer NetSerialize()
        {
            BitBuffer bitBuffer = new BitBuffer();
            int val = 0;
            foreach (List<CrowdDuck> member in _members)
            {
                foreach (CrowdDuck crowdDuck in member)
                {
                    if (crowdDuck.empty)
                    {
                        ++val;
                    }
                    else
                    {
                        if (val > 0)
                        {
                            bitBuffer.Write(false);
                            bitBuffer.Write((byte)val);
                            val = 0;
                        }
                        bitBuffer.Write(true);
                        bitBuffer.WritePacked(crowdDuck.duckColor, 2);
                        bitBuffer.WritePacked((crowdDuck.lastLoyalty != null ? crowdDuck.lastLoyalty.networkIndex : -1) + 1, 3);
                        bitBuffer.WritePacked((crowdDuck.loyalty != null ? crowdDuck.loyalty.networkIndex : -1) + 1, 3);
                        if (crowdDuck.loyalty != null || crowdDuck.lastLoyalty != null)
                            bitBuffer.Write(crowdDuck.loyal);
                    }
                }
            }
            if (val > 0)
            {
                bitBuffer.Write(false);
                bitBuffer.Write((byte)val);
            }
            bitBuffer.Write(crowdSeed);
            return bitBuffer;
        }

        public void NetDeserialize(BitBuffer data)
        {
            foreach (List<CrowdDuck> member in _members)
            {
                foreach (Thing thing in member)
                    Level.Remove(thing);
            }
            _members.Clear();
            int num1 = 0;
            float num2 = -18f;
            float num3 = 2f;
            float zpos = -30f;
            for (int index = 0; index < 4; ++index)
            {
                _members.Add(new List<CrowdDuck>());
                for (int dist = 0; dist < 45; ++dist)
                {
                    Profile varLoyalty = null;
                    Profile varLastLoyalty = null;
                    bool varLoyal = false;
                    int varColor = 0;
                    bool flag = false;
                    if (num1 == 0)
                    {
                        if (!data.ReadBool())
                        {
                            num1 = data.ReadByte();
                        }
                        else
                        {
                            varColor = (int)data.ReadBits(typeof(int), 2);
                            int num4 = (int)data.ReadBits(typeof(int), 3);
                            int num5 = (int)data.ReadBits(typeof(int), 3);
                            if (num4 > 0 || num5 > 0)
                            {
                                if (num5 > 0)
                                    varLoyalty = DuckNetwork.profiles[num5 - 1];
                                if (num4 > 0)
                                    varLastLoyalty = DuckNetwork.profiles[num4 - 1];
                                varLoyal = data.ReadBool();
                            }
                        }
                    }
                    if (num1 > 0)
                    {
                        flag = true;
                        --num1;
                    }
                    int facing = dist < 9 ? 0 : (dist < 16 ? 1 : 2);
                    CrowdDuck crowdDuck = new CrowdDuck(dist * 30 - 30 + 3, num3 + num2, zpos, facing, index, dist, flag ? 0 : 1, varLoyalty, varLastLoyalty, varLoyal, varColor);
                    _members[index].Add(crowdDuck);
                }
                zpos -= 20f;
                num3 -= 11f;
            }
            crowdSeed = data.ReadInt();
            foreach (List<CrowdDuck> member in _members)
            {
                foreach (Thing thing in member)
                    Level.Add(thing);
            }
            InitSigns();
        }

        public static void GoHome()
        {
            _members.Clear();
            fansUsed = 0;
        }

        public static void ThrowHats(Profile p)
        {
            foreach (List<CrowdDuck> member in _members)
            {
                foreach (CrowdDuck crowdDuck in member)
                    crowdDuck.ThrowHat(p);
            }
        }

        public static void InitializeCrowd()
        {
            if (Network.isClient)
                return;
            if (_members.Count == 0)
            {
                fanList.Clear();
                foreach (Profile key in Profiles.active)
                {
                    if (key.slotType != SlotType.Spectator)
                        fanList[key] = new FanNum()
                        {
                            profile = key,
                            loyalFans = key.stats.loyalFans,
                            unloyalFans = key.stats.unloyalFans
                        };
                }
                int totalFansThisGame = Profile.totalFansThisGame;
                int max = (int)(20f + Profile.totalFansThisGame * 0.1f);
                if (max > 36)
                    max = 36;
                if (max < 0)
                    max = 0;
                extraFans = Rando.Int(max / 2, max);
                float num = -18f;
                _members.Add(new List<CrowdDuck>());
                List<int> list1 = new List<int>();
                for (int index = 0; index < 45; ++index)
                    list1.Add(index);
                list1.Shuffle();
                foreach (int dist in list1)
                {
                    int facing = dist < 9 ? 0 : (dist < 16 ? 1 : 2);
                    _members[0].Add(new CrowdDuck(dist * 30 - 30 + 3, 2f + num, -30f, facing, 0, dist));
                }
                _members[0].Sort(helperChairDistanceSorter);
                _members.Add(new List<CrowdDuck>());
                List<int> list2 = new List<int>();
                for (int index = 0; index < 45; ++index)
                    list2.Add(index);
                list2.Shuffle();
                foreach (int dist in list2)
                {
                    int facing = dist < 9 ? 0 : (dist < 16 ? 1 : 2);
                    _members[1].Add(new CrowdDuck(dist * 30 - 30 + 3, num - 9f, -50f, facing, 1, dist));
                }
                _members[1].Sort(helperChairDistanceSorter);
                _members.Add(new List<CrowdDuck>());
                List<int> list3 = new List<int>();
                for (int index = 0; index < 45; ++index)
                    list3.Add(index);
                list3.Shuffle();
                foreach (int dist in list3)
                {
                    int facing = dist < 9 ? 0 : (dist < 16 ? 1 : 2);
                    _members[2].Add(new CrowdDuck(dist * 30 - 30 + 3, num - 20f, -70f, facing, 2, dist));
                }
                _members[2].Sort(helperChairDistanceSorter);
                _members.Add(new List<CrowdDuck>());
                List<int> list4 = new List<int>();
                for (int index = 0; index < 45; ++index)
                    list4.Add(index);
                list4.Shuffle();
                foreach (int dist in list4)
                {
                    int facing = dist < 9 ? 0 : (dist < 16 ? 1 : 2);
                    _members[3].Add(new CrowdDuck(dist * 30 - 30 + 3, num - 31f, -90f, facing, 3, dist));
                }
                _members[3].Sort(helperChairDistanceSorter);
            }
            if (Level.current is RockScoreboard)
            {
                foreach (List<CrowdDuck> member in _members)
                {
                    foreach (CrowdDuck crowdDuck in member)
                    {
                        crowdDuck.ClearActions();
                        Level.Add(crowdDuck);
                    }
                }
            }
            crowdSeed = Rando.Int(1999999);
            InitSigns();
        }

        public static void UpdateFans()
        {
            float num1 = 0f;
            float num2 = 999f;
            float num3 = -999f;
            List<float> floatList = new List<float>();
            List<Profile> profileList = new List<Profile>();
            int num4 = 0;
            foreach (Profile profile in Profiles.active)
            {
                if (profile.slotType != SlotType.Spectator)
                {
                    ++num4;
                    float profileScore = profile.endOfRoundStats.CalculateProfileScore();
                    if (profileScore < num2)
                        num2 = profileScore;
                    else if (profileScore > num3)
                        num3 = profileScore;
                    num1 += profileScore;
                    floatList.Add(profileScore);
                    profileList.Add(profile);
                }
            }
            float num5 = num1 / num4;
            foreach (List<CrowdDuck> member in _members)
            {
                foreach (CrowdDuck crowdDuck in member)
                {
                    if (!crowdDuck.empty)
                    {
                        for (int index = 0; index < floatList.Count; ++index)
                        {
                            float awesomeness = floatList[index] - num5;
                            if (awesomeness > 0.5)
                                awesomeness = 0.5f;
                            if (awesomeness < -0.5)
                                awesomeness = -0.5f;
                            crowdDuck.TryChangingAllegiance(profileList[index], awesomeness);
                        }
                    }
                }
            }
        }

        public static bool HasFansLeft()
        {
            foreach (KeyValuePair<Profile, FanNum> fan in fanList)
            {
                if (fan.Value.totalFans > 0)
                    return true;
            }
            return extraFans > 0;
        }

        public static FanNum GetFan()
        {
            if (extraFans > 0 && Rando.Float(1f) > 0.5)
            {
                --extraFans;
                return null;
            }
            List<FanNum> fanNumList = new List<FanNum>();
            foreach (KeyValuePair<Profile, FanNum> fan in fanList)
            {
                if (fan.Value.totalFans > 0)
                    fanNumList.Add(fan.Value);
            }
            if (fanNumList.Count == 0)
                return null;
            FanNum fanNum;
            while (true)
            {
                do
                {
                    fanNum = fanNumList[Rando.Int(fanNumList.Count - 1)];
                    if (fanNumList.Count == 1)
                        goto label_14;
                }
                while (Math.Min(fanNum.loyalFans, 100) / 100f * 0.5f + Rando.Float(0.5f) >= Rando.Float(1f));
                fanNumList.Remove(fanNum);
            }
        label_14:
            Profile profile = fanNum.profile;
            if (fanNum.loyalFans > 0 && fanNum.unloyalFans == 0 | Rando.Float(1f) > 0.3f)
            {
                --fanNum.loyalFans;
                return new FanNum()
                {
                    profile = profile,
                    loyalFans = 1
                };
            }
            --fanNum.unloyalFans;
            return new FanNum()
            {
                profile = profile,
                unloyalFans = 1
            };
        }

        public static int totalFans
        {
            get
            {
                int extraFans = Crowd.extraFans;
                foreach (KeyValuePair<Profile, FanNum> fan in fanList)
                    extraFans += fan.Value.totalFans;
                return extraFans;
            }
        }

        private static void InitSigns()
        {
            Random generator = Rando.generator;
            Rando.generator = new Random(crowdSeed);
            for (int rowOnly = 0; rowOnly < 4; ++rowOnly)
            {
                string str = "DUCK GAME";
                if (Rando.Int(10000) == 1)
                    str = "LOL";
                else if (Rando.Int(100) == 1)
                    str = "WE LOVE IT";
                else if (Rando.Int(20) == 1)
                    str = "LETS ROCK";
                else if (Rando.Int(1000000) == 1)
                    str = "www.wonthelp.info";
                Profile p = null;
                if (Rando.Float(1f) > 0.5)
                {
                    List<Team> winning = Teams.winning;
                    if (winning.Count > 0)
                    {
                        Team team = winning[Rando.Int(winning.Count - 1)];
                        Profile activeProfile = team.activeProfiles[Rando.Int(team.activeProfiles.Count - 1)];
                        str = !Profiles.IsDefault(activeProfile) ? activeProfile.nameUI : activeProfile.team.name;
                        p = activeProfile;
                    }
                }
                List<CrowdDuck> availableRow = GetAvailableRow(str.Length, p, rowOnly);
                if (Rando.Float(1f) > 0.96f && availableRow != null)
                {
                    int num = 0;
                    foreach (CrowdDuck crowdDuck in availableRow)
                    {
                        crowdDuck.SetLetter(str.Substring(num, 1), num, p: p);
                        ++num;
                    }
                }
            }
            Rando.generator = generator;
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeCrowd();
        }

        private static List<CrowdDuck> GetAvailableRow(int num, Profile p, int rowOnly = -1)
        {
            List<List<CrowdDuck>> crowdDuckListList = new List<List<CrowdDuck>>();
            int index = 0;
            if (rowOnly != -1)
                index = rowOnly;
            for (; index < (rowOnly != -1 ? rowOnly + 1 : 4); ++index)
            {
                List<CrowdDuck> crowdDuckList = new List<CrowdDuck>();
                foreach (CrowdDuck crowdDuck in _members[index])
                {
                    if (!crowdDuck.empty && !crowdDuck.busy && (p == null || crowdDuck.loyalty == p))
                    {
                        crowdDuckList.Add(crowdDuck);
                    }
                    else
                    {
                        if (crowdDuckList.Count >= num)
                            crowdDuckListList.Add(crowdDuckList);
                        crowdDuckList = new List<CrowdDuck>();
                    }
                }
                if (crowdDuckList.Count >= num)
                    crowdDuckListList.Add(crowdDuckList);
            }
            if (crowdDuckListList.Count <= 0)
                return null;
            List<CrowdDuck> range = crowdDuckListList[Rando.Int(crowdDuckListList.Count - 1)];
            if (range.Count > num)
                range = range.GetRange(Rando.Int(range.Count - num), num);
            return range;
        }

        public override void Update()
        {
            if (_core._newMood == _core._mood)
                return;
            _core._moodWait -= 0.15f;
            if (_core._moodWait >= 0.0)
                return;
            _core._mood = _core._newMood;
            _core._moodWait = 1f;
        }

        private class HelperChairDistanceSorter : IComparer<CrowdDuck>
        {
            int IComparer<CrowdDuck>.Compare(CrowdDuck a, CrowdDuck b) => a.distVal - b.distVal;
        }
    }
}
