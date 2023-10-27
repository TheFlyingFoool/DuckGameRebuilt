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
            if (pMethod.GetParameters().Length != pParameters.Length)
                throw new Exception("NMRunNetworkActionParameters.pParameters.Length != MethodInfo.GetParameters().Length. Are you including the correct parameters in your SyncNetworkAction call?");
        }

        public NMRunNetworkActionParameters()
        {
        }

        protected override void OnSerialize()
        {
            BitBuffer val = new BitBuffer();
            val.Write(target);
            val.Write(Editor.NetworkActionIndex(target.GetType(), _method));
            for (int index = 0; index < _parameters.Length; ++index)
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
