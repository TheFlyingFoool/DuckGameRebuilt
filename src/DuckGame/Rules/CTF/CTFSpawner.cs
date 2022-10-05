// Decompiled with JetBrains decompiler
// Type: DuckGame.CTFSpawner
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    public class CTFSpawner : ItemSpawner
    {
        public EditorProperty<bool> team = new EditorProperty<bool>(false);
        private CTFPresent _present;

        public CTFSpawner(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("ctf/spawner", 16, 4);
            graphic = _sprite;
            center = new Vec2(8f, 2f);
            collisionOffset = new Vec2(-8f, -2f);
            collisionSize = new Vec2(16f, 4f);
            randomSpawn = true;
        }

        public override void SetHoverItem(Holdable hover)
        {
            if (hover != _present || _hoverItem == hover)
                return;
            base.SetHoverItem(hover);
        }

        public override void Update()
        {
            spawnTime = 4f;
            if (_present != null && _present.removeFromLevel)
                _present = null;
            CTFPresent ctfPresent1 = null;
            CTFPresent ctfPresent2 = null;
            foreach (CTFPresent ctfPresent3 in Level.CheckCircleAll<CTFPresent>(position, 16f))
            {
                if (ctfPresent3 != _present)
                    ctfPresent2 = ctfPresent3;
                else
                    ctfPresent1 = ctfPresent3;
            }
            if (ctfPresent2 != null & ctfPresent1 != null)
            {
                if (ctfPresent2.duck != null)
                    ctfPresent2.duck.ThrowItem();
                Level.Remove(ctfPresent2);
                if (DGRSettings.S_ParticleMultiplier != 0) Level.Add(SmallSmoke.New(ctfPresent2.x, ctfPresent2.y));
                CTF.CaptureFlag((bool)team);
                SFX.Play("equip");
            }
            base.Update();
        }

        public override void Draw()
        {
            _sprite.frame = (bool)team ? 0 : 1;
            base.Draw();
        }

        public override void SpawnItem()
        {
            if (_present != null)
                return;
            _spawnWait = 0f;
            _present = new CTFPresent(x, y, (bool)team)
            {
                x = x
            };
            _present.y = (float)(top + (_present.y - _present.bottom) - 6.0);
            _present.vSpeed = -2f;
            Level.Add(_present);
            if (!_seated)
                return;
            SetHoverItem(_present);
        }
    }
}
