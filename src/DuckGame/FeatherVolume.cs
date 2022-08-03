// Decompiled with JetBrains decompiler
// Type: DuckGame.FeatherVolume
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class FeatherVolume : MaterialThing
    {
        private Duck _duckOwner;

        public Duck duckOwner => _duckOwner;

        public FeatherVolume(Duck duckOwner)
          : base(0f, 0f)
        {
            thickness = 0.1f;
            _duckOwner = duckOwner;
            _editorCanModify = false;
            ignoreCollisions = true;
            visible = false;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            Gun owner = bullet.owner as Gun;
            if (bullet.owner != null && (bullet.owner == _duckOwner || owner != null && owner.owner == _duckOwner))
                return false;
            Feather feather = Feather.New(0f, 0f, _duckOwner.persona);
            feather.hSpeed = (float)(-bullet.travelDirNormalized.x * (1.0 + Rando.Float(1f)));
            feather.vSpeed = -Rando.Float(2f);
            feather.position = hitPos;
            Level.Add(feather);
            Vec2 point = hitPos + bullet.travelDirNormalized * 3f;
            if (bullet.isLocal && _duckOwner.sliding && _duckOwner.ragdoll == null && point.x > left + 2.0 && point.x < right - 2.0 && point.y > top + 2.0 && point.y < bottom - 2.0)
            {
                foreach (Equipment equipment in Level.CheckPointAll<Equipment>(point))
                {
                    if (equipment is Helmet || equipment is ChestPlate)
                        return false;
                }
                _duckOwner.Kill(new DTShot(bullet));
            }
            return false;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            Gun owner = bullet.owner as Gun;
            if (bullet.owner != null && (bullet.owner == _duckOwner || owner != null && owner.owner == _duckOwner))
                return;
            Feather feather = Feather.New(0f, 0f, _duckOwner.persona);
            feather.hSpeed = (float)(-bullet.travelDirNormalized.x * (1.0 + Rando.Float(1f)));
            feather.vSpeed = -Rando.Float(2f);
            feather.position = exitPos;
            Level.Add(feather);
        }
    }
}
