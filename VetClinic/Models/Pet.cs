using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClinic.Models
{
    public class Pet : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Species is required.")]
        [StringLength(50)]
        public string Species { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Breed { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateOnly BirthDate { get; set; }

        [Range(0.1, 200, ErrorMessage = "Weight must be between 0.1 and 200 kg.")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Weight (kg)")]
        public decimal WeightKg { get; set; }

        // Owner (one-to-many: one user has many pets)
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BirthDate > DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "Birth date cannot be in the future.",
                    new[] { nameof(BirthDate) });


            }
        }
        public List<VaccineRecord> VaccineRecords { get; set; } = new();

        public List<Appointment> Appointments { get; set; } = new();
    }
}