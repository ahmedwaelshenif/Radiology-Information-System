
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
        public DbSet<StudyUID> StudyUIDs => Set<StudyUID>();
        public DbSet<Price> Prices { get; set; }

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

            // Seed the available DICOM study instance UIDs by modality.
            modelBuilder.Entity<StudyUID>().HasData(
                new StudyUID { Id = 1, Modality = Domain.Enums.Modality.XRay, StudyInstanceUID = "1.3.6.1.4.1.14519.5.2.1.9999.103.2003757953720308493720544525327" },
                new StudyUID { Id = 2, Modality = Domain.Enums.Modality.XRay, StudyInstanceUID = "1.2.392.200036.9125.0.199302241758.16" },
                new StudyUID { Id = 3, Modality = Domain.Enums.Modality.CT, StudyInstanceUID = "1.3.6.1.4.1.14519.5.2.1.6919.4624.107720852982298931479046337475" },
                new StudyUID { Id = 4, Modality = Domain.Enums.Modality.CT, StudyInstanceUID = "1.3.6.1.4.1.14519.5.2.1.6919.4624.508771521556732938904058410751" },
                new StudyUID { Id = 5, Modality = Domain.Enums.Modality.Mammography, StudyInstanceUID = "1.3.6.1.4.1.14519.5.2.1.1239.1759.860502043886043938075747571015" },
                new StudyUID { Id = 6, Modality = Domain.Enums.Modality.MRI, StudyInstanceUID = "1.2.840.4892943.343.20240528040031.5710.1656205" },
                new StudyUID { Id = 7, Modality = Domain.Enums.Modality.MRI, StudyInstanceUID = "1.3.46.670589.11.84425.5.0.14744.2025051016220214026" },
                new StudyUID { Id = 8, Modality = Domain.Enums.Modality.MRI, StudyInstanceUID = "1.2.840.4892943.343.20250413070448.1553.2122473" },
                new StudyUID { Id = 9, Modality = Domain.Enums.Modality.Ultrasound, StudyInstanceUID = "1.3.6.1.4.1.14519.5.2.1.1.23350688958676093611472873878291718902" },
                new StudyUID { Id = 10, Modality = Domain.Enums.Modality.Ultrasound, StudyInstanceUID = "1.3.6.1.4.1.14519.5.2.1.205603453180402812780236219545985083918" }
            );

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
                .HasForeignKey(m => m.RecordedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreatmentPlan>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RadiologyOrder>()
                .HasOne(o => o.OrderedByUser)
                .WithMany()
                .HasForeignKey(o => o.OrderedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

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
