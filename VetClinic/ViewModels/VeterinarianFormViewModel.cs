using System.ComponentModel.DataAnnotations;

namespace VetClinic.ViewModels
{
    public class VeterinarianFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        // Ids of the ticked checkboxes (sent back by the form)
        public List<int> SelectedSpecialtyIds { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();

        // All options, to draw the checkboxes
        public List<Specialty> AllSpecialties { get; set; } = new();
        public List<Service> AllServices { get; set; } = new();
    }
}