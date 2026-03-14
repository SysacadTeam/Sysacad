namespace Sysacad.Server.Data.Attributes
{
    /// <summary>
    /// Atributo para especificar el repositorio personalizado de una entidad
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RepositoryAttribute : Attribute
    {
        public Type RepositoryType { get; }

        public RepositoryAttribute(Type repositoryType)
        {
            RepositoryType = repositoryType;
        }
    }
}
