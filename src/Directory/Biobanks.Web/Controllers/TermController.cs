using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.Search;
using Directory.Services.Contracts;
using Biobanks.Web.Extensions;
using Directory.Data.Constants;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class TermController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;

        public TermController(IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
        }

        private async Task<TermPageModel> PopulateTermPageModel(TermPageModel model)
        {
            //Populate Term Page Info
            model.TermpageContentModel = new TermpageContentModel
            {
                PageInfo = Config.Get(ConfigKey.TermpageInfo, "")
            };

            
            // List of Unique Diagnoses With Sample Sets
            var snomedTerms = (await _biobankReadService.ListCollectionsAsync())
                .Where(x => x.SampleSets.Any())
                .GroupBy(x => x.SnomedTermId)
                .Select(x => x.First().SnomedTerm);
            
            // Populate Diagnoses Model
            var diagnosesModel = new SnomedTermModel
            {
                SnomedTerms = snomedTerms.Select(x =>
                        Task.Run(async () => new ReadDiagnosisModel
                        { 
                            SnomedTermId = x.Id,
                            Description = x.Description,
                            CollectionCapabilityCount = await _biobankReadService.GetSnomedTermCollectionCapabilityCount(x.Id),
                            OtherTerms = x.OtherTerms
                        })
                        .Result
                    )
                    .ToList()
            };

            model.DiagnosesModel = new List<SnomedTermModel> { diagnosesModel };

            return model;
        }


        // GET: Term
        public async Task<ActionResult> Index()
        {
            return View((TermPageModel)(await PopulateTermPageModel(new TermPageModel())));

        }
    }
}