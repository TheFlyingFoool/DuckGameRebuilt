// Decompiled with JetBrains decompiler
// Type: DuckGame.Vote
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Vote
    {
        private static List<RegisteredVote> _votes = new List<RegisteredVote>();
        private static string _voteButton = "";
        private static bool _votingOpen = false;

        public static void OpenVoting(string voteMessage, string voteButton, bool openCorners = true)
        {
            if (openCorners)
            {
                Vote._voteButton = voteButton;
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomRight, "@" + voteButton + "@" + voteMessage);
            }
            Vote._votingOpen = true;
        }

        public static RegisteredVote GetVote(Profile who) => Vote._votes.FirstOrDefault<RegisteredVote>(x => x.who == who);

        public static void RegisterVote(Profile who, VoteType vote)
        {
            if (!Vote._votingOpen && Network.isActive && vote != VoteType.None)
                return;
            RegisteredVote registeredVote = Vote._votes.FirstOrDefault<RegisteredVote>(x => x.who == who);
            if (registeredVote == null)
                Vote._votes.Add(new RegisteredVote()
                {
                    who = who,
                    vote = vote
                });
            else if (registeredVote.vote != VoteType.None)
            {
                registeredVote.wobble = 1f;
            }
            else
            {
                registeredVote.vote = vote;
                registeredVote.open = true;
                registeredVote.doClose = false;
            }
        }

        public static void CloseVoting()
        {
            foreach (RegisteredVote vote in Vote._votes)
                vote.doClose = true;
            Vote._voteButton = "";
            Vote._votingOpen = false;
        }

        public static void ClearVotes() => Vote._votes.Clear();

        public static bool Passed(VoteType type)
        {
            int num = 0;
            foreach (RegisteredVote vote in Vote._votes)
            {
                if (vote.open && vote.vote == type && (vote.who == null || vote.who.slotType != SlotType.Spectator))
                    ++num;
            }
            IEnumerable<Profile> source = Profiles.all.Where<Profile>(x => x.team != null && x.slotType != SlotType.Spectator);
            return num >= source.Count<Profile>();
        }

        public static void Update()
        {
            if (Vote._voteButton != "")
            {
                foreach (Profile who in Profiles.all.Where<Profile>(x => x.team != null))
                {
                    if (who.inputProfile != null && who.inputProfile.Pressed(Vote._voteButton))
                        Vote.RegisterVote(who, VoteType.Skip);
                }
            }
            if (!Vote._votes.Exists(x => x.open && x.slide < 0.899999976158142))
            {
                foreach (RegisteredVote vote in Vote._votes)
                {
                    if (vote.doClose)
                        vote.open = false;
                }
            }
            foreach (RegisteredVote vote in Vote._votes)
            {
                if (vote.vote == VoteType.None)
                    vote.open = false;
                vote.slide = Lerp.FloatSmooth(vote.slide, vote.open ? 1f : -0.1f, 0.1f, 1.1f);
                vote.wobble = Lerp.Float(vote.wobble, 0.0f, 0.05f);
                vote.wobbleInc += 0.5f;
                if (!vote.open && vote.slide < 0.00999999977648258)
                    vote.vote = VoteType.None;
            }
        }

        public static void Draw()
        {
            int num1 = 0;
            foreach (RegisteredVote vote in Vote._votes)
            {
                if (vote.who != null && vote.who.inputProfile != null)
                {
                    float num2 = (float)(Math.Sin(vote.wobbleInc) * vote.wobble * 3.0);
                    Vec2 vec2 = Network.isActive ? vote.leftStick : vote.who.inputProfile.leftStick;
                    vote.who.persona.skipSprite.angle = (float)((double)num2 * 0.0299999993294477 + vec2.y * 0.400000005960464);
                    float num3 = 0.0f;
                    float num4 = 3f;
                    float num5 = 49f;
                    if (vote.vote == VoteType.Skip)
                    {
                        vote.who.persona.skipSprite.frame = 0;
                    }
                    else
                    {
                        num3 = -50f;
                        num4 = 20f;
                        num5 = 68f;
                        vote.who.persona.skipSprite.frame = 1;
                    }
                    Graphics.Draw(vote.who.persona.skipSprite, (float)((double)Layer.HUD.width + (double)num5 - vote.slide * 48.0 + vec2.x * (double)num4) + num3, (float)((double)Layer.HUD.height - 28.0 - num1 * 16 - vec2.y * (double)num4), (Depth)0.9f);
                    vote.who.persona.skipSprite.frame = 1;
                    Vec2 p2 = Network.isActive ? vote.rightStick : vote.who.inputProfile.rightStick;
                    if (vote.vote == VoteType.None)
                        num3 = -50f;
                    vote.who.persona.skipSprite.angle = num2 * 0.03f + Maths.DegToRad(Maths.PointDirection(Vec2.Zero, p2) - 180f);
                    Graphics.Draw(vote.who.persona.skipSprite, (float)((double)Layer.HUD.width + 68.0 - vote.slide * 48.0 + p2.x * 20.0) + num3, (float)((double)Layer.HUD.height - 32.0 - num1 * 16 - p2.y * 20.0), (Depth)0.9f);
                    ++num1;
                }
            }
        }
    }
}
