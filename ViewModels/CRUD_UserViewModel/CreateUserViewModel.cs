using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_UserViewModel
{
    /// <summary>
    /// View model for creating a new user account in the hospital management system.
    /// Captures user details and assigns roles for authentication and authorization, managed by admin.
    /// </summary>
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