using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models
{
    public class Veterinarian
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        // Link to the login account (used later, so a vet can log in)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Many-to-many navigations
        public List<Specialty> Specialties { get; set; } = new();
        public List<Service> Services { get; set; } = new();
    }
}