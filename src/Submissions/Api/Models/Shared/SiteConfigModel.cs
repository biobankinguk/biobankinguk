using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared;

public class SiteConfigModel
{
    [Required]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool ReadOnly { get; set; }

    public bool? IsFeatureFlag { get; set; }
    
}
