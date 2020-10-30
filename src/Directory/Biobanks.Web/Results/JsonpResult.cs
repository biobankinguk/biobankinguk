using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Biobanks.Web.Results
{
    public class JsonpResult : JsonResult
    {
        public string Callback { get; set; }

        public JsonpResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public JsonpResult(string callback = "")
        {
            Callback = callback;
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //no callback specified? behave like a normal JsonResult
            if (string.IsNullOrEmpty(Callback)) base.ExecuteResult(context);
            else ExecuteJsonpResult(context);
        }

        private void ExecuteJsonpResult(ControllerContext context)
        {
            //This code taken from base.ExecuteResult, but modified to perform the callback wrapper
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request.To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/x-javascript";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data == null) return;

            var serializer = new JavaScriptSerializer();
            if (MaxJsonLength.HasValue)
            {
                serializer.MaxJsonLength = MaxJsonLength.Value;
            }
            if (RecursionLimit.HasValue)
            {
                serializer.RecursionLimit = RecursionLimit.Value;
            }
            response.Write($"{Callback}({serializer.Serialize(Data)});");
        }
    }
}
