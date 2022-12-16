using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Collections
{
  public abstract class AbstractCRUDSampleSetModel
  {
    private IList<MaterialDetailModel> _materialPreservationDetails;

    #region Form Collections
    public IEnumerable<ReferenceDataModel> Sexes { get; set; }
    public IEnumerable<ReferenceDataModel> AgeRanges { get; set; }
    public IEnumerable<ReferenceDataModel> DonorCounts { get; set; }
    public IEnumerable<ReferenceDataModel> MaterialTypes { get; set; }
    public IEnumerable<ReferenceDataModel> PreservationTypes { get; set; }
    public IEnumerable<ReferenceDataModel> StorageTemperatures { get; set; }
    public IEnumerable<ReferenceDataModel> Percentages { get; set; }
    public IEnumerable<ReferenceDataModel> MacroscopicAssessments { get; set; }
    public IEnumerable<OntologyTermModel> ExtractionProcedures { get; set; }
    #endregion

    #region Properties
    public int CollectionId { get; set; }

    [Required(ErrorMessage = "Please select a sex.")]
    [Display(Name = "Sex")]
    public int Sex { get; set; }

    [Required(ErrorMessage = "Please select an age range.")]
    [Display(Name = "Age range")]
    public int AgeRange { get; set; }

    [Required(ErrorMessage = "Please select a range for the number of donors.")]
    [Display(Name = "Number of donors")]
    public int DonorCountId { get; set; }
    public int DonorCountSliderPosition { get; set; }

    public string MaterialPreservationDetailsJson { get; set; } = "";
    public IList<MaterialDetailModel> MaterialPreservationDetails
    {
      get
      {
        return _materialPreservationDetails = _materialPreservationDetails ??
                                              (List<MaterialDetailModel>)JsonConvert.DeserializeObject(
                                                  MaterialPreservationDetailsJson, typeof(List<MaterialDetailModel>));
      }
    }

    public bool ShowMacroscopicAssessment { get; set; }

    #endregion

    public bool IsValid(ModelStateDictionary modelState)
    {
      if (modelState.IsValid)
      {
        foreach (var mpd in MaterialPreservationDetails)
        {
          if (mpd.materialType == 0 ||
              mpd.storageTemperature == 0 ||
              mpd.percentage == 0 ||
              mpd.macroscopicAssessment == 0)
          {
            modelState.AddModelError("",
                "Please make sure you have selected a value for all properties of each Material Preservation Detail.");

            return false;
          }
        }
      }
      else
      {
        return false;
      }

      return true;
    }
  }

  public class MaterialDetailModel
  {
    public int? id { get; set; }
    public int materialType { get; set; }
    public int? preservationType { get; set; }
    public int storageTemperature { get; set; }
    public int? percentage { get; set; }
    public int macroscopicAssessment { get; set; }
    public string extractionProcedure { get; set; }
  }
}
