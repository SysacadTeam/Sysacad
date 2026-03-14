using Sysacad.Server.Data.Attributes;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Data.Repositories;
using System.Collections.Concurrent;
using System.Reflection;

namespace Sysacad.Server.Data
{
    public class EntityManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EntityManager> _logger;
        private readonly ConcurrentDictionary<Type, Type> _repositoryCache;

        public EntityManager(IServiceProvider serviceProvider, ILogger<EntityManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _repositoryCache = new ConcurrentDictionary<Type, Type>();
        }

        /// <summary>
        /// Obtiene el repositorio concreto para una entidad específica.
        /// Si la entidad tiene el atributo [Repository], devuelve ese tipo concreto.
        /// Si no, devuelve GenericRepository.
        /// </summary>
        /// <typeparam name="TEntity">Tipo de la entidad</typeparam>
        /// <returns>Instancia del repositorio que implementa IRepository&lt;TEntity&gt;</returns>
        public IRepository<TEntity> GetBaseRepository<TEntity>() where TEntity : GenericEntity
        {
            var entityType = typeof(TEntity);

            var repositoryType = _repositoryCache.GetOrAdd(entityType, type =>
            {
                var repositoryAttribute = type.GetCustomAttribute<RepositoryAttribute>();

                if (repositoryAttribute != null)
                {
                    _logger.LogDebug("Repositorio personalizado encontrado para {EntityType}: {RepositoryType}",
                        type.Name, repositoryAttribute.RepositoryType.Name);
                    return repositoryAttribute.RepositoryType;
                }

                var genericRepositoryType = typeof(GenericRepository<>).MakeGenericType(type);
                _logger.LogDebug("Usando GenericRepository para {EntityType}", type.Name);
                return genericRepositoryType;
            });

            var repository = _serviceProvider.GetService(repositoryType);

            if (repository == null)
            {
                throw new InvalidOperationException(
                    $"No se pudo resolver el repositorio {repositoryType.Name} para la entidad {entityType.Name}. " +
                    $"Asegúrate de que esté registrado en el contenedor de dependencias.");
            }

            if (repository is not IRepository<TEntity> typedRepository)
            {
                throw new InvalidOperationException(
                    $"El repositorio {repositoryType.Name} no implementa IRepository<{entityType.Name}>");
            }

            return typedRepository;
        }

        /// <summary>
        /// Obtiene el repositorio como interfaz IRepository (para compatibilidad)
        /// </summary>
        /// <typeparam name="TEntity">Tipo de la entidad</typeparam>
        /// <returns>Instancia del repositorio como IRepository</returns>
        public IRepository<TEntity> GetRepositoryInterface<TEntity>() where TEntity : GenericEntity
        {
            return GetBaseRepository<TEntity>();
        }

        /// <summary>
        /// Obtiene el repositorio para una entidad por su tipo
        /// </summary>
        /// <param name="entityType">Tipo de la entidad</param>
        /// <returns>Instancia del repositorio</returns>
        public object GetRepository(Type entityType)
        {
            if (!typeof(GenericEntity).IsAssignableFrom(entityType))
            {
                throw new ArgumentException(
                    $"El tipo {entityType.Name} no es una entidad válida. Debe heredar de GenericEntity.",
                    nameof(entityType));
            }

            var method = GetType().GetMethod(nameof(GetBaseRepository), Array.Empty<Type>());
            var genericMethod = method!.MakeGenericMethod(entityType);

            return genericMethod.Invoke(this, null)!;
        }

        /// <summary>
        /// Obtiene un repositorio específico por su tipo concreto.
        /// Permite: var repo = _entityManager.GetRepository&lt;UsuarioRepository&gt;();
        /// </summary>
        /// <typeparam name="TRepository">Tipo concreto del repositorio</typeparam>
        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            // 1. Intentamos resolverlo directamente del contenedor (si está registrado como servicio propio)
            var service = _serviceProvider.GetService(typeof(TRepository));
            if (service != null)
            {
                return (TRepository)service;
            }

            // 2. Si no, intentamos deducir la entidad que maneja para usar la lógica centralizada
            var repoType = typeof(TRepository);
            var repoInterface = repoType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>));

            if (repoInterface != null)
            {
                var entityType = repoInterface.GetGenericArguments()[0];

                // Usamos la lógica existente para obtener el repositorio de esa entidad
                var resolvedRepo = GetRepository(entityType);

                if (resolvedRepo is TRepository typedRepo)
                {
                    return typedRepo;
                }

                throw new InvalidOperationException(
                    $"El repositorio configurado para la entidad {entityType.Name} es {resolvedRepo.GetType().Name}, " +
                    $"pero se solicitó {repoType.Name}. Verifique la configuración del atributo [Repository].");
            }

            throw new InvalidOperationException(
                $"No se pudo resolver el repositorio {repoType.Name}. Asegúrese de que implemente IRepository<T>.");
        }
    }
}


