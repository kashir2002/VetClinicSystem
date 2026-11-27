using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetClinic.Models;

namespace VetClinic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Veterinarian> Veterinarians { get; set; }

        public DbSet<Availability> Availabilities { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<VaccineRecord> VaccineRecords { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // keep this first: Identity needs it

            // Don't allow deleting a service or vet that has appointments
            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Veterinarian)
                .WithMany(v => v.Appointments)
                .HasForeignKey(a => a.VeterinarianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
