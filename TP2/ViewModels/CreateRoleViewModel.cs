using System.ComponentModel.DataAnnotations;

namespace TP3_Identity.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; } = string.Empty;
    }
}