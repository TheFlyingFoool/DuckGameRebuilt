using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ThingContainer : Thing
    {
        protected List<Thing> _things;
        protected Type _type;
        public bool bozocheck;
        public bool quickSerialize;

        public List<Thing> things => _things;

        public override void SetTranslation(Vec2 translation)
        {
            foreach (Thing thing in _things)
                thing.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public ThingContainer(List<Thing> things, Type t)
          : base()
        {
            _things = things;
            _type = t;
        }

        public ThingContainer()
          : base()
        {
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = new BinaryClassChunk();
            binaryClassChunk.AddProperty("type", ModLoader.SmallTypeName(GetType()));
            binaryClassChunk.AddProperty("blockType", ModLoader.SmallTypeName(_type));
            BitBuffer bitBuffer1 = new BitBuffer(false);
            BitBuffer bitBuffer2 = new BitBuffer(false);
            bitBuffer1.Write(_things.Count);
            if (typeof(AutoBlock).IsAssignableFrom(_type))
            {
                foreach (Thing thing in _things)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.groupedWithNeighbors = false;
                    autoBlock.neighborsInitialized = false;
                }
                BitBuffer bitBuffer3 = new BitBuffer(false);
                bitBuffer3.Write((ushort)0);
                ushort val = 0;
                foreach (Thing thing in _things)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.InitializeNeighbors();
                    bitBuffer1.Write(thing.x);
                    bitBuffer1.Write(thing.y);
                    bitBuffer1.Write((byte)thing.frame);
                    bitBuffer1.Write(autoBlock.upBlock != null ? (short)_things.IndexOf(autoBlock.upBlock) : (short)-1);
                    bitBuffer1.Write(autoBlock.downBlock != null ? (short)_things.IndexOf(autoBlock.downBlock) : (short)-1);
                    bitBuffer1.Write(autoBlock.rightBlock != null ? (short)_things.IndexOf(autoBlock.rightBlock) : (short)-1);
                    bitBuffer1.Write(autoBlock.leftBlock != null ? (short)_things.IndexOf(autoBlock.leftBlock) : (short)-1);
                    if (!Editor.miniMode)
                    {
                        BlockGroup blockGroup = autoBlock.GroupWithNeighbors(false);
                        if (blockGroup != null)
                        {
                            bitBuffer3.Write(blockGroup.x);
                            bitBuffer3.Write(blockGroup.y);
                            bitBuffer3.Write(blockGroup.collisionOffset.x);
                            bitBuffer3.Write(blockGroup.collisionOffset.y);
                            bitBuffer3.Write(blockGroup.collisionSize.x);
                            bitBuffer3.Write(blockGroup.collisionSize.y);
                            bitBuffer3.Write(blockGroup.blocks.Count);
                            foreach (Block block in blockGroup.blocks)
                                bitBuffer3.Write((short)_things.IndexOf(block));
                            ++val;
                        }
                    }
                }
                bitBuffer3.position = 0;
                bitBuffer3.Write(val);
                foreach (Thing thing in _things)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.groupedWithNeighbors = false;
                    autoBlock.neighborsInitialized = false;
                }
                if (bitBuffer3.lengthInBytes > 2)
                    binaryClassChunk.AddProperty("groupData", bitBuffer3);
            }
            else
            {
                foreach (Thing thing in _things)
                {
                    if ((byte)thing.frame == byte.MaxValue)
                        bitBuffer1.Write(-999999f);
                    bitBuffer1.Write(thing.x);
                    bitBuffer1.Write(thing.y);
                    if (thing.flipHorizontal)
                        bitBuffer1.Write(byte.MaxValue);
                    if ((byte)thing.frame != byte.MaxValue)
                        bitBuffer1.Write((byte)thing.frame);
                    else
                        bitBuffer1.Write((byte)0);
                }
            }
            binaryClassChunk.AddProperty("data", bitBuffer1);
            binaryClassChunk.AddProperty("ct20", true);
            return binaryClassChunk;
        }

        private bool DoDeserialize(BinaryClassChunk node)
        {
            Type type = Editor.GetType(node.GetProperty<string>("blockType"));
            if (type == null)
                return false;
            bool flag1 = typeof(AutoBlock).IsAssignableFrom(type);
            _things = new List<Thing>();
            BitBuffer property1 = node.GetProperty<BitBuffer>("data");
            if (!typeof(AutoBlock).IsAssignableFrom(type))
                flag1 = false;
            bool property2 = node.GetProperty<bool>("ct20");
            List<AutoBlock> autoBlockList = new List<AutoBlock>();
            int num1 = property1.ReadInt();
            for (int index = 0; index < num1; ++index)
            {
                bool flag2 = false;
                float num2 = property1.ReadFloat();
                if (!flag1 && property2 && num2 < -99999)
                {
                    num2 = property1.ReadFloat();
                    flag2 = true;
                }
                float num3 = property1.ReadFloat();
                bool flag3 = false;
                int num4;
                if (property2 && !flag1)
                {
                    num4 = property1.ReadByte();
                    if (num4 == byte.MaxValue)
                    {
                        flag3 = true;
                        num4 = property1.ReadByte();
                    }
                    if (flag2)
                        num4 = byte.MaxValue;
                }
                else
                {
                    num4 = property1.ReadByte();
                    if (num4 == byte.MaxValue && (loadingLevel == null || loadingLevel.GetVersion() == 2))
                    {
                        flag3 = true;
                        num4 = property1.ReadByte();
                    }
                }
                bool flag4 = Level.flipH;
                if (Level.loadingOppositeSymmetry)
                    flag4 = !flag4;
                if (flag4)
                    num2 = 192f - num2 - 16f;
                Thing thing = Editor.CreateThing(type);
                if (flag4 && thing is AutoBlock)
                {
                    (thing as AutoBlock).needsRefresh = true;
                    (thing as AutoBlock).isFlipped = true;
                }
                if (thing is BackgroundTile)
                {
                    if (flag4)
                        (thing as BackgroundTile).isFlipped = true;
                    (thing as BackgroundTile).oppositeSymmetry = !Level.loadingOppositeSymmetry;
                }
                if (flag4 && thing is AutoPlatform)
                    (thing as AutoPlatform).needsRefresh = true;
                if (flag3)
                    thing.flipHorizontal = true;
                thing.x = num2;
                thing.y = num3;
                thing.placed = true;
                if (thing.isStatic)
                    _isStatic = true;
                if (flag1)
                {
                    short num5 = property1.ReadShort();
                    short num6 = property1.ReadShort();
                    short num7 = property1.ReadShort();
                    short num8 = property1.ReadShort();
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.northIndex = num5;
                    autoBlock.southIndex = num6;
                    if (flag4)
                    {
                        autoBlock.westIndex = num7;
                        autoBlock.eastIndex = num8;
                    }
                    else
                    {
                        autoBlock.eastIndex = num7;
                        autoBlock.westIndex = num8;
                    }
                    autoBlockList.Add(autoBlock);
                }
                bool flag5 = true;
                if (Level.symmetry)
                {
                    if (Level.leftSymmetry && num2 > 80)
                        flag5 = false;
                    if (!Level.leftSymmetry && num2 < 96)
                        flag5 = false;
                }
                if (flag5)
                {
                    thing.frame = num4;
                    _things.Add(thing);
                }
            }
            if (flag1 && !(Level.current is Editor))
            {
                foreach (AutoBlock autoBlock in autoBlockList)
                {
                    if (autoBlock.northIndex != -1)
                        autoBlock.upBlock = autoBlockList[autoBlock.northIndex];
                    if (autoBlock.southIndex != -1)
                        autoBlock.downBlock = autoBlockList[autoBlock.southIndex];
                    if (autoBlock.eastIndex != -1)
                        autoBlock.rightBlock = autoBlockList[autoBlock.eastIndex];
                    if (autoBlock.westIndex != -1)
                        autoBlock.leftBlock = autoBlockList[autoBlock.westIndex];
                }
                BitBuffer property3 = node.GetProperty<BitBuffer>("groupData");
                if (property3 != null)
                {
                    ushort num9 = property3.ReadUShort();
                    int num10;
                    for (int index1 = 0; index1 < num9; index1 = num10 + 1)
                    {
                        BlockGroup blockGroup = new BlockGroup
                        {
                            position = new Vec2(property3.ReadFloat(), property3.ReadFloat())
                        };
                        bool flag6 = Level.flipH;
                        if (Level.loadingOppositeSymmetry)
                            flag6 = !flag6;
                        if (flag6)
                            blockGroup.position.x = (float)(192 - blockGroup.position.x - 16);
                        blockGroup.collisionOffset = new Vec2(property3.ReadFloat(), property3.ReadFloat());
                        blockGroup.collisionSize = new Vec2(property3.ReadFloat(), property3.ReadFloat());
                        float num11 = 88f;
                        if (Level.symmetry)
                        {
                            if (Level.leftSymmetry)
                            {
                                if (blockGroup.left < num11 && blockGroup.right > num11)
                                {
                                    float num12 = blockGroup.right - num11;
                                    float x = blockGroup.collisionSize.x - num12;
                                    blockGroup.position.x -= num12;
                                    blockGroup.position.x += x / 2f;
                                    blockGroup.collisionSize = new Vec2(x, blockGroup.collisionSize.y);
                                    blockGroup.collisionOffset = new Vec2((float)-(x / 2), blockGroup.collisionOffset.y);
                                    blockGroup.right = num11;
                                }
                            }
                            else
                            {
                                num11 = 88f;
                                if (blockGroup.right > num11 && blockGroup.left < num11)
                                {
                                    float num13 = num11 - blockGroup.left;
                                    float x = blockGroup.collisionSize.x - num13;
                                    blockGroup.position.x += num13;
                                    blockGroup.position.x -= x / 2f;
                                    blockGroup.collisionSize = new Vec2(x, blockGroup.collisionSize.y);
                                    blockGroup.collisionOffset = new Vec2((float)-(x / 2), blockGroup.collisionOffset.y);
                                    blockGroup.left = num11;
                                }
                            }
                        }
                        int num14 = property3.ReadInt();
                        for (int index2 = 0; index2 < num14; ++index2)
                        {
                            int index3 = property3.ReadShort();
                            if (index3 >= 0)
                            {
                                AutoBlock b = autoBlockList[index3];
                                bool flag7 = true;
                                if (Level.symmetry)
                                {
                                    if (Level.leftSymmetry && b.x > 80)
                                        flag7 = false;
                                    if (!Level.leftSymmetry && b.x < 96)
                                        flag7 = false;
                                }
                                if (flag7)
                                {
                                    b.groupedWithNeighbors = true;
                                    blockGroup.Add(b);
                                    blockGroup.physicsMaterial = b.physicsMaterial;
                                    blockGroup.thickness = b.thickness;
                                }
                                _things.Remove(b);
                            }
                        }
                        num10 = index1 + num14;
                        if (flag6)
                            blockGroup.needsRefresh = true;
                        if (Level.symmetry)
                        {
                            if (Level.leftSymmetry && blockGroup.left < num11)
                                _things.Add(blockGroup);
                            else if (!Level.leftSymmetry && blockGroup.right > num11)
                                _things.Add(blockGroup);
                        }
                        else
                            _things.Add(blockGroup);
                    }
                }
            }
            return true;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            if (!Level.symmetry)
                return DoDeserialize(node);
            Level.leftSymmetry = true;
            Level.loadingOppositeSymmetry = false;
            DoDeserialize(node);
            List<Thing> collection = new List<Thing>(_things);
            Level.loadingOppositeSymmetry = true;
            Level.leftSymmetry = false;
            DoDeserialize(node);
            _things.AddRange(collection);
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = new DXMLNode("Object");
            dxmlNode.Add(new DXMLNode("type", GetType().AssemblyQualifiedName));
            dxmlNode.Add(new DXMLNode("blockType", _type.AssemblyQualifiedName));
            string str1 = "n,";
            string str2 = "";
            if (typeof(AutoBlock).IsAssignableFrom(_type))
            {
                foreach (Thing thing in _things)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.groupedWithNeighbors = false;
                    autoBlock.neighborsInitialized = false;
                }
                foreach (Thing thing in _things)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.InitializeNeighbors();
                    str1 = str1 + Change.ToString(thing.x) + ",";
                    str1 = str1 + Change.ToString(thing.y) + ",";
                    str1 = str1 + thing.frame.ToString() + ",";
                    str1 = autoBlock.upBlock == null ? str1 + "-1," : str1 + Change.ToString(_things.IndexOf(autoBlock.upBlock)) + ",";
                    str1 = autoBlock.downBlock == null ? str1 + "-1," : str1 + Change.ToString(_things.IndexOf(autoBlock.downBlock)) + ",";
                    str1 = autoBlock.rightBlock == null ? str1 + "-1," : str1 + Change.ToString(_things.IndexOf(autoBlock.rightBlock)) + ",";
                    str1 = autoBlock.leftBlock == null ? str1 + "-1," : str1 + Change.ToString(_things.IndexOf(autoBlock.leftBlock)) + ",";
                    BlockGroup blockGroup = autoBlock.GroupWithNeighbors(false);
                    if (blockGroup != null)
                    {
                        str2 = str2 + Change.ToString(blockGroup.x) + ",";
                        str2 = str2 + Change.ToString(blockGroup.y) + ",";
                        str2 = str2 + Change.ToString(blockGroup.collisionOffset.x) + ",";
                        str2 = str2 + Change.ToString(blockGroup.collisionOffset.y) + ",";
                        str2 = str2 + Change.ToString(blockGroup.collisionSize.x) + ",";
                        str2 = str2 + Change.ToString(blockGroup.collisionSize.y) + ",";
                        str2 = str2 + Change.ToString(blockGroup.blocks.Count) + ",";
                        foreach (Block block in blockGroup.blocks)
                            str2 = str2 + Change.ToString(_things.IndexOf(block)) + ",";
                    }
                }
                foreach (Thing thing in _things)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.groupedWithNeighbors = false;
                    autoBlock.neighborsInitialized = false;
                }
                if (str2.Length > 0)
                {
                    string varValue = str2.Substring(0, str2.Length - 1);
                    dxmlNode.Add(new DXMLNode("groupData", varValue));
                }
            }
            else
            {
                foreach (Thing thing in _things)
                {
                    str1 = str1 + Change.ToString(thing.x) + ",";
                    str1 = str1 + Change.ToString(thing.y) + ",";
                    str1 = str1 + thing.frame.ToString() + ",";
                }
            }
            string varValue1 = str1.Substring(0, str1.Length - 1);
            dxmlNode.Add(new DXMLNode("data", varValue1));
            return dxmlNode;
        }

        private bool LegacyDoDeserialize(DXMLNode node)
        {
            Type type = Editor.GetType(node.Element("blockType").Value);
            bool flag1 = typeof(AutoBlock).IsAssignableFrom(type);
            _things = new List<Thing>();
            string[] source1 = node.Element("data").Value.Split(',');
            int num1 = source1[0] == "n" ? 1 : 0;
            if (num1 == 0)
                flag1 = false;
            List<AutoBlock> autoBlockList = new List<AutoBlock>();
            for (int index = num1 != 0 ? 1 : 0; index < source1.Length; index += 3)
            {
                float num2 = Change.ToSingle(source1[index]);
                float single = Change.ToSingle(source1[index + 1]);
                int int32 = Convert.ToInt32(source1[index + 2]);
                bool flag2 = Level.flipH;
                if (Level.loadingOppositeSymmetry)
                    flag2 = !flag2;
                if (flag2)
                    num2 = (float)(192 - num2 - 16);
                Thing thing = Editor.CreateThing(type);
                if (flag2 && thing is AutoBlock)
                {
                    (thing as AutoBlock).needsRefresh = true;
                    (thing as AutoBlock).isFlipped = true;
                }
                if (flag2 && thing is AutoPlatform)
                    (thing as AutoPlatform).needsRefresh = true;
                thing.x = num2;
                thing.y = single;
                thing.placed = true;
                if (thing.isStatic)
                    _isStatic = true;
                if (flag1)
                {
                    AutoBlock autoBlock = thing as AutoBlock;
                    autoBlock.northIndex = Convert.ToInt32(source1[index + 3]);
                    autoBlock.southIndex = Convert.ToInt32(source1[index + 4]);
                    if (flag2)
                    {
                        autoBlock.westIndex = Convert.ToInt32(source1[index + 5]);
                        autoBlock.eastIndex = Convert.ToInt32(source1[index + 6]);
                    }
                    else
                    {
                        autoBlock.eastIndex = Convert.ToInt32(source1[index + 5]);
                        autoBlock.westIndex = Convert.ToInt32(source1[index + 6]);
                    }
                    autoBlockList.Add(autoBlock);
                    index += 4;
                }
                bool flag3 = true;
                if (Level.symmetry)
                {
                    if (Level.leftSymmetry && num2 > 80)
                        flag3 = false;
                    if (!Level.leftSymmetry && num2 < 96)
                        flag3 = false;
                }
                if (flag3)
                {
                    thing.frame = int32;
                    _things.Add(thing);
                }
            }
            if (flag1 && !(Level.current is Editor))
            {
                foreach (AutoBlock autoBlock in autoBlockList)
                {
                    if (autoBlock.northIndex != -1)
                        autoBlock.upBlock = autoBlockList[autoBlock.northIndex];
                    if (autoBlock.southIndex != -1)
                        autoBlock.downBlock = autoBlockList[autoBlock.southIndex];
                    if (autoBlock.eastIndex != -1)
                        autoBlock.rightBlock = autoBlockList[autoBlock.eastIndex];
                    if (autoBlock.westIndex != -1)
                        autoBlock.leftBlock = autoBlockList[autoBlock.westIndex];
                    autoBlock.neighborsInitialized = true;
                }
                DXMLNode dxmlNode = node.Element("groupData");
                if (dxmlNode != null)
                {
                    string[] source2 = dxmlNode.Value.Split(',');
                    int num3;
                    for (int index1 = 0; index1 < source2.Length; index1 = num3 + 7)
                    {
                        BlockGroup blockGroup = new BlockGroup
                        {
                            position = new Vec2(Change.ToSingle(source2[index1]), Change.ToSingle(source2[index1 + 1]))
                        };
                        bool flag4 = Level.flipH;
                        if (Level.loadingOppositeSymmetry)
                            flag4 = !flag4;
                        if (flag4)
                            blockGroup.position.x = (float)(192 - blockGroup.position.x - 16);
                        blockGroup.collisionOffset = new Vec2(Change.ToSingle(source2[index1 + 2]), Change.ToSingle(source2[index1 + 3]));
                        blockGroup.collisionSize = new Vec2(Change.ToSingle(source2[index1 + 4]), Change.ToSingle(source2[index1 + 5]));
                        float num4 = 88f;
                        if (Level.symmetry)
                        {
                            if (Level.leftSymmetry)
                            {
                                if (blockGroup.left < num4 && blockGroup.right > num4)
                                {
                                    float num5 = blockGroup.right - num4;
                                    float x = blockGroup.collisionSize.x - num5;
                                    blockGroup.position.x -= num5;
                                    blockGroup.position.x += x / 2f;
                                    blockGroup.collisionSize = new Vec2(x, blockGroup.collisionSize.y);
                                    blockGroup.collisionOffset = new Vec2((float)-(x / 2), blockGroup.collisionOffset.y);
                                    blockGroup.right = num4;
                                }
                            }
                            else
                            {
                                num4 = 88f;
                                if (blockGroup.right > num4 && blockGroup.left < num4)
                                {
                                    float num6 = num4 - blockGroup.left;
                                    float x = blockGroup.collisionSize.x - num6;
                                    blockGroup.position.x += num6;
                                    blockGroup.position.x -= x / 2f;
                                    blockGroup.collisionSize = new Vec2(x, blockGroup.collisionSize.y);
                                    blockGroup.collisionOffset = new Vec2((float)-(x / 2), blockGroup.collisionOffset.y);
                                    blockGroup.left = num4;
                                }
                            }
                        }
                        int int32_1 = Convert.ToInt32(source2[index1 + 6]);
                        for (int index2 = 0; index2 < int32_1; ++index2)
                        {
                            int int32_2 = Convert.ToInt32(source2[index1 + 7 + index2]);
                            AutoBlock b = autoBlockList[int32_2];
                            bool flag5 = true;
                            if (Level.symmetry)
                            {
                                if (Level.leftSymmetry && b.x > 80)
                                    flag5 = false;
                                if (!Level.leftSymmetry && b.x < 96)
                                    flag5 = false;
                            }
                            if (flag5)
                            {
                                b.groupedWithNeighbors = true;
                                blockGroup.Add(b);
                                blockGroup.physicsMaterial = b.physicsMaterial;
                                blockGroup.thickness = b.thickness;
                            }
                            _things.Remove(b);
                        }
                        num3 = index1 + int32_1;
                        if (flag4)
                            blockGroup.needsRefresh = true;
                        if (Level.symmetry)
                        {
                            if (Level.leftSymmetry && blockGroup.left < num4)
                                _things.Add(blockGroup);
                            else if (!Level.leftSymmetry && blockGroup.right > num4)
                                _things.Add(blockGroup);
                        }
                        else
                            _things.Add(blockGroup);
                    }
                }
            }
            return true;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            if (!Level.symmetry)
                return LegacyDoDeserialize(node);
            Level.leftSymmetry = true;
            Level.loadingOppositeSymmetry = false;
            LegacyDoDeserialize(node);
            List<Thing> collection = new List<Thing>(_things);
            Level.loadingOppositeSymmetry = true;
            Level.leftSymmetry = false;
            LegacyDoDeserialize(node);
            _things.AddRange(collection);
            return true;
        }
    }
}
