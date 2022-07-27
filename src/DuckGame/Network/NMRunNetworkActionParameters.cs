// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRunNetworkActionParameters
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    public class NMRunNetworkActionParameters : NMEvent
    {
        public PhysicsObject target;
        private MethodInfo _method;
        private object[] _parameters;

        public NMRunNetworkActionParameters(
          PhysicsObject pTarget,
          MethodInfo pMethod,
          object[] pParameters)
        {
            this.target = pTarget;
            this._method = pMethod;
            this._parameters = pParameters;
            if (pMethod.GetParameters().Count<ParameterInfo>() != pParameters.Count<object>())
                throw new Exception("NMRunNetworkActionParameters.pParameters.Count() != MethodInfo.GetParameters().Count(). Are you including the correct parameters in your SyncNetworkAction call?");
        }

        public NMRunNetworkActionParameters()
        {
        }

        protected override void OnSerialize()
        {
            BitBuffer val = new BitBuffer();
            val.Write(target);
            val.Write(Editor.NetworkActionIndex(this.target.GetType(), this._method));
            for (int index = 0; index < _parameters.Count<object>(); ++index)
                val.Write(this._parameters[index]);
            this._serializedData.Write(val, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            BitBuffer bitBuffer = d.ReadBitBuffer();
            this.target = bitBuffer.Read<PhysicsObject>();
            if (this.target == null)
                return;
            this._method = Editor.MethodFromNetworkActionIndex(this.target.GetType(), bitBuffer.ReadByte());
            if (!(this._method != null))
                return;
            List<object> objectList = new List<object>();
            foreach (ParameterInfo parameter in this._method.GetParameters())
                objectList.Add(bitBuffer.Read(parameter.ParameterType));
            this._parameters = objectList.ToArray();
        }

        public override void Activate()
        {
            if (this.target == null || !(this._method != null))
                return;
            this._method.Invoke(target, this._parameters);
        }
    }
}
