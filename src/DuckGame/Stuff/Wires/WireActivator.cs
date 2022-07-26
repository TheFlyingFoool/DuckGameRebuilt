// Decompiled with JetBrains decompiler
// Type: DuckGame.WireActivator
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        public StateBinding _actionBinding = (StateBinding)new WireActivatorFlagBinding();
        private SpriteMap _sprite;
        public bool action;
        private List<Thing> _controlledObjects = new List<Thing>();
        private bool _preparedObjects;
        private bool _prevAction;

        public WireActivator(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("activatorBlock", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = - 0.5f;
            this._editorName = "Wire Activator";
            this.editorTooltip = "Activates nearby objects when powered.";
            this.layer = Layer.Foreground;
            this._canFlip = true;
            this._placementCost += 4;
        }

        public override void Update()
        {
            if (!this._preparedObjects)
            {
                foreach (MaterialThing materialThing in this.level.CollisionCircleAll<MaterialThing>(this.position, 16f))
                {
                    if (!(materialThing is PhysicsObject))
                    {
                        if (materialThing is VerticalDoor)
                            (materialThing as VerticalDoor).slideLocked = true;
                        if (materialThing is FunBeam)
                            (materialThing as FunBeam).enabled = false;
                        this._controlledObjects.Add((Thing)materialThing);
                    }
                }
                this._preparedObjects = true;
            }
            this.UpdateObjectTriggers();
            if (this.action != this._prevAction)
            {
                this.UpdateAction(this.action);
                this._prevAction = this.action;
            }
            base.Update();
        }

        public override void Terminate() => base.Terminate();

        public override void Draw() => base.Draw();

        public void UpdateObjectTriggers()
        {
            if (!this.action)
                return;
            foreach (PhysicsObject physicsObject in this.level.CollisionCircleAll<PhysicsObject>(this.position, 16f))
            {
                if (physicsObject is Holdable)
                    (physicsObject as Holdable).triggerAction = true;
            }
        }

        public void UpdateAction(bool pOn)
        {
            foreach (Thing controlledObject in this._controlledObjects)
            {
                if (controlledObject is VerticalDoor)
                    (controlledObject as VerticalDoor).slideLockOpened = pOn;
                if (controlledObject is FunBeam)
                    (controlledObject as FunBeam).enabled = pOn;
            }
        }

        public void Pulse(int type, WireTileset wire)
        {
            Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            switch (type)
            {
                case 0:
                    this.action = true;
                    this.UpdateAction(true);
                    this.action = false;
                    break;
                case 1:
                    this.action = true;
                    break;
                case 2:
                    this.action = false;
                    break;
                case 3:
                    this.action = false;
                    this.UpdateAction(false);
                    this.action = true;
                    break;
            }
        }
    }
}
