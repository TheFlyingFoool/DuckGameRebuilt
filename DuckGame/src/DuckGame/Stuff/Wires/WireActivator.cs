// Decompiled with JetBrains decompiler
// Type: DuckGame.WireActivator
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Wires")]
    [BaggedProperty("isOnlineCapable", true)]
    public class WireActivator : Thing, IWirePeripheral
    {
        public StateBinding _actionBinding = new WireActivatorFlagBinding();
        private SpriteMap _sprite;
        public bool action;
        private List<Thing> _controlledObjects = new List<Thing>();
        private bool _preparedObjects;
        private bool _prevAction;

        public WireActivator(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("activatorBlock", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "Wire Activator";
            editorTooltip = "Activates nearby objects when powered.";
            layer = Layer.Foreground;
            _canFlip = true;
            _placementCost += 4;
        }

        public override void Update()
        {
            if (!_preparedObjects)
            {
                foreach (MaterialThing materialThing in level.CollisionCircleAll<MaterialThing>(position, 16f))
                {
                    if (!(materialThing is PhysicsObject))
                    {
                        if (materialThing is VerticalDoor)
                            (materialThing as VerticalDoor).slideLocked = true;
                        if (materialThing is FunBeam)
                            (materialThing as FunBeam).enabled = false;
                        _controlledObjects.Add(materialThing);
                    }
                }
                _preparedObjects = true;
            }
            UpdateObjectTriggers();
            if (action != _prevAction)
            {
                UpdateAction(action);
                _prevAction = action;
            }
            base.Update();
        }

        public override void Terminate() => base.Terminate();

        public override void Draw() => base.Draw();

        public void UpdateObjectTriggers()
        {
            if (!action)
                return;
            foreach (PhysicsObject physicsObject in level.CollisionCircleAll<PhysicsObject>(position, 16f))
            {
                if (physicsObject is Holdable)
                    (physicsObject as Holdable).triggerAction = true;
            }
        }

        public void UpdateAction(bool pOn)
        {
            foreach (Thing controlledObject in _controlledObjects)
            {
                if (controlledObject is VerticalDoor)
                    (controlledObject as VerticalDoor).slideLockOpened = pOn;
                if (controlledObject is FunBeam)
                    (controlledObject as FunBeam).enabled = pOn;
            }
        }

        public void Pulse(int type, WireTileset wire)
        {
            Thing.Fondle(this, DuckNetwork.localConnection);
            switch (type)
            {
                case 0:
                    action = true;
                    UpdateAction(true);
                    action = false;
                    break;
                case 1:
                    action = true;
                    break;
                case 2:
                    action = false;
                    break;
                case 3:
                    action = false;
                    UpdateAction(false);
                    action = true;
                    break;
            }
        }
    }
}
