// Decompiled with JetBrains decompiler
// Type: DuckGame.Album
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class Album : Level
    {
        private List<AlbumPic> _images = new List<AlbumPic>();
        private List<AlbumPage> _pages = new List<AlbumPage>();
        private List<Texture2D> _textures = new List<Texture2D>();
        private int _currentPage;
        private int _prevPage = -1;
        private Sprite _album;
        private Sprite _screen;
        //private Material _pageMaterial;
        private BitmapFont _font;
        private List<LockerStat> _stats = new List<LockerStat>();
        private bool _quit;

        public Album() => _centeredView = true;

        public override void Initialize()
        {
            _album = new Sprite("album");
            //_screen = new Sprite("albumpic");
            //this._pageMaterial = (Material)new MaterialAlbum();
            _font = new BitmapFont("biosFont", 8);
            _stats.Add(new LockerStat("QUACKS: " + Global.data.quacks.valueInt.ToString(), Color.DarkSlateGray));
            _stats.Add(new LockerStat("", Color.Red));
            _stats.Add(new LockerStat("TIME SPENT", Color.DarkSlateGray));
            _stats.Add(new LockerStat("IN MATCHES: " + TimeSpan.FromSeconds((float)Global.data.timeInMatches).ToString("hh\\:mm\\:ss"), Color.DarkSlateGray));
            _stats.Add(new LockerStat("IN ARCADE: " + TimeSpan.FromSeconds((float)Global.data.timeInArcade).ToString("hh\\:mm\\:ss"), Color.DarkSlateGray));
            _stats.Add(new LockerStat("IN EDITOR: " + TimeSpan.FromSeconds((float)Global.data.timeInEditor).ToString("hh\\:mm\\:ss"), Color.DarkSlateGray));
            foreach (string file in DuckFile.GetFiles(DuckFile.albumDirectory))
            {
                try
                {
                    DateTime dateTime = DateTime.Parse(Path.GetFileNameWithoutExtension(file).Replace(';', ':'), CultureInfo.InvariantCulture);
                    _images.Add(new AlbumPic()
                    {
                        file = file,
                        date = dateTime
                    });
                }
                catch
                {
                }
            }
            _pages.Add(new AlbumPage()
            {
                caption = "LIFETIME STATS",
                statPage = true
            });
            _images = _images.OrderBy(x => x.date).ToList();
            AlbumPage albumPage = null;
            int num = 1;
            foreach (AlbumPic image in _images)
            {
                string str = image.date.ToString("MMMM", CultureInfo.InvariantCulture) + " " + image.date.Year.ToString();
                if (albumPage == null)
                {
                    num = 1;
                    albumPage = new AlbumPage
                    {
                        caption = str
                    };
                    _pages.Add(albumPage);
                }
                if (!albumPage.caption.Contains(str))
                {
                    num = 1;
                    albumPage = new AlbumPage
                    {
                        caption = str
                    };
                    _pages.Add(albumPage);
                }
                if (albumPage.pics.Count == 4)
                {
                    ++num;
                    albumPage = new AlbumPage
                    {
                        caption = str + " (" + num.ToString() + ")"
                    };
                    _pages.Add(albumPage);
                }
                albumPage.pics.Add(image);
            }
            HUD.AddCornerControl(HUDCorner.BottomRight, "@WASD@FLIP PAGE");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();
            if (_pages.Count > 0)
            {
                if (Input.Pressed("MENURIGHT"))
                    ++_currentPage;
                if (Input.Pressed("MENULEFT"))
                    --_currentPage;
                if (_currentPage < 0)
                    _currentPage = 0;
                if (_currentPage == _pages.Count)
                    _currentPage = _pages.Count - 1;
                if (_currentPage != _prevPage)
                {
                    _prevPage = _currentPage;
                    foreach (GraphicsResource texture in _textures)
                        texture.Dispose();
                    _textures.Clear();
                    SFX.Play("page");
                    foreach (AlbumPic pic in _pages[_currentPage].pics)
                    {
                        try
                        {
                            Texture2D texture2D;
                            using (FileStream fileStream = new FileStream(pic.file, FileMode.Open))
                                texture2D = Texture2D.FromStream(Graphics.device, fileStream);
                            _textures.Add(texture2D);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (Input.Pressed("CANCEL"))
            {
                _quit = true;
                HUD.CloseAllCorners();
            }
            Graphics.fade = Lerp.Float(Graphics.fade, _quit ? 0f : 1f, 0.05f);
            if (Graphics.fade >= 0.01f || !_quit)
                return;
            current = new DoorRoom();
        }

        public override void Draw() => base.Draw();

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
            {
                _album.depth = -0.8f;
                Graphics.Draw(_album, 0f, 0f);
                //_screen.depth = -0.6f;
                if (_pages.Count > 0)
                {
                    int index1 = 0;
                    if (_pages[_currentPage].statPage)
                    {
                        int num = 0;
                        foreach (LockerStat stat in _stats)
                        {
                            Vec2 vec2 = new Vec2(160f, 40 + num * 10);
                            string name = stat.name;
                            Graphics.DrawString(name, vec2 + new Vec2((-Graphics.GetStringWidth(name) / 2f), 0f), stat.color, (Depth)0.5f);
                            ++num;
                        }
                    }
                    else
                    {
                        for (int index2 = 0; index2 < 2; ++index2)
                        {
                            for (int index3 = 0; index3 < 2; ++index3)
                            {
                                if (index1 < _textures.Count)
                                {
                                    if (Graphics.width > 1280)
                                    {
                                        Vec2 vec2_1 = new Vec2(52f, 35f);
                                        float num = 0.3f;
                                        Vec2 vec2_2 = new Vec2(vec2_1.x + index3 * 110, vec2_1.y + index2 * 65);
                                        Graphics.Draw((Tex2D)_textures[index1], vec2_2.x, vec2_2.y, num, num);
                                        Graphics.DrawRect(vec2_2 + new Vec2(-3f, -3f), vec2_2 + new Vec2((float)(_textures[index1].Width * num + 3f), (float)(_textures[index1].Height * num + 3f)), Color.White, -0.7f);
                                    }
                                    else
                                    {
                                        Vec2 vec2_3 = new Vec2(65f, 40f);
                                        float num = 0.25f;
                                        Vec2 vec2_4 = new Vec2(vec2_3.x + index3 * 100, vec2_3.y + index2 * 65);
                                        Graphics.Draw((Tex2D)_textures[index1], vec2_4.x, vec2_4.y, num, num);
                                        Graphics.DrawRect(vec2_4 + new Vec2(-3f, -3f), vec2_4 + new Vec2((_textures[index1].Width * num + 3f), (_textures[index1].Height * num + 3f)), Color.White, -0.7f);
                                    }
                                }
                                ++index1;
                            }
                        }
                    }
                    string caption = _pages[_currentPage].caption;
                    _font.Draw(caption, new Vec2((Layer.HUD.width / 2f - _font.GetWidth(caption) / 2f - 4f), 18f), Color.DarkSlateGray, -0.5f);
                }
                else
                {
                    string text = "EMPTY ALBUM :(";
                    _font.Draw(text, new Vec2((Layer.HUD.width / 2f - _font.GetWidth(text) / 2f - 4f), 18f), Color.DarkSlateGray, -0.5f);
                }
            }
            base.PostDrawLayer(layer);
        }
    }
}
