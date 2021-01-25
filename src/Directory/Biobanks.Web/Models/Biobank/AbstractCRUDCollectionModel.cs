using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Services.Contracts;

namespace Biobanks.Web.Models.Biobank
{
    public abstract class AbstractCRUDCollectionModel : AbstractCRUDWithAssociatedDataModel
    {
        #region Form Collections

        public IEnumerable<ReferenceDataModel> AccessConditions { get; set; }
        public IEnumerable<ReferenceDataModel> CollectionTypes { get; set; }
        public IEnumerable<ReferenceDataModel> CollectionStatuses { get; set; }
        public IEnumerable<ReferenceDataModel> CollectionPoints { get; set; }
        public IEnumerable<ReferenceDataModel> HtaStatuses { get; set; }

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

        //[Required(ErrorMessage = "Please select a HTA status.")]
        [Display(Name = "HTA status")]
        public int? HTAStatus { get; set; }

        [Required(ErrorMessage = "Please select an access condition.")]
        [Display(Name = "Access condition")]
        public int AccessCondition { get; set; }

        [Display(Name = "Collection type")]
        public int? CollectionType { get; set; }

        [Required(ErrorMessage = "Please select a collection status.")]
        [Display(Name = "Collection status")]
        public int CollectionStatus { get; set; }

        [Required(ErrorMessage = "Please select a collection point.")]
        [Display(Name = "Collection point")]
        public int CollectionPoint { get; set; }

        [Required(ErrorMessage = "Please select a consent restriction.")]
        [Display(Name = "Consent restriction")]

        
        public IEnumerable<ConsentRestrictionModel> ConsentRestrictions { get; set; }
        #endregion

        #region Validation

        public async Task<bool> IsValid(ModelStateDictionary modelState, IBiobankReadService biobankReadService)
        {
            if (modelState != null && modelState.IsValid)
            {
                if (!await biobankReadService.ValidSnomedTermDescriptionAsync(Diagnosis))
                {
                    modelState.AddModelError("Diagnosis",
                        "Please enter a valid Diagnosis or select one from the type ahead results.");

                    return false;
                }
                
                if (AssociatedDataModelsValid())
                {
                    return true;
                }

                modelState.AddModelError("AssociatedData",
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
}
