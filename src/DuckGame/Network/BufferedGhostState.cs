// Decompiled with JetBrains decompiler
// Type: DuckGame.BufferedGhostState
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public override string ToString() => "Tick: " + this.tick.ToString() + " FA: " + this._framesApplied.ToString() + " Mask: " + this.mask.ToString();

        public Thing owner => this.properties.Count <= 0 ? null : this.properties[0].binding.owner as Thing;

        public BufferedGhostState()
        {
            for (int index = 0; index < NetworkConnection.packetsEvery; ++index)
                this.inputStates.Add(0);
        }

        ~BufferedGhostState()
        {
        }

        public void ReInitialize()
        {
            this.properties.Clear();
            this._framesApplied = 0;
        }

        public void Reset(bool clearProperties = true)
        {
            if (clearProperties)
                this.properties.Clear();
            this._framesApplied = 0;
        }

        public void Apply(float lerp, BufferedGhostState updateNetworkState, bool pApplyPosition = true)
        {
            foreach (BufferedGhostProperty property in this.properties)
            {
                try
                {
                    if (property.isNetworkStateValue)
                        property.Apply(1f);
                    else if (!(updateNetworkState.properties[property.index].tick > property.tick))
                    {
                        if (!pApplyPosition)
                            property.Apply(0.0f);
                        else
                            property.Apply(this._framesApplied >= NetworkConnection.packetsEvery ? 1f : NetworkConnection.ghostLerpDivisor);
                        updateNetworkState.properties[property.index].UpdateFrom(property);
                    }
                }
                catch (Exception ex)
                {
                    this.System_ApplyException(ex, property, updateNetworkState);
                }
            }
            ++this._framesApplied;
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
            foreach (BufferedGhostProperty property in this.properties)
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
                            this.System_ApplyException(ex, property, updateNetworkState);
                        }
                    }
                }
            }
            this._framesApplied = NetworkConnection.packetsEvery;
        }

        public void ApplyImmediately(BufferedGhostState updateNetworkState)
        {
            foreach (BufferedGhostProperty property in this.properties)
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
                            this.System_ApplyException(ex, property, updateNetworkState);
                        }
                    }
                }
            }
            this._framesApplied = NetworkConnection.packetsEvery;
        }

        public void ApplyImmediately()
        {
            foreach (BufferedGhostProperty property in this.properties)
            {
                try
                {
                    property.Apply(1f);
                }
                catch (Exception ex)
                {
                    this.System_ApplyException(ex, property);
                }
            }
            this._framesApplied = NetworkConnection.packetsEvery;
        }

        private Vec2 Slerp(Vec2 from, Vec2 to, float step)
        {
            if ((double)step == 0.0)
                return from;
            if (from == to || (double)step == 1.0)
                return to;
            double a = Math.Acos((double)Vec2.Dot(from, to));
            if (a == 0.0)
                return to;
            double num = Math.Sin(a);
            return (float)(Math.Sin((1.0 - (double)step) * a) / num) * from + (float)(Math.Sin((double)step * a) / num) * to;
        }
    }
}
