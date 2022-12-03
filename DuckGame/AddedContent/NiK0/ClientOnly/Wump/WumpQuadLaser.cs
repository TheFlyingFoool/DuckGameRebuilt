using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Wump|Lasers")]
    public class WumpQuadLaser : Gun
    {
        public const string wumpquadlaser = "iVBORw0KGgoAAAANSUhEUgAAABMAAAANCAMAAAB8UqUVAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAPUExURQAAALLc7zGi8i9ITgAAANMGmIMAAAAFdFJOU/////8A+7YOUwAAAAlwSFlzAAAOwwAADsMBx2+oZAAAAFJJREFUKFOFj0kKwCAQBJ3l/29OLyNJQLAO0lNqo6vBIgzGLiLkvYEVES7J6yKKnFyVC1SfuR2hmapMGXBz/7tIaONA/IVxeghgoNOBQe4zd3c/AGcB9CKH9aUAAAAASUVORK5CYII=";
        public WumpQuadLaser(float xval, float yval) : base(xval, yval)
        {
            ammo = 1;
            _ammoType = new AT9mm();
            graphic = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(wumpquadlaser))), "wumpquadlaser"));
            graphic.Namebase = "wumpquadlaser";
            Content.textures[graphic.Namebase] = graphic.texture;
            center = new Vec2(9.5f, 6.5f);
            collisionOffset = new Vec2(-9.5f, -6.5f);
            collisionSize = new Vec2(19, 13);
            _barrelOffsetTL = new Vec2(28f, 8f);
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(2f, -2f);
            _editorName = "Quad Laser";
            editorTooltip = "Shoots a FAST-MOVING supernatural block of DOOM that passes through dimensions.";
        }
        public override void Fire()
        {
        }
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                Vec2 vec = Offset(barrelOffset);
                if (isServerForObject)
                {
                    WumpQuadLaserBullet quadLaserBullet = new WumpQuadLaserBullet(vec.x, vec.y, barrelVector * 16);
                    quadLaserBullet.killThingType = GetType();
                    if (infiniteAmmoVal) quadLaserBullet.theholysee = true;
                    Level.Add(quadLaserBullet);
                    if (duck != null)
                    {
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                        duck.hSpeed = -barrelVector.x * 20f;
                        duck.vSpeed = -barrelVector.y * 18f - 2f;
                        duck.hMax = 24;
                        duck.vMax = 16;
                        quadLaserBullet.responsibleProfile = duck.profile;
                    }
                }
                Level.Remove(this);
                SFX.Play("laserBlast", 1f, -0.5f);
                SFX.Play("laserBlast", 1f, -0.6f);
                SFX.Play("laserBlast", 1f, -0.7f);
            }
        }
    }
}
