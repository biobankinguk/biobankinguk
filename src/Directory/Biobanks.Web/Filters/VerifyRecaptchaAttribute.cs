using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;

namespace Biobanks.Web.Filters
{
    public class VerifyRecaptchaAttribute : ActionFilterAttribute
    {
        public HttpClient RecaptchaClient { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var recaptchaResponse = context.HttpContext.Request.Form["g-recaptcha-response"];

            if (recaptchaResponse is null)
            {
                context.Controller.ViewData.ModelState.AddModelError(
                    "ReCAPTCHA",
                    "The server is expecting a reCAPTCHA value for this form, but did not receive one.");

                context.Result = new ViewResult
                {
                    ViewName = context.RouteData.Values["action"].ToString(),
                    TempData = context.Controller.TempData,
                    ViewData = context.Controller.ViewData
                };

                return;
            }

            var verifyResponse = Task.Run(async () => await RecaptchaClient.PostAsync("",
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("secret",
                            ConfigurationManager.AppSettings["GoogleRecaptchaSecret"]),
                        new KeyValuePair<string, string>("response", recaptchaResponse)
                    })))
                .Result;

            verifyResponse.EnsureSuccessStatusCode();

            var json = Task.Run(async () =>
                    JsonConvert.DeserializeObject<dynamic>(
                        await verifyResponse.Content.ReadAsStringAsync()))
                .Result;

            try
            {
                if (json.success == "true") return; //all good, proceed as normal

                ((List<string>) json.errorCodes)
                    .ForEach(error =>
                        context.Controller.ViewData.ModelState.AddModelError(
                            "ReCAPTCHA",
                            error));

                context.Result = new ViewResult
                {
                    ViewName = context.RouteData.Values["action"].ToString(),
                    TempData = context.Controller.TempData,
                    ViewData = context.Controller.ViewData
                };
            }
            catch (RuntimeBinderException e)
            {
                throw new Exception("reCAPTCHA verification response was not in the expected format", e);
            }
        }
    }
}