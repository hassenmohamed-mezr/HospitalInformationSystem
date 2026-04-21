using HospitalInformationSystem.Helpers;
using HospitalInformationSystem.Models;

namespace HospitalInformationSystem.Data
{
    public static class SeedData
    {
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