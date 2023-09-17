using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Stuff")]
    public class ItemFrame : MaterialThing
    {
        public EditorProperty<int> tint = new EditorProperty<int>(0, max: Window.windowColors.Count - 1, increment: 1f);
        public EditorProperty<int> style = new EditorProperty<int>(0, null, 0, 3, 1);
        public EditorProperty<bool> locked = new EditorProperty<bool>(false);

        public SpriteMap sprite;
        public SpriteMap lockedSprite;
        public ItemFrame(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("itemFrame", 24, 24);
            lockedSprite = new SpriteMap("itemFrameLock", 24, 24);
            lockedSprite.center = new Vec2(24);
            graphic = sprite;
            _canFlip = false;

            thickness = 0;
            collisionSize = new Vec2(20, 20);
            _collisionOffset = new Vec2(-10, -10);
            center = new Vec2(12);
            depth = -1;
            _editorIcon = new Sprite("itemFramePrev");
            editorTooltip = "Fancy ducks store their treasure in these frames. Sometimes they lock them up with keys too";
        }
        public bool _hasGlass = true;
        private Vec2 _enter;
        public float maxHealth = 5f;
        public float hitPoints = 5f;
        public float damageMultiplier = 1f;
        private List<Vec2> _hits = new List<Vec2>();
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
                Fondle(this, DuckNetwork.localConnection);
            if (!_hasGlass)
                return base.Hit(bullet, hitPos);
            _enter = hitPos + bullet.travelDirNormalized;
            if (_enter.x < x && _enter.x < left + 2f)
                _enter.x = left;
            else if (_enter.x > x && _enter.x > right - 2f)
                _enter.x = right;
            if (_enter.y < y && _enter.y < top + 2f)
                _enter.y = top;
            else if (_enter.y > y && _enter.y > bottom - 2f)
                _enter.y = bottom;
            if (hitPoints <= 0)
                return false;
            hitPos -= bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
            {
                Level.Add(new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized, tint.value));
                if (index > 32) break;
                //Anticrash measure, since damagemultiplier is synced you can make it an insanely high number to spawn infinite particles on someone elses side
                //this still doesn't completely solve the problem but its a good enough bandaid since the particles will remove themselves from the cap, making it
                //only a lag exploit rather than a softlock/crash like it is in base dg
                //-NiK0
            }
            SFX.Play("glassHit", 0.5f);
            if (isServerForObject && bullet.isLocal)
            {
                hitPoints -= damageMultiplier;
                ++damageMultiplier;
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            if (!_hasGlass)
                return;
            _hits.Add(_enter);
            Vec2 vec2 = exitPos - bullet.travelDirNormalized;
            if (vec2.x < x && vec2.x < left + 2.0f)
                vec2.x = left;
            else if (vec2.x > x && vec2.x > right - 2.0f)
                vec2.x = right;
            if (vec2.y < y && vec2.y < top + 2.0f)
                vec2.y = top;
            else if (vec2.y > y && vec2.y > bottom - 2.0f)
                vec2.y = bottom;
            _hits.Add(vec2);
            exitPos += bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
            {
                Level.Add(new GlassParticle(exitPos.x, exitPos.y, -bullet.travelDirNormalized, tint.value));
                if (index > 32) break;
            }
        }
        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!_hasGlass) return base.OnDestroy(type);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(x - 4f + Rando.Float(8f), y - 16f + Rando.Float(32f), Vec2.Zero, tint.value)
                {
                    hSpeed = (Rando.Float(1f) > 0.5 ? 1f : -1f) * Rando.Float(3f),
                    vSpeed = -Rando.Float(1f)
                };
                Level.Add(glassParticle);
            }
            int ix = (int)(12f * DGRSettings.ActualParticleMultiplier);
            for (int index = 0; index < ix; ++index)
            {
                Level.Add(new GlassDebris(false, Rando.Float(left, right), Rando.Float(top, bottom), Rando.Float(-2f, 2f), Rando.Float(-2f, 2f), 1, tint.value));
            }
            SFX.Play("glassBreak");
            _hasGlass = false;
            _destroyed = true;
            return base.OnDestroy(type);
        }
        public override void Update()
        {
            Key k = Level.CheckRect<Key>(topLeft, bottomRight);
            if (k != null && k.isServerForObject)
            {
                Fondle(this);
                UnlockDoor(k);
            }

            if (hitPoints <= 0 && _hasGlass) Destroy(new DTImpact(null));

            if (_hasGlass)
            {
                UpdateContainedObject();

            }
            if (containedObject != null)
            {
                if (isServerForObject)
                {
                    if (locked && !didUnlock)
                    {
                        containedObject.canPickUp = false;
                    }
                    else
                    {
                        containedObject.canPickUp = true;

                    }
                }
                if ((containedObject.owner != null && containedObject.isServerForObject) || (!_hasGlass && (!locked || didUnlock)))
                {
                    Fondle(this);
                    hitPoints = -1;
                    containedObject.solid = true;
                    containedObject.alpha = 1;
                    containedObject.enablePhysics = true;
                    containedObject = null;
                }
                else
                {
                    containedObject.solid = false;
                    containedObject.alpha = 0;
                }
                if (contains != null && (previewThing == null || previewThing.GetType() != contains))
                {
                    previewThing = Editor.GetThing(contains);
                    if (previewThing != null)
                    {
                        previewSprite = previewThing.GeneratePreview(22, 22, true);
                    }
                }
            }

            base.Update();
        }
        public virtual void UpdateContainedObject()
        {
            if (!isServerForObject && loadingLevel == null || containedObject != null)
                return;
            containedObject = GetSpawnItem();
            if (containedObject == null) return;
            containedObject.visible = true;
            containedObject.alpha = 0;
            containedObject.enablePhysics = false;
            containedObject.position = position;
            Level.Add(containedObject);
        }
        public Holdable containedObject;
        public Type contains { get; set; }

        public void UnlockDoor(Key with)
        {
            if (!locked || !with.isServerForObject)
                return;
            if (Network.isActive)
            {
                ExtraFondle(this, with.connection);
                //Send.Message(new NMUnlockDoor(this));
                //networkUnlockMessage = true;
            }
            locked = false;
            if (with.owner is Duck owner)
            {
                RumbleManager.AddRumbleEvent(owner.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                owner.ThrowItem();
            }
            Level.Remove(with);
            if (Network.isActive)
                return;
            DoUnlock(with.position);
        }

        public Vec2 ps;
        public bool didUnlock;
        public void DoUnlock(Vec2 keyPos)
        {
            ps = keyPos;
            SFX.DontSave = 1;
            SFX.Play("deedleBeep");
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                Level.Add(SmallSmoke.New(keyPos.x, keyPos.y));
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 3; ++index)
                    Level.Add(SmallSmoke.New(x + Rando.Float(-3f, 3f), y + Rando.Float(-3f, 3f)));
            }
            didUnlock = true;
        }
        public override void Draw()
        {
            if (contains != null && Level.current is Editor && previewThing != null)
            {
                previewSprite.CenterOrigin();
                previewSprite.flipH = offDir < 0;
                Graphics.Draw(previewSprite, x, y, depth - 4);
            }

            frame = style.value;
                if (previewSprite != null && containedObject != null)
                { 
                    previewSprite.CenterOrigin();
                    previewSprite.flipH = offDir < 0;
                    Graphics.Draw(previewSprite, x, y, depth - 4);
                }
            if (_hasGlass)
            {

                Color windowColor = Window.windowColors[tint.value];
                Graphics.DrawRect(topLeft, bottomRight, windowColor * 0.5f, depth - 2);

                for (int index = 0; index < _hits.Count; index += 2)
                {
                    if (index + 1 > _hits.Count)
                        return;

                    Color col = new Color((byte)(windowColor.r * 0.5f), (byte)(windowColor.g * 0.5f), (byte)(windowColor.b * 0.8f), (byte)178);
                    Graphics.DrawLine(_hits[index], _hits[index + 1], col, 1f, depth - 1);
                }
            }
            if (locked && !didUnlock)
            {
                lockedSprite.imageIndex = frame;
                Graphics.Draw(lockedSprite, x + center.x, y + center.y, depth + 1);
            }
            base.Draw();
        }
        public Thing previewThing;
        public Sprite previewSprite;
        public override void EditorUpdate()
        {
            if (contains != null && Level.current is Editor && (previewThing == null || previewThing.GetType() != contains))
            {
                previewThing = Editor.GetThing(contains);
                if (previewThing != null)
                {
                    previewSprite = previewThing.GeneratePreview(22, 22, true);
                }
            }
            base.EditorUpdate();
        }
        public virtual Holdable GetSpawnItem()
        {
            if (contains == null)
                return null;
            IReadOnlyPropertyBag bag = ContentProperties.GetBag(contains);
            return !Network.isActive || bag.GetOrDefault("isOnlineCapable", true)
                ? Editor.CreateThing(contains) as Holdable
                : Activator.CreateInstance(typeof(Pistol), Editor.GetConstructorParameters(typeof(Pistol))) as
                    Holdable;
        }
        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("contains", Editor.SerializeTypeName(contains));
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            contains = Editor.DeSerializeTypeName(node.GetProperty<string>("contains"));
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("contains", contains != null ? contains.AssemblyQualifiedName : (object)""));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("contains");
            if (dxmlNode != null)
                contains = Editor.GetType(dxmlNode.Value);
            return true;
        }

        public override ContextMenu GetContextMenu()
        {
            FieldBinding radioBinding = new FieldBinding(this, "contains");
            EditorGroupMenu contextMenu = base.GetContextMenu() as EditorGroupMenu;
            contextMenu.InitializeGroups(new EditorGroup(typeof(Holdable)), radioBinding);
            return contextMenu;
        }

        public override string GetDetailsString()
        {
            string str = "EMPTY";
            if (contains != null)
                str = contains.Name;
            return contains == null ? base.GetDetailsString() : base.GetDetailsString() + "Contains: " + str;
        }

        public override void DrawHoverInfo()
        {
            string text = "EMPTY";
            if (contains != null)
                text = contains.Name;
            Graphics.DrawString(text, position + new Vec2((float)(-Graphics.GetStringWidth(text) / 2), -16f),
                Color.White, 0.9f);
        }
    }
}
