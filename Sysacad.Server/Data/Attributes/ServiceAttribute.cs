using Microsoft.Extensions.DependencyInjection;

namespace Sysacad.Server.Data.Attributes
{
    /// <summary>
    /// Marks a class for automatic DI registration via <see cref="Extensions.ServiceCollectionExtensions.AddAnnotatedServices"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }
        public Type? InterfaceType { get; }
        public bool RegisterAsConcrete { get; }

        /// <summary>Scoped, auto-detects interface by convention "I+ClassName".</summary>
        public ServiceAttribute()
        {
            Lifetime = ServiceLifetime.Scoped;
        }

        /// <summary>Explicit lifetime, auto-detects interface by convention "I+ClassName".</summary>
        public ServiceAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>Scoped, explicit interface type.</summary>
        public ServiceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
            Lifetime = ServiceLifetime.Scoped;
        }

        /// <summary>Explicit interface type and lifetime.</summary>
        public ServiceAttribute(Type interfaceType, ServiceLifetime lifetime)
        {
            InterfaceType = interfaceType;
            Lifetime = lifetime;
        }

        /// <summary>Registers as concrete type (no interface).</summary>
        public ServiceAttribute(bool registerAsConcrete, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            RegisterAsConcrete = registerAsConcrete;
            Lifetime = lifetime;
        }
    }
}
