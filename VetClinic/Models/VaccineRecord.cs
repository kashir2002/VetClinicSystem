using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models
{
    public class VaccineRecord : IValidatableObject
    {
        public int Id { get; set; }

        public int PetId { get; set; }
        public Pet? Pet { get; set; }

        [Required(ErrorMessage = "Vaccine name is required.")]
        [StringLength(100)]
        [Display(Name = "Vaccine")]
        public string VaccineName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date given")]
        public DateOnly DateGiven { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Next dose")]
        public DateOnly? NextDoseDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (DateGiven > today)
            {
                yield return new ValidationResult(
                    "Date given cannot be in the future.",
                    new[] { nameof(DateGiven) });
            }

            if (NextDoseDate != null && NextDoseDate <= DateGiven)
            {
                yield return new ValidationResult(
                    "Next dose must be after the date given.",
                    new[] { nameof(NextDoseDate) });
            }
        }
    }
}