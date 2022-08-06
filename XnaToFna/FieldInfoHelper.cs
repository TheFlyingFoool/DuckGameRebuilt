// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyReflection.FieldInfoHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace XnaToFna.ProxyReflection
{
  public static class FieldInfoHelper
  {
    private static readonly Dictionary<Type, Dictionary<string, FieldInfoHelper.XnaToFnaFieldInfo>> Map = new Dictionary<Type, Dictionary<string, FieldInfoHelper.XnaToFnaFieldInfo>>()
    {
      {
        typeof (StringBuilder),
        new Dictionary<string, FieldInfoHelper.XnaToFnaFieldInfo>()
        {
          {
            "m_StringValue",
            new FieldInfoHelper.XnaToFnaFieldInfo(typeof (string), (Func<object, object>) (obj => (object) ((StringBuilder) obj).ToString()), (Action<object, object>) ((obj, val) => ((StringBuilder) obj).Clear().Append(val)))
          }
        }
      }
    };

    public static FieldInfo GetField(Type self, string name, BindingFlags bindingAttr)
    {
      Dictionary<string, FieldInfoHelper.XnaToFnaFieldInfo> dictionary;
      FieldInfoHelper.XnaToFnaFieldInfo xnaToFnaFieldInfo;
      return FieldInfoHelper.Map.TryGetValue(self, out dictionary) && dictionary.TryGetValue(name, out xnaToFnaFieldInfo) ? (FieldInfo) xnaToFnaFieldInfo : self.GetField(name, bindingAttr);
    }

  

    public static FieldInfo GetField(Type self, string name) => FieldInfoHelper.GetField(self, name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

    public class XnaToFnaFieldInfo : FieldInfo
    {
      internal Type _DeclaringType;
      internal readonly Type _FieldType;
      internal string _Name;
      internal readonly Func<object, object> _OnGetValue;
      internal readonly Action<object, object> _OnSetValue;

      public override FieldAttributes Attributes => throw new NotSupportedException();

      public override Type DeclaringType => this._DeclaringType;

      public override RuntimeFieldHandle FieldHandle => throw new NotSupportedException();

      public override Type FieldType => this._FieldType;

      public override string Name => this._Name;

      public override Type ReflectedType => throw new NotSupportedException();

      public XnaToFnaFieldInfo(
        Type fieldType,
        Func<object, object> onGetValue = null,
        Action<object, object> onSetValue = null)
      {
        this._FieldType = fieldType;
        this._OnGetValue = onGetValue;
        this._OnSetValue = onSetValue;
      }

      public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

      public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

      public override bool IsDefined(Type attributeType, bool inherit) => throw new NotSupportedException();

      public override object GetValue(object obj)
      {
        Func<object, object> onGetValue = this._OnGetValue;
        return onGetValue == null ? (object) null : onGetValue(obj);
      }

      public override void SetValue(
        object obj,
        object value,
        BindingFlags invokeAttr,
        Binder binder,
        CultureInfo culture)
      {
        Action<object, object> onSetValue = this._OnSetValue;
        if (onSetValue == null)
          return;
        onSetValue(obj, value);
      }
    }
  }
}
