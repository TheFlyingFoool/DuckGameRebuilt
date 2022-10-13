// Decompiled with JetBrains decompiler
// Type: XnaToFna.BinaryFormatterHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace XnaToFna
{
    public static class BinaryFormatterHelper
    {
        public static readonly Assembly FNA = typeof(Game).Assembly;

        public static BinaryFormatter Create() => new BinaryFormatter()
        {
            Binder = new BinaryFormatterHelper.XnaToFnaSerializationBinderWrapper(null)
        };

        public static BinaryFormatter Create(
          ISurrogateSelector selector,
          StreamingContext context)
        {
            return new BinaryFormatter(selector, context)
            {
                Binder = new BinaryFormatterHelper.XnaToFnaSerializationBinderWrapper(null)
            };
        }

        public static SerializationBinder get_Binder(this BinaryFormatter self) => (self.Binder is BinaryFormatterHelper.XnaToFnaSerializationBinderWrapper binder ? binder.Inner : null) ?? self.Binder;

        public static void set_Binder(this BinaryFormatter self, SerializationBinder binder) => self.Binder = new BinaryFormatterHelper.XnaToFnaSerializationBinderWrapper(binder);

        public class XnaToFnaSerializationBinderWrapper : SerializationBinder
        {
            public readonly SerializationBinder Inner;

            public XnaToFnaSerializationBinderWrapper(SerializationBinder inner) => this.Inner = inner;

            public override Type BindToType(string assemblyName, string typeName)
            {
                if (!(assemblyName != "Microsoft.Xna.Framework") || assemblyName.StartsWith("Microsoft.Xna.Framework,") || assemblyName.StartsWith("Microsoft.Xna.Framework."))
                    return BinaryFormatterHelper.FNA.GetType(typeName);
                return this.Inner?.BindToType(assemblyName, typeName);
            }

            public override void BindToName(
              Type serializedType,
              out string assemblyName,
              out string typeName)
            {
                if (this.Inner != null)
                    this.Inner.BindToName(serializedType, out assemblyName, out typeName);
                else
                    base.BindToName(serializedType, out assemblyName, out typeName);
            }
        }
    }
}
