using System.Security.Cryptography;
using System.Text;

namespace HospitalInformationSystem.Helpers
{
    /// <summary>
    /// Provides password hashing and verification utilities for secure authentication.
    /// Uses SHA256 hashing to protect user passwords in the hospital system.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashes a plain-text password using SHA256.
        /// Returns a hexadecimal string representation of the hash.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The hashed password as a string.</returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies a plain-text password against a stored hash.
        /// Compares the hash of the entered password with the stored hash.
        /// </summary>
        /// <param name="enteredPassword">The plain-text password entered by the user.</param>
        /// <param name="storedHash">The hashed password stored in the database.</param>
        /// <returns>True if the passwords match, false otherwise.</returns>
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            string enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }
}