using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Biobanks.Web.App_Start
{
    public class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {

            //Use Attribute routing
            config.MapHttpAttributeRoutes();

            // Configure JSON as default
            config.Formatters.Add(new BrowserJsonFormatter());
        }
        
        public class BrowserJsonFormatter : JsonMediaTypeFormatter
        {
            /* Custom formatter that supports JSON responses properly whilst still supporting XML if requested
             * https://stackoverflow.com/a/20556625
            */
            public BrowserJsonFormatter()
            {
                this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                this.SerializerSettings.Formatting = Formatting.Indented;
            }

            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}