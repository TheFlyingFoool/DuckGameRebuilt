// Decompiled with JetBrains decompiler
// Type: DuckGame.AmmoType
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public abstract class AmmoType
    {
        public float accuracy;
        public float range;
        public float rangeVariation;
        public float penetration;
        public float bulletSpeed = 28f;
        public float speedVariation = 3f;
        public float bulletLength = 100f;
        public Color bulletColor = Color.White;
        public bool softRebound;
        public bool rebound;
        public float bulletThickness = 1f;
        public bool affectedByGravity;
        public float airFrictionMultiplier = 1f;
        public bool deadly = true;
        public float barrelAngleDegrees;
        public bool immediatelyDeadly;
        public float weight;
        public float gravityMultiplier = 1f;
        public bool combustable;
        public float impactPower = 2f;
        public bool flawlessPipeTravel;
        public bool canBeReflected = true;
        public bool canTeleport = true;
        public int ownerSafety;
        public bool complexSync;
        private static Dictionary<System.Type, AmmoType.ComplexDiffData> _complexDiff = new Dictionary<System.Type, AmmoType.ComplexDiffData>();
        private static Map<byte, System.Type> _types = new Map<byte, System.Type>();
        public Sprite sprite;
        public System.Type bulletType = typeof(Bullet);

        public static Map<byte, System.Type> indexTypeMap => AmmoType._types;

        public static void InitializeTypes()
        {
            if (MonoMain.moddingEnabled)
            {
                byte key = 0;
                foreach (System.Type sortedType in ManagedContent.AmmoTypes.SortedTypes)
                {
                    AmmoType._types[key] = sortedType;
                    ++key;
                }
            }
            else
            {
                List<System.Type> list = Editor.GetSubclasses(typeof(AmmoType)).ToList<System.Type>();
                byte key = 0;
                foreach (System.Type type in list)
                {
                    AmmoType._types[key] = type;
                    ++key;
                }
            }
        }

        public void WriteComplexValues(BitBuffer pBuffer)
        {
            if (!this.complexSync)
                return;
            ComplexDiffData complexDiffData;
            if (!AmmoType._complexDiff.TryGetValue(this.GetType(), out complexDiffData))
                AmmoType._complexDiff[this.GetType()] = complexDiffData = new AmmoType.ComplexDiffData(this.GetType());
            foreach (ClassMember binding in complexDiffData.bindings)
            {
                object objB = binding.GetValue(this);
                if (!object.Equals(binding.GetValue(complexDiffData.original), objB))
                {
                    pBuffer.Write(true);
                    pBuffer.Write(objB);
                }
                else
                    pBuffer.Write(false);
            }
        }

        public void ReadComplexValues(BitBuffer pBuffer)
        {
            if (!this.complexSync)
                return;
            ComplexDiffData complexDiffData;
            if (!AmmoType._complexDiff.TryGetValue(this.GetType(), out complexDiffData))
                AmmoType._complexDiff[this.GetType()] = complexDiffData = new AmmoType.ComplexDiffData(this.GetType());
            foreach (ClassMember binding in complexDiffData.bindings)
            {
                if (pBuffer.ReadBool())
                    binding.SetValue(this, pBuffer.Read(binding.type));
            }
        }

        public virtual void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
        }

        public virtual void WriteAdditionalData(BitBuffer b) => this.WriteComplexValues(b);

        public virtual void ReadAdditionalData(BitBuffer b) => this.ReadComplexValues(b);

        public virtual void PopShell(float x, float y, int dir)
        {
        }

        public Bullet GetBullet(
          float x,
          float y,
          Thing owner = null,
          float angle = -1f,
          Thing firedFrom = null,
          float distance = -1f,
          bool tracer = false,
          bool network = true)
        {
            angle *= -1f;
            Bullet bullet;
            if (this.bulletType == typeof(Bullet))
                bullet = new Bullet(x, y, this, angle, owner, this.rebound, distance, tracer, network);
            else
                bullet = Activator.CreateInstance(this.bulletType, x, y, this, angle, owner, rebound, distance, tracer, network) as Bullet;
            bullet.firedFrom = firedFrom;
            bullet.color = this.bulletColor;
            return bullet;
        }

        public virtual Bullet FireBullet(
          Vec2 position,
          Thing owner = null,
          float angle = 0f,
          Thing firedFrom = null)
        {
            Bullet bullet = this.GetBullet(position.x, position.y, owner, angle, firedFrom);
            Level.current.AddThing(bullet);
            return bullet;
        }

        public virtual void OnHit(bool destroyed, Bullet b)
        {
        }

        private class ComplexDiffData
        {
            public AmmoType original;
            public List<ClassMember> bindings;

            public ComplexDiffData(System.Type pType)
            {
                this.original = Activator.CreateInstance(pType) as AmmoType;
                List<ClassMember> members = Editor.GetMembers(pType);
                this.bindings = new List<ClassMember>();
                foreach (ClassMember classMember in members)
                {
                    if (!(classMember.name == "complexSync") && (classMember.type.IsPrimitive || classMember.type == typeof(Vec2) || classMember.type == typeof(Color)))
                        this.bindings.Add(classMember);
                }
            }
        }
    }
}
