using Sysacad.Server.Data.Attributes;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Data.Repositories;
using System.Reflection;

namespace Sysacad.Server.Data.Extensions
{
    public static class RepositoryServiceExtensions
    {
        /// <summary>
        /// Registra automáticamente todos los repositorios (genéricos y personalizados) en el contenedor de DI
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="assemblies">Ensamblados donde buscar las entidades (por defecto: ensamblado actual)</param>
        /// <returns>Colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddRepositories(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            // Si no se especifican ensamblados, usar el ensamblado actual
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetExecutingAssembly() };
            }

            // Registrar el repositorio genérico base
            services.AddScoped(typeof(GenericRepository<>));

            // Buscar todas las entidades en los ensamblados especificados
            var entityTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(GenericEntity).IsAssignableFrom(t))
                .ToList();

            var registeredRepositories = new HashSet<Type>();

            foreach (var entityType in entityTypes)
            {
                // Buscar si la entidad tiene un atributo [Repository]
                var repositoryAttribute = entityType.GetCustomAttribute<RepositoryAttribute>();

                if (repositoryAttribute != null)
                {
                    var customRepositoryType = repositoryAttribute.RepositoryType;

                    // Validar que el repositorio implemente IRepository<TEntity>
                    var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);

                    if (!repositoryInterface.IsAssignableFrom(customRepositoryType))
                    {
                        throw new InvalidOperationException(
                            $"El repositorio {customRepositoryType.Name} especificado para la entidad {entityType.Name} " +
                            $"no implementa IRepository<{entityType.Name}>");
                    }

                    // Evitar registrar el mismo repositorio múltiples veces
                    if (!registeredRepositories.Contains(customRepositoryType))
                    {
                        services.AddScoped(customRepositoryType);
                        registeredRepositories.Add(customRepositoryType);

                        Console.WriteLine($"[RepositoryRegistration] Repositorio personalizado registrado: {customRepositoryType.Name} para {entityType.Name}");
                    }
                }
            }

            // Registrar el EntityManager
            services.AddScoped<EntityManager>();

            Console.WriteLine($"[RepositoryRegistration] Total de repositorios personalizados registrados: {registeredRepositories.Count}");
            Console.WriteLine($"[RepositoryRegistration] EntityManager registrado");

            return services;
        }

        /// <summary>
        /// Registra automáticamente todos los repositorios escaneando el ensamblado que contiene el tipo especificado
        /// </summary>
        /// <typeparam name="TMarker">Tipo marcador para identificar el ensamblado</typeparam>
        /// <param name="services">Colección de servicios</param>
        /// <returns>Colección de servicios para encadenamiento</returns>
        public static IServiceCollection AddRepositories<TMarker>(this IServiceCollection services)
        {
            return services.AddRepositories(typeof(TMarker).Assembly);
        }
    }
}
