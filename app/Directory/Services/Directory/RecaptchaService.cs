using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Register;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Biobanks.Directory.Services.Directory;

public class RecaptchaService : IRecaptchaService
{
    private HttpClient RecaptchaClient { get; set; }
    private readonly SitePropertiesOptions _siteConfig;
    
    public RecaptchaService(IOptions<SitePropertiesOptions> siteConfigOptions)
    {
      _siteConfig = siteConfigOptions.Value;
    }
  
    public Task<RecaptchaResponse> VerifyToken(string recaptchaToken)
    { 
        var recaptchaKey = _siteConfig.GoogleRecaptchaPublicKey;
        var recaptchaSecret = _siteConfig.GoogleRecaptchaSecret;
        
        var enabled = !string.IsNullOrEmpty(recaptchaKey) && !string.IsNullOrEmpty(recaptchaSecret);
        
        if (enabled)
        {
            // No Recaptcha Response -> Throw Error
          if (recaptchaToken == StringValues.Empty)
          {
              return Task.FromResult(new RecaptchaResponse
              {
                  Success = false,
                  Errors = new List<string> {"The server is expecting a reCAPTCHA value for this form, but did not receive one."}
              });
          }
        
          // Verify Response
          var verifyResponse = Task.Run(async () => await RecaptchaClient.PostAsync("",
                  new FormUrlEncodedContent(new[]
                  {
                      new KeyValuePair<string, string>("secret", recaptchaSecret),
                      new KeyValuePair<string, string>("response", recaptchaToken)
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
                  var recaptchaResponse = new RecaptchaResponse
                  {
                      Success = false
                  };
                  
                  ((List<string>)json.errorCodes)
                      .ForEach(error =>
                          recaptchaResponse.Errors.Append(error));

                  return Task.FromResult(recaptchaResponse);
              }
          }
          catch (RuntimeBinderException e)
          {
              throw new Exception("reCAPTCHA verification response was not in the expected format", e);
          }
        }
        
        // Reaching End Means Valid Recaptcha
        return Task.FromResult(new RecaptchaResponse
        {
            Success = true
        });
    }
}
