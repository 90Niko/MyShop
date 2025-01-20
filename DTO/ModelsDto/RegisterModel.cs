using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace MyShop.DTO.ModelsDto
{
    public class RegisterModel
    {
        [Required]
        [MinLength(Constants.Constants.UserFirstNameMin)]
        [MaxLength(Constants.Constants.UserFirstNameMax)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(Constants.Constants.UserLastNameMin)]
        [MaxLength(Constants.Constants.UserLastNameMax)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Male { get; set; } = string.Empty;

        [Required]
        [Range(Constants.Constants.UserAgeMin, Constants.Constants.UserAgeMax)]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        [RegexStringValidator(Constants.Constants.EmailRegex)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
