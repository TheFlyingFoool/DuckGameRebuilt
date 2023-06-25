using DuckGame.ConsoleInterface;
using DuckGame.MMConfig;
using System;
using System.Linq;
using System.Reflection;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ACKeybindAttribute : Attribute
    {
        private FieldInfo _fieldInfo;
        private TypeInfo _fieldParentTypeInfo;
        public string KeybindString =>
            _fieldInfo.GetValue(typeof(MMKeymapConfig).GetFields()
                    .First(x => x.FieldType.FullName == _fieldParentTypeInfo.FullName)
                    .GetValue(MallardManager.Config.Keymap))
                .ToString();
        
        public void InitializeMembers(FieldInfo fieldInfo, TypeInfo fieldParentTypeInfo)
        {
            _fieldInfo = fieldInfo;
            _fieldParentTypeInfo = fieldParentTypeInfo;
        }
    }
}