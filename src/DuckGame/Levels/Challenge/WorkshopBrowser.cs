// Decompiled with JetBrains decompiler
// Type: DuckGame.WorkshopBrowser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DuckGame
{
    internal class WorkshopBrowser : Level
    {
        private List<WorkshopBrowser.Group> groups = new List<WorkshopBrowser.Group>();
        private SpriteMap _quackLoader;
        private FancyBitmapFont _font;
        private int _selectedGroup;
        private int _selectedItem;
        private WorkshopBrowser.Item _openedItem;

        public override void Initialize()
        {
            _quackLoader = new SpriteMap("quackLoader", 31, 31)
            {
                speed = 0.2f
            };
            _quackLoader.CenterOrigin();
            _quackLoader.scale = new Vec2(0.5f, 0.5f);
            _font = new FancyBitmapFont("smallFont");
            Layer.HUD.camera.width *= 2f;
            Layer.HUD.camera.height *= 2f;
            groups.Add(new WorkshopBrowser.Group("Subscribed", WorkshopQueryFilterOrder.RankedByVote, Steam.user.id, null, new string[1]
            {
        "Mod"
            }));
            groups.Add(new WorkshopBrowser.Group("Hats", WorkshopQueryFilterOrder.RankedByVote, 0UL, "hat", new string[1]
            {
        "Mod"
            }));
            groups.Add(new WorkshopBrowser.Group("Mods", WorkshopQueryFilterOrder.RankedByVote, 0UL, null, new string[1]
            {
        "Mod"
            }));
            groups.Add(new WorkshopBrowser.Group("Maps", WorkshopQueryFilterOrder.RankedByVote, 0UL, null, new string[1]
            {
        "Map"
            }));
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.Pressed("UP") && _selectedGroup > 0)
            {
                SFX.Play("rainpop");
                --_selectedGroup;
            }
            if (Input.Pressed("DOWN") && _selectedGroup < groups.Count - 1)
            {
                SFX.Play("rainpop");
                ++_selectedGroup;
            }
            if (Input.Pressed("LEFT") && _selectedItem > 0)
            {
                SFX.Play("rainpop");
                --_selectedItem;
            }
            if (Input.Pressed("RIGHT") && _selectedItem < 8)
            {
                SFX.Play("rainpop");
                ++_selectedItem;
            }
            if (_selectedItem >= groups[_selectedGroup].items.Count)
                _selectedItem = groups[_selectedGroup].items.Count - 1;
            if (_selectedItem < 0)
                _selectedItem = 0;
            if (Input.Pressed("SELECT"))
                _openedItem = groups[_selectedGroup].items[_selectedItem];
            if (Input.Pressed("CANCEL"))
                _openedItem = null;
            base.Update();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
            {
                if (_openedItem != null)
                {
                    _font.scale = new Vec2(1f, 1f);
                    _font.Draw(_openedItem.name, new Vec2(16f, 16f), Color.White, (Depth)0.5f);
                    if (_openedItem.preview != null)
                        DuckGame.Graphics.Draw(_openedItem.preview, 16f, 32f, (float)(256.0 / _openedItem.preview.height * 0.5), (float)(256.0 / _openedItem.preview.height * 0.5), (Depth)0.5f);
                    _font.maxWidth = 300;
                    _font.Draw(_openedItem.description, new Vec2(16f, 170f), Color.White, (Depth)0.5f);
                    _font.maxWidth = 0;
                }
                else
                {
                    Vec2 pos = new Vec2(32f, 16f);
                    Vec2 vec2_1 = new Vec2(64f, 64f);
                    int num1 = 0;
                    foreach (WorkshopBrowser.Group group in groups)
                    {
                        Vec2 vec2_2 = pos + new Vec2(0f, 12f);
                        _font.scale = new Vec2(1f, 1f);
                        _font.Draw(group.name, pos, Color.White, (Depth)0.5f);
                        int num2 = 0;
                        foreach (WorkshopBrowser.Item obj in group.items)
                        {
                            Vec2 vec2_3 = new Vec2(0f);
                            float num3 = 0.25f;
                            float num4 = 0.1f;
                            if (num1 == _selectedGroup && num2 == _selectedItem)
                            {
                                vec2_3 = new Vec2(-4f, -4f);
                                DuckGame.Graphics.DrawRect(vec2_2 + vec2_3 + new Vec2(-1f, -1f), vec2_2 + vec2_3 + vec2_1 + new Vec2(8f, 8f) + new Vec2(1f, 1f), Color.White, (Depth)0.5f, false, 2f);
                                num3 = 0.28f;
                                num4 = 0.5f;
                            }
                            if (obj.preview != null)
                            {
                                float num5 = 256f / obj.preview.height;
                                float x = obj.preview.width / 2 - obj.preview.height / 2;
                                DuckGame.Graphics.Draw(obj.preview, vec2_2 + vec2_3, new Rectangle?(new Rectangle(x, 0f, obj.preview.height, obj.preview.height)), Color.White, 0f, Vec2.Zero, new Vec2(num5 * num3, num5 * num3), SpriteEffects.None, (Depth)num4);
                            }
                            else
                                DuckGame.Graphics.Draw(_quackLoader, vec2_2.x + vec2_1.x / 2f, vec2_2.y + vec2_1.y / 2f);
                            _font.scale = new Vec2(0.5f, 0.5f);
                            string text = obj.name.Reduced(21);
                            _font.Draw(text, vec2_2 + vec2_3 + new Vec2(2f, 2f), Color.White, (Depth)(num4 + 0.1f));
                            DuckGame.Graphics.DrawRect(vec2_2 + vec2_3 + new Vec2(1f, 1f), vec2_2 + vec2_3 + new Vec2(_font.GetWidth(text) + 6f, 8f), Color.Black * 0.7f, (Depth)(num4 + 0.05f));
                            vec2_2.x += vec2_1.x;
                            if (vec2_2.x + vec2_1.x <= Layer.HUD.width)
                                ++num2;
                            else
                                break;
                        }
                        ++num1;
                        pos.y += 84f;
                    }
                }
            }
            base.PostDrawLayer(layer);
        }

        private class Item
        {
            public string description;
            public WorkshopQueryResultDetails details;
            private Tex2D _preview;
            public PNGData _previewData;
            private static Dictionary<ulong, WorkshopBrowser.Item> _items = new Dictionary<ulong, WorkshopBrowser.Item>();

            public string name => details.title;

            public Tex2D preview
            {
                get
                {
                    if (_preview == null && _previewData != null)
                    {
                        _preview = new Tex2D(_previewData.width, _previewData.height);
                        _preview.SetData<int>(_previewData.data);
                    }
                    return _preview;
                }
            }

            public static WorkshopBrowser.Item Get(ulong pID)
            {
                Item obj;
                if (!WorkshopBrowser.Item._items.TryGetValue(pID, out obj))
                    obj = WorkshopBrowser.Item._items[pID] = new WorkshopBrowser.Item();
                return obj;
            }

            internal Item()
            {
            }
        }

        private class Group
        {
            public string name;
            public List<WorkshopBrowser.Item> items = new List<WorkshopBrowser.Item>();
            public List<string> tags;
            public string searchText;
            public ulong userID;
            public WorkshopQueryFilterOrder orderMode;
            private WorkshopQueryUGC _currentQuery;

            public Group(
              string pName,
              WorkshopQueryFilterOrder pOrder,
              ulong pUserID,
              string pSearchText,
              params string[] pTags)
            {
                name = pName;
                orderMode = pOrder;
                tags = pTags.ToList<string>();
                searchText = pSearchText;
                userID = pUserID;
                OpenPage(0);
            }

            public void OpenPage(int pIndex)
            {
                if (userID != 0UL)
                {
                    _currentQuery = Steam.CreateQueryUser(userID, WorkshopList.Subscribed, WorkshopType.Items, WorkshopSortOrder.SubscriptionDateDesc);
                }
                else
                {
                    _currentQuery = Steam.CreateQueryAll(orderMode, WorkshopType.Items);
                    (_currentQuery as WorkshopQueryAll).searchText = searchText;
                }
                foreach (string tag in tags)
                    _currentQuery.requiredTags.Add(tag);
                _currentQuery.justOnePage = true;
                _currentQuery.QueryFinished += new WorkshopQueryFinished(FinishedQuery);
                _currentQuery.ResultFetched += new WorkshopQueryResultFetched(Fetched);
                _currentQuery.fetchedData = WorkshopQueryData.AdditionalPreviews | WorkshopQueryData.PreviewURL;
                _currentQuery.Request();
            }

            private void Fetched(object sender, WorkshopQueryResult result)
            {
                WorkshopBrowser.Item item = WorkshopBrowser.Item.Get(result.details.publishedFile.id);
                if (item.preview == null)
                {
                    string previewUrl = result.previewURL;
                    if (string.IsNullOrEmpty(previewUrl) && result.additionalPreviews != null)
                    {
                        foreach (WorkshopQueryResultAdditionalPreview additionalPreview in result.additionalPreviews)
                        {
                            if (additionalPreview.isImage)
                            {
                                previewUrl = additionalPreview.urlOrVideoID;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(previewUrl))
                        new Task(() =>
                       {
                           using (WebClient webClient = new WebClient())
                               item._previewData = ContentPack.LoadPNGDataFromStream(new MemoryStream(webClient.DownloadData(new Uri(previewUrl))));
                       }).Start();
                }
                item.details = result.details;
                items.Add(item);
            }

            private void FinishedQuery(object sender)
            {
            }
        }
    }
}
