using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

namespace DuckGame
{ 
    public class SomethingSync
    {
        public SomethingSync(Type what)
        {
            supossedToBe = what;
        }
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }
        public string name;
        public int Count
        {
            get
            {
                return items.Count;
            }
        }
        public object Last()
        {
            return items.Last();
        }
        public object this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }
        public Type supossedToBe;
        public void Add(object obj)
        {
            items.Add(obj);
        }
        public List<object> items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }
        public List<object> _items = new List<object>();
    }
    public class SomethingSomethingVessel : Thing, IDrawToDifferentLayers
    {
        public Thing t;
        public bool playBack;
        public int addTime;
        public int deleteTime = -2;
        public int exFrames;
        public int skipPositioning;
        public int skipAngles;
        public List<Type> tatchedTo = new List<Type>();
        public Dictionary<string, SomethingSync> syncled = new Dictionary<string, SomethingSync>();
        public Map<string, byte> indexedSyncled = new Map<string, byte>();
        public List<byte> changeRemove = new List<byte>();
        public BitArray bArray;
        public string destroyedReason = "NONE";
        public static string somethingCrash;

        public int IndexPriority = 0; //this is for backwards compatability stuff

        public virtual void OnRemove()
        {

        }
        public void AddSynncl(string name, SomethingSync s)
        {
            if (indexedSyncled.Count == 8) throw new Exception("This vessel has hit the SomethingSync Cap!");
            s.name = name;
            byte c = 0;
            while (indexedSyncled.ContainsValue(c)) c++;
            indexedSyncled.Add(name, c);
            if (!playBack)
            {
                syncled.Add(name, s);
            }
        }
        public virtual void OnAdd()
        {

        }
        public void RemoveSynncl(string name)
        {
            indexedSyncled.Remove(name);
            syncled.Remove(name);

            if (indexedSyncled.Count > 0)
            {
                List<string> strangs = new List<string>();
                for (int i = 0; i < indexedSyncled.Count; i++)
                {
                    strangs.Add(indexedSyncled.ElementAt(i).Key);
                }
                indexedSyncled.Clear();
                for (int i = 0; i < strangs.Count; i++)
                {
                    indexedSyncled.Add((byte)i, strangs[i]);
                }
            }
        }
        public object valOf(string name)
        {
            Main.SpecialCode = "getting val of " + name + " in " + GetType().Name;
            return syncled[name][0];
        }
        public bool HistoryDisplay;
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.Console)
            {
                if (HistoryDisplay)
                {
                    Vec2 v = Vec2.Zero;
                    for (int i = 0; i < syncled.Count; i++)
                    {
                        SomethingSync ss = syncled.ElementAt(i).Value;
                        Graphics.DrawString(ss.name, v, Color.White, 1, null, 0.5f);
                        for (int x = 0; x < ss.items.Count; x++)
                        {
                            v.y += 4;
                            object val = ss.items[x];
                            if (val is float f) val = Math.Round(f, 1);
                            else if (val is Vec2 v2)
                            {
                                v2 = new Vec2((float)Math.Round(v2.x, 1), (float)Math.Round(v2.y, 1));
                                Graphics.DrawString(v2.x + ":" + v2.y, v, Color.White, 1, null, 0.5f);
                                continue;
                            }
                            string ld = val.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                            Graphics.DrawString(ld, v, Color.White, 1, null, 0.5f);
                        }
                        v.x += ss.name.Length*4 + 16;
                        v.y = 0;
                    }
                }
            }
        }
        public override void Draw()
        {
            if (Keyboard.Down(Keys.LeftControl) && playBack) Graphics.DrawString(myIndex.ToString(), t.topLeft, Color.White, 1);
            base.Draw();
        }
        public void addVal(string name, object obj)
        {
            if (!syncled.ContainsKey(name)) return;
            SomethingSync ss = syncled[name];
            if (ss.Count > 0 && ss.Last().Equals(obj)) return;
            ss.Add(obj);
            if (ss.Count > 1) bArray[indexedSyncled[name]] = true;
        }
        public bool doIndex = true;
        public SomethingSomethingVessel(Thing th = null)
        {
            if (doDestroy)
            {
                playBack = true;
                addTime = addDestroy;
                if (doIndex) myIndex = indexDestroy;
                syncled = syncledDestroy;
                deleteTime = KILLDESTROY;
                changeRemove = passOnTheBytes;
                if (th != null) th.active = false;
                //DESTRUCTION
            }
            else
            {
                if (doIndex)
                {
                    myIndex = somethingIndex;
                    somethingIndex++;
                }
            }
            t = th;
        }
        public static Dictionary<Type, byte> typeWow = new Dictionary<Type, byte>();
        public static Map<ushort, Type> TypeVessels = new Map<ushort, Type>();
        public static Dictionary<Type, Type> tatchedVessels = new Dictionary<Type, Type>();
        public static void YeahFillMeUpWithLists()
        {
            List<Type> zTyped = Extensions.GetSubclasses(typeof(SomethingSomethingVessel)).ToList();
            int count = 0;
            int cPriori = 0;
            int added = 1;
            while (added > 0)
            {
                added = 0;
                for (int i = 0; i < zTyped.Count; i++)
                {
                    Type t = zTyped[i];
                    if (t == typeof(SomethingSomethingVessel) || t == typeof(NilVessel)) continue;
                    object[] args = new object[] { null };
                    SomethingSomethingVessel vs = (SomethingSomethingVessel)Activator.CreateInstance(t, args);
                    if (vs.IndexPriority != cPriori) continue;
                    if (vs.tatchedTo.Count > 0)
                    {
                        for (int zx = 0; zx < vs.tatchedTo.Count; zx++)
                        {
                            tatchedVessels.Add(vs.tatchedTo[zx], t);
                        }
                    }
                    TypeVessels.Add((ushort)count, zTyped[i]);
                    added++;
                    count++;
                }
                cPriori++;
            }
            typeWow.Add(typeof(byte), 0);
            typeWow.Add(typeof(sbyte), 1);
            typeWow.Add(typeof(int), 2);
            typeWow.Add(typeof(float), 3);
            typeWow.Add(typeof(Vec2), 4);
            typeWow.Add(typeof(ushort), 5);
            typeWow.Add(typeof(string), 6);
            typeWow.Add(typeof(bool), 7);
            typeWow.Add(typeof(Vec4), 9);
            typeWow.Add(typeof(BitBuffer), 10);
            typeWow.Add(typeof(short), 11);
        }
        public virtual void PlaybackUpdate()
        {
            if (skipPositioning > 0) skipPositioning--;
            else skipPositioning = 0;
            if (skipAngles > 0) skipAngles--;
            else skipAngles = 0;
            DoUpdateThing();
        }
        public virtual void DoUpdateThing()
        {
            if (!t.removeFromLevel)
            {
                t.shouldbegraphicculled = false;
                Level.current.things.UpdateObject(t);
                t.DoUpdate();
            }
        }
        public virtual void RecordUpdate()
        {

        }
        public override void Update()
        {
            if (t != null) t.currentVessel = this;
            if (playBack)
            {
                if (Corderator.Paused) return;
                position = t.position;
                if (changeRemove.Count > 0)
                {
                    bArray = new BitArray(new byte[] { changeRemove[0] });
                    PlaybackUpdate();
                    for (int x = 0; x < syncled.Count; x++)
                    {
                        SomethingSync ss = syncled[indexedSyncled[(byte)x]];
                        if (bArray[x]) ss.RemoveAt(0);
                    }
                    changeRemove.RemoveAt(0);
                }
            }
            else
            {
                bArray = new BitArray(8);

                RecordUpdate();

                byte[] B = new byte[1];
                bArray.CopyTo(B, 0);
                changeRemove.Add(B.First());
            }
            exFrames++;
        }

        public virtual BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            return prevBuffer;
        }
        public virtual SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            return null;
        }
        public byte[] ReSerialize()
        {
            BitBuffer b = new BitBuffer();
            //THE TYPE
            Main.SpecialCode = "THE TYPE";
            b.Write(TypeVessels[GetType()]);
            Main.SpecialCode = "WHEN IT WAS ADDED";
            //WHEN IT WAS ADDED
            b.Write(addTime);
            Main.SpecialCode = "WHEN IT WAS REMOVED";
            //WHEN IT WAS REMOVED
            b.Write(deleteTime);
            Main.SpecialCode = "THE INDEX";
            //THE INDEX (helps when a duck grabs something)
            if (doIndex) b.Write(myIndex);

            Main.SpecialCode = "change remove";
            b.Write(changeRemove.Count);
            for (int i = 0; i < changeRemove.Count; i++)
            {
                b.Write(changeRemove[i]);
            }

            Main.SpecialCode = "syncled.count i++";
            for (int i = 0; i < syncled.Count; i++)
            {
                Main.SpecialCode = "syncled was non existant";
                SomethingSync s = syncled.ElementAt(i).Value;
                Main.SpecialCode = "wait what was it supossed to be again";
                b.Write(typeWow[s.supossedToBe]);
                Main.SpecialCode = "WHY DID THIS CRASH";
                b.Write(s.items.Count);
                Main.SpecialCode = "index died on something sync:\"" + s.name + "\"";
                if (s.items.Count > 0 && s.items[0] is Vec4)
                {
                    Main.SpecialCode = "vec4 got fucked";
                    for (int z = 0; z < s.items.Count; z++)
                    {
                        Vec4 v = (Vec4)s.items[z];
                        b.Write(v.x);
                        b.Write(v.y);
                        b.Write(v.z);
                        b.Write(v.w);
                    }
                }
                else
                {
                    Main.SpecialCode = "i have none of the idea";
                    for (int z = 0; z < s.items.Count; z++)
                    {
                        b.Write(s.items[z]);
                    }
                }
            }
            Main.SpecialCode = "";
            Main.SpecialCode2 = "something RecSerialize " + editorName;
            BitBuffer buffer = RecSerialize(b);
            Main.SpecialCode2 = "outside RecSerialize " + editorName;
            List<byte> bf = buffer.buffer.ToList();
            if (buffer.position + 13 < buffer.buffer.Count())
            {
                bf.RemoveRange(buffer.position + 10, buffer.buffer.Count() - buffer.position - 11);
            }
            return bf.ToArray();
        }
        public int myIndex;
        public static int somethingIndex;

        public static int indexDestroy;
        public static int addDestroy;
        public static int KILLDESTROY;
        public static List<byte> passOnTheBytes = new List<byte>();
        public static Dictionary<ushort, byte> KILL = new Dictionary<ushort, byte>();
        public static Dictionary<string, SomethingSync> syncledDestroy = new Dictionary<string, SomethingSync>();
        public static bool doDestroy;
        public static bool FirstDeser;
        public static SomethingSomethingVessel RCDeserialize(BitBuffer b)
        {
            ushort u = b.ReadUShort();
            addDestroy = b.ReadInt();
            KILLDESTROY = b.ReadInt();
            passOnTheBytes = new List<byte>();

            object[] args = new object[] { null };
            string more = "couldn't find type vessel";
            if (TypeVessels.Contains(u)) more = " AKA " + TypeVessels[u].ToString();
            Main.SpecialCode = "Tried creating vessel idx: " + u + more;
            Main.SpecialCode2 = "amount o' type vessels " + TypeVessels.Count();
            SomethingSomethingVessel v = (SomethingSomethingVessel)Activator.CreateInstance(TypeVessels[u], args);
            //if (FirstDeser && v is BulletVessel) return v;
            if (v.doIndex) indexDestroy = b.ReadInt();
            Main.SpecialCode2 = "";
            Main.SpecialCode = "ChangeDestroy reads";
            int read = b.ReadInt();
            for (int i = 0; i < read; i++)
            {
                passOnTheBytes.Add(b.ReadByte());
            }
            for (int i = 0; i < v.syncled.Count; i++)
            {
                SomethingSync ss = v.syncled.ElementAt(i).Value;
                byte tope = b.ReadByte();
                int x = b.ReadInt();
                Main.SpecialCode = "ChangeDestroy loop";
                /* 
                typeWow.Add(typeof(byte), 0);
                typeWow.Add(typeof(sbyte), 1);
                typeWow.Add(typeof(int), 2);
                typeWow.Add(typeof(float), 3);
                typeWow.Add(typeof(Vec2), 4);
                typeWow.Add(typeof(ushort), 5);
                typeWow.Add(typeof(string), 6);
                typeWow.Add(typeof(bool), 7);
                 */
                switch (tope)
                {
                    case 0:
                        {
                            for (int q = 0; q < x; q++)
                            {
                                ss.items.Add(b.ReadByte());
                            }
                            break;
                        }
                    case 1:
                        {
                            for (int q = 0; q < x; q++)
                            {
                                ss.items.Add(b.ReadSByte());
                            }
                            break;
                        }
                    case 2:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadInt());
                            break;
                        }
                    case 3:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadFloat());
                            break;
                        }
                    case 4:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadVec2());
                            break;
                        }
                    case 5:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadUShort());
                            break;
                        }
                    case 6:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadString());
                            break;
                        }
                    case 7:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadBool());
                            break;
                        }
                    case 9:
                        {
                            for (int q = 0; q < x; q++)
                            {
                                float a = b.ReadFloat();
                                float b3 = b.ReadFloat();
                                float c2 = b.ReadFloat();
                                float d2 = b.ReadFloat();
                                ss.items.Add(new Vec4(a, b3, c2, d2));
                            }
                            break;
                        }
                    case 10:
                        {
                            for (int q = 0; q < x; q++)
                            {
                                ss.items.Add(b.ReadBitBuffer());
                            }
                            break;
                        }
                    case 11:
                        {
                            for (int q = 0; q < x; q++) ss.items.Add(b.ReadShort());
                            break;
                        }
                }
            }
            Main.SpecialCode = "Out of change destroy reads";

            syncledDestroy = v.syncled;
            doDestroy = true;
            Main.SpecialCode = "Ves RecDeserialize pre-create\n" + more;  
            SomethingSomethingVessel ves = v.RecDeserialize(b);
            Main.SpecialCode = "Ves RecDeserialize post-create " + ves.GetType().Name;
            doDestroy = false;
            return ves;
        }
    }
}
