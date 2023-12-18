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
            get => _selected;
            set => _selected = value;
        }

        public UnlockTree(UnlockScreen screen, Layer putLayer)
          : base()
        {
            _box = new SpriteMap("arcade/unlockBox", 39, 40);
            _box.CenterOrigin();
            _icons = new SpriteMap("arcade/unlockIcons", 25, 25);
            _icons.CenterOrigin();
            _lock = new Sprite("arcade/unlockLock");
            _lock.CenterOrigin();
            _screen = screen;
            layer = putLayer;
        }

        public override void Initialize()
        {
            _selected = Unlocks.unlocks.FirstOrDefault();
            _screen.ChangeSpeech();
        }

        public override void Update()
        {
            if (alpha < 0.01f)
                return;
            Duck duck = Level.First<Duck>();
            InputProfile inputProfile = InputProfile.DefaultPlayer1;
            if (duck != null)
                inputProfile = duck.inputProfile;
            UnlockData selected = _selected;
            List<UnlockData> treeLayer = Unlocks.GetTreeLayer(_selected.layer);
            if (inputProfile.Pressed(Triggers.MenuLeft))
            {
                UnlockData unlockData1 = null;
                foreach (UnlockData unlockData2 in treeLayer)
                {
                    if (unlockData2 != _selected)
                        unlockData1 = unlockData2;
                    else
                        break;
                }
                if (unlockData1 != null)
                {
                    _selected = unlockData1;
                    _screen.ChangeSpeech();
                    SFX.Play("menuBlip01");
                }
            }
            else if (inputProfile.Pressed(Triggers.MenuRight))
            {
                UnlockData unlockData3 = null;
                UnlockData unlockData4 = null;
                foreach (UnlockData unlockData5 in treeLayer)
                {
                    if (unlockData3 == _selected)
                    {
                        unlockData4 = unlockData5;
                        break;
                    }
                    unlockData3 = unlockData5;
                }
                if (unlockData4 != null)
                {
                    _selected = unlockData4;
                    _screen.ChangeSpeech();
                    SFX.Play("menuBlip01");
                }
            }
            else if (inputProfile.Pressed(Triggers.MenuUp))
            {
                if (_selected.parent != null)
                {
                    UnlockData parent = _selected.parent;
                    if (parent != _selected)
                    {
                        SFX.Play("menuBlip01");
                        _selected = parent;
                        _screen.ChangeSpeech();
                    }
                }
            }
            else if (inputProfile.Pressed(Triggers.MenuDown))
            {
                bool flag = false;
                if (_selected.children.Count > 0)
                {
                    _selected = _selected.children[0];
                    flag = true;
                }
                else if (_selected.parent != null)
                {
                    foreach (UnlockData child in _selected.parent.children)
                    {
                        if (child != _selected && child.children.Count > 0)
                        {
                            _selected = child.children[0];
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    _screen.ChangeSpeech();
                    SFX.Play("menuBlip01");
                }
            }
            else if (inputProfile.Pressed(Triggers.Select) && !_selected.ProfileUnlocked(Profiles.active[0]))
            {
                bool flag = false;
                if (_selected.parent != null)
                {
                    foreach (UnlockData unlockData in Unlocks.GetTreeLayer(_selected.parent.layer))
                    {
                        if (unlockData.children.Contains(_selected) && !unlockData.ProfileUnlocked(Profiles.active[0]))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (Profiles.active[0].ticketCount >= _selected.cost && (_selected.parent == null || !flag))
                    _screen.OpenBuyConfirmation(_selected);
                else
                    SFX.Play("consoleError");
            }
            if (_selected != selected)
            {
                _desiredLayer = _selected.layer;
                _screen.SelectionChanged();
            }
            if (_desiredLayer == _topLayer)
                return;
            if (_desiredLayer < _topLayer)
            {
                _layerScroll -= 0.1f;
                if (_layerScroll > -1f)
                    return;
                _layerScroll = 0f;
                --_topLayer;
            }
            else
            {
                if (_desiredLayer <= _topLayer + 1)
                    return;
                _layerScroll += 0.1f;
                if (_layerScroll < 1f)
                    return;
                _layerScroll = 0f;
                ++_topLayer;
            }
        }

        public override void Draw()
        {
            if (alpha < 0.01f)
                return;
            int num1;
            if (_desiredLayer < _topLayer)
                num1 = _desiredLayer;
            int num2 = 3;
            if (_desiredLayer > 1 || _topLayer > 0)
                num2 = 4;
            int layer = _topLayer - 1;
            if (layer < 0)
                layer = 0;
            List<UnlockData> treeLayer = Unlocks.GetTreeLayer(layer);
            float num3 = 0f;
            if (layer < _topLayer)
                num3 += (_topLayer - layer) * 60f;
            Vec2 vec2_1 = new Vec2(50f, (float)(45f - _layerScroll * 67f) - num3);
            Vec2 vec2_2 = new Vec2(Layer.HUD.width - 180f, 100f);
            List<UnlockData> collection = new List<UnlockData>();
            int num4 = 0;
            for (int index1 = 0; index1 < treeLayer.Count && num4 < num2; ++index1)
            {
                UnlockData unlockData1 = treeLayer[index1];
                float num5 = 1f;
                if (unlockData1 != _selected)
                    num5 = 0.5f;
                Color color1 = Color.Green;
                if (!unlockData1.ProfileUnlocked(Profiles.active[0]))
                    color1 = Color.DarkRed;
                bool flag = true;
                if (!unlockData1.AllParentsUnlocked(Profiles.active[0]))
                {
                    flag = false;
                    color1 = new Color(40, 40, 40);
                    num5 = unlockData1 == _selected ? 0.8f : 0.2f;
                }
                color1 = new Color((byte)(color1.r * num5), (byte)(color1.g * num5), (byte)(color1.b * num5));
                float num6 = treeLayer.Count != 1 ? (treeLayer.Count != 2 ? index1 * (vec2_2.x / (treeLayer.Count - 1)) : (float)(vec2_2.x / 2f - vec2_2.x / 4f + index1 * (vec2_2.x / 2f))) : vec2_2.x / 2f;
                Vec2 p1 = new Vec2(vec2_1.x + num6, vec2_1.y + num4 * 60);
                _box.depth = (Depth)0.1f;
                _box.frame = 2;
                _box.alpha = alpha;
                _box.color = color1;
                Graphics.Draw(_box, p1.x, p1.y);
                _box.depth = (Depth)0.2f;
                _box.frame = 1;
                _box.color = new Color(num5, num5, num5);
                Graphics.Draw(_box, p1.x, p1.y);
                if (unlockData1.icon != -1)
                {
                    _icons.depth = (Depth)0.2f;
                    _icons.frame = flag ? unlockData1.icon : 25;
                    _icons.color = new Color(num5, num5, num5);
                    _icons.alpha = alpha;
                    Graphics.Draw(_icons, p1.x - 1f, p1.y - 1f);
                }
                if (unlockData1 == _selected)
                {
                    _box.frame = 0;
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
                            if (unlockData2 != _selected)
                                num7 = 0.5f;
                            Color color2 = Color.Green;
                            if (!unlockData2.ProfileUnlocked(Profiles.active[0]))
                                color2 = Color.DarkRed;
                            if (!unlockData2.AllParentsUnlocked(Profiles.active[0]))
                                color2 = new Color(90, 90, 90);
                            color2 = new Color((byte)(color2.r * num7), (byte)(color2.g * num7), (byte)(color2.b * num7));
                            float num8 = treeLayer.Count != 1 ? (treeLayer.Count != 2 ? index2 * (vec2_2.x / (treeLayer.Count - 1)) : (float)(vec2_2.x / 2f - vec2_2.x / 4f + index2 * (vec2_2.x / 2f))) : vec2_2.x / 2f;
                            p1 = new Vec2(vec2_1.x + num8, vec2_1.y + num4 * 60);
                            Graphics.DrawLine(p1, p1 + new Vec2(0f, 30f), color2 * alpha, 6f, -0.2f);
                            Color color3 = new Color(50, 50, 50);
                            if (!unlockData2.ProfileUnlocked(Profiles.active[0]))
                            {
                                _lock.depth = (Depth)0.5f;
                                _lock.alpha = alpha;
                                Graphics.Draw(_lock, p1.x, p1.y + 30f);
                            }
                            else
                                color3 = Color.Green;
                            color3 = new Color((byte)(color3.r * num7), (byte)(color3.g * num7), (byte)(color3.b * num7));
                            for (int index3 = 0; index3 < collection.Count; ++index3)
                            {
                                UnlockData unlockData3 = collection[index3];
                                if (unlockData2.children.Contains(unlockData3))
                                {
                                    float num9 = collection.Count != 1 ? (collection.Count != 2 ? index3 * (vec2_2.x / (collection.Count - 1)) : (float)(vec2_2.x / 2f - vec2_2.x / 4f + index3 * (vec2_2.x / 2f))) : vec2_2.x / 2f;
                                    Vec2 vec2_3 = new Vec2(vec2_1.x + num9, vec2_1.y + (num4 + 1) * 60);
                                    float num10 = 0f;
                                    if (vec2_3.x < p1.x)
                                        num10 = -3f;
                                    else if (vec2_3.x > p1.x)
                                        num10 = 3f;
                                    float num11 = 0f;
                                    if (vec2_3.x < p1.x)
                                        num11 = 3f;
                                    else if (vec2_3.x > p1.x)
                                        num11 = -3f;
                                    Graphics.DrawLine(new Vec2(vec2_3.x + num10, p1.y + 30f), new Vec2(p1.x + num11, p1.y + 30f), color3 * alpha, 6f, (Depth)(float)((_selected == unlockData2 ? 0.1f : 0.0f) - 0.2f));
                                    Graphics.DrawLine(new Vec2(vec2_3.x, p1.y + 30f), new Vec2(vec2_3.x, vec2_3.y), color3 * alpha, 6f, (Depth)(float)((_selected == unlockData2 ? 0.1f : 0.0f) - 0.2f));
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
