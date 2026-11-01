using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DuckGame
{
    //hi hello yes, everyone's luckiest lucky here
    //ghosts and whatnot need to be silenced while in the editor otherwise stuff sorta explodes but players still playing receive ghosts
    //the player priority thingamajig thats usually used for deciding who gets to grab what first indicates who owns the levels objects
    //albeit the host seemingly takes priority because duck game!! also mpeditorsync manages all the syncing of stuff
    //so yep, good luck if you try and do anything here also
    public static class MPEditorNet
    {

        public static MultiplayerEditor editor;
        public static MPEditorTestArea testArea;
        public static DuckGameEditor savedMainEditor;
        public static LevelData savedLevelData;
        public static Profile owner;
        public static Dictionary<NetworkConnection, Peer> peers = new Dictionary<NetworkConnection, Peer>();
        public static List<NetMessage> pending = new List<NetMessage>();
        public static Dictionary<NetMessage, int> pendingAge = new Dictionary<NetMessage, int>();

        //list of da events that players still need to receive
        public static readonly HashSet<Type> _eventWhitelist = new HashSet<Type>
        {
            typeof(NMLevel),
            typeof(NMLevelData),
            typeof(NMMessageFragment),
            typeof(NMClientClosedGame),
            typeof(NMClientCrashed),
            typeof(NMConnectionTrouble),
            typeof(NMEnableFiftyPlayerMode),
            typeof(NMLogEvent),
            typeof(NMLogPartWasReceived),
            typeof(NMLogRequestChunk),
            typeof(NMLogRequestIncoming),
            typeof(NMRequestLogs),
            typeof(NMMatchSettingsChanged),
            typeof(NMNetworkIndexSync),
            typeof(NMNetworkTrafficGarbage),
            typeof(NMPeerInformation),
            typeof(NMRequestPersona),
            typeof(NMSetPersona),
            typeof(NMSpecChangeIndexUpdated),
            typeof(NMMakePlayer),
            typeof(NMMakeSpectator),
            typeof(NMTransferScores),
            typeof(NMScoresReceived),
        };

        public static bool active => editor != null && (Level.current == editor || Level.current is MPEditorTestArea);

        public static bool testing => active && Level.current is MPEditorTestArea && Level.current == testArea;

        public static bool suppressLocalGhosts => active && !testing;

        public static bool CanExchangeGhosts(NetworkConnection pConnection)
        {
            if (!active)
                return true;
            if (!testing || pConnection == null)
                return false;
            return peers.TryGetValue(pConnection, out Peer peer) && InMySession(peer);
        }

        public static bool ShouldDropEvent(NetMessage m)
        {
            if (!active || !(m is NMEvent) || m is IMPEditorMessage || _eventWhitelist.Contains(m.GetType()))
                return false;
            return !CanExchangeGhosts(m.connection);
        }

        public static bool InMySession(Peer pPeer) => testArea != null && editor != null && pPeer.session == editor.session && pPeer.testing && pPeer.testKey == testArea.testKey;

        public static Peer GetPeer(NetworkConnection pConnection)
        {
            if (!peers.TryGetValue(pConnection, out Peer peer))
            {
                peer = new Peer { connection = pConnection };
                peers[pConnection] = peer;
            }
            return peer;
        }

        public static void OnEditorCreated(MultiplayerEditor pEditor)
        {
            if (editor != null && editor != pEditor)
                Cleanup();
            editor = pEditor;
            //a lot of the editor code goes through Main.editor, so it has to point to us while we're around
            if (savedMainEditor == null)
            {
                savedMainEditor = Main.editor;
                savedLevelData = Editor._currentLevelData;
            }
            Main.editor = pEditor;
        }

        public static void OnEditorEntered(MultiplayerEditor pEditor)
        {
            if (editor != pEditor)
                return;
            if (testArea != null)
            {
                testArea = null;
                owner = null;
                //the test level is gone, and so are its ghosts. kinda wonky buty whatevs
                GhostManager.context.Clear();
            }
            SendPeerState(false);
        }

        public static void Update()
        {
            if (editor == null)
                return;
            if (!active || !Network.isActive)
            {
                Cleanup();
                return;
            }
            //players who left the game
            foreach (Peer peer in peers.Values.ToList())
            {
                if (peer.connection.status != ConnectionStatus.Disconnected && peer.profile != null)
                    continue;
                bool wasInSession = InMySession(peer);
                peers.Remove(peer.connection);
                if (wasInSession)
                    OnPeerLeftSession(peer);
            }
        }

        public static void Cleanup()
        {
            editor = null;
            testArea = null;
            owner = null;
            peers.Clear();
            pending.Clear();
            pendingAge.Clear();
            if (savedMainEditor != null)
            {
                Main.editor = savedMainEditor;
                Editor._currentLevelData = savedLevelData ?? new LevelData();
                savedMainEditor = null;
                savedLevelData = null;
            }
            Editor.active = false;
            Editor.isTesting = false;
            History.Clear();
        }

        public static void SendPeerState(bool pReply, NetworkConnection pTo = null)
        {
            if (editor == null || !Network.isActive) return;
            bool isTesting = testArea != null;
            NMMPEditorPeerState msg = new NMMPEditorPeerState(editor.session, isTesting, isTesting ? testArea.testKey : 0, pReply);
            if (pTo != null) Send.Message(msg, pTo);
            else Send.Message(msg);
        }

        public static void SendCursor(Vec2 pPosition)
        {
            if (editor == null || !Network.isActive) return;
            Send.Message(new NMMPEditorCursor(editor.session, pPosition), NetMessagePriority.Volatile);
        }

        public static void SendTesterGhost(Vec2 pPosition, byte pFrame, bool pFlip)
        {
            if (editor == null || !Network.isActive)
                return;
            Send.Message(new NMMPEditorTesterGhost(editor.session, pPosition, pFrame, pFlip), NetMessagePriority.Volatile);
        }

        public static void SendCustomTile(byte pSlot, uint pVersion, byte pWriter, CustomTileDataChunk pTile)
        {
            if (editor == null || !Network.isActive) return;
            Send.Message(new NMMPEditorCustomTile(editor.session, pSlot, pVersion, pWriter, pTile));
        }

        public static void SendOps(List<MPEditorOp> pOps)
        {
            if (editor == null || !Network.isActive || pOps.Count == 0)
                return;
            Send.Message(new NMMPEditorOps(editor.session, pOps));
        }

        //hosts thingy to send the whole level
        public static void SendSnapshot(NetworkConnection pTo, bool pReset)
        {
            if (editor == null || !Network.isActive || !Network.isServer)
                return;
            NMMPEditorSnapshot msg = new NMMPEditorSnapshot(editor.session, pReset, BuildSnapshot(editor));
            if (pTo != null)
                Send.Message(msg, pTo);
            else
                Send.Message(msg);
        }

        public static byte[] BuildSnapshot(MultiplayerEditor pEditor)
        {
            BitBuffer b = new BitBuffer();
            b.Write(new BitBuffer(pEditor.GetSettingsData().Serialize().GetBytes()), true);
            List<MPEditorOp> ops = pEditor.sync.GetAllAsOps();
            b.Write(ops.Count);
            foreach (MPEditorOp op in ops)
                op.Write(b);
            return Compress(b.GetBytes());
        }

        public static void ReadSnapshot(byte[] pPayload, out LevelData pSettings, out List<MPEditorOp> pOps)
        {
            BitBuffer b = new BitBuffer(Decompress(pPayload));
            pSettings = DuckFile.LoadLevel(b.ReadBitBuffer().GetBytes()) ?? new LevelData();
            int count = b.ReadInt();
            pOps = new List<MPEditorOp>(count);
            for (int i = 0; i < count; ++i)
                pOps.Add(MPEditorOp.Read(b));
        }

        public static byte[] Compress(byte[] pData)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(output, CompressionMode.Compress))
                    gzip.Write(pData, 0, pData.Length);
                return output.ToArray();
            }
        }

        public static byte[] Decompress(byte[] pData)
        {
            using (MemoryStream input = new MemoryStream(pData))
            using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }

        public static bool ReadyForLevelChanges(uint pSession) => editor != null && editor.session == pSession && Level.core.nextLevel == null && Level.core.currentLevel == editor && editor.initializeFunctionHasBeenRun;

        public static void Queue(NetMessage pMessage)
        {
            if (pending.Contains(pMessage))
                return;
            //players who never enter the editor (still in the lobby) shouldn't pile these up forever
            if (pending.Count >= 4096)
            {
                pendingAge.Remove(pending[0]);
                pending.RemoveAt(0);
            }
            pending.Add(pMessage);
            pendingAge[pMessage] = 0;
        }

        public static void ProcessPending()
        {
            if (pending.Count == 0 || editor == null || !ReadyForLevelChanges(editor.session))
                return;
            List<NetMessage> pnd = pending.ToList();
            pending.Clear();
            foreach (NetMessage message in pnd)
            {
                uint session = SessionOf(message);
                if (editor.session != session)
                {
                    //hanges for an editor session we haven't entered yet, give it a bit of time to show up
                    pendingAge.TryGetValue(message, out int age);
                    if (age < 600)
                    {
                        pending.Add(message);
                        pendingAge[message] = age + 1;
                    }
                    else
                        pendingAge.Remove(message);
                    continue;
                }
                pendingAge.Remove(message);
                if (message is NMMPEditorOps opsMessage)
                    OnOps(opsMessage);
                else if (message is NMMPEditorCustomTile tileMessage)
                    OnCustomTile(tileMessage);
                else
                    OnSnapshot(message as NMMPEditorSnapshot);
            }
        }

        public static uint SessionOf(NetMessage pMessage)
        {
            switch (pMessage)
            {
                case NMMPEditorOps ops:
                    return ops.editorSession;
                case NMMPEditorCustomTile tile:
                    return tile.editorSession;
                default:
                    return (pMessage as NMMPEditorSnapshot).editorSession;
            }
        }

        public static void OnCustomTile(NMMPEditorCustomTile pMessage)
        {
            //applying it while testing would swap the art of the level being tested
            if (!ReadyForLevelChanges(pMessage.editorSession))
            {
                Queue(pMessage);
                return;
            }
            editor.customSlots.Apply(pMessage);
        }

        public static void OnOps(NMMPEditorOps pMessage)
        {
            if (!ReadyForLevelChanges(pMessage.editorSession))
            {
                Queue(pMessage);
                return;
            }
            editor.sync.Apply(pMessage.ops);
        }

        public static void OnSnapshot(NMMPEditorSnapshot pMessage)
        {
            if (pMessage.connection != null && pMessage.connection != Network.host)
                return;
            if (!ReadyForLevelChanges(pMessage.editorSession))
            {
                Queue(pMessage);
                return;
            }
            try
            {
                ReadSnapshot(pMessage.payload, out LevelData settings, out List<MPEditorOp> ops);
                if (pMessage.reset || editor.sync.count == 0)
                {
                    editor.sync.LoadSnapshot(settings, ops);
                    //the art came with the level, it's nothing new to send back out
                    editor.customSlots.Rebaseline();
                }
                else
                    editor.sync.Apply(ops);
                if (pMessage.reset)
                    HUD.AddPlayerChangeDisplay("@PLUG@|DGGREEN| The host loaded a level.", 2f);
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.DuckNet, "|DGRED|MPEditor snapshot failed to load: " + ex.Message);
            }
        }

        public static void OnCursor(NMMPEditorCursor pMessage)
        {
            if (pMessage.connection == null)
                return;
            Peer peer = GetPeer(pMessage.connection);
            if (peer.session != pMessage.editorSession && peer.session != 0)
                return;
            peer.cursor = pMessage.position;
            peer.hasCursor = true;
        }

        public static void OnTesterGhost(NMMPEditorTesterGhost pMessage)
        {
            if (pMessage.connection == null)
                return;
            Peer peer = GetPeer(pMessage.connection);
            if (peer.session != pMessage.editorSession && peer.session != 0)
                return;
            peer.ghostPosition = pMessage.position;
            peer.ghostFrame = pMessage.frame;
            peer.ghostFlip = pMessage.flip;
            peer.ghostReceivedFrame = Graphics.frame;
        }

        public static void OnPeerState(NMMPEditorPeerState pMessage)
        {
            NetworkConnection connection = pMessage.connection;
            if (connection == null)
                return;
            Peer peer = GetPeer(connection);
            bool wasInSession = InMySession(peer);
            peer.session = pMessage.editorSession;
            peer.testing = pMessage.testing;
            peer.testKey = pMessage.testKey;
            if (peer.testing)
                peer.hasCursor = false;
            bool inSession = InMySession(peer);
            if (!wasInSession && inSession)
                OnPeerJoinedSession(peer);
            else if (wasInSession && !inSession)
                OnPeerLeftSession(peer);
            else if (inSession && pMessage.reply)
            {
                //they answered our state, so they now accept our ghosts. anything we sent before got ignored, send it again
                GhostManager.context.OnDisconnect(connection);
            }
            if (!pMessage.reply && editor != null && editor.session == pMessage.editorSession)
                SendPeerState(true, connection);
        }

        public static void OnTestPrune(NMMPEditorTestPrune pMessage)
        {
            if (!testing || editor.session != pMessage.editorSession || testArea.testKey != pMessage.testKey)
                return;
            GhostManager.receivingDestroyMessage = true;
            try
            {
                foreach (ushort id in pMessage.removed)
                {
                    GhostObject ghost = GhostManager.context.GetGhost((NetIndex16)id);
                    if (ghost != null)
                        GhostManager.context.RemoveGhost(ghost);
                }
            }
            finally
            {
                GhostManager.receivingDestroyMessage = false;
            }
        }

        public static void OnLocalTestStarted(MPEditorTestArea pArea)
        {
            if (editor == null)
                return;
            testArea = pArea;
            foreach (Peer peer in peers.Values)
            {
                if (InMySession(peer))
                    ClearRemovedGhosts(peer.connection);
            }
            owner = ComputeOwner();
            foreach (GhostObject ghost in GetGhosts())
            {
                if (pArea.initGhostIds.Contains((int)ghost.ghostObjectIndex))
                    SetOwner(ghost.thing, owner);
            }
            SendPeerState(false);
        }

        public static List<GhostObject> GetGhosts()
        {
            lock (GhostManager.context._ghosts)
                return GhostManager.context._ghosts.Where(x => x.thing != null).ToList();
        }


        public static Profile ComputeOwner()
        {
            Profile owner = DuckNetwork.localProfile;
            foreach (Peer peer in peers.Values)
            {
                if (!InMySession(peer))
                    continue;
                Profile profile = peer.profile;
                if (profile != null && (owner == null || profile.networkIndex < owner.networkIndex))
                    owner = profile;
            }
            return owner;
        }

        public static NetworkConnection OwnerConnection(Profile pOwner)
        {
            if (pOwner == null || pOwner == DuckNetwork.localProfile)
                return DuckNetwork.localConnection;
            return pOwner.connection;
        }

        public static void SetOwner(Thing pThing, Profile pOwner)
        {
            NetworkConnection connection = OwnerConnection(pOwner);
            if (connection == DuckNetwork.localConnection)
            {
                if (pThing.connection != DuckNetwork.localConnection)
                    Thing.SuperFondle(pThing, DuckNetwork.localConnection);
                pThing.connection = DuckNetwork.localConnection;
            }
            else
                pThing.connection = connection;
        }

        public static void UpdateOwner(NetworkConnection pDeparted)
        {
            Profile newOwner = ComputeOwner();
            NetworkConnection previous = OwnerConnection(owner);
            bool ownerChanged = newOwner != owner;
            foreach (GhostObject ghost in GetGhosts())
            {
                NetworkConnection connection = ghost.thing.connection;
                if (connection == null)
                    continue;
                bool reassign = pDeparted != null && connection == pDeparted;
                if (!reassign && ownerChanged && connection == previous && testArea.initGhostIds.Contains((int)ghost.ghostObjectIndex))
                    reassign = true;
                if (reassign)
                    SetOwner(ghost.thing, newOwner);
            }
            owner = newOwner;
        }

        public static void ClearRemovedGhosts(NetworkConnection pConnection)
        {
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection == pConnection)
                    profile.removedGhosts.Clear();
            }
        }

        public static void OnPeerJoinedSession(Peer pPeer)
        {
            //their ghost indexes start over every time they load the level
            ClearRemovedGhosts(pPeer.connection);
            //they just loaded a fresh copy of the level, send them the state of everything we own
            GhostManager.context.OnDisconnect(pPeer.connection);
            UpdateOwner(null);
            if (owner == DuckNetwork.localProfile)
            {
                List<ushort> removed = new List<ushort>();
                foreach (int id in testArea.initGhostIds)
                {
                    if (GhostManager.context.GetGhost((NetIndex16)id) == null)
                        removed.Add((ushort)id);
                }
                if (removed.Count > 0)
                    Send.Message(new NMMPEditorTestPrune(editor.session, testArea.testKey, removed), pPeer.connection);
            }
        }

        public static void OnPeerLeftSession(Peer pPeer)
        {
            if (testArea == null)
                return;
            //whatever they created (their duck, bullets, things they spawned) is gone with them
            //maybe change later, person leaving oftentimes messes with itemboxes/spawners/stuffs
            List<GhostObject> remove = new List<GhostObject>();
            foreach (GhostObject ghost in GetGhosts())
            {
                if (!testArea.initGhostIds.Contains((int)ghost.ghostObjectIndex) && GhostObject.IndexToProfile(ghost.ghostObjectIndex)?.connection == pPeer.connection)
                    remove.Add(ghost);
            }
            GhostManager.receivingDestroyMessage = true;
            try
            {
                foreach (GhostObject ghost in remove)
                    GhostManager.context.RemoveGhost(ghost);
            }
            finally
            {
                GhostManager.receivingDestroyMessage = false;
            }
            ClearRemovedGhosts(pPeer.connection);
            //and whatever they were holding or owning goes to whoever owns the session now
            UpdateOwner(pPeer.connection);
        }

        public static void DrawCursors(SpriteMap pCursor)
        {
            foreach (Peer peer in peers.Values)
            {
                if (!peer.hasCursor || peer.testing || peer.session != editor.session)
                    continue;
                Profile profile = peer.profile;
                if (profile == null)
                    continue;
                Color color = profile.persona != null ? profile.persona.colorUsable : Color.White;
                pCursor.frame = 0;
                pCursor.depth = 1f;
                pCursor.scale = new Vec2(1f, 1f);
                pCursor.color = color;
                pCursor.position = peer.cursor;
                pCursor.Draw();
                pCursor.color = Color.White;
                Graphics.DrawString(profile.name, peer.cursor + new Vec2(10f, 12f), color, 1f, null, 0.5f);
            }
        }

        public static void DrawTesterGhosts()
        {
            foreach (Peer peer in peers.Values)
            {
                if (!peer.testing || peer.session != editor.session || Graphics.frame - peer.ghostReceivedFrame > 30)
                    continue;
                Profile profile = peer.profile;
                if (profile == null || profile.persona == null)
                    continue;
                if (peer.ghostSprite == null || peer.ghostPersona != profile.persona)
                {
                    peer.ghostPersona = profile.persona;
                    peer.ghostSprite = profile.persona.sprite.CloneMap();
                    peer.ghostSprite.center = new Vec2(16f, 16f);
                }
                SpriteMap sprite = peer.ghostSprite;
                sprite.frame = peer.ghostFrame;
                sprite.flipH = peer.ghostFlip;
                sprite.alpha = 0.45f;
                sprite.depth = 0.9f;
                sprite.position = peer.ghostPosition;
                sprite.Draw();
                string name = profile.name;
                Graphics.DrawString(name, peer.ghostPosition + new Vec2(-Graphics.GetStringWidth(name, false, 0.5f) / 2f, -22f), profile.persona.colorUsable * 0.8f, 0.95f, null, 0.5f);
            }
        }

        public static void DrawPlayerList()
        {
            float y = 4f;
            float right = Layer.HUD.camera.width - 4f;
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.connection == null) continue;

                string status;
                if (profile.connection == DuckNetwork.localConnection) status = "EDITING";
                else if (peers.TryGetValue(profile.connection, out Peer peer) && peer.session == editor.session) status = peer.testing ? "TESTING" : "EDITING";
                else status = "LOADING";
                string text = profile.name + " - " + status;
                Color color = profile.persona != null ? profile.persona.colorUsable : Color.White;
                Graphics.DrawString(text, new Vec2(right - Graphics.GetStringWidth(text, false, 0.5f), y), color, 0.9f, null, 0.5f);
                y += 6f;
            }
        }
    }
    public class Peer
    {
        public NetworkConnection connection;
        public uint session;
        public bool testing;
        public uint testKey;
        public Vec2 cursor;
        public bool hasCursor;
        public Vec2 ghostPosition;
        public byte ghostFrame;
        public bool ghostFlip;
        public long ghostReceivedFrame = -1000;
        public SpriteMap ghostSprite;
        public DuckPersona ghostPersona;

        public Profile profile => DuckNetwork.profiles.FirstOrDefault(x => x.connection == connection);
    }
}
