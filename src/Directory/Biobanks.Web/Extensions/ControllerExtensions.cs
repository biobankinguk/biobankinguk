using Biobanks.Web.Results;
using System.Text;
using System.Web.Mvc;

namespace Biobanks.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static JsonpResult Jsonp(this Controller controller, object data, JsonRequestBehavior behavior)
        {
            return Jsonp(controller, data, null /* contentType */, null /* contentEncoding */, behavior);
        }

        public static JsonpResult Jsonp(this Controller controller, object data, string callback, JsonRequestBehavior behavior)
        {
            return Jsonp(controller, data, callback, null /* contentType */, null /* contentEncoding */, behavior);
        }

        public static JsonpResult Jsonp(this Controller controller, object data, string callback, string contentType, JsonRequestBehavior behavior)
        {
            return Jsonp(controller, data, callback, contentType, null /* contentEncoding */, behavior);
        }

        public static JsonpResult Jsonp(this Controller controller, object data, string callback = null, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            return new JsonpResult
            {
                Data = data,
                Callback = callback,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}
