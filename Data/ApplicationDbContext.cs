using HospitalInformationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.NationalId)
                .IsUnique();

            modelBuilder.Entity<DoctorProfile>()
                .HasOne(dp => dp.User)
                .WithOne()
                .HasForeignKey<DoctorProfile>(dp => dp.UserId);

            modelBuilder.Entity<DoctorProfile>()
                .HasIndex(dp => dp.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);
        }
    }
}
