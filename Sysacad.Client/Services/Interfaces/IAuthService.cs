using Sysacad.Shared.Auth;

namespace Sysacad.Client.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task LogoutAsync(bool notifyServer = true, CancellationToken cancellationToken = default);
    }
}