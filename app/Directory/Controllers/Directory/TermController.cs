using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Search;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Controllers.Directory;

[AllowAnonymous]
public class TermController : Controller
{
    private readonly IConfigService _configService;
    private readonly IOntologyTermService _ontologyTermService;
    private readonly ICollectionService _collectionService;

    public TermController(
        IConfigService configService,
        ICollectionService collectionService,
        IOntologyTermService ontologyTermService)
    {
        _configService = configService;
        _ontologyTermService = ontologyTermService;
        _collectionService = collectionService;
    }

    // GET: Term
    public async Task<ActionResult> Index()
    {
        // Term Page Info
        var termContentModel = new TermpageContentModel
        {
            PageInfo = await _configService.GetSiteConfigValue(ConfigKey.TermpageInfo)
        };

        // List of Unique Diagnoses With Sample Sets
        var ontologyTerms = (await _collectionService.List())
            .Where(x => x.SampleSets.Any() && x.OntologyTerm.DisplayOnDirectory && !x.Organisation.IsSuspended)
            .GroupBy(x => x.OntologyTermId)
            .Select(x => x.First().OntologyTerm);

        var ontologyTermsModel = ontologyTerms.Select(x => 
                
            Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    CollectionCapabilityCount = await _ontologyTermService.CountCollectionCapabilityUsage(x.Id),
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
