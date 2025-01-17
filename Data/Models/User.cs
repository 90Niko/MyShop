using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace MyShop.Data.Models
{
    public class User: IdentityUser
    {
        
        [Required]
        public string FirstName { get; set; }= string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Male { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }
    }
}
