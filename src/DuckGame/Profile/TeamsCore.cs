// Decompiled with JetBrains decompiler
// Type: DuckGame.TeamsCore
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class TeamsCore
    {
        public bool facadesChanged;
        public bool appliedFacades;
        public Dictionary<Profile, Team> _facadeMap = new Dictionary<Profile, Team>();
        public List<Team> teams;
        public Team nullTeam = new Team("???", "hats/cluehat");
        public List<Team> extraTeams = new List<Team>();
        public SpriteMap hats;
        public List<Team> SpectatorTeams = new List<Team>();
        private List<Team> _folders = new List<Team>();

        public Team Player1 => teams[0];

        public Team Player2 => teams[1];

        public Team Player3 => teams[2];

        public Team Player4 => teams[3];

        public Team Player5 => teams[4];

        public Team Player6 => teams[5];

        public Team Player7 => teams[6];

        public Team Player8 => teams[7];

        public int numTeams => teams.Count;

        public List<Team> all
        {
            get
            {
                List<Team> all = new List<Team>(teams);
                if (!Network.isActive)
                {
                    all.AddRange(extraTeams);
                }
                else
                {
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile == DuckNetwork.localProfile)
                            all.AddRange(extraTeams);
                        else
                            all.AddRange(profile.customTeams);
                    }
                }
                return all;
            }
        }

        public List<Team> folders => _folders;

        public List<Team> allStock => new List<Team>(teams);

        public void Initialize()
        {
            hats = new SpriteMap("hatCollection", 32, 32)
            {
                center = new Vec2(16f, 16f)
            };
            teams = new List<Team>()
      {
        new Team("Player 1", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 2", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 3", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 4", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 5", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 6", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 7", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Player 8", "hats/noHat", true)
        {
          defaultTeam = true
        },
        new Team("Sombreros", "hats/sombrero", true),
        new Team("Dappers", "hats/dapper", true),
        new Team("Dicks", "hats/dicks", true),
        new Team(true, "Frank", "hats/frank", lockd: true),
        new Team("DUCKS", "hats/reallife", lockd: true),
        new Team("Frogs?", "hats/frogs", true),
        new Team("Drunks", "hats/drunks"),
        new Team("Joey", "hats/joey", lockd: true),
        new Team("BALLZ", "hats/ballhead"),
        new Team("Agents", "hats/agents"),
        new Team("Sailors", "hats/sailors"),
        new Team("astropal", "hats/astrobud", lockd: true),
        new Team("Cowboys", "hats/cowboys", lockd: true),
        new Team(true, "Pulpy", "hats/pulpy", lockd: true),
        new Team("SKULLY", "hats/skelly", lockd: true),
        new Team("Hearts", "hats/hearts"),
        new Team("LOCKED", "hats/locked"),
        new Team("Jazzducks", "hats/jazzducks", false, false, new Vec2(-2f, -7f)),
        new Team("Divers", "hats/divers"),
        new Team("Uglies", "hats/uglies"),
        new Team("Dinos", "hats/dinos"),
        new Team("Caps", "hats/caps"),
        new Team("Burgers", "hats/burgers"),
        new Team("Turing", "hats/turing", true),
        new Team("Retro", "hats/retros"),
        new Team("Senpai", "hats/sensei"),
        new Team("BAWB", "hats/bawb", false, true, new Vec2(-1f, -10f))
        {
          noCrouchOffset = true
        },
        new Team("SWACK", "hats/guac", true, true),
        new Team("eggpal", "hats/eggy", lockd: true),
        new Team("Valet", "hats/valet"),
        new Team("Pilots", "hats/pilots"),
        new Team("Cyborgs", "hats/cyborgs"),
        new Team("Tubes", "hats/tube", false, false, new Vec2(-1f, 0f)),
        new Team("Gents", "hats/gents"),
        new Team("Potheads", "hats/pots"),
        new Team("Skis", "hats/ski"),
        new Team("Fridges", "hats/fridge"),
        new Team("Witchtime", "hats/witchtime"),
        new Team("Wizards", "hats/wizbiz"),
        new Team("FUNNYMAN", "hats/FunnyMan"),
        new Team("Pumpkins", "hats/Dumplin"),
        new Team("CAPTAIN", "hats/devhat", lockd: true, capeTex: ((Texture2D) Content.Load<Tex2D>("hats/devCape"))),
        new Team("BRICK", "hats/brick", lockd: true),
        new Team(true, "Pompadour", "hats/pompadour"),
        new Team(true, "Super", "hats/super"),
        new Team("Chancy", "hats/chancy", lockd: true),
        new Team("Log", "hats/log"),
        new Team("Meeee", "hats/toomany", lockd: true),
        new Team("BRODUCK", "hats/broduck", lockd: true),
        new Team("brad", "hats/handy", lockd: true),
        new Team("eyebob", "hats/gross"),
        new Team("masters", "hats/master"),
        new Team("clams", "hats/clams"),
        new Team("waffles", "hats/waffles", capeTex: ((Texture2D) Content.Load<Tex2D>("hats/waffleCape"))),
        new Team("HIGHFIVES", "hats/highfives", lockd: true, desc: "Right on!!"),
        new Team("toeboys", "hats/toeboys", false, false, new Vec2(-1f, -2f)),
        new Team("bigearls", "hats/bigearls", false, false, new Vec2(0f, 1f)),
        new Team("zeros", "hats/katanaman", false, false, new Vec2()),
        new Team(true, "CYCLOPS", "hats/cyclops", lockd: true, desc: "These wounds they will not heal."),
        new Team("MOTHERS", "hats/motherduck", lockd: true, desc: "Not a goose."),
        new Team("BIG ROBO", "hats/newrobo", false, true, new Vec2()),
        new Team("TINCAN", "hats/oldrobo", false, true, new Vec2()),
        new Team("WELDERS", "hats/WELDER", lockd: true, desc: "Safety has never looked so cool."),
        new Team("PONYCAP", "hats/ponycap", false, true, new Vec2()),
        new Team("TRICORNE", "hats/tricorne", lockd: true, desc: "We fight for freedom!"),
        new Team(true, "TWINTAIL", "hats/twintail", lockd: true, desc: "Two tails are better than one."),
        new Team("MAJESTY", "hats/royalty", lockd: true, capeTex: ((Texture2D) Content.Load<Tex2D>("hats/royalCape"))),
        new Team("MOONWALK", "hats/moonwalker", lockd: true, capeTex: ((Texture2D) Content.Load<Tex2D>("hats/moonCape"))),
        new Team("kerchiefs", "hats/kerchief", false, false, new Vec2(0f, -1f)),
        new Team("postals", "hats/mailbox"),
        new Team("wahhs", "hats/wahhs"),
        new Team("uufos", "hats/ufos"),
        new Team(true, "B52s", "hats/b52s"),
        new Team("diplomats", "hats/suit"),
        new Team("johnnygrey", "hats/johnnys"),
        new Team("wolfy", "hats/werewolves")
      };
        }
    }
}
