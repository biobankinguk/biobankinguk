using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Settings;

public class SubmissionsModel
{
    #region Form Collection
    public IEnumerable<ReferenceDataModel> AccessConditions { get; set; }
    public IEnumerable<ReferenceDataModel> CollectionTypes { get; set; }

    #endregion

    #region Properties

    [HiddenInput(DisplayValue = false)]
    public int BiobankId { get; set; }

    [Required(ErrorMessage = "Please select an access condition.")]
    [Display(Name = "Access condition")]
    public int? AccessCondition { get; set; }

    [Required(ErrorMessage = "Please select a collection Type.")]
    [Display(Name = "Collection type")]
    public int? CollectionType { get; set; }

    [Display(Name = "Client ID")]
    public string ClientId { get; set; }

    #endregion
}
