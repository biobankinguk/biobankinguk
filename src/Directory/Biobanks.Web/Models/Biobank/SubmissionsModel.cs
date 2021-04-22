using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Models.Biobank
{
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

        [Display(Name = "Public Key")]
        public string PublicKey { get; set; }

        #endregion
    }
}