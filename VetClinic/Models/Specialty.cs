using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models
{
    public class Specialty
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Specialty name is required.")]
        [StringLength(100)]
        [Display(Name = "Specialty")]
        public string Name { get; set; } = string.Empty;

        // Navigation: the vets who have this specialty
        public List<Veterinarian> Veterinarians { get; set; } = new();
    }
}