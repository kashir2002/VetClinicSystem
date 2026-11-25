using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VetClinic.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;
    }
}