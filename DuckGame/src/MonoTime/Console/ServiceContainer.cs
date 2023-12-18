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
        public void AddService<T>(T service) => services.Add(typeof(T), service);

        /// <summary>Looks up the specified service.</summary>
        public object GetService(Type serviceType) => services[serviceType];
    }
}
