using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sysacad.Server.Data.Attributes;

namespace Sysacad.Server.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnnotatedServices(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<ServiceAttribute>() != null);

            foreach (var implementationType in types)
            {
                var attr = implementationType.GetCustomAttribute<ServiceAttribute>()!;
                var serviceType = ResolveServiceType(implementationType, attr);

                if (services.Any(d => d.ServiceType == serviceType && d.ImplementationType == implementationType))
                    continue;

                switch (attr.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(serviceType, implementationType);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(serviceType, implementationType);
                        break;
                    default:
                        services.AddScoped(serviceType, implementationType);
                        break;
                }
            }

            return services;
        }

        private static Type ResolveServiceType(Type implementationType, ServiceAttribute attr)
        {
            if (attr.RegisterAsConcrete)
                return implementationType;

            if (attr.InterfaceType != null)
            {
                AssertImplements(implementationType, attr.InterfaceType);
                return attr.InterfaceType;
            }

            var conventionName = "I" + implementationType.Name;
            var conventionInterface = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == conventionName);

            if (conventionInterface != null)
                return conventionInterface;

            return implementationType;
        }

        private static void AssertImplements(Type implementation, Type iface)
        {
            if (!iface.IsAssignableFrom(implementation))
                throw new InvalidOperationException(
                    $"[Service] on '{implementation.Name}' declares interface '{iface.Name}' but the class does not implement it.");
        }
    }
}
