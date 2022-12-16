using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Constants;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Collections;

public class AbstractCRUDCollectionModel : AbstractCRUDWithAssociatedDataModel
{
  #region Form Collections

  public IEnumerable<ReferenceDataModel> AccessConditions { get; set; }
  public IEnumerable<ReferenceDataModel> CollectionTypes { get; set; }
  public IEnumerable<ReferenceDataModel> CollectionStatuses { get; set; }

  #endregion

  #region Properties
  [Required(ErrorMessage = "Please enter a valid disease status or select one from the type ahead results.")]
  [Display(Name = "Disease status")]
  public string Diagnosis { get; set; }

  [StringLength(250, ErrorMessage = "The collection title can't be more than 250 characters in length.")]
  [Display(Name = "Title")]
  public string Title { get; set; }

  [Display(Name = "Description")]
  public string Description { get; set; }

  [Display(Name = "FromApi")]
  public bool FromApi { get; set; }

  [Required(ErrorMessage = "Please enter the year in which the collection was started.")]
  [Range(1000, 9999, ErrorMessage = "Please enter a valid 4-digit year.")]
  [Display(Name = "Year started")]
  public int? StartDate { get; set; }

  [Required(ErrorMessage = "Please select an access condition.")]
  [Display(Name = "Access condition")]
  public int AccessCondition { get; set; }

  [Display(Name = "Collection type")]
  public int? CollectionType { get; set; }

  [Required(ErrorMessage = "Please select a collection status.")]
  [Display(Name = "Collection status")]
  public int CollectionStatus { get; set; }

  [Required(ErrorMessage = "Please select a consent restriction.")]
  [Display(Name = "Consent restriction")]


  public IEnumerable<ConsentRestrictionModel> ConsentRestrictions { get; set; }
  #endregion

  #region Validation

  public async Task<bool> IsValid(ModelStateDictionary modelState, IOntologyTermService ontologyTermService)
  {
    if (modelState != null && modelState.IsValid)
    {
      var valid = await ontologyTermService.Exists(value: Diagnosis, tags: new List<string>
                {
                    SnomedTags.Disease, SnomedTags.Finding
                });

      if (!valid)
      {
        modelState.AddModelError("Diagnosis",
            "Please enter a valid Diagnosis or select one from the type ahead results.");

        return false;
      }

      if (AssociatedDataModelsValid())
      {
        return true;
      }

      modelState.AddModelError("Groups",
          "Please make sure you have selected a provision time for all selected data types.");
    }

    return false;
  }

  #endregion
}

public class ConsentRestrictionModel
{
  public int ConsentRestrictionId { get; set; }
  public string Description { get; set; }
  public bool Active { get; set; }
}

