using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class MultiplayerEditor : DuckGameEditor
    {
        public const string kNetworkIdentifier = "@MPEDITOR|";
        public uint session;
        public MPEditorSync sync;
        public MPEditorCustomSlots customSlots;
        public SpriteMap remoteCursor;
        public Vec2 lastSentCursor = Vec2.MinValue;

        public MultiplayerEditor(uint pSession)
        {
            session = pSession;
            sync = new MPEditorSync(this);
            customSlots = new MPEditorCustomSlots(this);
            MPEditorNet.OnEditorCreated(this);
        }
        public static uint NewSession() => (uint)Rando.Int(int.MaxValue - 1) + 1;
        public static uint ParseSession(string pIdentifier)
        {
            uint.TryParse(pIdentifier.Substring(kNetworkIdentifier.Length), out uint session);
            return session;
        }

        public List<Thing> syncedThings => _levelThingsNormal;
        public bool suppressMirror
        {
            get => processingMirror;
            set => processingMirror = value;
        }
        public override string networkIdentifier => kNetworkIdentifier + session;
        public override bool canLoadLevels => !Network.isActive || Network.isServer;

        public override void Initialize()
        {
            base.Initialize();
            SkipStart();
            localNetworkTransition = true;
            remoteCursor = new SpriteMap("cursors", 16, 16);
            customSlots.Rebaseline();
        }

        public override void DoInitialize()
        {
            base.DoInitialize();

            shoulddo = false;
            MPEditorNet.OnEditorEntered(this);
        }

        public override void Update()
        {
            shoulddo = false;
            MPEditorNet.ProcessPending();
            sync.Update();
            customSlots.Update();
            UpdateCursor();
            base.Update();
        }

        public void UpdateCursor()
        {
            Vec2 position = inputMode == EditorInput.Mouse ? Mouse.positionScreen : cursorTilePosition;
            if (position == lastSentCursor)
                return;
            lastSentCursor = position;
            MPEditorNet.SendCursor(position);
        }

        public override void PostDrawLayer(Layer layer)
        {
            base.PostDrawLayer(layer);
            if (layer == Layer.Game) MPEditorNet.DrawTesterGhosts();
            else if (layer == Layer.Foreground) MPEditorNet.DrawCursors(remoteCursor);
            else if (layer == Layer.HUD) MPEditorNet.DrawPlayerList();
        }

        public override void SendLevelData(NetworkConnection c)
        {
            sync.SyncNow();
            MPEditorNet.SendSnapshot(c, false);
        }

        public override void Load()
        {
            if (!canLoadLevels)
            {
                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| Only the host can load levels!", 2f);
                CloseMenu();
                return;
            }
            base.Load();
        }
        public override void SteamUpload()
        {
            if (Rando.Int(100) == 0) HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| No, why?", 2f);
            else HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| Only available offline!", 2f);
            CloseMenu();
        }

        public override void LoadLevel(string load)
        {
            if (!canLoadLevels)
            {
                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| Only the host can load levels!", 2f);
                return;
            }
            base.LoadLevel(load);
            sync.ResetToCurrent();
            customSlots.Rebaseline();
            MPEditorNet.SendSnapshot(null, true);
        }

        public override void Quit()
        {
            if (Network.isActive && !Network.isServer)
            {
                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| Only the host can close the editor!", 2f);
                return;
            }
            current = Network.isActive ? new TeamSelect2(true) : (Level)new TitleScreen();
        }

        public override void Play()
        {
            if (!_levelThingsNormal.Any(x => x is FreeSpawn || x is TeamSpawn || x is CustomCamera))
            {
                CloseMenu();
                ShowNoSpawnsDialogue();
                return;
            }
            isTesting = true;
            CloseMenu();
            RunTestLevel(null);
        }

        public override void RunTestLevel(string name)
        {
            //make sure the test level and its key include everything up to this frame
            sync.SyncNow();
            customSlots.SyncNow();
            LevelData data = BuildTestLevelData();
            if (data == null)
            {
                isTesting = false;
                HUD.AddPlayerChangeDisplay("@UNPLUG@|RED| Could not build the test level!", 2f);
                return;
            }
            current = new MPEditorTestArea(this, data, sync.ComputeStateKey());
        }

        public LevelData GetSettingsData()
        {
            if (_currentLevelData.GetExtraHeaderInfo() == null) _currentLevelData.SetExtraHeaderInfo(_currentLevelData.metaData);
            LevelData data = _currentLevelData.Clone();
            data.objects.objects.Clear();
            data.proceduralData.openAirAlternateObjects.objects.Clear();
            return data;
        }

        public LevelData BuildTestLevelData()
        {
            LevelData data = GetSettingsData();
            data.objects.objects.AddRange(sync.GetSortedChunks());
            return DuckFile.LoadLevel(data.Serialize().GetBytes());
        }
    }
}
