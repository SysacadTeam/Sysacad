using Microsoft.Extensions.DependencyInjection;
using Sysacad.Client.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Sysacad.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRegisteredServices(this IServiceCollection services)
        {
            Assembly assembly = typeof(Program).Assembly;

            var typesWithAttribute = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<RegisterServiceAttribute>() != null);

            foreach (var type in typesWithAttribute)
            {
                var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
                if (attribute == null) continue;

                var serviceType = attribute.InterfaceType ?? type;

                switch (attribute.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(serviceType, type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(serviceType, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(serviceType, type);
                        break;
                }
            }
        }
    }
}