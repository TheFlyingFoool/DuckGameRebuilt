// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserSpawner
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", false)]
    public class LaserSpawner : Thing
    {
        protected float _spawnWait;
        public float initialDelay;
        public float spawnTime = 10f;
        public bool spawnOnStart = true;
        public int spawnNum = -1;
        protected int _numSpawned;
        public float fireDirection;
        public float firePower = 1f;

        public float direction => fireDirection + (flipHorizontal ? 180f : 0f);

        public LaserSpawner(float xpos, float ypos, System.Type c = null)
          : base(xpos, ypos)
        {
            graphic = new Sprite("laserSpawner");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(12f, 12f);
            collisionOffset = new Vec2(-6f, -6f);
            depth = (Depth)0.99f;
            hugWalls = WallHug.None;
            _visibleInGame = false;
            editorTooltip = "Spawns Quad Laser bullets in the specified direction.";
        }

        public override void Initialize()
        {
            if (!spawnOnStart)
                return;
            _spawnWait = spawnTime;
        }

        public override void Update()
        {
            if (Level.current.simulatePhysics)
                _spawnWait += 0.0166666f;
            if (Level.current.simulatePhysics && Network.isServer && (_numSpawned < spawnNum || spawnNum == -1) && _spawnWait >= spawnTime)
            {
                if (initialDelay > 0.0)
                {
                    initialDelay -= 0.0166666f;
                }
                else
                {
                    Vec2 travel = Maths.AngleToVec(Maths.DegToRad(direction)) * firePower;
                    Vec2 vec2 = position - travel.normalized * 16f;
                    Level.Add(new QuadLaserBullet(vec2.x, vec2.y, travel));
                    _spawnWait = 0f;
                    ++_numSpawned;
                }
            }
            angleDegrees = -direction;
        }

        public override void Terminate()
        {
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("spawnTime", spawnTime);
            binaryClassChunk.AddProperty("initialDelay", initialDelay);
            binaryClassChunk.AddProperty("spawnOnStart", spawnOnStart);
            binaryClassChunk.AddProperty("spawnNum", spawnNum);
            binaryClassChunk.AddProperty("fireDirection", fireDirection);
            binaryClassChunk.AddProperty("firePower", firePower);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            spawnTime = node.GetProperty<float>("spawnTime");
            initialDelay = node.GetProperty<float>("initialDelay");
            spawnOnStart = node.GetProperty<bool>("spawnOnStart");
            spawnNum = node.GetProperty<int>("spawnNum");
            fireDirection = node.GetProperty<float>("fireDirection");
            firePower = node.GetProperty<float>("firePower");
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("spawnTime", Change.ToString(spawnTime)));
            dxmlNode.Add(new DXMLNode("initialDelay", Change.ToString(initialDelay)));
            dxmlNode.Add(new DXMLNode("spawnOnStart", Change.ToString(spawnOnStart)));
            dxmlNode.Add(new DXMLNode("spawnNum", Change.ToString(spawnNum)));
            dxmlNode.Add(new DXMLNode("fireDirection", Change.ToString(fireDirection)));
            dxmlNode.Add(new DXMLNode("firePower", Change.ToString(firePower)));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode1 = node.Element("spawnTime");
            if (dxmlNode1 != null)
                spawnTime = Change.ToSingle(dxmlNode1.Value);
            DXMLNode dxmlNode2 = node.Element("initialDelay");
            if (dxmlNode2 != null)
                initialDelay = Change.ToSingle(dxmlNode2.Value);
            DXMLNode dxmlNode3 = node.Element("spawnOnStart");
            if (dxmlNode3 != null)
                spawnOnStart = Convert.ToBoolean(dxmlNode3.Value);
            DXMLNode dxmlNode4 = node.Element("spawnNum");
            if (dxmlNode4 != null)
                spawnNum = Convert.ToInt32(dxmlNode4.Value);
            DXMLNode dxmlNode5 = node.Element("fireDirection");
            if (dxmlNode5 != null)
                fireDirection = Convert.ToSingle(dxmlNode5.Value);
            DXMLNode dxmlNode6 = node.Element("firePower");
            if (dxmlNode6 != null)
                firePower = Convert.ToSingle(dxmlNode6.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.AddItem(new ContextSlider("Delay", null, new FieldBinding(this, "spawnTime", 1f, 100f)));
            contextMenu.AddItem(new ContextSlider("Initial Delay", null, new FieldBinding(this, "initialDelay", max: 100f)));
            contextMenu.AddItem(new ContextCheckBox("Start Spawned", null, new FieldBinding(this, "spawnOnStart")));
            contextMenu.AddItem(new ContextSlider("Number", null, new FieldBinding(this, "spawnNum", -1f, 100f), 1f, "INF"));
            contextMenu.AddItem(new ContextSlider("Angle", null, new FieldBinding(this, "fireDirection", max: 360f), 1f));
            contextMenu.AddItem(new ContextSlider("Power", null, new FieldBinding(this, "firePower", 1f, 20f)));
            return contextMenu;
        }

        public override void DrawHoverInfo() => Graphics.DrawLine(position, position + Maths.AngleToVec(Maths.DegToRad(direction)) * (firePower * 5f), Color.Red, 2f, (Depth)1f);

        public override void Draw()
        {
            angleDegrees = -direction;
            base.Draw();
        }
    }
}
