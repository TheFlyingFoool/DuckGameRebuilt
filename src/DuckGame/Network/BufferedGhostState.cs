// Decompiled with JetBrains decompiler
// Type: DuckGame.BufferedGhostState
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class BufferedGhostState
    {
        public BufferedGhostState previousState;
        public BufferedGhostState nextState;
        public List<ushort> inputStates = new List<ushort>();
        public List<BufferedGhostProperty> properties = new List<BufferedGhostProperty>(30);
        public long mask;
        public NetIndex16 tick = (NetIndex16)0;
        public NetIndex8 authority = (NetIndex8)0;
        public int _framesApplied;

        public override string ToString() => "Tick: " + tick.ToString() + " FA: " + _framesApplied.ToString() + " Mask: " + mask.ToString();

        public Thing owner => properties.Count <= 0 ? null : properties[0].binding.owner as Thing;

        public BufferedGhostState()
        {
            for (int index = 0; index < NetworkConnection.packetsEvery; ++index)
                inputStates.Add(0);
        }

        ~BufferedGhostState()
        {
        }

        public void ReInitialize()
        {
            properties.Clear();
            _framesApplied = 0;
        }

        public void Reset(bool clearProperties = true)
        {
            if (clearProperties)
                properties.Clear();
            _framesApplied = 0;
        }

        public void Apply(float lerp, BufferedGhostState updateNetworkState, bool pApplyPosition = true)
        {
            foreach (BufferedGhostProperty property in properties)
            {
                try
                {
                    if (property.isNetworkStateValue)
                        property.Apply(1f);
                    else if (!(updateNetworkState.properties[property.index].tick > property.tick))
                    {
                        if (!pApplyPosition)
                            property.Apply(0f);
                        else
                            property.Apply(_framesApplied >= NetworkConnection.packetsEvery ? 1f : NetworkConnection.ghostLerpDivisor);
                        updateNetworkState.properties[property.index].UpdateFrom(property);
                    }
                }
                catch (Exception ex)
                {
                    System_ApplyException(ex, property, updateNetworkState);
                }
            }
            ++_framesApplied;
        }

        private void System_ApplyException(
          Exception e,
          BufferedGhostProperty prop,
          BufferedGhostState updateNetworkState = null)
        {
            string str = "";
            if (GhostObject.applyContext != null)
            {
                if (prop == null)
                    str += GhostObject.applyContext.thing.GetType().Name;
                else if (updateNetworkState == null)
                    str = str + GhostObject.applyContext.thing.GetType().Name + "." + prop.binding.name + "=" + (prop.value != null ? prop.value.ToString() : "null");
                else
                    str = str + GhostObject.applyContext.thing.GetType().Name + "." + prop.binding.name + "=" + (prop.value != null ? prop.value.ToString() : "null") + "(" + prop.index.ToString() + "/" + updateNetworkState.properties.Count.ToString() + ")";
            }
            DevConsole.LogComplexMessage("Error applying BufferedGhostProperty (" + str + "): " + e.Message, Color.Red);
        }

        public void ApplyImmediately(long pMask, BufferedGhostState updateNetworkState)
        {
            foreach (BufferedGhostProperty property in properties)
            {
                if (!property.isNetworkStateValue && !(updateNetworkState.properties[property.index].tick > property.tick))
                {
                    long num = 1L << property.index;
                    if ((pMask & num) != 0L)
                    {
                        try
                        {
                            property.Apply(1f);
                            updateNetworkState.properties[property.index].UpdateFrom(property.binding);
                        }
                        catch (Exception ex)
                        {
                            System_ApplyException(ex, property, updateNetworkState);
                        }
                    }
                }
            }
            _framesApplied = NetworkConnection.packetsEvery;
        }

        public void ApplyImmediately(BufferedGhostState updateNetworkState)
        {
            foreach (BufferedGhostProperty property in properties)
            {
                if (!property.isNetworkStateValue)
                {
                    if (!(updateNetworkState.properties[property.index].tick > property.tick))
                    {
                        try
                        {
                            property.Apply(1f);
                            updateNetworkState.properties[property.index].UpdateFrom(property);
                        }
                        catch (Exception ex)
                        {
                            System_ApplyException(ex, property, updateNetworkState);
                        }
                    }
                }
            }
            _framesApplied = NetworkConnection.packetsEvery;
        }

        public void ApplyImmediately()
        {
            foreach (BufferedGhostProperty property in properties)
            {
                try
                {
                    property.Apply(1f);
                }
                catch (Exception ex)
                {
                    System_ApplyException(ex, property);
                }
            }
            _framesApplied = NetworkConnection.packetsEvery;
        }

        //private Vec2 Slerp(Vec2 from, Vec2 to, float step)
        //{
        //    if (step == 0f)
        //        return from;
        //    if (from == to || step == 1f)
        //        return to;
        //    double a = Math.Acos(Vec2.Dot(from, to));
        //    if (a == 0f)
        //        return to;
        //    double num = Math.Sin(a);
        //    return (float)(Math.Sin((1f - step) * a) / num) * from + (float)(Math.Sin(step * a) / num) * to;
        //}
    }
}
