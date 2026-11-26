using System.ComponentModel.DataAnnotations;

namespace VetClinic.Models
{
    public class Availability : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Veterinarian")]
        public int VeterinarianId { get; set; }
        public Veterinarian? Veterinarian { get; set; }

        [Display(Name = "Day")]
        public DayOfWeek DayOfWeek { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Start time")]
        public TimeOnly StartTime { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "End time")]
        public TimeOnly EndTime { get; set; }

        // Custom validation: end must be after start
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "End time must be after start time.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}