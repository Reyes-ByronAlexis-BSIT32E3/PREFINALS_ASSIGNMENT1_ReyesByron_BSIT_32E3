using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProtectedApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        [Authorize]
        [HttpGet("userinfo")]
        public IActionResult UserInfo()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { Username = username, Section = "A", Course = "Computer Science" });
        }

        [Authorize]
        [HttpGet("funfacts")]
        public IActionResult FunFacts()
        {

            string[] facts = {
                "I enjoy hiking in my free time.",
                "I am passionate about machine learning.",
                "I love experimenting with new programming languages."
            
            };

            return Ok(new { FunFacts = facts });
        }
    }
}
