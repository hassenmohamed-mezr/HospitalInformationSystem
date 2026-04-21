using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_UserViewModel
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Role { get; set; }

        public bool IsActive { get; set; }
    }
}