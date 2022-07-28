// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorGroup
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class EditorGroup
    {
        public bool autopinGroup;
        public bool rootGroup;
        private List<Thing> _things = new List<Thing>();
        private List<Thing> _filteredThings = new List<Thing>();
        private List<Thing> _onlineFilteredThings = new List<Thing>();
        private List<System.Type> _types = new List<System.Type>();
        public List<EditorGroup> SubGroups = new List<EditorGroup>();
        public string Name = "";

        public List<Thing> Things
        {
            get
            {
                //if (Main.isDemo)
                //    return this._filteredThings;
                return Editor._currentLevelData.metaData.onlineMode ? this._onlineFilteredThings : this._things;
            }
        }

        public List<Thing> AllThings => this._things;

        public List<System.Type> AllTypes => this._types;

        public EditorGroup()
        {
        }

        public EditorGroup(System.Type filter = null, HashSet<System.Type> types = null)
        {
            this.rootGroup = filter == null;
            this.Initialize(filter, types);
        }

        public bool Contains(System.Type t)
        {
            if (this._types.Contains(t))
                return true;
            foreach (EditorGroup subGroup in this.SubGroups)
            {
                if (subGroup.Contains(t))
                    return true;
            }
            return false;
        }

        private void AddType(System.Type t, string group)
        {
            if (group == "survival")
                return;
            if (group == "")
            {
                Main.SpecialCode = "creating " + t.AssemblyQualifiedName;
                Thing typeInstance = Editor.GetOrCreateTypeInstance(t);
                Main.SpecialCode = "accessing " + t.AssemblyQualifiedName;
                IReadOnlyPropertyBag bag = ContentProperties.GetBag(t);
                if (bag.GetOrDefault("isInDemo", false))
                    this._filteredThings.Add(typeInstance);
                if (bag.GetOrDefault("isOnlineCapable", true))
                    this._onlineFilteredThings.Add(typeInstance);
                this._things.Add(typeInstance);
                this._types.Add(t);
                Editor.MapThing(typeInstance);
                ++MonoMain.lazyLoadyBits;
                Main.SpecialCode = "finished " + t.AssemblyQualifiedName;
            }
            else
            {
                string[] groupName = group.Split('|');
                EditorGroup editorGroup = this.SubGroups.FirstOrDefault<EditorGroup>(x => x.Name == groupName[0]);
                if (editorGroup == null)
                {
                    editorGroup = new EditorGroup
                    {
                        Name = groupName[0]
                    };
                    this.SubGroups.Add(editorGroup);
                }
                string str = group;
                string group1 = groupName.Count<string>() <= 1 ? str.Remove(0, groupName[0].Length) : str.Remove(0, groupName[0].Length + 1);
                editorGroup.AddType(t, group1);
            }
        }

        private void Sort()
        {
            this.SubGroups.Sort((x, y) => string.Compare(x.Name, y.Name));
            this.Things.Sort((x, y) => string.Compare(x.editorName, y.editorName));
            foreach (EditorGroup subGroup in this.SubGroups)
                subGroup.Sort();
            int index1 = 12;
            if (this.SubGroups.Count <= index1 || !this.rootGroup)
                return;
            EditorGroup editorGroup = new EditorGroup
            {
                Name = "More...",
                autopinGroup = true
            };
            int num = this.SubGroups.Count - index1;
            for (int index2 = 0; index2 < num; ++index2)
            {
                editorGroup.SubGroups.Add(this.SubGroups[index1]);
                this.SubGroups.RemoveAt(index1);
            }
            this.SubGroups.Add(editorGroup);
        }

        private void Initialize(System.Type filter = null, HashSet<System.Type> types = null)
        {
            List<System.Type> typeList = new List<System.Type>();
            if (types == null)
                typeList.AddRange(Editor.ThingTypes);
            else
                typeList.AddRange(types);
            for (int index = 0; index < typeList.Count; ++index)
            {
                System.Type type = typeList[index];
                if (!(filter != null) || !(type != filter) || Editor.AllBaseTypes[type].Contains(filter))
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(EditorGroupAttribute), false);
                    if (customAttributes.Length != 0)
                    {
                        EditorGroupAttribute editorGroupAttribute = customAttributes[0] as EditorGroupAttribute;
                        if (this.GroupIsAllowed(editorGroupAttribute.editorType))
                        {
                            string editorGroup = editorGroupAttribute.editorGroup;
                            this.AddType(type, editorGroup);
                        }
                    }
                }
            }
            this.Sort();
        }

        public bool GroupIsAllowed(EditorItemType pType) => Options.Data.powerUser || pType != EditorItemType.Arcade && pType != EditorItemType.Debug;
    }
}
