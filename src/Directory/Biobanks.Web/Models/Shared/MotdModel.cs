using System.Web.Mvc;

namespace Biobanks.Web.Models.Shared
{
    public class MotdModel
    {
        public string Id { get; set; }

        [AllowHtml]
        public string Message { get; set; }
        public bool Active { get; set; }
    }
}