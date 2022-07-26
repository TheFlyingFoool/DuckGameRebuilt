// Decompiled with JetBrains decompiler
// Type: DuckGame.WorkshopMetaData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class WorkshopMetaData : BinaryClassChunk
    {
        public string name;
        public string description;
        public string author;
        public RemoteStoragePublishedFileVisibility visibility;
        public List<string> tags;
        public List<ulong> dependencies;

        public WorkshopMetaData()
        {
            if (Steam.user != null)
                this.author = Steam.user.name;
            this.Reset();
        }

        public void Reset()
        {
            this.name = "";
            this.description = "";
            this.author = "";
            this.visibility = RemoteStoragePublishedFileVisibility.Public;
            this.tags = new List<string>();
            this.dependencies = new List<ulong>();
        }
    }
}
