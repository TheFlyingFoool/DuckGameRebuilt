// Decompiled with JetBrains decompiler
// Type: DuckGame.RecorderFrame
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public struct RecorderFrame
    {
        private static int kMaxObjects = 1200;
        public RecorderFrameItem[] objects;
        public Dictionary<long, RecorderFrameItem> sortedObjects;
        public int currentObject;
        public Dictionary<int, RecorderFrameStateChange> _states;
        public List<RecorderSoundItem> sounds;
        public Color backgroundColor;
        public float totalVelocity;
        public byte deaths;
        public byte actions;
        public byte bonus;
        public byte coolness;

        public void Initialize()
        {
            this.currentObject = 0;
            this.objects = new RecorderFrameItem[RecorderFrame.kMaxObjects];
            this._states = new Dictionary<int, RecorderFrameStateChange>();
            this.sortedObjects = new Dictionary<long, RecorderFrameItem>();
            this.sounds = new List<RecorderSoundItem>();
            this.backgroundColor = Color.White;
        }

        public void Reset()
        {
            this.currentObject = 0;
            this.totalVelocity = 0.0f;
            this.actions = (byte)0;
            this.bonus = (byte)0;
            this.deaths = (byte)0;
            this.coolness = (byte)0;
            this._states.Clear();
            this.sounds.Clear();
            this.sortedObjects.Clear();
        }

        public RecorderFrameStateChange GetStateWithIndex(int index) => this._states.FirstOrDefault<KeyValuePair<int, RecorderFrameStateChange>>((Func<KeyValuePair<int, RecorderFrameStateChange>, bool>)(x => x.Value.stateIndex == index)).Value;

        public bool HasStateWithIndex(int index) => this._states.Where<KeyValuePair<int, RecorderFrameStateChange>>((Func<KeyValuePair<int, RecorderFrameStateChange>, bool>)(x => x.Value.stateIndex == index)).Count<KeyValuePair<int, RecorderFrameStateChange>>() > 0;

        public void StateChange(
          SpriteSortMode sortModeVal,
          BlendState blendStateVal,
          SamplerState samplerStateVal,
          DepthStencilState depthStencilStateVal,
          RasterizerState rasterizerStateVal,
          MTEffect effectVal,
          Matrix cameraVal,
          Rectangle sciss)
        {
            this._states[this.currentObject] = new RecorderFrameStateChange()
            {
                sortMode = sortModeVal,
                blendState = blendStateVal,
                samplerState = samplerStateVal,
                depthStencilState = depthStencilStateVal,
                rasterizerState = rasterizerStateVal,
                effectIndex = effectVal != null ? effectVal.effectIndex : (short)-1,
                camera = cameraVal,
                stateIndex = DuckGame.Graphics.currentStateIndex,
                scissor = sciss
            };
        }

        public void IncrementObject()
        {
            ++this.currentObject;
            if (this.currentObject < RecorderFrame.kMaxObjects)
                return;
            this.currentObject = RecorderFrame.kMaxObjects - 1;
        }

        public void Render()
        {
            bool flag = false;
            DuckGame.Graphics.Clear(this.backgroundColor * DuckGame.Graphics.fade);
            for (int key = 0; key < this.currentObject; ++key)
            {
                if (this._states.ContainsKey(key))
                {
                    if (flag)
                        DuckGame.Graphics.screen.End();
                    RecorderFrameStateChange state = this._states[key];
                    flag = true;
                    MTEffect mtEffectFromIndex = Content.GetMTEffectFromIndex(state.effectIndex);
                    if (Layer.IsBasicLayerEffect(mtEffectFromIndex))
                    {
                        mtEffectFromIndex.effect.Parameters["fade"].SetValue((Vector3)new Vec3(DuckGame.Graphics.fade));
                        mtEffectFromIndex.effect.Parameters["add"].SetValue((Vector3)new Vec3(DuckGame.Graphics.fadeAddRenderValue));
                    }
                    DuckGame.Graphics.screen.Begin(state.sortMode, state.blendState, state.samplerState, state.depthStencilState, state.rasterizerState, Content.GetMTEffectFromIndex(state.effectIndex), state.camera);
                    DuckGame.Graphics.SetScissorRectangle(state.scissor);
                }
                DuckGame.Graphics.DrawRecorderItem(ref this.objects[key]);
            }
            if (!flag)
                return;
            DuckGame.Graphics.screen.End();
        }

        public void Update()
        {
            foreach (RecorderSoundItem sound in this.sounds)
                SFX.Play(sound.sound, sound.volume, sound.pitch, sound.pan);
        }
    }
}
