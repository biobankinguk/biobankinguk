using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Upload.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
            => new JsonResult(User.Claims.Select(x => new { x.Type, x.Value }));
    }
}