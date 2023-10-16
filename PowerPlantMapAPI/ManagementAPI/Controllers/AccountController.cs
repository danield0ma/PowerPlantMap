using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public string GetUsers()
        {
            return "Admin";
        }

        [HttpGet]
        public string GetPassword()
        {
            return "Admin123";
        }
    }
}
