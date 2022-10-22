// Decompiled with JetBrains decompiler
// Type: DuckGame.Challenges
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class Challenges
    {
        public static int valueBronze = 15;
        public static int valueSilver = 5;
        public static int valueGold = 5;
        public static int valuePlatinum = 12;
        private static Dictionary<string, ChallengeData> _challenges = new Dictionary<string, ChallengeData>();
        private static List<ChallengeData> _challengesInArcade;

        public static Dictionary<string, ChallengeData> challenges => Challenges._challenges;

        public static ChallengeData LoadChallengeData(string pLevel)
        {
            if (pLevel == null)
                return null;
            ChallengeData challengeData;
            if (Challenges._challenges.TryGetValue(pLevel, out challengeData))
                return challengeData;
            LevelData levelData = Content.GetLevel(pLevel) ?? DuckFile.LoadLevel(pLevel);
            if (levelData != null)
            {
                string guid = levelData.metaData.guid;
                foreach (BinaryClassChunk node in levelData.objects.objects)
                {
                    string property = node.GetProperty<string>("type");
                    ChallengeMode challengeMode = null;
                    try
                    {
                        if (property != null && property.Contains("DuckGame.ChallengeMode,"))
                            challengeMode = Thing.LoadThing(node, false) as ChallengeMode;
                        else if (property != null && property.Contains("DuckGame.ChallengeModeNew,"))
                            challengeMode = Thing.LoadThing(node, false) as ChallengeModeNew;
                        if (challengeMode != null)
                        {
                            challengeMode.challenge.fileName = pLevel;
                            challengeMode.challenge.levelID = guid;
                            challengeMode.challenge.preview = levelData.previewData.preview;
                            Challenges._challenges.Add(levelData.metaData.guid, challengeMode.challenge);
                            return challengeMode.challenge;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return null;
        }

        public static void Initialize()
        {
            foreach (string level in Content.GetLevels("challenge", LevelLocation.Content, true, false, false))
            {
                string challenge = level;
                MonoMain.currentActionQueue.Enqueue(new LoadingAction(() => Challenges.LoadChallengeData(challenge)));
            }
        }

        public static void LoadElementsFromNode(string pName, DXMLNode pNode)
        {
            foreach (DXMLNode element in pNode.Elements("challengeSaveData"))
            {
                ChallengeSaveData challengeSaveData = new ChallengeSaveData();
                challengeSaveData.LegacyDeserialize(element);
                if (challengeSaveData.trophy == TrophyType.Developer)
                    Options.Data.gotDevMedal = true;
                challengeSaveData.challenge = pName;
                Profile profile = Profiles.Get(challengeSaveData.profileID);
                if (profile != null && !profile.challengeData.ContainsKey(pName))
                {
                    profile.challengeData.Add(pName, challengeSaveData);
                    challengeSaveData.profileID = profile.id;
                }
            }
        }

        public static void InitializeChallengeData()
        {
            foreach (string file in DuckFile.GetFiles(DuckFile.challengeDirectory))
            {
                DuckXML duckXml = DuckFile.LoadDuckXML(file);
                if (duckXml != null)
                {
                    string withoutExtension = Path.GetFileNameWithoutExtension(file);
                    DXMLNode pNode = duckXml.Element("Data");
                    if (pNode != null)
                        Challenges.LoadElementsFromNode(withoutExtension, pNode);
                }
            }
        }

        public static int GetNumTrophies(Profile p)
        {
            int numTrophies = 0;
            foreach (KeyValuePair<string, ChallengeData> challenge in Challenges._challenges)
            {
                ChallengeSaveData saveData = p.GetSaveData(challenge.Value.levelID, true);
                if (saveData != null && saveData.trophy != TrophyType.Baseline)
                    ++numTrophies;
            }
            return numTrophies;
        }

        public static ChallengeSaveData GetSaveData(
          string guid,
          Profile p,
          bool canBeNull = false)
        {
            return p.GetSaveData(guid, canBeNull);
        }

        public static List<ChallengeSaveData> GetAllSaveData(Profile p)
        {
            List<ChallengeSaveData> allSaveData = new List<ChallengeSaveData>();
            foreach (KeyValuePair<string, ChallengeSaveData> keyValuePair in p.challengeData)
                allSaveData.Add(keyValuePair.Value);
            return allSaveData;
        }

        public static List<ChallengeSaveData> GetAllSaveData()
        {
            List<ChallengeSaveData> allSaveData = new List<ChallengeSaveData>();
            foreach (Profile profile in Profiles.all)
            {
                foreach (KeyValuePair<string, ChallengeSaveData> keyValuePair in profile.challengeData)
                    allSaveData.Add(keyValuePair.Value);
            }
            return allSaveData;
        }

        public static ChallengeData GetChallenge(string name)
        {
            ChallengeData challengeData;
            return !Challenges._challenges.TryGetValue(name, out challengeData) ? Challenges.LoadChallengeData(name) : challengeData;
        }

        public static List<ChallengeData> GetEligibleChancyChallenges(Profile p)
        {
            List<ChallengeData> chancyChallenges = new List<ChallengeData>();
            foreach (KeyValuePair<string, ChallengeData> challenge1 in Challenges.challenges)
            {
                if (challenge1.Value.requirement != "" && challenge1.Value.CheckRequirement(p))
                {
                    if (challenge1.Value.prevchal != "")
                    {
                        ChallengeData challenge2 = Challenges.GetChallenge(challenge1.Value.prevchal);
                        ChallengeSaveData saveData = p.GetSaveData(challenge2.levelID, true);
                        if (saveData != null && saveData.trophy > TrophyType.Baseline)
                            chancyChallenges.Add(challenge1.Value);
                    }
                    else
                        chancyChallenges.Add(challenge1.Value);
                }
            }
            return chancyChallenges;
        }

        public static List<ChallengeData> GetAllChancyChallenges(
          List<ChallengeData> available = null)
        {
            List<ChallengeData> chancyChallenges = new List<ChallengeData>();
            foreach (KeyValuePair<string, ChallengeData> challenge in Challenges.challenges)
            {
                KeyValuePair<string, ChallengeData> d = challenge;
                if (d.Value.requirement != "" && (d.Value.prevchal == null || d.Value.prevchal == "" || available == null || available.FirstOrDefault<ChallengeData>(x => x != null && x.fileName == d.Value.prevchal) != null))
                    chancyChallenges.Add(d.Value);
            }
            return chancyChallenges;
        }

        public static List<ChallengeData> GetEligibleIncompleteChancyChallenges(
          Profile p)
        {
            List<ChallengeData> chancyChallenges1 = Challenges.GetEligibleChancyChallenges(p);
            List<ChallengeData> chancyChallenges2 = new List<ChallengeData>();
            foreach (ChallengeData challengeData in chancyChallenges1)
            {
                ChallengeSaveData saveData = p.GetSaveData(challengeData.levelID, true);
                if (saveData == null || saveData.trophy < TrophyType.Bronze)
                    chancyChallenges2.Add(challengeData);
            }
            return chancyChallenges2;
        }

        public static float GetChallengeSkillIndex()
        {
            int num1 = 0;
            int num2 = 0;
            List<ChallengeData> available = new List<ChallengeData>();
            if (!(Level.current is ArcadeLevel arcadeLevel))
                arcadeLevel = ArcadeLevel.currentArcade;
            if (arcadeLevel == null)
                return 0f;
            foreach (ArcadeMachine challenge1 in arcadeLevel._challenges)
            {
                foreach (string challenge2 in challenge1.data.challenges)
                    available.Add(Challenges.GetChallenge(challenge2));
            }
            foreach (ChallengeData allChancyChallenge in Challenges.GetAllChancyChallenges(available))
                available.Add(allChancyChallenge);
            foreach (KeyValuePair<string, ChallengeData> challenge in Challenges._challenges)
            {
                if (available.Contains(challenge.Value))
                {
                    num2 += 4;
                    ChallengeSaveData saveData = Profiles.active[0].GetSaveData(challenge.Value.levelID, true);
                    if (saveData != null)
                        num1 += (int)saveData.trophy;
                }
            }
            return num1 / (float)num2;
        }

        public static List<ChallengeData> challengesInArcade
        {
            get
            {
                if (Challenges._challengesInArcade == null)
                {
                    Challenges._challengesInArcade = new List<ChallengeData>();
                    ArcadeLevel arcadeLevel = new ArcadeLevel(Content.GetLevelID("arcade"))
                    {
                        bareInitialize = true
                    };
                    arcadeLevel.InitializeMachines();
                    if (arcadeLevel != null)
                    {
                        foreach (ArcadeMachine challenge1 in arcadeLevel._challenges)
                        {
                            if (!(challenge1 is ImportMachine))
                            {
                                foreach (string challenge2 in challenge1.data.challenges)
                                {
                                    ChallengeData challenge3 = Challenges.GetChallenge(challenge2);
                                    if (challenge3 != null)
                                        Challenges.challengesInArcade.Add(challenge3);
                                }
                            }
                        }
                        foreach (ChallengeData allChancyChallenge in Challenges.GetAllChancyChallenges(Challenges._challengesInArcade))
                            Challenges._challengesInArcade.Add(allChancyChallenge);
                    }
                }
                return Challenges._challengesInArcade;
            }
        }

        public static int GetTicketCount(Profile p)
        {
            int ticketCount = 0;
            foreach (KeyValuePair<string, ChallengeData> challenge in Challenges._challenges)
            {
                if (Challenges.challengesInArcade.Contains(challenge.Value))
                {
                    ChallengeSaveData saveData = p.GetSaveData(challenge.Value.levelID, true);
                    if (saveData != null)
                    {
                        if (saveData.trophy >= TrophyType.Bronze)
                            ticketCount += Challenges.valueBronze;
                        if (saveData.trophy >= TrophyType.Silver)
                            ticketCount += Challenges.valueSilver;
                        if (saveData.trophy >= TrophyType.Gold)
                            ticketCount += Challenges.valueGold;
                        if (saveData.trophy >= TrophyType.Platinum)
                            ticketCount += Challenges.valuePlatinum;
                    }
                }
            }
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Any))
            {
                if (unlock.ProfileUnlocked(p))
                    ticketCount -= unlock.cost;
            }
            return ticketCount;
        }
    }
}
