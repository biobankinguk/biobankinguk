using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Upload.Controllers
{
    // TODO: This is a dummy controller that gives some identity info to prove auth is working
    // It doesn't need to be permanently kept around

    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
            => new JsonResult(User.Claims.Select(x => $"{x.Type}, {x.Value}"));
    }
}