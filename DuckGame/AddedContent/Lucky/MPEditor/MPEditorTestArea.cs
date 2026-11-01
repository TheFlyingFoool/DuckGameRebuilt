using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class MPEditorTestArea : XMLLevel
    {
        public MPEditorTestArea(MultiplayerEditor pEditor, LevelData pData, uint pTestKey)
          : base(pData)
        {
            editor = pEditor;
            testKey = pTestKey;
            localNetworkTransition = true;
            networkIndex = DuckNetwork.levelIndex;
            seed = (int)(pTestKey & int.MaxValue);
            followCam = new FollowCam
            {
                lerpMult = 1.1f
            };
            camera = followCam;
        }

        public override void Initialize()
        {
            base.Initialize();
            _levelStart = true;
            SkipStart();
            lock (GhostManager.context._ghosts)
            {
                foreach (GhostObject ghost in GhostManager.context._ghosts)
                    initGhostIds.Add((int)ghost.ghostObjectIndex);
            }
            MPEditorNet.OnLocalTestStarted(this);
            CreatePauseMenu();
            foreach (Profile profile in LocalProfiles())
                SpawnDuck(profile);
        }


        //there is no handshake for we have no hands in the realm of the editor
        public override void ClientReady(NetworkConnection c)
        {
        }

        //also spectators dont have a soul
        public static IEnumerable<Profile> LocalProfiles() => DuckNetwork.profiles.Where(x => x.connection != null && x.connection == DuckNetwork.localConnection && x.team != null);

        public void CreatePauseMenu()
        {
            pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            pauseMenu = new UIMenu("@LWING@TESTING@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            pauseMenu.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(pauseGroup), UIAlign.Left), true);
            pauseMenu.Add(new UIMenuItem("RESPAWN", new UIMenuActionCloseMenuSetBoolean(pauseGroup, respawn), UIAlign.Left), true);
            pauseMenu.Add(new UIText("", Color.White), true);
            pauseMenu.Add(new UIMenuItem("|DGRED|BACK TO EDITOR", new UIMenuActionCloseMenuSetBoolean(pauseGroup, quit), UIAlign.Left), true);
            pauseMenu.Close();
            pauseGroup.Add(pauseMenu, false);
            pauseGroup.isPauseMenu = true;
            pauseGroup.Close();
            pauseGroup.Update();
            pauseGroup.Update();
            Add(pauseGroup);
        }

        public void SpawnDuck(Profile pProfile)
        {
            List<SpawnPoint> spawns = things[typeof(SpawnPoint)].Cast<SpawnPoint>().ToList();
            Vec2 position = Vec2.Zero;
            sbyte offDir = 1;
            if (spawns.Count > 0)
            {
                SpawnPoint spawn = spawns[Rando.Int(spawns.Count - 1)];
                position = spawn.position;
                offDir = spawn.offDir;
            }
            else if (FirstOfType<CustomCamera>() is CustomCamera customCamera)
                position = customCamera.position;
            //the duck uses the lobby profile, so it keeps the player's lobby color
            Duck duck = new Duck(position.x, position.y - 7f, pProfile)
            {
                offDir = offDir
            };
            AddThing(duck);

            duck.netProfileIndex = (byte)DuckNetwork.IndexOf(pProfile);
            if (ducks.TryGetValue(pProfile, out Duck old) && old != null)
                followCam.Remove(old);
            ducks[pProfile] = duck;
            respawnWait[pProfile] = 0;
            followCam.Add(duck);
        }

        public void PauseLogic()
        {
            if (MonoMain.pauseMenu == null && Input.Pressed(Triggers.Start))
            {
                pauseGroup.Open();
                pauseMenu.Open();
                MonoMain.pauseMenu = pauseGroup;
                if (!paused)
                {
                    SFX.Play("pause", 0.6f);
                    paused = true;
                }
            }
            else if (paused && MonoMain.pauseMenu == null)
            {
                paused = false;
                SFX.Play("resume", 0.6f);
            }
        }

        public Profile ghostProfile => LocalProfiles().FirstOrDefault();

        public void SendGhost(Duck pDuck)
        {
            Vec2 position = pDuck.ragdoll != null && pDuck.ragdoll.part1 != null ? pDuck.ragdoll.part1.position : pDuck.position;
            MPEditorNet.SendTesterGhost(position, (byte)pDuck._sprite.frame, pDuck._sprite.flipH);
        }

        public override void Update()
        {
            PauseLogic();
            if (quit.value)
            {
                quit.value = false;
                current = editor;
                return;
            }
            bool forceRespawn = respawn.value;
            respawn.value = false;
            foreach (Profile profile in LocalProfiles().ToList())
            {
                ducks.TryGetValue(profile, out Duck duck);
                if (forceRespawn)
                {
                    if (duck != null && !duck.removeFromLevel)
                        Remove(duck);
                    SpawnDuck(profile);
                    continue;
                }
                if (duck != null && !duck.dead && !duck.removeFromLevel)
                {
                    if (profile == ghostProfile)
                        SendGhost(duck);
                    continue;
                }
                respawnWait.TryGetValue(profile, out int wait);
                wait++;
                if (wait >= 120) SpawnDuck(profile);
                else respawnWait[profile] = wait;
            }
            base.Update();
        }
        public MultiplayerEditor editor;
        public uint testKey;
        public FollowCam followCam;
        public Dictionary<Profile, Duck> ducks = new Dictionary<Profile, Duck>();
        public Dictionary<Profile, int> respawnWait = new Dictionary<Profile, int>();
        public MenuBoolean quit = new MenuBoolean();
        public MenuBoolean respawn = new MenuBoolean();
        public UIComponent pauseGroup;
        public UIMenu pauseMenu;
        public bool paused;
        public HashSet<int> initGhostIds = new HashSet<int>();
    }
}
