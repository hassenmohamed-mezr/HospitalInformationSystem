using HospitalInformationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalInformationSystem.Data
{
    /// <summary>
    /// Entity Framework DbContext for the hospital management system.
    /// Defines DbSets for all entities and configures relationships, indexes, and constraints.
    /// </summary>
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
        public DbSet<Appointment> Appointments { get; set; }

        /// <summary>
        /// Configures the model with relationships, unique indexes, and delete behaviors.
        /// Ensures data integrity for users, patients, doctors, and visits.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the entities.</param>
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

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.DoctorProfile)
                .WithMany(dp => dp.Appointments)
                .HasForeignKey(a => a.DoctorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for conflict checking
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorProfileId, a.AppointmentDateTime });
        }
    }
}
