using System.Net;
using System.Web.Mvc;

namespace Biobanks.Web.Results
{
    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public HttpForbiddenResult() : base(HttpStatusCode.Forbidden) { }
    }
}