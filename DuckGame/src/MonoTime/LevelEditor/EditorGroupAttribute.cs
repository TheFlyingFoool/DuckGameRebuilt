// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorGroupAttribute
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>Declares which group this Thing is in the editor</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EditorGroupAttribute : Attribute
    {
        public readonly string editorGroup;
        public readonly EditorItemType editorType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DuckGame.EditorGroupAttribute" /> class.
        /// </summary>
        /// <param name="group">The editor group, in the format of "root|sub|sub|sub..."</param>
        public EditorGroupAttribute(string group)
        {
            editorGroup = group;
            editorType = EditorItemType.Normal;
        }

        public EditorGroupAttribute(string pGroup, EditorItemType pType)
        {
            editorGroup = pGroup;
            editorType = pType;
        }
    }
}
