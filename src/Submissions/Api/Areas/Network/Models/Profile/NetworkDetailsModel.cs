using Biobanks.Submissions.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Areas.Network.Models.Profile
{
  public class NetworkDetailsModel
  {
    //non editable stuff that stays the same for edit / empty for create
    public int? NetworkId { get; set; } //nullable to allow for creation of new?

    [Required(ErrorMessage = "Please enter the name of the network.")]
    [MaxLength(100, ErrorMessage = ModelErrors.MaxLength)]
    [Display(Name = "Name")]
    public string NetworkName { get; set; }

    [Required(ErrorMessage = "Please enter a description of the network.")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }

    [DataType(DataType.Url)]
    [Display(Name = "URL")]
    public string Url { get; set; }

    [Required(ErrorMessage = "Please enter a contact email address.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [Display(Name = "Contact email")]
    public string ContactEmail { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile Logo { get; set; }
    public string LogoName { get; set; } //used to display an already stored logo
    public bool RemoveLogo { get; set; }

    //SOP Status
    public IEnumerable<KeyValuePair<int, string>> SopStatuses { get; set; } //radio options

    [Display(Name = "SOP status")]
    [Required(ErrorMessage = "Please select a SOP Status.")]
    public int SopStatus { get; set; } //radio value

    public string SopStatusDescription { get; set; } //the description of the current Sop Status, used for display (not edit)

    public bool ContactHandoverEnabled { get; set; }
    public string HandoverBaseUrl { get; set; }
    public string HandoverOrgIdsUrlParamName { get; set; }
    public bool MultipleHandoverOrdIdsParams { get; set; }
    public bool HandoverNonMembers { get; set; }
    public string HandoverNonMembersUrlParamName { get; set; }
  }
}
