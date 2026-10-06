using System.ComponentModel.DataAnnotations;

namespace TP3_Identity.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; } = string.Empty;

        public List<string> Users { get; set; }
    }
}