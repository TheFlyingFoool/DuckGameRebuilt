using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using static DuckGame.CMD;

namespace DuckGame
{
    public class NMKillDuck : NMEvent
    {
        public byte index;
        public bool crush;
        public bool cook;
        public bool fall;
        public byte lifeChange;
        private byte _levelIndex;

        public NMKillDuck(byte idx, bool wasCrush, bool wasCook, bool wasFall, byte pLifeChange)
        {
            index = idx;
            crush = wasCrush;
            cook = wasCook;
            fall = wasFall;
            lifeChange = pLifeChange;
        }
        //public bool forceConnection; no adding fields to pre-existing net messages
        public void K4PLogic()
        {
            Profile killed = DuckNetwork.profiles[index];
            NetworkConnection conn = connection;
            if (conn.profile != null && conn.profile.team != null && killed != null && killed.team != null)
            {

                Profile killer = conn.profile;
                if (killer.duck != null && killer.duck.converted != null)
                {
                    killer = killer.duck.converted.profile;
                }
                if (killed.duck != null && killed.duck.converted != null)
                {
                    killed = killed.duck.converted.profile;
                }

                if (killer.team != killed.team)
                {
                    killer.team.score++;

                    List<int> scores = new List<int>();
                    foreach (Profile p3 in DuckNetwork.profiles)
                    {
                        if (p3 == null) continue;
                        if (p3.team != null) scores.Add(p3.team.score);
                        else scores.Add(0);
                    }
                    Send.Message(new NMTransferScores(scores));
                }
            }
        }
        public NMKillDuck(byte idx, bool wasCrush, bool wasCook)
        {
            index = idx;
            crush = wasCrush;
            cook = wasCook;
        }

        public NMKillDuck()
        {
        }

        public override void Activate()
        {
            if (DuckNetwork.levelIndex != _levelIndex || index >= DuckNetwork.profiles.Count)
                return;
            Profile profile = DuckNetwork.profiles[index];
            if (profile.duck == null || !profile.duck.WillAcceptLifeChange(lifeChange))
                return;
            DestroyType type = !crush ? (!fall ? new DTImpact(null) : new DTFall()) : new DTCrush(null);
            profile.duck.isKillMessage = true;
            if (Network.isServer && TeamSelect2.KillsForPoints)
            {
                K4PLogic();
            }
            if (profile.duck.Kill(type))
            {
                if (!cook)
                    profile.duck.GoRagdoll();
                Thing.Fondle(profile.duck, connection);
                if (profile.duck._ragdollInstance != null)
                    Thing.Fondle(profile.duck._ragdollInstance, connection);
                if (profile.duck._trappedInstance != null)
                    Thing.Fondle(profile.duck._trappedInstance, connection);
                if (profile.duck._cookedInstance != null)
                    Thing.Fondle(profile.duck._cookedInstance, connection);
            }
            profile.duck.isKillMessage = false;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write(DuckNetwork.levelIndex);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            _levelIndex = d.ReadByte();
        }
    }
}
