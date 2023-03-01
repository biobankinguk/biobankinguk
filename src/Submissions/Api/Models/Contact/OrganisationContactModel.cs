using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Contact;

public class OrganisationContactModel
{
  public string To { get; set; }
  public List<string> Ids { get; set; }
  public bool ContactMe { get; set; }

}
