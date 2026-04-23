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
        public DbSet<Visit> Visits { get; set; }

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

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.DoctorProfile)
                .WithMany(dp => dp.Visits)
                .HasForeignKey(v => v.DoctorProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
