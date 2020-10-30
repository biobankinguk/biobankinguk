using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Directory.Data.Migrations;
using Directory.Services.Contracts;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class BlobController : Controller
    {
        private readonly IBiobankReadService _biobankReadService;

        public BlobController(IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string resourceName)
        {
            try
            {
                var blob = await _biobankReadService.GetBlobAsync(resourceName);
                return File(blob.Content, blob.ContentType);
            }
            catch
            {
                throw new HttpException(404, "The requested logo resource could not be found.");
            }
        }
    }
}
