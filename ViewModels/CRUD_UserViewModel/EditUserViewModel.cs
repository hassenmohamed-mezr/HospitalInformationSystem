using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels.CRUD_UserViewModel
{
    /// <summary>
    /// View model for editing an existing user account in the hospital management system.
    /// Allows updating user details and roles without changing the password, managed by admin.
    /// </summary>
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