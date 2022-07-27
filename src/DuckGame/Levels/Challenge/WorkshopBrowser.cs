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
            this._quackLoader = new SpriteMap("quackLoader", 31, 31)
            {
                speed = 0.2f
            };
            this._quackLoader.CenterOrigin();
            this._quackLoader.scale = new Vec2(0.5f, 0.5f);
            this._font = new FancyBitmapFont("smallFont");
            Layer.HUD.camera.width *= 2f;
            Layer.HUD.camera.height *= 2f;
            this.groups.Add(new WorkshopBrowser.Group("Subscribed", WorkshopQueryFilterOrder.RankedByVote, Steam.user.id, null, new string[1]
            {
        "Mod"
            }));
            this.groups.Add(new WorkshopBrowser.Group("Hats", WorkshopQueryFilterOrder.RankedByVote, 0UL, "hat", new string[1]
            {
        "Mod"
            }));
            this.groups.Add(new WorkshopBrowser.Group("Mods", WorkshopQueryFilterOrder.RankedByVote, 0UL, null, new string[1]
            {
        "Mod"
            }));
            this.groups.Add(new WorkshopBrowser.Group("Maps", WorkshopQueryFilterOrder.RankedByVote, 0UL, null, new string[1]
            {
        "Map"
            }));
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.Pressed("UP") && this._selectedGroup > 0)
            {
                SFX.Play("rainpop");
                --this._selectedGroup;
            }
            if (Input.Pressed("DOWN") && this._selectedGroup < this.groups.Count - 1)
            {
                SFX.Play("rainpop");
                ++this._selectedGroup;
            }
            if (Input.Pressed("LEFT") && this._selectedItem > 0)
            {
                SFX.Play("rainpop");
                --this._selectedItem;
            }
            if (Input.Pressed("RIGHT") && this._selectedItem < 8)
            {
                SFX.Play("rainpop");
                ++this._selectedItem;
            }
            if (this._selectedItem >= this.groups[this._selectedGroup].items.Count)
                this._selectedItem = this.groups[this._selectedGroup].items.Count - 1;
            if (this._selectedItem < 0)
                this._selectedItem = 0;
            if (Input.Pressed("SELECT"))
                this._openedItem = this.groups[this._selectedGroup].items[this._selectedItem];
            if (Input.Pressed("CANCEL"))
                this._openedItem = null;
            base.Update();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
            {
                if (this._openedItem != null)
                {
                    this._font.scale = new Vec2(1f, 1f);
                    this._font.Draw(this._openedItem.name, new Vec2(16f, 16f), Color.White, (Depth)0.5f);
                    if (this._openedItem.preview != null)
                        DuckGame.Graphics.Draw(this._openedItem.preview, 16f, 32f, (float)(256.0 / _openedItem.preview.height * 0.5), (float)(256.0 / _openedItem.preview.height * 0.5), (Depth)0.5f);
                    this._font.maxWidth = 300;
                    this._font.Draw(this._openedItem.description, new Vec2(16f, 170f), Color.White, (Depth)0.5f);
                    this._font.maxWidth = 0;
                }
                else
                {
                    Vec2 pos = new Vec2(32f, 16f);
                    Vec2 vec2_1 = new Vec2(64f, 64f);
                    int num1 = 0;
                    foreach (WorkshopBrowser.Group group in this.groups)
                    {
                        Vec2 vec2_2 = pos + new Vec2(0.0f, 12f);
                        this._font.scale = new Vec2(1f, 1f);
                        this._font.Draw(group.name, pos, Color.White, (Depth)0.5f);
                        int num2 = 0;
                        foreach (WorkshopBrowser.Item obj in group.items)
                        {
                            Vec2 vec2_3 = new Vec2(0.0f);
                            float num3 = 0.25f;
                            float num4 = 0.1f;
                            if (num1 == this._selectedGroup && num2 == this._selectedItem)
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
                                DuckGame.Graphics.Draw(obj.preview, vec2_2 + vec2_3, new Rectangle?(new Rectangle(x, 0.0f, obj.preview.height, obj.preview.height)), Color.White, 0.0f, Vec2.Zero, new Vec2(num5 * num3, num5 * num3), SpriteEffects.None, (Depth)num4);
                            }
                            else
                                DuckGame.Graphics.Draw(_quackLoader, vec2_2.x + vec2_1.x / 2f, vec2_2.y + vec2_1.y / 2f);
                            this._font.scale = new Vec2(0.5f, 0.5f);
                            string text = obj.name.Reduced(21);
                            this._font.Draw(text, vec2_2 + vec2_3 + new Vec2(2f, 2f), Color.White, (Depth)(num4 + 0.1f));
                            DuckGame.Graphics.DrawRect(vec2_2 + vec2_3 + new Vec2(1f, 1f), vec2_2 + vec2_3 + new Vec2(this._font.GetWidth(text) + 6f, 8f), Color.Black * 0.7f, (Depth)(num4 + 0.05f));
                            vec2_2.x += vec2_1.x;
                            if (vec2_2.x + (double)vec2_1.x <= (double)Layer.HUD.width)
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

            public string name => this.details.title;

            public Tex2D preview
            {
                get
                {
                    if (this._preview == null && this._previewData != null)
                    {
                        this._preview = new Tex2D(this._previewData.width, this._previewData.height);
                        this._preview.SetData<int>(this._previewData.data);
                    }
                    return this._preview;
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
                this.name = pName;
                this.orderMode = pOrder;
                this.tags = pTags.ToList<string>();
                this.searchText = pSearchText;
                this.userID = pUserID;
                this.OpenPage(0);
            }

            public void OpenPage(int pIndex)
            {
                if (this.userID != 0UL)
                {
                    this._currentQuery = Steam.CreateQueryUser(this.userID, WorkshopList.Subscribed, WorkshopType.Items, WorkshopSortOrder.SubscriptionDateDesc);
                }
                else
                {
                    this._currentQuery = Steam.CreateQueryAll(this.orderMode, WorkshopType.Items);
                    (this._currentQuery as WorkshopQueryAll).searchText = this.searchText;
                }
                foreach (string tag in this.tags)
                    this._currentQuery.requiredTags.Add(tag);
                this._currentQuery.justOnePage = true;
                this._currentQuery.QueryFinished += new WorkshopQueryFinished(this.FinishedQuery);
                this._currentQuery.ResultFetched += new WorkshopQueryResultFetched(this.Fetched);
                this._currentQuery.fetchedData = WorkshopQueryData.AdditionalPreviews | WorkshopQueryData.PreviewURL;
                this._currentQuery.Request();
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
                this.items.Add(item);
            }

            private void FinishedQuery(object sender)
            {
            }
        }
    }
}
