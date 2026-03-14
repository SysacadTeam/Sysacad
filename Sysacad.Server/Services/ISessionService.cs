namespace Sysacad.Server.Services
{
    /// <summary>
    /// Defines a contract for session management services that provide asynchronous validation of user credentials.
    /// </summary>
    /// <remarks>Implementations of this interface should securely handle sensitive information such as
    /// passwords. The validation method returns a user identifier if the credentials are valid, or an error message if
    /// validation fails. This interface is intended to be used in authentication workflows where user identity
    /// verification is required.</remarks>
    public interface ISessionService
    {
        /// <summary>
        /// Asynchronously validates the specified username and password against the authentication system.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to verify user credentials. Callers
        /// should ensure that the username and password meet any requirements imposed by the authentication provider.
        /// Handle potential errors appropriately, as the returned error message may indicate the reason for
        /// authentication failure.</remarks>
        /// <param name="username">The username of the user attempting to authenticate. This parameter cannot be null or empty.</param>
        /// <param name="password">The password associated with the specified username. This parameter cannot be null or empty.</param>
        /// <returns>A tuple containing the user ID if the credentials are valid; otherwise, null. An error message is returned
        /// if the validation fails.</returns>
        Task<(int? userId, string? errorMessage)> ValidateCredentialsAsync(string username, string password);
    }
}