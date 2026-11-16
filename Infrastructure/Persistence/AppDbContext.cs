
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Radiologist> Radiologists => Set<Radiologist>();
        public DbSet<Receptionist> Receptionists => Set<Receptionist>();
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<RadiologyOrder> RadiologyOrders { get; set; }
        public DbSet<RadiologyStudy> RadiologyStudies { get; set; }
        public DbSet<RadiologyReport> RadiologyReports { get; set; }
        public DbSet<CDSSResult> CDSSResults { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<HL7Message> HL7Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired();
            });

            // Patient - User (One-to-One)
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithOne(u => u.Patient)
                    .HasForeignKey<Patient>(p => p.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Radiologist - User (One-to-One)
            modelBuilder.Entity<Radiologist>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithOne(u => u.Radiologist)
                    .HasForeignKey<Radiologist>(r => r.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => r.LicenseNumber)
                    .IsUnique();

                entity.Property(r => r.LicenseNumber)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Receptionist - User (One-to-One)
            modelBuilder.Entity<Receptionist>(entity =>
            {
                entity.HasOne(r => r.User)
                    .WithOne(u => u.Receptionist)
                    .HasForeignKey<Receptionist>(r => r.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // order - invoice (one to one)
            modelBuilder.Entity<Invoice>().HasIndex(i => i.OrderId).IsUnique();

            // report - study (one to one)

            modelBuilder.Entity<RadiologyReport>().HasIndex(r => r.StudyId).IsUnique();

            // Gender configuration
            modelBuilder.Entity<Patient>()
            .Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);
        }
    }
}
