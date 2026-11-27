using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service name is required.")]
        [StringLength(100)]
        [Display(Name = "Service name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes.")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Range(0, 100000, ErrorMessage = "Price cannot be negative.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Navigation: the vets who can perform this service
        public List<Veterinarian> Veterinarians { get; set; } = new();

        public List<Appointment> Appointments { get; set; } = new();
    }
}