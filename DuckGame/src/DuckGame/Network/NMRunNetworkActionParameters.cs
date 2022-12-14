// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRunNetworkActionParameters
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            target = pTarget;
            _method = pMethod;
            _parameters = pParameters;
            if (pMethod.GetParameters().Count() != pParameters.Count())
                throw new Exception("NMRunNetworkActionParameters.pParameters.Count() != MethodInfo.GetParameters().Count(). Are you including the correct parameters in your SyncNetworkAction call?");
        }

        public NMRunNetworkActionParameters()
        {
        }

        protected override void OnSerialize()
        {
            BitBuffer val = new BitBuffer();
            val.Write(target);
            val.Write(Editor.NetworkActionIndex(target.GetType(), _method));
            for (int index = 0; index < _parameters.Count(); ++index)
                val.Write(_parameters[index]);
            _serializedData.Write(val, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            BitBuffer bitBuffer = d.ReadBitBuffer();
            target = bitBuffer.Read<PhysicsObject>();
            if (target == null)
                return;
            _method = Editor.MethodFromNetworkActionIndex(target.GetType(), bitBuffer.ReadByte());
            if (!(_method != null))
                return;
            List<object> objectList = new List<object>();
            foreach (ParameterInfo parameter in _method.GetParameters())
                objectList.Add(bitBuffer.Read(parameter.ParameterType));
            _parameters = objectList.ToArray();
        }

        public override void Activate()
        {
            if (target == null || !(_method != null))
                return;
            _method.Invoke(target, _parameters);
        }
    }
}
