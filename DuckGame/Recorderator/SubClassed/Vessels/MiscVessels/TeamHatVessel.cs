using System.Collections.Generic;

namespace DuckGame
{
    public class TeamHatVessel : HoldableVessel
    {
        public TeamHatVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(TeamHat));
            AddSynncl("equipped", new SomethingSync(typeof(int)));
            AddSynncl("team", new SomethingSync(typeof(ushort)));
        }
        public static Dictionary<ushort, Team> regTems = new Dictionary<ushort, Team>();
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Team team;
            byte byt = b.ReadByte();
            int ush = b.ReadUShort();
            if (byt == 1) team = Corderator.instance.teams[ush];
            else team = Teams.ParseFromIndex((ushort)ush);

            TeamHatVessel v = new TeamHatVessel(new TeamHat(0, -2000, team));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            TeamHat th = (TeamHat)t;
            if (Corderator.instance.teams.Contains(th.team))
            {
                prevBuffer.Write((byte)1);
                prevBuffer.Write((ushort)(Corderator.instance.teams.IndexOf(th.team)));
            }
            else
            {
                prevBuffer.Write((byte)0);
                prevBuffer.Write((ushort)th.netTeamIndex);
            }
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            TeamHat th = (TeamHat)t;


            int hObj = (int)valOf("equipped");
            if (hObj == -1)
            {
                if (th._equippedDuck != null)
                {
                    th._equippedDuck.Unequip(th);
                }
                th._equippedDuck = null;
            }
            else if (hObj != -1 && Corderator.instance.somethingMap.Contains(hObj))
            {
                Duck d = (Duck)Corderator.instance.somethingMap[hObj];
                if (th._equippedDuck == null)
                {
                    d.Equip(th, false);
                }
                th._equippedDuck = d;
            }

            if (bArray[7])
            {
                if (Corderator.instance != null)
                {
                    int team = (ushort)valOf("team");
                    if (th.team == null || th.team.recordIndex != team) th.team = Corderator.instance.teams[team];
                }
            }
            else
            {
                ushort team = (ushort)valOf("team");
                if (th.netTeamIndex != team) th.netTeamIndex = team;
            }

            base.PlaybackUpdate();
        }
        public bool addedTeam;
        public Team lastTeam;
        public override void RecordUpdate()
        {
            TeamHat th = (TeamHat)t;
            if (th.team != lastTeam) addedTeam = false; //this is to account for animated hats or hats that change texture AKA cheating
            lastTeam = th.team;
            if (!addedTeam && Corderator.instance != null && th.team != null && th.team.customData != null && !Corderator.instance.teams.Contains(th.team))
            {
                th.team.recordIndex = Corderator.instance.teams.Count;
                Corderator.instance.teams.Add(th.team);
            }


            if (Corderator.instance != null && th.team != null && th.team.recordIndex != -1)
            {
                addVal("team", (ushort)th.team.recordIndex);
                bArray[7] = true;
            }
            else
            {
                addVal("team", th.netTeamIndex);
                bArray[7] = false;
            }

            if (th._equippedDuck != null)
            {
                if (Corderator.instance != null && Corderator.instance.somethingMap.Contains(th._equippedDuck)) addVal("equipped", Corderator.instance.somethingMap[th._equippedDuck]);
                else addVal("equipped", -1);
            }
            else addVal("equipped", -1);

            base.RecordUpdate();
        }
    }
}
