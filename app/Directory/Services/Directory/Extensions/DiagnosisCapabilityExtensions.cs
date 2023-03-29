using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Search.Dto.Documents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Services.Directory.Extensions
{
    public static class DiagnosisCapabilityExtensions
    {
        public static CapabilityDocument ToCapabilitySearchDocument(this DiagnosisCapability capability, IList<DonorCount> donorCounts)
        {
            //We'll need these
            var donorExpectation = GetAnnualDonorExpectationRange(
                donorCounts,
                capability.AnnualDonorExpectation);

            return new CapabilityDocument
            {
                Id = capability.DiagnosisCapabilityId,
                OntologyTerm = capability.OntologyTerm.Value,
                OntologyOtherTerms = SampleSetExtensions.ParseOtherTerms(capability.OntologyTerm.OtherTerms),
                BiobankId = capability.OrganisationId,
                BiobankExternalId = capability.Organisation.OrganisationExternalId,
                Biobank = capability.Organisation.Name,

                Networks = capability.Organisation.OrganisationNetworks
                    .Select(x => new NetworkDocument { Name = x.Network.Name }).ToList(),

                Protocols = capability.SampleCollectionMode.Value,

                AssociatedData = capability.AssociatedData
                    .Select(x => new AssociatedDataDocument
                    {
                        Text = x.AssociatedDataType.Value,
                        Timeframe = x.AssociatedDataProcurementTimeframe.Value,
                        TimeframeMetadata = JsonConvert.SerializeObject(new
                        {
                            Name = x.AssociatedDataProcurementTimeframe.Value,
                            x.AssociatedDataProcurementTimeframe.SortOrder
                        })
                    }),

                BiobankServices = capability.Organisation.OrganisationServiceOfferings
                    .Select(x => new BiobankServiceDocument
                    {
                        Name = x.ServiceOffering.Value
                    }),

                AnnualDonorExpectation = donorExpectation.Key,
                AnnualDonorExpectationMetadata = JsonConvert.SerializeObject(new
                {
                    Name = donorExpectation.Key,
                    SortOrder = donorExpectation.Value
                })
            };
        }

        public static KeyValuePair<string, int> GetAnnualDonorExpectationRange(
            IEnumerable<DonorCount> donorCounts,
            int annualDonorCount)
        {
            var donorCount = donorCounts.First(
                x => x.LowerBound <= annualDonorCount & (x.UpperBound >= annualDonorCount || x.UpperBound == null));
            return new KeyValuePair<string, int>(
                donorCount.Value,
                donorCount.SortOrder);
        }
    }
}
