// Decompiled with JetBrains decompiler
// Type: DuckGame.ProfileStats
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class ProfileStats : DataClass
    {
        private List<string> _hotnessStrings = new List<string>()
    {
      "Absolute Zero",
      "Icy Moon",
      "Antarctica",
      "Ice Cube",
      "Ice Cream",
      "Coffee",
      "Fire",
      "A Volcanic Eruption",
      "The Sun"
    };
        private Dictionary<string, int> _timesKilledBy = new Dictionary<string, int>();

        public float GetStatCalculation(StatInfo info)
        {
            switch (info)
            {
                case StatInfo.KillDeathRatio:
                    return kills / timesKilled;
                case StatInfo.Coolness:
                    return coolness;
                case StatInfo.ProfileScore:
                    return GetProfileScore();
                default:
                    return 0f;
            }
        }

        public int GetProfileScore() => (int)Math.Round(Maths.Clamp((float)(CalculateProfileScore() * 0.3f * 250.0), -50f, 200f));

        public string GetCoolnessString() => _hotnessStrings[(int)Math.Floor((Maths.Clamp(GetProfileScore(), -50, 200) + 50) / 250.0 * 8.99f)];

        public string currentTitle { get; set; }

        public int kills { get; set; }

        public int suicides { get; set; }

        public int timesKilled { get; set; }

        public int matchesWon { get; set; }

        public int trophiesSinceLastWinCounter { get; set; }

        public int trophiesSinceLastWin { get; set; }

        public int timesSpawned { get; set; }

        public int trophiesWon { get; set; }

        public int gamesPlayed { get; set; }

        public int fallDeaths { get; set; }

        public int timesSwore { get; set; }

        public int bulletsFired { get; set; }

        public int bulletsThatHit { get; set; }

        public int trickShots { get; set; }

        public void LogKill(Profile p)
        {
            string key = "";
            if (p != null)
                key = p.name;
            if (!_timesKilledBy.ContainsKey(key))
                _timesKilledBy[key] = 0;
            ++_timesKilledBy[key];
        }

        public ProfileStats()
        {
            lastPlayed = DateTime.Now;
            lastWon = DateTime.MinValue;
            currentTitle = "";
            _nodeName = "Stats";
        }

        public DateTime lastPlayed { get; set; }

        public DateTime lastWon { get; set; }

        public DateTime lastKillTime { get; set; }

        public int coolness { get; set; }

        public int unarmedDucksShot { get; set; }

        public int killsFromTheGrave { get; set; }

        public int timesNetted { get; set; }

        public float timeInNet { get; set; }

        public int GetFans()
        {
            if (loyalFans < 0)
                loyalFans = 0;
            if (unloyalFans < 0)
                unloyalFans = 0;
            return loyalFans + unloyalFans;
        }

        public bool TryFanTransfer(Profile to, float awesomeness, bool loyal)
        {
            if (unloyalFans > 0 && !loyal)
            {
                --unloyalFans;
                return true;
            }
            if (loyalFans > 0 && Rando.Float(3f) < awesomeness)
            {
                MakeFanUnloyal();
                if (loyal)
                    return true;
            }
            return false;
        }

        public void MakeFanLoyal()
        {
            --unloyalFans;
            ++loyalFans;
        }

        public void MakeFanUnloyal()
        {
            ++unloyalFans;
            --loyalFans;
        }

        public bool FanConsidersLeaving(float awfulness, bool loyal)
        {
            if (unloyalFans > 0 && !loyal)
            {
                --unloyalFans;
                return true;
            }
            if (loyalFans > 0 && Rando.Float(3f) < Math.Abs(awfulness))
            {
                MakeFanUnloyal();
                if (loyal)
                    return true;
            }
            return false;
        }

        public int loyalFans { get; set; }

        public int unloyalFans { get; set; }

        public float timeUnderMindControl { get; set; }

        public int timesMindControlled { get; set; }

        public float timeOnFire { get; set; }

        public int timesLitOnFire { get; set; }

        public float airTime { get; set; }

        public int timesJumped { get; set; }

        public int disarms { get; set; }

        public int timesDisarmed { get; set; }

        public int quacks { get; set; }

        public float timeWithMouthOpen { get; set; }

        public float timeSpentOnMines { get; set; }

        public int minesSteppedOn { get; set; }

        public float timeSpentReloadingOldTimeyWeapons { get; set; }

        public int presentsOpened { get; set; }

        public int respectGivenToDead { get; set; }

        public int funeralsPerformed { get; set; }

        public int funeralsRecieved { get; set; }

        public float timePreaching { get; set; }

        public int conversions { get; set; }

        public int timesConverted { get; set; }

        public float CalculateProfileScore(bool log = false)
        {
            List<StatContribution> statContributionList = new List<StatContribution>();
            float num1 = 0f;
            float num2 = 0f;
            float num3 = 0f;
            float num4 = 0f;
            if (timesSpawned > 0)
                num4 = (matchesWon / timesSpawned * 0.4f);
            float num5 = num1 + num4;
            if (num4 > 0f)
                num3 += num4;
            else if (num4 < 0f)
                num2 += num4;
            statContributionList.Add(new StatContribution()
            {
                name = "MAT",
                amount = num4
            });
            if (gamesPlayed > 0)
                num4 = (trophiesWon / gamesPlayed * 0.4f);
            float num6 = num5 + num4;
            if (num4 > 0f)
                num3 += num4;
            else if (num4 < 0f)
                num2 += num4;
            statContributionList.Add(new StatContribution()
            {
                name = "WON",
                amount = num4
            });
            int num7 = timesKilled;
            if (num7 < 1)
                num7 = 1;
            float num8 = (float)Math.Log(1f + kills / num7) * 0.4f;
            float num9 = num6 + num8;
            if (num8 > 0f)
                num3 += num8;
            else if (num8 < 0f)
                num2 += num8;
            statContributionList.Add(new StatContribution()
            {
                name = "KDR",
                amount = num8
            });
            float num10 = (float)(Maths.Clamp((DateTime.Now - lastPlayed).Days, 0, 60) / 60f * 0.5f);
            float num11 = num9 + num10;
            if (num10 > 0f)
                num3 += num10;
            else if (num10 < 0f)
                num2 += num10;
            statContributionList.Add(new StatContribution()
            {
                name = "LVE",
                amount = num10
            });
            float num12 = (float)Math.Log(1.0 + quacks * 0.0001f) * 0.4f;
            float num13 = num11 + num12;
            if (num12 > 0f)
                num3 += num12;
            else if (num12 < 0f)
                num2 += num12;
            statContributionList.Add(new StatContribution()
            {
                name = "CHR",
                amount = num12
            });
            float num14 = (float)Math.Log(0.75f + coolness * 0.025f);
            float num15 = num13 + num14;
            if (num14 > 0f)
                num3 += num14;
            else if (num14 < 0f)
                num2 += num14;
            statContributionList.Add(new StatContribution()
            {
                name = "COO",
                amount = num14
            });
            float num16 = (float)Math.Log(1f + bulletsFired * 0.0001f);
            float num17 = num15 + num16;
            if (num16 > 0f)
                num3 += num16;
            else if (num16 < 0f)
                num2 += num16;
            statContributionList.Add(new StatContribution()
            {
                name = "SHT",
                amount = num16
            });
            if (bulletsFired > 0)
                num16 = (float)(bulletsThatHit / bulletsFired * 0.2f - 0.1f);
            float num18 = num17 + num16;
            if (num16 > 0f)
                num3 += num16;
            else if (num16 < 0f)
                num2 += num16;
            statContributionList.Add(new StatContribution()
            {
                name = "ACC",
                amount = num16
            });
            float num19 = (float)Math.Log(1f + disarms * 0.0005f) * 0.5f;
            float num20 = num18 + num19;
            if (num19 > 0f)
                num3 += num19;
            else if (num19 < 0f)
                num2 += num19;
            statContributionList.Add(new StatContribution()
            {
                name = "DSM",
                amount = num19
            });
            float num21 = (float)-(Math.Log(1f + (timesLitOnFire + timesMindControlled + timesNetted + timesDisarmed + minesSteppedOn + fallDeaths) * 0.0005f) * 0.5f);
            float num22 = num20 + num21;
            if (num21 > 0f)
                num3 += num21;
            else if (num21 < 0f)
                num2 += num21;
            statContributionList.Add(new StatContribution()
            {
                name = "BAD",
                amount = num21
            });
            float num23 = (float)(-(Maths.Clamp((DateTime.Now - lastWon).Days, 0, 60) / 60f) * 0.3f);
            float num24 = num22 + num23;
            if (num23 > 0f)
                num3 += num23;
            else if (num23 < 0f)
                num2 += num23;
            statContributionList.Add(new StatContribution()
            {
                name = "LOS",
                amount = num23
            });
            float num25 = (float)Math.Log(1f + timesJumped * 0.0001f) * 0.2f;
            float num26 = num24 + num25;
            if (num25 > 0f)
                num3 += num25;
            else if (num25 < 0f)
                num2 += num25;
            statContributionList.Add(new StatContribution()
            {
                name = "JMP",
                amount = num25
            });
            float num27 = (float)Math.Log(1f + timeWithMouthOpen * (1f / 1000f)) * 0.5f;
            float num28 = num26 + num27;
            if (num27 > 0f)
                num3 += num27;
            else if (num27 < 0f)
                num2 += num27;
            statContributionList.Add(new StatContribution()
            {
                name = "MTH",
                amount = num27
            });
            float num29 = (float)Math.Log(1f + timesSwore) * 0.5f;
            float profileScore = num28 + num29;
            if (num29 > 0f)
                num3 += num29;
            else if (num29 < 0f)
                num2 += num29;
            statContributionList.Add(new StatContribution()
            {
                name = "SWR",
                amount = num29
            });
            if (log && profileScore != 0f)
            {
                foreach (StatContribution statContribution in statContributionList)
                {
                    float num30 = 0f;
                    if (statContribution.amount != 0f)
                        num30 = statContribution.amount <= 0f ? (Math.Abs(statContribution.amount) / Math.Abs(num2) * (Math.Abs(num2) / (num3 + Math.Abs(num2)))) : (Math.Abs(statContribution.amount) / Math.Abs(num3) * (num3 / (num3 + Math.Abs(num2))));
                    if (statContribution.amount < 0f)
                        num30 = -num30;
                    float num31 = 0.5f;
                    float num32 = 0.5f;
                    float r;
                    float g;
                    if (num30 < 0f)
                    {
                        r = num31 + Math.Abs(num30) * 0.5f;
                        g = num32 - Math.Abs(num30) * 0.5f;
                    }
                    else
                    {
                        g = num32 + Math.Abs(num30) * 0.5f;
                        r = num31 - Math.Abs(num30) * 0.5f;
                    }
                    DevConsole.Log(statContribution.name + ": " + (num30 * 100f).ToString("0.000") + "%", new Color(r, g, 0f), 1f);
                }
            }
            return profileScore;
        }
    }
}
