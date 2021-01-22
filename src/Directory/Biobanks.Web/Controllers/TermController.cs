using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.Search;
using Biobanks.Directory.Services.Contracts;
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

        // GET: Term
        public async Task<ActionResult> Index()
        {
            // Term Page Info
            var termContentModel = new TermpageContentModel
            {
                PageInfo = Config.Get(ConfigKey.TermpageInfo, "")
            };

            // List of Unique Diagnoses With Sample Sets
            var snomedTerms = (await _biobankReadService.ListCollectionsAsync())
                .Where(x => x.SampleSets.Any())
                .GroupBy(x => x.SnomedTermId)
                .Select(x => x.First().SnomedTerm);

            var snomedTermsModel = snomedTerms.Select(x => 
                
                Task.Run(async () => new ReadSnomedTermModel
                {
                    SnomedTermId = x.Id,
                    Description = x.Description,
                    CollectionCapabilityCount = await _biobankReadService.GetSnomedTermCollectionCapabilityCount(x.Id),
                    OtherTerms = x.OtherTerms
                })
                .Result
            );

            return View(new TermPageModel
            {
                SnomedTermsModel = snomedTermsModel,
                TermpageContentModel = termContentModel,
            });
        }
    }
}