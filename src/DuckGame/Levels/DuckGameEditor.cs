// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckGameEditor
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DuckGameEditor : Editor
    {
        public override void RunTestLevel(string name)
        {
            LevGenType genType = LevGenType.Any;
            if (Editor._currentLevelData.proceduralData.enableSingle && !Editor._currentLevelData.proceduralData.enableMulti)
                genType = LevGenType.SinglePlayer;
            else if (!Editor._currentLevelData.proceduralData.enableSingle && Editor._currentLevelData.proceduralData.enableMulti)
                genType = LevGenType.Deathmatch;
            if (_levelThings.Exists(x => x is ChallengeMode))
            {
                foreach (Profile prof in Profiles.active)
                {
                    if (prof.team != null)
                        prof.team.Leave(prof);
                }
                Profiles.experienceProfile.team = Teams.Player1;
                Level.current = new ChallengeLevel(name);
            }
            else if (_levelThings.Exists(x => x is ImportMachine))
                Level.current = new ArcadeLevel(DuckFile.contentDirectory + "Levels/arcade_machine_preview.lev")
                {
                    genType = LevGenType.CustomArcadeMachine,
                    customMachine = name,
                    editor = this
                };
            else if (_levelThings.Exists(x => x is ArcadeMode))
            {
                foreach (Profile prof in Profiles.active)
                {
                    if (prof.team != null)
                        prof.team.Leave(prof);
                }
                Profiles.experienceProfile.team = Teams.Player1;
                Level.current = new ArcadeLevel(name)
                {
                    editor = this
                };
            }
            else
            {
                foreach (Profile prof in Profiles.active)
                {
                    if (prof.team != null)
                        prof.team.Leave(prof);
                }
                Profiles.experienceProfile.team = Teams.Player1;
                Profiles.DefaultPlayer2.team = Teams.Player2;
                Profiles.DefaultPlayer3.team = Teams.Player3;
                Profiles.DefaultPlayer4.team = Teams.Player4;
                Profiles.DefaultPlayer5.team = Teams.Player5;
                Profiles.DefaultPlayer6.team = Teams.Player6;
                Profiles.DefaultPlayer7.team = Teams.Player7;
                Profiles.DefaultPlayer8.team = Teams.Player8;
                Level.current = new DuckGameTestArea(this, name, _procSeed, _centerTile, genType);
            }
            Level.current.AddThing(new EditorTestLevel(this));
        }

        public override void Update() => base.Update();
    }
}
