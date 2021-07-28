using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.Search;
using Biobanks.Services.Contracts;
using Biobanks.Web.Extensions;
using Biobanks.Directory.Data.Constants;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class TermController : ApplicationBaseController
    {
        private readonly IOntologyTermService _ontologyTermService;

        private readonly IBiobankReadService _biobankReadService;

        public TermController(
            IOntologyTermService ontologyTermService,
            IBiobankReadService biobankReadService)
        {
            _ontologyTermService = ontologyTermService;
            _biobankReadService = biobankReadService;
        }

        // GET: Term
        public async Task<ActionResult> Index()
        {
            // Term Page Info
            var termContentModel = new TermpageContentModel
            {
                PageInfo = Config.Get(ConfigKey.TermpageInfo, "")
            };

            // List of Unique Diagnoses With Sample Sets
            var ontologyTerms = (await _biobankReadService.ListCollectionsAsync())
                .Where(x => x.SampleSets.Any() && x.OntologyTerm.DisplayOnDirectory && !x.Organisation.IsSuspended)
                .GroupBy(x => x.OntologyTermId)
                .Select(x => x.First().OntologyTerm);

            var ontologyTermsModel = ontologyTerms.Select(x => 
                
                Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    CollectionCapabilityCount = await _ontologyTermService.GetCollectionCapabilityCount(x.Id),
                    OtherTerms = x.OtherTerms
                })
                .Result
            );

            return View(new TermPageModel
            {
                OntologyTermsModel = ontologyTermsModel,
                TermpageContentModel = termContentModel,
            });
        }
    }
}