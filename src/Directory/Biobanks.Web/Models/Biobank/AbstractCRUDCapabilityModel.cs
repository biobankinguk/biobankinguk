using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Services.Contracts;
using DataAnnotationsExtensions;

namespace Biobanks.Web.Models.Biobank
{
    public class AbstractCRUDCapabilityModel : AbstractCRUDWithAssociatedDataModel
    {
        #region Properties
        [Required(ErrorMessage = "Please enter a valid disease status or select one from the type ahead results.")]
        [Display(Name = "Disease status")]
        public string Diagnosis { get; set; }

        [Display(Name = "Bespoke consent form")]
        public bool BespokeConsentForm { get; set; }

        [Display(Name = "Bespoke SOP")]
        public bool BespokeSOP { get; set; }

        [Required(ErrorMessage = "Please enter the number of expected donors per year.")]
        [Min(1, ErrorMessage = "Please enter a number greater than 0.")]
        [Integer(ErrorMessage = "Please enter an integer.")]
        [Display(Name = "Annual donor expectation")]
        public int? AnnualDonorExpectation { get; set; }
        #endregion

        #region Validation
        public async Task<bool> IsValid(ModelStateDictionary modelState, IBiobankReadService biobankReadService)
        {
            if (modelState != null && modelState.IsValid)
            {
                if (!await biobankReadService.ValidOntologyTermDescriptionAsync(Diagnosis))
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
}
