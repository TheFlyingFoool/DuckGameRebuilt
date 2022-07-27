// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockTree
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UnlockTree : Thing
    {
        private SpriteMap _box;
        private Sprite _lock;
        private SpriteMap _icons;
        private int _topLayer;
        private int _desiredLayer;
        private float _layerScroll;
        private UnlockData _selected;
        private UnlockScreen _screen;

        public UnlockData selected
        {
            get => this._selected;
            set => this._selected = value;
        }

        public UnlockTree(UnlockScreen screen, Layer putLayer)
          : base()
        {
            this._box = new SpriteMap("arcade/unlockBox", 39, 40);
            this._box.CenterOrigin();
            this._icons = new SpriteMap("arcade/unlockIcons", 25, 25);
            this._icons.CenterOrigin();
            this._lock = new Sprite("arcade/unlockLock");
            this._lock.CenterOrigin();
            this._screen = screen;
            this.layer = putLayer;
        }

        public override void Initialize()
        {
            this._selected = Unlocks.unlocks.FirstOrDefault<UnlockData>();
            this._screen.ChangeSpeech();
        }

        public override void Update()
        {
            if ((double)this.alpha < 0.00999999977648258)
                return;
            Duck duck = Level.First<Duck>();
            InputProfile inputProfile = InputProfile.DefaultPlayer1;
            if (duck != null)
                inputProfile = duck.inputProfile;
            UnlockData selected = this._selected;
            List<UnlockData> treeLayer = Unlocks.GetTreeLayer(this._selected.layer);
            if (inputProfile.Pressed("MENULEFT"))
            {
                UnlockData unlockData1 = null;
                foreach (UnlockData unlockData2 in treeLayer)
                {
                    if (unlockData2 != this._selected)
                        unlockData1 = unlockData2;
                    else
                        break;
                }
                if (unlockData1 != null)
                {
                    this._selected = unlockData1;
                    this._screen.ChangeSpeech();
                    SFX.Play("menuBlip01");
                }
            }
            else if (inputProfile.Pressed("MENURIGHT"))
            {
                UnlockData unlockData3 = null;
                UnlockData unlockData4 = null;
                foreach (UnlockData unlockData5 in treeLayer)
                {
                    if (unlockData3 == this._selected)
                    {
                        unlockData4 = unlockData5;
                        break;
                    }
                    unlockData3 = unlockData5;
                }
                if (unlockData4 != null)
                {
                    this._selected = unlockData4;
                    this._screen.ChangeSpeech();
                    SFX.Play("menuBlip01");
                }
            }
            else if (inputProfile.Pressed("MENUUP"))
            {
                if (this._selected.parent != null)
                {
                    UnlockData parent = this._selected.parent;
                    if (parent != this._selected)
                    {
                        SFX.Play("menuBlip01");
                        this._selected = parent;
                        this._screen.ChangeSpeech();
                    }
                }
            }
            else if (inputProfile.Pressed("MENUDOWN"))
            {
                bool flag = false;
                if (this._selected.children.Count > 0)
                {
                    this._selected = this._selected.children[0];
                    flag = true;
                }
                else if (this._selected.parent != null)
                {
                    foreach (UnlockData child in this._selected.parent.children)
                    {
                        if (child != this._selected && child.children.Count > 0)
                        {
                            this._selected = child.children[0];
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    this._screen.ChangeSpeech();
                    SFX.Play("menuBlip01");
                }
            }
            else if (inputProfile.Pressed("SELECT") && !this._selected.ProfileUnlocked(Profiles.active[0]))
            {
                bool flag = false;
                if (this._selected.parent != null)
                {
                    foreach (UnlockData unlockData in Unlocks.GetTreeLayer(this._selected.parent.layer))
                    {
                        if (unlockData.children.Contains(this._selected) && !unlockData.ProfileUnlocked(Profiles.active[0]))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (Profiles.active[0].ticketCount >= this._selected.cost && (this._selected.parent == null || !flag))
                    this._screen.OpenBuyConfirmation(this._selected);
                else
                    SFX.Play("consoleError");
            }
            if (this._selected != selected)
            {
                this._desiredLayer = this._selected.layer;
                this._screen.SelectionChanged();
            }
            if (this._desiredLayer == this._topLayer)
                return;
            if (this._desiredLayer < this._topLayer)
            {
                this._layerScroll -= 0.1f;
                if (_layerScroll > -1.0)
                    return;
                this._layerScroll = 0.0f;
                --this._topLayer;
            }
            else
            {
                if (this._desiredLayer <= this._topLayer + 1)
                    return;
                this._layerScroll += 0.1f;
                if (_layerScroll < 1.0)
                    return;
                this._layerScroll = 0.0f;
                ++this._topLayer;
            }
        }

        public override void Draw()
        {
            if ((double)this.alpha < 0.00999999977648258)
                return;
            int num1;
            if (this._desiredLayer < this._topLayer)
                num1 = this._desiredLayer;
            int num2 = 3;
            if (this._desiredLayer > 1 || this._topLayer > 0)
                num2 = 4;
            int layer = this._topLayer - 1;
            if (layer < 0)
                layer = 0;
            List<UnlockData> treeLayer = Unlocks.GetTreeLayer(layer);
            float num3 = 0.0f;
            if (layer < this._topLayer)
                num3 += (this._topLayer - layer) * 60f;
            Vec2 vec2_1 = new Vec2(50f, (float)(45.0 - _layerScroll * 67.0) - num3);
            Vec2 vec2_2 = new Vec2(Layer.HUD.width - 180f, 100f);
            List<UnlockData> collection = new List<UnlockData>();
            int num4 = 0;
            for (int index1 = 0; index1 < treeLayer.Count && num4 < num2; ++index1)
            {
                UnlockData unlockData1 = treeLayer[index1];
                float num5 = 1f;
                if (unlockData1 != this._selected)
                    num5 = 0.5f;
                Color color1 = Color.Green;
                if (!unlockData1.ProfileUnlocked(Profiles.active[0]))
                    color1 = Color.DarkRed;
                bool flag = true;
                if (!unlockData1.AllParentsUnlocked(Profiles.active[0]))
                {
                    flag = false;
                    color1 = new Color(40, 40, 40);
                    num5 = unlockData1 == this._selected ? 0.8f : 0.2f;
                }
                color1 = new Color((byte)(color1.r * (double)num5), (byte)(color1.g * (double)num5), (byte)(color1.b * (double)num5));
                float num6 = treeLayer.Count != 1 ? (treeLayer.Count != 2 ? index1 * (vec2_2.x / (treeLayer.Count - 1)) : (float)(vec2_2.x / 2.0 - vec2_2.x / 4.0 + index1 * (vec2_2.x / 2.0))) : vec2_2.x / 2f;
                Vec2 p1 = new Vec2(vec2_1.x + num6, vec2_1.y + num4 * 60);
                this._box.depth = (Depth)0.1f;
                this._box.frame = 2;
                this._box.alpha = this.alpha;
                this._box.color = color1;
                Graphics.Draw(_box, p1.x, p1.y);
                this._box.depth = (Depth)0.2f;
                this._box.frame = 1;
                this._box.color = new Color(num5, num5, num5);
                Graphics.Draw(_box, p1.x, p1.y);
                if (unlockData1.icon != -1)
                {
                    this._icons.depth = (Depth)0.2f;
                    this._icons.frame = flag ? unlockData1.icon : 25;
                    this._icons.color = new Color(num5, num5, num5);
                    this._icons.alpha = this.alpha;
                    Graphics.Draw(_icons, p1.x - 1f, p1.y - 1f);
                }
                if (unlockData1 == this._selected)
                {
                    this._box.frame = 0;
                    Graphics.Draw(_box, p1.x, p1.y);
                }
                foreach (UnlockData child in unlockData1.children)
                {
                    if (!collection.Contains(child))
                        collection.Add(child);
                }
                if (index1 == treeLayer.Count - 1 && collection.Count > 0)
                {
                    for (int index2 = 0; index2 < treeLayer.Count; ++index2)
                    {
                        UnlockData unlockData2 = treeLayer[index2];
                        if (unlockData2.children.Count > 0)
                        {
                            float num7 = 1f;
                            if (unlockData2 != this._selected)
                                num7 = 0.5f;
                            Color color2 = Color.Green;
                            if (!unlockData2.ProfileUnlocked(Profiles.active[0]))
                                color2 = Color.DarkRed;
                            if (!unlockData2.AllParentsUnlocked(Profiles.active[0]))
                                color2 = new Color(90, 90, 90);
                            color2 = new Color((byte)(color2.r * (double)num7), (byte)(color2.g * (double)num7), (byte)(color2.b * (double)num7));
                            float num8 = treeLayer.Count != 1 ? (treeLayer.Count != 2 ? index2 * (vec2_2.x / (treeLayer.Count - 1)) : (float)(vec2_2.x / 2.0 - vec2_2.x / 4.0 + index2 * (vec2_2.x / 2.0))) : vec2_2.x / 2f;
                            p1 = new Vec2(vec2_1.x + num8, vec2_1.y + num4 * 60);
                            Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 30f), color2 * this.alpha, 6f, - 0.2f);
                            Color color3 = new Color(50, 50, 50);
                            if (!unlockData2.ProfileUnlocked(Profiles.active[0]))
                            {
                                this._lock.depth = (Depth)0.5f;
                                this._lock.alpha = this.alpha;
                                Graphics.Draw(this._lock, p1.x, p1.y + 30f);
                            }
                            else
                                color3 = Color.Green;
                            color3 = new Color((byte)(color3.r * (double)num7), (byte)(color3.g * (double)num7), (byte)(color3.b * (double)num7));
                            for (int index3 = 0; index3 < collection.Count; ++index3)
                            {
                                UnlockData unlockData3 = collection[index3];
                                if (unlockData2.children.Contains(unlockData3))
                                {
                                    float num9 = collection.Count != 1 ? (collection.Count != 2 ? index3 * (vec2_2.x / (collection.Count - 1)) : (float)(vec2_2.x / 2.0 - vec2_2.x / 4.0 + index3 * (vec2_2.x / 2.0))) : vec2_2.x / 2f;
                                    Vec2 vec2_3 = new Vec2(vec2_1.x + num9, vec2_1.y + (num4 + 1) * 60);
                                    float num10 = 0.0f;
                                    if (vec2_3.x < (double)p1.x)
                                        num10 = -3f;
                                    else if (vec2_3.x > (double)p1.x)
                                        num10 = 3f;
                                    float num11 = 0.0f;
                                    if (vec2_3.x < (double)p1.x)
                                        num11 = 3f;
                                    else if (vec2_3.x > (double)p1.x)
                                        num11 = -3f;
                                    Graphics.DrawLine(new Vec2(vec2_3.x + num10, p1.y + 30f), new Vec2(p1.x + num11, p1.y + 30f), color3 * this.alpha, 6f, (Depth)(float)((this._selected == unlockData2 ? 0.100000001490116 : 0.0) - 0.200000002980232));
                                    Graphics.DrawLine(new Vec2(vec2_3.x, p1.y + 30f), new Vec2(vec2_3.x, vec2_3.y), color3 * this.alpha, 6f, (Depth)(float)((this._selected == unlockData2 ? 0.100000001490116 : 0.0) - 0.200000002980232));
                                }
                            }
                        }
                    }
                    treeLayer.Clear();
                    treeLayer.AddRange(collection);
                    collection.Clear();
                    ++num4;
                    index1 = -1;
                }
            }
        }
    }
}
