using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace MyShop.Data.Models
{
    [Comment("User")]
    public class User: IdentityUser
    {
        
        [Required]
        [Comment("First name of the User")]
        public string FirstName { get; set; }= string.Empty;

        [Required]
        [Comment("Last name of the User")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Comment("Gender of the User")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Range(Constants.Constants.UserAgeMin,Constants.Constants.UserAgeMax)]
        [Comment("Age of the User")]
        public int Age { get; set; }
    }
}
