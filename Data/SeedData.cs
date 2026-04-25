using HospitalInformationSystem.Helpers;
using HospitalInformationSystem.Models;

namespace HospitalInformationSystem.Data
{
    /// <summary>
    /// Provides data seeding functionality for the hospital management system.
    /// Initializes default user accounts for admin, doctor, and reception roles.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Seeds the database with initial user data if no users exist.
        /// Creates default accounts for system testing and initial access.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        FullName = "System Admin",
                        Username = "admin",
                        Email = "admin@his.com",
                        PasswordHash = PasswordHelper.HashPassword("123456"),
                        Role = "Admin",
                        IsActive = true
                    },
                    new User
                    {
                        FullName = "Doctor User",
                        Username = "doctor1",
                        Email = "doctor1@his.com",
                        PasswordHash = PasswordHelper.HashPassword("123456"),
                        Role = "Doctor",
                        IsActive = true
                    },
                    new User
                    {
                        FullName = "Reception User",
                        Username = "reception1",
                        Email = "reception1@his.com",
                        PasswordHash = PasswordHelper.HashPassword("123456"),
                        Role = "Reception",
                        IsActive = true
                    }
                );

                context.SaveChanges();
            }
        }
    }
}