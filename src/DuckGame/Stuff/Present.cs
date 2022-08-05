// Decompiled with JetBrains decompiler
// Type: DuckGame.Present
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class Present : Holdable, IPlatform
    {
        private SpriteMap _sprite;
        private System.Type _contains;

        public Present(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("presents", 16, 16)
            {
                frame = Rando.Int(0, 7)
            };
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(14f, 11f);
            depth = -0.5f;
            thickness = 0f;
            weight = 3f;
            flammable = 0.3f;
            charThreshold = 0.5f;
            collideSounds.Add("presentLand");
            editorTooltip = "You never know what you'll find inside! Spawns a random item once.";
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (type is DTIncinerate && isServerForObject)
            {
                SFX.Play("flameExplode");
                for (int index = 0; index < 3; ++index)
                    Level.Add(SmallSmoke.New(x + Rando.Float(-2f, 2f), y + Rando.Float(-2f, 2f)));
                Holdable holdable = SpawnPresent(null);
                if (holdable != null)
                    holdable.velocity = Rando.Vec2(-1f, 1f, -2f, 0f);
                Level.Remove(this);
            }
            return base.OnDestroy(type);
        }

        public override void Initialize()
        {
            List<System.Type> physicsObjects = ItemBox.GetPhysicsObjects(Editor.Placeables);
            physicsObjects.RemoveAll(t => 
                t == typeof(Present) 
                || t == typeof(LavaBarrel) 
                || t == typeof(Grapple));
            _contains = physicsObjects[Rando.Int(physicsObjects.Count - 1)];
        }

        public static void OpenEffect(Vec2 pPosition, int pFrame, bool pIsNetMessage)
        {
            Level.Add(new OpenPresent(pPosition.x, pPosition.y, pFrame));
            for (int index = 0; index < 4; ++index)
                Level.Add(SmallSmoke.New(pPosition.x + Rando.Float(-2f, 2f), pPosition.y + Rando.Float(-2f, 2f)));
            SFX.Play("harp", 0.8f);
            if (pIsNetMessage)
                return;
            Send.Message(new NMPresentOpen(pPosition, (byte)pFrame));
        }

        public Holdable SpawnPresent(Thing pOwner)
        {
            if (!isServerForObject)
                return null;
            if (_contains == null)
                Initialize();
            Holdable thing1 = Editor.CreateThing(_contains) as Holdable;
            if (thing1 != null)
            {
                Thing thing = pOwner;
                Duck duck = pOwner as Duck;
                if (Rando.Int(500) == 1 && thing1 is Gun && (thing1 as Gun).CanSpawnInfinite())
                {
                    (thing1 as Gun).infiniteAmmoVal = true;
                    (thing1 as Gun).infinite.value = true;
                }
                if (thing != null)
                {
                    thing1.x = thing.x;
                    thing1.y = thing.y;
                }
                else
                {
                    thing1.x = x;
                    thing1.y = y;
                }
                Level.Add(thing1);
                if (duck != null)
                {
                    duck.GiveHoldable(thing1);
                    duck.resetAction = true;
                }
            }
            return thing1;
        }

        public override void OnPressAction()
        {
            if (this.owner == null || !isServerForObject)
                return;
            Thing owner = this.owner;
            Duck duck = this.duck;
            if (duck != null)
            {
                ++duck.profile.stats.presentsOpened;
                ++Global.data.presentsOpened.valueInt;
                this.duck.ThrowItem();
            }
            Level.Remove(this);
            Present.OpenEffect(position, _sprite.frame, false);
            SpawnPresent(owner);
        }
    }
}
