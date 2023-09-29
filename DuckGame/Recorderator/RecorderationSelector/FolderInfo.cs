using System.Collections.Generic;
using System.IO;

namespace DuckGame
{
    public class FolderInfo
    {
        public string DisplayName;
        public List<FolderInfo> SubFolders = new();
        public List<ReplayInfo> Replays = new();
        public int ScrollIndex;
        public FolderInfo Parent;
        public bool IsInitialized;
        public string Path;

        public FolderInfo(string path)
        {
            Path = path;
            FileInfo fileInfo = new FileInfo(path);
            DisplayName = fileInfo.Name;
        }
        
        public void Initialize()
        {
            if (Path == null) 
                return;
            
            List<string> replayPaths = DuckFile.ReGetFiles(Path, "*.crf", SearchOption.TopDirectoryOnly);
            List<string> subFolderPaths = DuckFile.ReGetDirectories(Path);

            for (int i = 0; i < subFolderPaths.Count; i++)
            {
                SubFolders.Add(new FolderInfo(subFolderPaths[i]) { Parent = this });
            }
            
            for (int i = 0; i < replayPaths.Count; i++)
            {
                Replays.Add(new ReplayInfo(replayPaths[i]));
            }
            
            IsInitialized = true;
        }
    }
}