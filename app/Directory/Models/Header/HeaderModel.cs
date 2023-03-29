using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Header
{
    public class HeaderModel
    {
        public HeaderItemModel Logo { get; set; }

        public HeaderItemModel Link { get; set; }

        public IEnumerable<NavItemModel> NavigationItems { get; set; }
    }
}