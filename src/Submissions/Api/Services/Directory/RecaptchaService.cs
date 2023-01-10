using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Services.Directory;

public class RecaptchaService : IRecaptchaService
{
    private HttpClient RecaptchaClient { get; set; }
    private readonly SitePropertiesOptions _siteConfig;
    
    public RecaptchaService(SitePropertiesOptions siteConfig)
    {
      _siteConfig = siteConfig;
    }
  
    public Task<bool> ValidateRegisterEntity()
    { 
        var recaptchaKey = _siteConfig.GoogleRecaptchaPublicKey;
        var recaptchaSecret = _siteConfig.GoogleRecaptchaSecret;
        
        var enabled = !string.IsNullOrEmpty(recaptchaKey) && !string.IsNullOrEmpty(recaptchaSecret);
        
        if (enabled)
        {
          // To access TempData and ViewData
          Controller controller = context.Controller as Controller;
        
          var recaptchaResponse = context.HttpContext.Request.Form["g-recaptcha-response"];
          
          // No Recaptcha Response -> Throw Error
          if (recaptchaResponse == StringValues.Empty)
          {
            context.ModelState.AddModelError(
                  "ReCAPTCHA",
                  "The server is expecting a reCAPTCHA value for this form, but did not receive one.");
        
              context.Result = new ViewResult
              {
                  ViewName = context.RouteData.Values["action"].ToString(),
                  TempData = controller.TempData,
                  ViewData = controller.ViewData
              };
        
              return;
          }
        
          // Verify Response
          var verifyResponse = Task.Run(async () => await RecaptchaClient.PostAsync("",
                  new FormUrlEncodedContent(new[]
                  {
                      new KeyValuePair<string, string>("secret", recaptchaSecret),
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
              if (json.success != "true")
              {
                  ((List<string>)json.errorCodes)
                      .ForEach(error =>
                          controller.ViewData.ModelState.AddModelError(
                              "ReCAPTCHA",
                              error));
        
                  context.Result = new ViewResult
                  {
                      ViewName = context.RouteData.Values["action"].ToString(),
                      TempData = controller.TempData,
                      ViewData = controller.ViewData
                  };
              }
          }
          catch (RuntimeBinderException e)
          {
              throw new Exception("reCAPTCHA verification response was not in the expected format", e);
          }
          // Reaching End Means Valid Recaptcha
        }
    }
}
