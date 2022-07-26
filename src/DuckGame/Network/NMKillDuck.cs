// Decompiled with JetBrains decompiler
// Type: DuckGame.NMKillDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMKillDuck : NMEvent
    {
        public byte index;
        public bool crush;
        public bool cook;
        public bool fall;
        public byte lifeChange;
        private byte _levelIndex;

        public NMKillDuck(byte idx, bool wasCrush, bool wasCook, bool wasFall, byte pLifeChange)
        {
            this.index = idx;
            this.crush = wasCrush;
            this.cook = wasCook;
            this.fall = wasFall;
            this.lifeChange = pLifeChange;
        }

        public NMKillDuck(byte idx, bool wasCrush, bool wasCook)
        {
            this.index = idx;
            this.crush = wasCrush;
            this.cook = wasCook;
        }

        public NMKillDuck()
        {
        }

        public override void Activate()
        {
            if ((int)DuckNetwork.levelIndex != (int)this._levelIndex || (int)this.index >= DuckNetwork.profiles.Count)
                return;
            Profile profile = DuckNetwork.profiles[(int)this.index];
            if (profile.duck == null || !profile.duck.WillAcceptLifeChange(this.lifeChange))
                return;
            DestroyType type = !this.crush ? (!this.fall ? (DestroyType)new DTImpact((Thing)null) : (DestroyType)new DTFall()) : (DestroyType)new DTCrush((PhysicsObject)null);
            profile.duck.isKillMessage = true;
            if (profile.duck.Kill(type))
            {
                if (!this.cook)
                    profile.duck.GoRagdoll();
                Thing.Fondle((Thing)profile.duck, this.connection);
                if (profile.duck._ragdollInstance != null)
                    Thing.Fondle((Thing)profile.duck._ragdollInstance, this.connection);
                if (profile.duck._trappedInstance != null)
                    Thing.Fondle((Thing)profile.duck._trappedInstance, this.connection);
                if (profile.duck._cookedInstance != null)
                    Thing.Fondle((Thing)profile.duck._cookedInstance, this.connection);
            }
            profile.duck.isKillMessage = false;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            this._serializedData.Write(DuckNetwork.levelIndex);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            this._levelIndex = d.ReadByte();
        }
    }
}
