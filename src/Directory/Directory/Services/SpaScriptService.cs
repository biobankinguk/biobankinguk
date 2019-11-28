using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Directory.Services
{
    /// <summary>
    /// This is a hilarious hack dressed up as a sane service
    /// designed to get the <script> tags from an SPA entrypoint file
    /// so we can use them in our own entrypoint files to load the bundled scripts
    /// </summary>
    public class SpaScriptService
    {
        private readonly IWebHostEnvironment _env;

        private const string _staticFilesPath = "ClientApp/build"; // TODO: find a nice way to inject this

        public SpaScriptService(IWebHostEnvironment env, IHttpClientFactory httpClient)
        {
            _env = env;
            _httpClient = httpClient;
        }

        private string scripts = string.Empty;
        private readonly IHttpClientFactory _httpClient;

        public async Task<string> GetScripts(Uri origin)
        {
            try
            {
                // TODO: a way of knowing when Webpack has rebuilt would be nice?
                // for now we make this request EVERY Razor Page load for Dev environment...
                if (string.IsNullOrWhiteSpace(scripts) || _env.IsDevelopment())
                    scripts = ParseHtmlForScripts(await ReadHtml(origin));

                return scripts;
            }
            catch (FileNotFoundException e)
            {
                return $"<script>console.error(\"Unable to find SPA {e.FileName} in {_staticFilesPath}\");</script>";
            }
        }


        private async Task<string> ReadHtml(Uri origin)
            => _env.IsDevelopment()
                ? await ReadHtmlFromDevServer(origin)
                : ReadHtmlFromStaticFile();

        /// <summary>
        /// Get all <script> tags (which don't have `nomodule` present)
        /// and concatenate them (in order) into a single string.
        /// </summary>
        /// <param name="html">the html document to parse</param>
        private string ParseHtmlForScripts(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var scripts = doc.DocumentNode.SelectNodes("//script[not(@nomodule)]");
            var sb = new StringBuilder();
            foreach (var s in scripts)
            {
                sb.Append(s.OuterHtml);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Fetch the index page as served by the SPA Static Files middleware
        /// e.g. from a Webpack Dev Server
        /// </summary>
        /// <param name="origin">
        /// We are making a request relative to the root of this app,
        /// so we need the origin to come from somewhere that has access to
        /// the ASP.NET Core Request Context
        /// </param>
        private async Task<string> ReadHtmlFromDevServer(Uri origin)
            => await _httpClient.CreateClient().GetStringAsync(new Uri(origin, "/index.html"));

        /// <summary>
        /// Read the index page directly from disk, since we know in a non-Dev environment
        /// it will be on disk, and this is cheaper than requesting it through the HTTP pipeline
        /// </summary>
        private string ReadHtmlFromStaticFile()
        {
            var filename = "index.html";
            var fileInfo = _env.ContentRootFileProvider
                .GetFileInfo(Path.Combine(_staticFilesPath, filename));

            if (!fileInfo.Exists || fileInfo.IsDirectory)
                throw new FileNotFoundException($"FullPath: {fileInfo.PhysicalPath}", filename);

            return File.ReadAllText(fileInfo.PhysicalPath);
        }
    }
}
