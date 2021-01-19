using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Biobanks.Web.ApiControllers
{
    public abstract class ApiBaseController : ApiController
    {

        protected IHttpActionResult JsonModelInvalidResponse(ModelStateDictionary state)
        {
            return Json(new
            {
                success = false,
                errors = state.Values
                    .Where(x => x.Errors.Count > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}