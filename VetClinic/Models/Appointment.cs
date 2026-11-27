using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    public enum AppointmentStatus
    {
        Pending,    // waiting for approval (every new appointment starts here)
        Approved,
        Rejected,
        Completed,
        Cancelled
    }

    public class Appointment
    {
        public int Id { get; set; }

        public int PetId { get; set; }
        public Pet? Pet { get; set; }

        public int VeterinarianId { get; set; }
        public Veterinarian? Veterinarian { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        [Display(Name = "Start")]
        public DateTime StartTime { get; set; }

        // Copied from the service at booking time
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [StringLength(2000)]
        [Display(Name = "Exam note")]
        public string? ExamNote { get; set; }

        public DateTime CreatedAt { get; set; }

        // Calculated, not stored in the database
        [NotMapped]
        public DateTime EndTime => StartTime.AddMinutes(DurationMinutes);
    }
}