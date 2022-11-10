using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Search.Dto.Documents;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Extensions;
using Hangfire;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Services.Directory;

public class CapabilityService
{

    private readonly BiobanksDbContext _db;
    private readonly IIndexProvider _indexProvider;
    private readonly DonorCountService _donorCountService;

    public CapabilityService(
        BiobanksDbContext db,
        IIndexProvider indexProvider,
        DonorCountService donorCountService
        )
    {
        _db = db;
        _indexProvider = indexProvider;
        _donorCountService = donorCountService;
    }

    // Formerly in index service.
    public async Task UpdateCapabilitiesOntologyOtherTerms(string ontologyTerm)
    {
        // Get the capabilities with the ontologyTerm.
        var capabilityIds = await _biobankReadService.GetCapabilityIdsByOntologyTermAsync(ontologyTerm);
        // Update all search documents that are relevant to this collection.
        foreach (var capabilityId in capabilityIds)
        {
            await UpdateCapabilityDetails(capabilityId);
        }
    }

    // Formerly in index service.
    public async Task UpdateCapabilityDetails(int capabilityId)
        {
            // Get the entire capability object from the database.
            var updatedCapability = await _biobankReadService.GetCapabilityByIdForIndexingAsync(capabilityId);

            // Get the donor counts and get expectations from them
            var donorExpectation = DiagnosisCapabilityExtensions.GetAnnualDonorExpectationRange(
                        await _donorCountService.List(),
                        updatedCapability.AnnualDonorExpectation);

            //Prep metadata for the facet value
            var donorExpectationMetadata = JsonConvert.SerializeObject(new
            {
                Name = donorExpectation.Key,
                SortOrder = donorExpectation.Value
            });

            // Queue up a job to update the capability in the search index.
            BackgroundJob.Enqueue(() => _indexProvider.UpdateCapabilitySearchDocument(
                updatedCapability.DiagnosisCapabilityId,
                new PartialCapability
                {
                    OntologyTerm = updatedCapability.OntologyTerm.Value,
                    Protocols = updatedCapability.SampleCollectionMode.Value,
                    AnnualDonorExpectation = donorExpectation.Key,
                    AnnualDonorExpectationMetadata = donorExpectationMetadata,
                    AssociatedData = updatedCapability.AssociatedData.Select(ad => new AssociatedDataDocument
                    {
                        Text = ad.AssociatedDataType.Value,
                        Timeframe = ad.AssociatedDataProcurementTimeframe.Value,
                        TimeframeMetadata = JsonConvert.SerializeObject(new
                        {
                            Name = ad.AssociatedDataProcurementTimeframe.Value,
                            ad.AssociatedDataProcurementTimeframe.SortOrder
                        })
                    }),
                    OntologyOtherTerms = SampleSetExtensions.ParseOtherTerms(updatedCapability.OntologyTerm.OtherTerms)
                }));
        }
    
}
