using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyShop.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok("Welcome to the admin dashboard!");
        }

        [HttpGet("manage-users")]
        public IActionResult ManageUsers()
        {
            var users = new List<string> { "User1", "User2", "User3" };
            return Ok(users);
        }
    }
}
