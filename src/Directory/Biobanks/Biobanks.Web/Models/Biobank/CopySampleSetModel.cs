using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Biobank
{
    public class CopySampleSetModel : AbstractCRUDSampleSetModel
    {
        /// <summary>
        /// The ID of the sampleset this one is being copied from
        /// </summary>
        public int OriginalId { get; set; }
    }
}