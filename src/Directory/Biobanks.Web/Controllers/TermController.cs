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

            //Populate Diagnoses Model
            // List of Unique Diagnoses With Sample Sets
            var diagnoses = (await _biobankReadService.ListCollectionsAsync())
                .Where(x => x.SampleSets.Any())
                .GroupBy(x => x.DiagnosisId)
                .Select(x => x.First().Diagnosis);

            model.DiagnosesModel = new List<DiagnosesModel>();

                var diagnosesModel = new DiagnosesModel
                {
                    Diagnoses = diagnoses
                        .Select(x =>
                        Task.Run(async () => new ReadDiagnosisModel
                    {
                        Id = x.DiagnosisId,
                        SnomedIdentifier = x.SnomedIdentifier,
                        Description = x.Description,
                        CollectionCapabilityCount = await _biobankReadService.GetDiagnosisCollectionCapabilityCount(x.DiagnosisId),
                        OtherTerms = x.OtherTerms
                    })
                    .Result
                    )
                    .ToList()
                };
                model.DiagnosesModel.Add(diagnosesModel);

        
            return model;
        }


        // GET: Term
        public async Task<ActionResult> Index()
        {
            return View((TermPageModel)(await PopulateTermPageModel(new TermPageModel())));

        }
    }
}