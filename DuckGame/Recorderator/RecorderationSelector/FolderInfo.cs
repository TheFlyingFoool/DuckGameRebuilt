using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DuckGame.RecorderationSelector;

namespace DuckGame
{
    public class FolderInfo : IRMenuItem
    {
        public string DisplayName;
        public List<IRMenuItem> Items;
        public string Path;
        private bool _open;
        public int FolderSub { get; set; }
        public IRMenuItem Parent { get; set; }

        public bool Open
        {
            get => _open && (Parent is not FolderInfo folder || folder.Open);
            set => _open = value;
        }

        public FolderInfo(string path)
        {
            Path = path;
            FileInfo fileInfo = new FileInfo(path);
            DisplayName = fileInfo.Name;

            Items = new List<IRMenuItem>();
            
            string[] replayPaths = Directory.GetFiles(path, "*.crf", SearchOption.TopDirectoryOnly);
            string[] folderPaths = Directory.GetDirectories(path);
            
            Items.AddRange(folderPaths.Select(x => new FolderInfo(x)));
            Items.AddRange(replayPaths.OrderByDescending(x => new FileInfo(x).CreationTime).Select(x => new ReplayInfo(x)));
        }
        
        public void Initialize()
        {
            if (Path == null) 
                return;

            Items = new List<IRMenuItem>();
            
            string[] replayPaths = Directory.GetFiles(CordsPath, "*.crf", SearchOption.TopDirectoryOnly);
            string[] folderPaths = Directory.GetDirectories(CordsPath);

            Items.AddRange(folderPaths.Select(x => new FolderInfo(x)));
            Items.AddRange(replayPaths.OrderByDescending(x => new FileInfo(x).CreationTime).Select(x => new ReplayInfo(x)));
        }

        public Rectangle Draw(Vec2 position, bool selected)
        {
            string textToDraw = $"@{(_open ? "SELECTICON" : "FOLDERICON")}@{DisplayName}";
            
            Vec2 textSize = Extensions.GetFancyStringSize(textToDraw, FONT_SIZE);
            Rectangle textBounds = new(position.x, position.y, textSize.x, textSize.y);
            
            Graphics.DrawFancyString(textToDraw, position, selected ? Color.Yellow : Color.White, 1f, FONT_SIZE);
            return textBounds;
        }

        public void UpdateHovered() { }
        public void OnHover() { }
        public void OnUnhover() { }
        public void OnSelect()
        {
            Open ^= true;
            current.UpdateMenuItemList();

            // close bebe folders when closing
            if (!_open)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] is FolderInfo childFolder)
                        childFolder.Open = false;
                }
            }
        }
    }
}