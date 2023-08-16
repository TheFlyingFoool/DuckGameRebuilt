// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyReflection.FieldInfoHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using DuckGame;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace XnaToFna.ProxyReflection
{
    public static class FieldInfoHelper
    {
        private static readonly Dictionary<Type, Dictionary<string, XnaToFnaFieldInfo>> Map = new Dictionary<Type, Dictionary<string, XnaToFnaFieldInfo>>()
        {
          {
            typeof (StringBuilder),
            new Dictionary<string, XnaToFnaFieldInfo>()
            {
              {
                "m_StringValue",
                new XnaToFnaFieldInfo(typeof (string),  obj =>  ((StringBuilder) obj).ToString(),  (obj, val) => ((StringBuilder) obj).Clear().Append(val))
              }
            }
          }
        };
        public static MethodInfo DrawSelectionHandler = typeof(FieldInfoHelper).GetMethod("DrawSelection", BindingFlags.Static | BindingFlags.Public);
        public static MethodInfo RealDrawSelection;
        public static FieldInfo GetField(Type self, string name, BindingFlags bindingAttr)
        {
            try
            {
                Dictionary<string, XnaToFnaFieldInfo> dictionary;
                XnaToFnaFieldInfo xnaToFnaFieldInfo;
                return Map.TryGetValue(self, out dictionary) && dictionary.TryGetValue(name, out xnaToFnaFieldInfo) ? xnaToFnaFieldInfo : self.GetField(name, bindingAttr);
            }
            catch(Exception  e)
            {
                DevConsole.Log("test");
            }
            return null;
        }

        public static MethodInfo GetMethod(Type self, string name, BindingFlags bindingAttr)
        {
            XnaToFnaFieldInfo xnaToFnaFieldInfo;
            DevConsole.Log("GetMethod " + name);
            if (self.FullName == "DuckGame.BetterChat.HarmonyPatches")
            {
                if (name == "DuckNetworkUpdate_Transpiler")
                {
                    return null;
                }
                else if (name == "DrawSelection")
                {
                    RealDrawSelection = self.GetMethod(name, bindingAttr);
                    return DrawSelectionHandler;
                }
            }
            return self.GetMethod(name, bindingAttr);
        }
        public static void DrawSelection(Vec2 messagePos) // for better chat
        {
            try
            {
                RealDrawSelection.Invoke(null, new object[] { messagePos });
            }
            catch
            { }
        }

        public static FieldInfo GetField(Type self, string name)
        {
            return GetField(self, name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        }

        public class XnaToFnaFieldInfo : FieldInfo
        {
            internal Type _DeclaringType;
            internal readonly Type _FieldType;
            internal string _Name;
            internal readonly Func<object, object> _OnGetValue;
            internal readonly Action<object, object> _OnSetValue;

            public override FieldAttributes Attributes => throw new NotSupportedException();

            public override Type DeclaringType => _DeclaringType;

            public override RuntimeFieldHandle FieldHandle => throw new NotSupportedException();

            public override Type FieldType => _FieldType;

            public override string Name => _Name;

            public override Type ReflectedType => throw new NotSupportedException();

            public XnaToFnaFieldInfo(
              Type fieldType,
              Func<object, object> onGetValue = null,
              Action<object, object> onSetValue = null)
            {
                _FieldType = fieldType;
                _OnGetValue = onGetValue;
                _OnSetValue = onSetValue;
            }

            public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

            public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

            public override bool IsDefined(Type attributeType, bool inherit) => throw new NotSupportedException();

            public override object GetValue(object obj)
            {
                Func<object, object> onGetValue = _OnGetValue;
                return onGetValue == null ? null : onGetValue(obj);
            }

            public override void SetValue(
              object obj,
              object value,
              BindingFlags invokeAttr,
              Binder binder,
              CultureInfo culture)
            {
                Action<object, object> onSetValue = _OnSetValue;
                if (onSetValue == null)
                    return;
                onSetValue(obj, value);
            }
        }
    }
}
