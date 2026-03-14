using BCrypt.Net;

namespace Sysacad.Server.Services
{
    /// <summary>
    /// Provides functionality for securely hashing and verifying passwords using the BCrypt algorithm.
    /// </summary>
    /// <remarks>This class implements the IPasswordHasher interface to facilitate secure password storage and
    /// validation. It uses a configurable work factor to increase computational cost, helping to protect against
    /// brute-force attacks. The hashing and verification methods are suitable for use in authentication systems where
    /// password security is critical.</remarks>
    public class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 11;

        /// <summary>
        /// Hashes a plain text password using the BCrypt algorithm.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>A BCrypt hashed representation of the password.</returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }
        /// <summary>
        /// Verifies whether the specified plain text password matches the provided password hash using the BCrypt
        /// algorithm. 
        /// </summary>
        /// <remarks>If an error occurs during verification, such as an invalid hash format, the method
        /// returns false instead of throwing an exception.</remarks>
        /// <param name="password">The plain text password to verify against the stored hash. Cannot be null.</param>
        /// <param name="passwordHash">The hashed password to compare the plain text password against. Cannot be null.</param>
        /// <returns>true if the password matches the hash; otherwise, false.</returns>
        public bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }
    }
}