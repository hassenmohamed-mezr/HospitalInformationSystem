using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels
{
    /// <summary>
    /// View model for user login in the hospital management system.
    /// Captures credentials for authentication across all user roles.
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        public required string UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}