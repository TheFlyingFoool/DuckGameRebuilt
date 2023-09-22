using System.Collections.Generic;

namespace DuckGame
{
    public class TeamHatVessel : HoldableVessel
    {
        public TeamHatVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(TeamHat));
            AddSynncl("equipped", new SomethingSync(typeof(int)));
        }
        public static Dictionary<ushort, Team> regTems = new Dictionary<ushort, Team>();
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            DevConsole.Log("rec deserialize teamhat");
            Team team;
            byte WHAT = b.ReadByte();
            ushort z = b.ReadUShort();
            if (WHAT > 0)
            {
                if (WHAT == 2)
                {
                    return null;
                }
                if (regTems.ContainsKey(z)) team = regTems[z];
                else
                {
                    //DevConsole.Log("yes custom");
                    
                    byte[] teamData = b.ReadBytes();

                    //File.WriteAllBytes(GetPath<RecorderatorMod>("t2"), teamData);
                    team = Team.Deserialize(teamData);
                    Teams.AddExtraTeam(team);
                    /*DevConsole.Log("byte amount " + teamData.Count());
                    MemoryStream baseStream = new MemoryStream(teamData);
                    if (new BinaryReader(baseStream).ReadInt64() == 630430737023345L)
                    {
                        BitBuffer bitBuffer = new BitBuffer(teamData, 0, true);
                        string teamName = bitBuffer.ReadString();
                        team = Team.DeserializeFromPNG(bitBuffer.ReadBitBuffer(true).buffer, teamName, null);
                    }*/
                    regTems.Add(z, team);

                    //DevConsole.Log("What is null ", Color.Blue);
                    //DevConsole.Log("data " + (teamData == null), Color.Blue);
                    //DevConsole.Log("team " + (team == null), Color.Blue);

                    //DevConsole.Log("team.hat" + (team.hat == null), Color.Blue);
                    //DevConsole.Log("team.customdata " + (team.customData == null), Color.Blue);

                    //Stream stream = File.Create(GetPath<RecorderatorMod>("test.png"));
                    //Texture2D text = Extensions.GetPrivateFieldValue<Texture2D>(team.hat.texture, "_base");
                    //text.SaveAsPng(stream, text.Width, text.Height);
                }
            }
            else team = Teams.all[z];
            TeamHatVessel v = new TeamHatVessel(new TeamHat(0, -2000, team));
            return v;
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            DevConsole.Log("rec serialize teamhat");
            TeamHat th = (TeamHat)t;
            somethingCrash = "teamhat init";
            if (th == null || th.team == null || (th.netTeamIndex >= Teams.kCustomOffset && th.team.customData == null))
            {
                prevBuffer.Write((byte)2);
                return prevBuffer;
            }
            somethingCrash = "teamhat is koffset";
            if (th.netTeamIndex >= Teams.kCustomOffset)
            {
                prevBuffer.Write((byte)1);
                somethingCrash = "teamhat netindex";
                prevBuffer.Write(th.netTeamIndex);
                somethingCrash = "teamhat check key";
                if (!regTems.ContainsKey(th.netTeamIndex))
                {
                    somethingCrash = "teamhat regteams add";
                    regTems.Add(th.netTeamIndex, th.team);
                    //DevConsole.Log("buf length " + th.team.customData.Length.ToString());
                    //File.WriteAllBytes(GetPath<RecorderatorMod>("t1"), th.team.customData);
                    somethingCrash = "teamhat data write";
                    prevBuffer.Write(th.team.customData, true);
                }
            }
            else
            {
                somethingCrash = "teamhat exit out";
                prevBuffer.Write((byte)0);
                prevBuffer.Write(th.netTeamIndex);
            }
            somethingCrash = "i question existance";
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            TeamHat th = (TeamHat)t;

            //DevConsole.Log("WHY", Color.Green);

            int hObj = (int)valOf("equipped");
            //Main.SpecialCode = "the pain is unending";
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
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            //DevConsole.Log("wht", Color.Green);
            TeamHat th = (TeamHat)t;
            if (th._equippedDuck != null)
            {
                if (Corderator.instance.somethingMap.Contains(th._equippedDuck)) addVal("equipped", Corderator.instance.somethingMap[th._equippedDuck]);
                else addVal("equipped", -1);
            }
            else addVal("equipped", -1);

            base.RecordUpdate();
        }
    }
}
