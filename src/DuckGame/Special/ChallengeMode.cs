// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeMode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ChallengeMode : Thing
    {
        public EditorProperty<bool> random = new EditorProperty<bool>(false);
        public EditorProperty<string> music = new EditorProperty<string>("");
        public EditorProperty<string> Next = new EditorProperty<string>("", increment: 0.0f, isLevel: true);
        private List<ChallengeTrophy> _eligibleTrophies = new List<ChallengeTrophy>();
        private List<ChallengeTrophy> _wonTrophies = new List<ChallengeTrophy>();
        private int _startGoodies;
        public static bool showReticles = true;
        public List<GoalType> goalTypes;
        protected int baselineTargetCount = -1;
        protected int baselineGoodyCount = -1;
        private bool hasTargetLimit;
        //private bool hasGoodyLimit;
        private bool reverseTimeLimit;
        private bool _ended;
        public Duck duck;
        private int restartWait;
        private ContextMenu _hatMenu;
        private int _hatIndex;
        protected ChallengeData _challenge = new ChallengeData();

        public ChallengeMode()
          : base()
        {
            this.graphic = new Sprite("challengeIcon");
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._editorName = "Challenge";
            this._canFlip = false;
            this._canHaveChance = false;
            this.random._tooltip = "If enabled, this challenge will activate a random target sequence whenever all targets are down.";
            this.music._tooltip = "The name of a music file (without extension) from the Duck Game Content/Audio/Music/InGame folder.";
            this.Next._tooltip = "If specified, the next challenge will play immediately after this one (for challenge rush modes).";
        }

        public List<ChallengeTrophy> wonTrophies => this._wonTrophies;

        public override void Initialize()
        {
            ChallengeMode.showReticles = true;
            if (Level.current is Editor)
                return;
            if (Level.current.camera is FollowCam camera)
                camera.minSize = 90f;
            this._eligibleTrophies.AddRange(_challenge.trophies);
            if (ChallengeLevel.timer != null)
            {
                if (this._challenge.trophies[0].timeRequirement != 0)
                    ChallengeLevel.timer.maxTime = new TimeSpan(0, 0, this._challenge.trophies[0].timeRequirement);
                else
                    ChallengeLevel.timer.maxTime = new TimeSpan();
            }
            this._startGoodies = Level.current.things[typeof(Goody)].Count<Thing>();
        }

        public virtual void PrepareCounts()
        {
            this.baselineTargetCount = this._challenge.trophies[0].targets;
            this.baselineGoodyCount = this._challenge.trophies[0].goodies;
            this.goalTypes = new List<GoalType>();
            foreach (GoalType goalType in Level.current.things[typeof(GoalType)])
                this.goalTypes.Add(goalType);
            for (int index = 0; index < this._eligibleTrophies.Count; ++index)
            {
                if (this._eligibleTrophies[index].targets > 0)
                    this.hasTargetLimit = true;
                //else if (this._eligibleTrophies[index].goodies > 0)
                //this.hasGoodyLimit = true;
                if (index > 0 && this._eligibleTrophies[index - 1].timeRequirement < this._eligibleTrophies[index].timeRequirement && this._eligibleTrophies[index - 1].timeRequirement != 0)
                    this.reverseTimeLimit = true;
            }
        }

        public override void Update()
        {
            if (this._ended || ChallengeLevel.timer == null)
                return;
            if (this.duck != null && this.duck.dead)
            {
                ++this.restartWait;
                if (this.restartWait >= 3)
                {
                    if (this._wonTrophies.Count > 0)
                    {
                        this._eligibleTrophies.Clear();
                    }
                    else
                    {
                        this._ended = true;
                        if (Level.current is ChallengeLevel current)
                            current.RestartChallenge();
                    }
                }
            }
            if (this.duck == null)
                return;
            bool flag1 = true;
            for (int index = 0; index < this._eligibleTrophies.Count; ++index)
            {
                bool flag2 = true;
                bool flag3 = false;
                ChallengeTrophy eligibleTrophy = this._eligibleTrophies[index];
                if (eligibleTrophy.type == TrophyType.Developer && (eligibleTrophy.goodies == -1 && eligibleTrophy.targets == -1 && eligibleTrophy.timeRequirement == 0 || !flag1))
                {
                    flag2 = false;
                }
                else
                {
                    bool flag4 = this.reverseTimeLimit;
                    if (eligibleTrophy.timeRequirement != 0 && (int)ChallengeLevel.timer.elapsed.TotalSeconds >= eligibleTrophy.timeRequirement && (eligibleTrophy.type == TrophyType.Baseline || Math.Abs(ChallengeLevel.timer.elapsed.TotalSeconds - eligibleTrophy.timeRequirement) > 0.01f) && (eligibleTrophy.timeRequirementMilliseconds == 0 || (int)Math.Round(ChallengeLevel.timer.elapsed.TotalSeconds % 1f * 100f) > eligibleTrophy.timeRequirementMilliseconds))
                    {
                        flag2 = false;
                        flag4 = !this.reverseTimeLimit;
                    }
                    if (!flag4)
                    {
                        flag3 = true;
                        if (this.baselineTargetCount == -1)
                        {
                            if (eligibleTrophy.targets == -1 && !SequenceItem.IsFinished(SequenceItemType.Target))
                                flag3 = false;
                            else if (eligibleTrophy.targets != -1 && ChallengeLevel.targetsShot < eligibleTrophy.targets)
                                flag3 = false;
                        }
                        else if (ChallengeLevel.targetsShot < this.baselineTargetCount)
                            flag3 = false;
                        if (this.baselineGoodyCount == -1 || this.baselineGoodyCount == 0 && this._challenge.countGoodies)
                        {
                            if ((eligibleTrophy.goodies == -1 || eligibleTrophy.goodies == 0 && this._challenge.countGoodies) && !SequenceItem.IsFinished(SequenceItemType.Goody))
                                flag3 = false;
                            else if (ChallengeLevel.goodiesGot < eligibleTrophy.goodies)
                                flag3 = false;
                        }
                        else if (ChallengeLevel.goodiesGot < this.baselineGoodyCount)
                            flag3 = false;
                        if (eligibleTrophy.targets == -1 && !this.hasTargetLimit)
                        {
                            foreach (GoalType goalType in this.goalTypes)
                            {
                                if (goalType.numObjectsRemaining != 0)
                                    flag3 = false;
                            }
                        }
                    }
                    foreach (GoalType goalType in this.goalTypes)
                    {
                        switch (goalType.Check())
                        {
                            case GoalType.Result.None:
                                flag3 = false;
                                continue;
                            case GoalType.Result.Lose:
                                flag2 = false;
                                continue;
                            default:
                                continue;
                        }
                    }
                    if (flag3)
                    {
                        flag2 = false;
                        if (eligibleTrophy.type != TrophyType.Baseline)
                        {
                            this._wonTrophies.Add(eligibleTrophy);
                            if (DuckNetwork.core.speedrunMaxTrophy > 0 && eligibleTrophy.type == (TrophyType)DuckNetwork.core.speedrunMaxTrophy)
                            {
                                this._eligibleTrophies.Clear();
                                break;
                            }
                        }
                    }
                }
                if (!flag2)
                {
                    this._eligibleTrophies.RemoveAt(index);
                    if (eligibleTrophy.type == TrophyType.Baseline && !flag3)
                        this._eligibleTrophies.Clear();
                    --index;
                }
            }
            if (Level.current != this.level || this._eligibleTrophies.Count != 0)
                return;
            foreach (ChallengeTrophy challengeTrophy in this._wonTrophies.ToList<ChallengeTrophy>())
            {
                if (challengeTrophy.targets != -1 && ChallengeLevel.targetsShot < challengeTrophy.targets)
                    this._wonTrophies.Remove(challengeTrophy);
                if (challengeTrophy.goodies != -1 && ChallengeLevel.goodiesGot < challengeTrophy.goodies)
                    this._wonTrophies.Remove(challengeTrophy);
            }
            if (this._wonTrophies.Count > 1)
            {
                ChallengeTrophy challengeTrophy = this._wonTrophies[0];
                foreach (ChallengeTrophy wonTrophy in this._wonTrophies)
                {
                    if (wonTrophy.type > challengeTrophy.type)
                        challengeTrophy = wonTrophy;
                }
                this._wonTrophies.Clear();
                this._wonTrophies.Add(challengeTrophy);
            }
            ChallengeLevel.timer.Stop();
            if (Level.current is ChallengeLevel current1)
                current1.ChallengeEnded(this);
            this._ended = true;
        }

        public override void Draw()
        {
            if (!(Level.current is Editor))
                return;
            base.Draw();
        }

        public int hatIndex
        {
            get => this._hatIndex;
            set
            {
                this._hatIndex = value;
                this.UpdateMenuHat();
            }
        }

        private void UpdateMenuHat()
        {
            if (this._hatMenu == null)
                return;
            if (Teams.all[this._hatIndex].hasHat)
            {
                this._hatMenu.image = Teams.all[this._hatIndex].hat.CloneMap();
                this._hatMenu.image.center = new Vec2(12f, 12f) + Teams.all[this._hatIndex].hatOffset;
            }
            else
                this._hatMenu.image = null;
        }

        public ChallengeData challenge => this._challenge;

        public override ContextMenu GetContextMenu()
        {
            FieldBinding fieldBinding = new FieldBinding(this, "hatIndex");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            ContextTextbox contextTextbox1 = new ContextTextbox("Name", null, new FieldBinding(_challenge, "name"), "The name of this Challenge.");
            contextMenu.AddItem(contextTextbox1);
            ContextTextbox contextTextbox2 = new ContextTextbox("Desc", null, new FieldBinding(_challenge, "description"), "The Story.");
            contextMenu.AddItem(contextTextbox2);
            ContextTextbox contextTextbox3 = new ContextTextbox("Goal Desc", null, new FieldBinding(_challenge, "goal"), "A description of how to win the challenge.");
            contextMenu.AddItem(contextTextbox3);
            if (!(this is ChallengeModeNew))
            {
                ContextTextbox contextTextbox4 = new ContextTextbox("Requires", null, new FieldBinding(_challenge, "requirement"), "Number of arcade trophies required to unlock. B15 = 15 Bronze. B2P2 = 2 Bronze, 2 Platinum.");
                contextMenu.AddItem(contextTextbox4);
                ContextTextbox contextTextbox5 = new ContextTextbox("noun(plural)", null, new FieldBinding(_challenge, "prefix"), "Name of goal object pluralized, for example \"Ducks\", \"Stars\", etc.");
                contextMenu.AddItem(contextTextbox5);
                contextMenu.AddItem(new ContextFile("Prev", null, new FieldBinding(_challenge, "prevchal"), ContextFileType.Level, "If set, Chancy will offer this challenge as a special challenge after PREV is completed, if REQUIRES is met."));
                contextMenu.AddItem(new ContextCheckBox("Goodies", null, new FieldBinding(_challenge, "countGoodies"), null, "If set, the goal for this challenge is to collect goodies (stars, finish flags, etc)."));
                contextMenu.AddItem(new ContextCheckBox("Targets", null, new FieldBinding(_challenge, "countTargets"), null, "If set, the goal for this challenge is to knock down targets."));
            }
            int num = 0;
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            foreach (ChallengeTrophy trophy in this._challenge.trophies)
            {
                SpriteMap image = new SpriteMap("challengeTrophyIcons", 16, 16)
                {
                    frame = num
                };
                ++num;
                EditorGroupMenu editorGroupMenu = new EditorGroupMenu(contextMenu, image: image);
                if (trophy.type == TrophyType.Bronze)
                    editorGroupMenu.tooltip = "This one should be pretty easy.";
                if (trophy.type == TrophyType.Silver)
                    editorGroupMenu.tooltip = "You're getting there!";
                if (trophy.type == TrophyType.Gold)
                    editorGroupMenu.tooltip = "Someone should win this when they've become pretty good at your challenge.";
                if (trophy.type == TrophyType.Platinum)
                    editorGroupMenu.tooltip = "A hidden trophy that should be pretty tricky to get.";
                if (trophy.type == TrophyType.Developer)
                    editorGroupMenu.tooltip = "Your very best score goes here, it should be harder to get than Platinum!";
                editorGroupMenu.text = trophy.type.ToString();
                if (trophy.type == TrophyType.Baseline)
                {
                    editorGroupMenu.text = "Goals";
                    editorGroupMenu.AddItem(new ContextSlider("Goodies", null, new FieldBinding(trophy, "goodies", -1f, 300f), 1f, "ALL", false, null, "You must always collect exactly this many goodies."));
                    editorGroupMenu.AddItem(new ContextSlider("Targets", null, new FieldBinding(trophy, "targets", -1f, 300f), 1f, "ALL", false, null, "You must always knock down exactly this many targets."));
                    editorGroupMenu.AddItem(new ContextSlider("Time Limit", null, new FieldBinding(trophy, "timeRequirement", max: 600f), 1f, "NONE", true, null, "You have at most this much time to complete the challenge."));
                    if (trophy.goodies >= 0)
                        flag2 = true;
                    if (trophy.targets >= 0)
                        flag3 = true;
                }
                else
                {
                    if (!flag2)
                        editorGroupMenu.AddItem(new ContextSlider("Goodies", null, new FieldBinding(trophy, "goodies", -1f, 300f), 1f, "ALL", false, null, "Collect this many items to get this trophy"));
                    if (!flag3)
                        editorGroupMenu.AddItem(new ContextSlider("Targets", null, new FieldBinding(trophy, "targets", -1f, 300f), 1f, "ALL", false, null, "Knock down this many targets to get this trophy"));
                    if (!flag1)
                    {
                        editorGroupMenu.AddItem(new ContextSlider("Time", null, new FieldBinding(trophy, "timeRequirement", max: 600f), 1f, "GOAL TIME", true, null, "Complete challenge in this time or less to get this trophy."));
                        editorGroupMenu.AddItem(new ContextSlider("Milis", null, new FieldBinding(trophy, "timeRequirementMilliseconds", max: 99f), 1f, "NONE", false, null, "Fine control over challenge time requirement."));
                    }
                }
                contextMenu.AddItem(editorGroupMenu);
            }
            return contextMenu;
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("hatIndex", hatIndex);
            binaryClassChunk.AddProperty("challengeData", this._challenge.Serialize());
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this.hatIndex = node.GetProperty<int>("hatIndex");
            BinaryClassChunk property = node.GetProperty<BinaryClassChunk>("challengeData");
            if (property != null)
            {
                this._challenge = new ChallengeData();
                this._challenge.Deserialize(property);
            }
            if (this.Next.value == null)
                this.Next.value = "";
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("hatIndex", hatIndex));
            dxmlNode.Add(this._challenge.LegacySerialize());
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("hatIndex");
            if (dxmlNode != null)
                this.hatIndex = Convert.ToInt32(dxmlNode.Value);
            DXMLNode node1 = node.Element("challengeData");
            if (node1 != null)
            {
                this._challenge = new ChallengeData();
                this._challenge.LegacyDeserialize(node1);
            }
            return true;
        }
    }
}
