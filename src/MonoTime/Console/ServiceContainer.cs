// Decompiled with JetBrains decompiler
// Type: WinFormsGraphicsDevice.ServiceContainer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace WinFormsGraphicsDevice
{
    /// <summary>
    /// Container class implements the IServiceProvider interface. This is used
    /// to pass shared services between different components, for instance the
    /// ContentManager uses it to locate the IGraphicsDeviceService implementation.
    /// </summary>
    public class ServiceContainer : IServiceProvider
    {
        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>Adds a new service to the collection.</summary>
        public void AddService<T>(T service) => this.services.Add(typeof(T), service);

        /// <summary>Looks up the specified service.</summary>
        public object GetService(Type serviceType)
        {
            object service;
            this.services.TryGetValue(serviceType, out service);
            return service;
        }
    }
}
