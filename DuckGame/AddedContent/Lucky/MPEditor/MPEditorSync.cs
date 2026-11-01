using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public enum MPEditorOpType : byte//lol.
    {
        Upsert,
        Remove,
        Move,
    }

    public class MPEditorOp
    {
        public MPEditorOpType type;
        public uint id;
        public uint version;
        public byte writer;
        public Vec2 position;
        public uint hash;
        public byte[] chunk;

        public void Write(BitBuffer b)
        {
            b.Write((byte)type);
            b.Write(id);
            b.Write(version);
            b.Write(writer);
            if (type == MPEditorOpType.Remove) return;
            b.Write(position.x);
            b.Write(position.y);
            if (type != MPEditorOpType.Upsert) return;
            b.Write(hash);
            b.Write(new BitBuffer(chunk), true);
        }

        public static MPEditorOp Read(BitBuffer b)
        {
            MPEditorOp op = new MPEditorOp
            {
                type = (MPEditorOpType)b.ReadByte(),
                id = b.ReadUInt(),
                version = b.ReadUInt(),
                writer = b.ReadByte()
            };
            if (op.type == MPEditorOpType.Remove) return op;
            op.position = new Vec2(b.ReadFloat(), b.ReadFloat());
            if (op.type != MPEditorOpType.Upsert) return op;
            op.hash = b.ReadUInt();
            op.chunk = b.ReadBitBuffer().GetBytes();
            return op;
        }
    }

    //hello hi yes, tldr every object gets a network wide id. every few frames the list is diffed between players so stuff is synced somewhat properly
    //desyncs are       unlikely? not sure   sorta need extensive testing for that but this Should Work
    //all objects also carry stamps to check between each other, if a stamp is newer than ours ties are broken and everybody endfs up happy and agreeing
    public class MPEditorSync
    {
        public class Entry
        {
            public uint id;
            public uint version;
            public byte writer;
            public Thing thing;
            public Vec2 position;
            public uint hash;
            public bool hashValid;
        }

        public struct Stamp
        {
            public uint version;
            public byte writer;
        }

        //how often local changes are looked for and sent
        public const int kScanInterval = 3;
        //how many objects get their contents re-checked every scan, besides the selected/hovered ones
        public const int kContentChecksPerScan = 48;
        //rough max size of a single NMMPEditorOps message
        public const int kMaxBatchBytes = 6000;

        public MultiplayerEditor editor;
        public Dictionary<uint, Entry> byId = new Dictionary<uint, Entry>();
        public Dictionary<Thing, Entry> byThing = new Dictionary<Thing, Entry>();
        public Dictionary<uint, Stamp> deceased = new Dictionary<uint, Stamp>();
        public List<MPEditorOp> _outgoing = new List<MPEditorOp>();
        public uint nextId = 1;
        public int roundRobin;
        public int frame;

        public MPEditorSync(MultiplayerEditor pEditor) => editor = pEditor;

        public int count => byId.Count;

        public static byte localWriter => DuckNetwork.localProfile != null ? DuckNetwork.localProfile.networkIndex : (byte)0;

        public static bool Newer(uint pVersion, byte pWriter, uint pOtherVersion, byte pOtherWriter) => pVersion > pOtherVersion || pVersion == pOtherVersion && pWriter > pOtherWriter;

        public uint NewId()
        {
            uint id;
            do
            {
                id = (uint)localWriter << 24 | nextId & 0xFFFFFF;
                nextId++;
            }
            while (byId.ContainsKey(id) || deceased.ContainsKey(id));
            return id;
        }

        public List<Thing> levelThings => editor.syncedThings;
        public static bool Syncable(Thing pThing) => pThing != null && pThing.editorCanModify;
        public static byte[] SerializeThing(Thing pThing, out uint pHash)
        {
            Vec2 position = pThing.position;
            BinaryClassChunk chunk;
            pThing.position = Vec2.Zero;
            try
            {
                chunk = pThing.Serialize();
            }
            finally
            {
                pThing.position = position;
            }
            byte[] bytes = chunk.Serialize(new BitBuffer(false), false).GetBytes();
            pHash = CRC32.Generate(bytes);
            return bytes;
        }

        public static uint HashThing(Thing pThing)
        {
            SerializeThing(pThing, out uint hash);
            return hash;
        }

        public static Thing DeserializeThing(byte[] pChunk, Vec2 pPosition)
        {
            try
            {
                BinaryClassChunk chunk = BinaryClassChunk.DeserializeHeader(typeof(BinaryClassChunk), new BitBuffer(pChunk, false), root: false);
                if (chunk == null || !chunk.Deserialize())
                    return null;
                Thing thing = Thing.LoadThing(chunk, false);
                if (thing == null || Thing.CheckForBozoData(thing))
                    return null;
                thing.position = pPosition;
                return thing;
            }
            catch (Exception ex)
            {
                DevConsole.Log(DCSection.General, "|DGRED|MPEditor failed to load a synced object: " + ex.Message);
                return null;
            }
        }

        public void Update()
        {
            frame++;
            if (frame % kScanInterval != 0) return;
            Scan();
            Flush();
        }

        public void SyncNow()
        {
            Scan();
            Flush();
        }

        public void Scan()
        {
            List<Thing> things = levelThings;
            HashSet<Thing> present = new HashSet<Thing>();
            byte writer = localWriter;

            //new objects
            foreach (Thing thing in things)
            {
                if (!Syncable(thing))
                    continue;
                present.Add(thing);
                if (byThing.ContainsKey(thing))
                    continue;
                Entry entry = new Entry
                {
                    id = NewId(),
                    version = 1,
                    writer = writer,
                    thing = thing,
                    position = thing.position
                };
                Track(entry);
                _outgoing.Add(MakeUpsert(entry));
            }

            //removed objects
            if (present.Count != byThing.Count)
            {
                foreach (Entry entry in byThing.Values.Where(x => !present.Contains(x.thing)).ToList())
                {
                    Untrack(entry);
                    Stamp stamp = new Stamp { version = entry.version + 1, writer = writer };
                    deceased[entry.id] = stamp;
                    _outgoing.Add(new MPEditorOp
                    {
                        type = MPEditorOpType.Remove,
                        id = entry.id,
                        version = stamp.version,
                        writer = writer
                    });
                }
            }

            //moved objects
            foreach (Entry entry in byThing.Values)
            {
                if (entry.thing.position == entry.position) continue;
                entry.position = entry.thing.position;
                entry.version++;
                entry.writer = writer;
                _outgoing.Add(new MPEditorOp
                {
                    type = MPEditorOpType.Move,
                    id = entry.id,
                    version = entry.version,
                    writer = writer,
                    position = entry.position
                });
            }

            //changed contents, checked for whatever the player is touching plus a few objects every scan
            HashSet<Thing> candidates = new HashSet<Thing>();
            if (editor.selection != null) candidates.UnionWith(editor.selection); //UNIONIZING!??
            if (editor.hoverThing != null) candidates.Add(editor.hoverThing);
            if (Editor.openContextThing != null) candidates.Add(Editor.openContextThing);
            if (things.Count > 0)
            {
                int checks = Math.Min(kContentChecksPerScan, things.Count);
                for (int i = 0; i < checks; ++i)
                {
                    roundRobin = (roundRobin + 1) % things.Count;
                    candidates.Add(things[roundRobin]);
                }
            }
            foreach (Thing thing in candidates)
            {
                if (!byThing.TryGetValue(thing, out Entry entry)) continue;
                byte[] chunk = SerializeThing(thing, out uint hash);
                if (!entry.hashValid)
                {
                    entry.hash = hash;
                    entry.hashValid = true;
                    continue;
                }
                if (hash == entry.hash) continue;
                entry.hash = hash;
                entry.version++;
                entry.writer = writer;
                _outgoing.Add(new MPEditorOp
                {
                    type = MPEditorOpType.Upsert,
                    id = entry.id,
                    version = entry.version,
                    writer = writer,
                    position = thing.position,
                    hash = hash,
                    chunk = chunk
                });
            }
        }

        public MPEditorOp MakeUpsert(Entry pEntry)
        {
            byte[] chunk = SerializeThing(pEntry.thing, out uint hash);
            pEntry.hash = hash;
            pEntry.hashValid = true;
            return new MPEditorOp
            {
                type = MPEditorOpType.Upsert,
                id = pEntry.id,
                version = pEntry.version,
                writer = pEntry.writer,
                position = pEntry.thing.position,
                hash = hash,
                chunk = chunk
            };
        }

        public void Flush()
        {
            if (_outgoing.Count == 0) return;
            List<MPEditorOp> batch = new List<MPEditorOp>();
            int size = 0;
            foreach (MPEditorOp op in _outgoing)
            {
                batch.Add(op);
                size += 24 + (op.chunk != null ? op.chunk.Length : 0);
                if (size >= kMaxBatchBytes)
                {
                    MPEditorNet.SendOps(batch);
                    batch = new List<MPEditorOp>();
                    size = 0;
                }
            }
            if (batch.Count > 0) MPEditorNet.SendOps(batch);
            _outgoing.Clear();
        }

        public void Track(Entry pEntry)
        {
            byId[pEntry.id] = pEntry;
            byThing[pEntry.thing] = pEntry;
            deceased.Remove(pEntry.id);
        }

        public void Untrack(Entry pEntry)
        {
            byId.Remove(pEntry.id);
            if (pEntry.thing != null)
                byThing.Remove(pEntry.thing);
        }

        public void Apply(List<MPEditorOp> pOps)
        {
            if (pOps.Count == 0)
                return;
            //anything the local player changed before these arrived is sent first, otherwise it would be lost
            Scan();
            Flush();
            List<Thing> touched = new List<Thing>();
            foreach (MPEditorOp op in pOps)
            {
                Thing thing = ApplyOp(op);
                if (thing != null)
                    touched.Add(thing);
            }
            RefreshNeighbours(touched);
        }

        public Thing ApplyOp(MPEditorOp pOp)
        {
            byId.TryGetValue(pOp.id, out Entry entry);
            if (entry == null && deceased.TryGetValue(pOp.id, out Stamp tomb) && !Newer(pOp.version, pOp.writer, tomb.version, tomb.writer))
                return null;
            if (entry != null && !Newer(pOp.version, pOp.writer, entry.version, entry.writer))
                return null;

            switch (pOp.type)
            {
                case MPEditorOpType.Remove:
                    deceased[pOp.id] = new Stamp { version = pOp.version, writer = pOp.writer };
                    if (entry == null)
                        return null;
                    Untrack(entry);
                    Thing removed = entry.thing;
                    RemoveThing(removed);
                    return removed;
                case MPEditorOpType.Move:
                    if (entry == null)
                        return null;
                    entry.version = pOp.version;
                    entry.writer = pOp.writer;
                    entry.position = pOp.position;
                    MoveThing(entry.thing, pOp.position);
                    return entry.thing;
                default:
                    if (entry != null)
                    {
                        entry.version = pOp.version;
                        entry.writer = pOp.writer;
                        if (entry.hashValid && entry.hash == pOp.hash || HashThing(entry.thing) == pOp.hash)
                        {
                            entry.hash = pOp.hash;
                            entry.hashValid = true;
                            entry.position = pOp.position;
                            MoveThing(entry.thing, pOp.position);
                            return entry.thing;
                        }
                    }
                    Thing thing = DeserializeThing(pOp.chunk, pOp.position);
                    if (thing == null)
                        return null;
                    if (entry != null)
                    {
                        //the old instance stops being tracked, if an undo brings it back it'll just be a new object
                        Untrack(entry);
                        RemoveThing(entry.thing);
                    }
                    if (!AddThing(thing))
                        return null;
                    //our own hash of what we ended up with, so we never send a remote change back out
                    Track(new Entry
                    {
                        id = pOp.id,
                        version = pOp.version,
                        writer = pOp.writer,
                        thing = thing,
                        position = thing.position,
                        hash = HashThing(thing),
                        hashValid = true
                    });
                    return thing;
            }
        }

        public bool AddThing(Thing pThing)
        {
            bool processingMirror = editor.suppressMirror;
            editor.suppressMirror = true;
            try
            {
                editor.AddObject(pThing);
            }
            finally
            {
                editor.suppressMirror = processingMirror;
            }
            return levelThings.Contains(pThing);
        }

        public void RemoveThing(Thing pThing)
        {
            if (pThing == null)
                return;
            bool processingMirror = editor.suppressMirror;
            editor.suppressMirror = true;
            try
            {
                editor.selection?.Remove(pThing);
                editor.RemoveObject(pThing);
            }
            finally
            {
                editor.suppressMirror = processingMirror;
            }
        }

        public void MoveThing(Thing pThing, Vec2 pPosition)
        {
            if (pThing.position == pPosition)
                return;
            pThing.position = pPosition;
            if (pThing is IDontMove)
            {
                editor.things.quadTree.Remove(pThing);
                editor.things.quadTree.Add(pThing);
            }
        }

        public void RefreshNeighbours(List<Thing> pTouched)
        {
            if (pTouched.Count == 0)
                return;
            editor.things.RefreshState();
            foreach (Thing touched in pTouched)
            {
                if (!(touched is IDontMove))
                    continue;
                foreach (Thing thing in Level.CheckRectAll<IDontMove>(touched.topLeft + new Vec2(-16f, -16f), touched.bottomRight + new Vec2(16f, 16f)))
                {
                    thing.EditorObjectsChanged();
                    //whatever changed because of this (autotile frames) is the same on the sender's side, dont echo it back pretty please
                    if (thing != touched && byThing.TryGetValue(thing, out Entry entry))
                        entry.hashValid = false;
                }
            }
            editor.things.CleanAddList();
        }

        //forget Everything you know
        public void ResetToCurrent()
        {
            byId.Clear();
            byThing.Clear();
            deceased.Clear();
            _outgoing.Clear();
            byte writer = localWriter;
            foreach (Thing thing in levelThings)
            {
                if (!Syncable(thing) || byThing.ContainsKey(thing)) continue;
                Track(new Entry
                {
                    id = NewId(),
                    version = 1,
                    writer = writer,
                    thing = thing,
                    position = thing.position
                });
            }
        }

        public List<MPEditorOp> GetAllAsOps()
        {
            List<MPEditorOp> ops = new List<MPEditorOp>();
            foreach (Entry entry in byId.Values.OrderBy(x => x.id)) ops.Add(MakeUpsert(entry));
            return ops;
        }
        public void LoadSnapshot(LevelData pSettings, List<MPEditorOp> pOps)
        {
            byId.Clear();
            byThing.Clear();
            deceased.Clear();
            _outgoing.Clear();
            List<Thing> loaded = new List<Thing>();
            List<Entry> entries = new List<Entry>();
            foreach (MPEditorOp op in pOps)
            {
                Thing thing = DeserializeThing(op.chunk, op.position);
                if (thing == null) continue;
                loaded.Add(thing);
                entries.Add(new Entry
                {
                    id = op.id,
                    version = op.version,
                    writer = op.writer,
                    thing = thing,

                    position = op.position
                });
            }
            bool processingMirror = editor.suppressMirror;
            editor.suppressMirror = true;
            try
            {
                editor.LoadLevelData(pSettings, "", loaded);
            }
            finally
            {
                editor.suppressMirror = processingMirror;
            }
            //single objects dont carry their tile frames, let them figure it out for fun
            editor.things.RefreshState();
            foreach (Thing thing in loaded)
            {
                if (thing is IDontMove)
                    thing.EditorObjectsChanged();
            }
            editor.things.CleanAddList();
            HashSet<Thing> things = new HashSet<Thing>(levelThings);
            foreach (Entry entry in entries)
            {
                if (!things.Contains(entry.thing))
                    continue;
                entry.hash = HashThing(entry.thing);
                entry.hashValid = true;
                Track(entry);
            }
        }

        public uint ComputeStateKey()
        {
            BitBuffer b = new BitBuffer();
            foreach (Entry entry in byId.Values.OrderBy(x => x.id))
            {
                b.Write(entry.id);
                b.Write(entry.version);
                b.Write(entry.writer);
            }
            b.Write(byId.Count);
            uint key = CRC32.Generate(b.GetBytes());
            return key == 0 ? 1 : key;
        }

        public List<BinaryClassChunk> GetSortedChunks()
        {
            //tiles keep their frames, neighbours and block groups in a ThingContainer (not in their own Serialize), so they're grouped exactly like Editor.SerializeObjects does it, otherwise they'd load without frames or nubbers
            List<BinaryClassChunk> chunks = new List<BinaryClassChunk>();
            List<Type> groupOrder = new List<Type>();
            Dictionary<Type, List<Thing>> groups = new Dictionary<Type, List<Thing>>();
            foreach (Entry entry in byId.Values.OrderBy(x => x.id))
            {
                Thing thing = entry.thing;
                if (thing == null) continue;
                if (!thing.canBeGrouped)
                {
                    chunks.Add(thing.Serialize());
                    continue;
                }
                Type type = thing.GetType();
                if (!groups.TryGetValue(type, out List<Thing> group))
                {
                    group = new List<Thing>();
                    groups[type] = group;
                    groupOrder.Add(type);
                }
                group.Add(thing);
            }
            foreach (Type type in groupOrder) chunks.Add(new ThingContainer(groups[type], type).Serialize());
            return chunks;
        }
    }
}
