using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VetClinic.ViewModels
{
    public class AppointmentFormViewModel
    {
        [Required(ErrorMessage = "Please choose a pet.")]
        [Display(Name = "Pet")]
        public int? PetId { get; set; }

        [Required(ErrorMessage = "Please choose a service.")]
        [Display(Name = "Service")]
        public int? ServiceId { get; set; }

        [Required(ErrorMessage = "Please choose a veterinarian.")]
        [Display(Name = "Veterinarian")]
        public int? VeterinarianId { get; set; }

        [Required(ErrorMessage = "Please choose a date.")]
        [DataType(DataType.Date)]
        public DateOnly? Date { get; set; }

        [Required(ErrorMessage = "Please choose a time.")]
        [DataType(DataType.Time)]
        public TimeOnly? Time { get; set; }

        // Options for the form
        public List<SelectListItem> Pets { get; set; } = new();
        public List<SelectListItem> Services { get; set; } = new();
        public List<Veterinarian> Vets { get; set; } = new();
    }
}