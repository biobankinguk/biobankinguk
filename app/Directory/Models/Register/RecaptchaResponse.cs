using System.Collections.Generic;

namespace Biobanks.Directory.Models.Register;

public class RecaptchaResponse
{
    public bool Success { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
