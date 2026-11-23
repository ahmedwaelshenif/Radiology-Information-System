
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

            // Patient - User (one-to-one)
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithOne(u => u.Patient)
                    .HasForeignKey<Patient>(p => p.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.MedicalRecords)
                    .WithOne(m => m.Patient)
                    .HasForeignKey(m => m.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.TreatmentPlans)
                    .WithOne(t => t.Patient)
                    .HasForeignKey(t => t.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.RadiologyOrders)
                    .WithOne(o => o.Patient)
                    .HasForeignKey(o => o.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Invoices)
                    .WithOne(i => i.Patient)
                    .HasForeignKey(i => i.PatientId);

                entity.HasMany(p => p.Prescriptions)
                    .WithOne()
                    .HasForeignKey(prescription => prescription.PatientId);
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

            // User relationships with multiple dependent entity types
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.RecordedByUser)
                .WithMany()
                .HasForeignKey(m => m.RecordedByUserId);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId);

            modelBuilder.Entity<TreatmentPlan>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId);

            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(o => o.OrderedByUser)
                .WithMany()
                .HasForeignKey(o => o.OrderedByUserId);

            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(o => o.Study)
                .WithOne(s => s.Order)
                .HasForeignKey<RadiologyStudy>(s => s.OrderId)
                .IsRequired();

            // Order - invoice (one-to-one)
            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(o => o.Invoice)
                .WithOne(i => i.Order)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .IsRequired();

            // Study - report (one-to-one)
            modelBuilder.Entity<RadiologyStudy>()
                .HasOne(s => s.Report)
                .WithOne(r => r.Study)
                .HasForeignKey<RadiologyReport>(r => r.StudyId)
                .IsRequired();

            modelBuilder.Entity<RadiologyReport>()
                .HasOne(r => r.CreatedByUser)
                .WithMany()
                .HasForeignKey(r => r.CreatedByUserId);

            modelBuilder.Entity<RadiologyStudy>()
                .HasMany(s => s.CDSSResults)
                .WithOne(c => c.Study)
                .HasForeignKey(c => c.StudyId);

            modelBuilder.Entity<RadiologyOrder>()
                .HasMany(o => o.HL7Messages)
                .WithOne(h => h.Order)
                .HasForeignKey(h => h.OrderId);

            // Gender configuration
            modelBuilder.Entity<Patient>()
            .Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);


            modelBuilder.Entity<TreatmentPlan>()
                .HasMany(t => t.Prescriptions)
                .WithOne(p => p.TreatmentPlan)
                .HasForeignKey(p => p.TreatmentPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
