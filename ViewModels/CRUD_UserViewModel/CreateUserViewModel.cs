using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_UserViewModel
{
    public class CreateUserViewModel
    {
        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}