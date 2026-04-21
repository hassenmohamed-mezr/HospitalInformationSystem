using System.ComponentModel.DataAnnotations;

namespace HospitalInformationSystem.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public required string UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}