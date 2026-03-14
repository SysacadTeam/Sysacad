namespace Sysacad.Server.Services
{
    public interface ICurrentUserService
    {
        int? GetCurrentUserId();
        Task<Sysacad.Server.Data.Entities.Usuario?> GetCurrentUserAsync();
        bool IsAuthenticated();
    }
}