using System.Collections.Generic;
using System.Linq;
using Entities.Data;
using Directory.Search.Dto.Documents;
using Newtonsoft.Json;

namespace Directory.Services.Extensions
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
                SnomedTerm = capability.SnomedTerm.Description,
                BiobankId = capability.OrganisationId,
                BiobankExternalId = capability.Organisation.OrganisationExternalId,
                Biobank = capability.Organisation.Name,

                Networks = capability.Organisation.OrganisationNetworks
                    .Select(x => new NetworkDocument {Name = x.Network.Name}).ToList(),

                Protocols = capability.SampleCollectionMode.Description,

                AssociatedData = capability.AssociatedData
                    .Select(x => new AssociatedDataDocument
                    {
                        Text = x.AssociatedDataType.Description,
                        Timeframe = x.AssociatedDataProcurementTimeframe.Description,
                        TimeframeMetadata = JsonConvert.SerializeObject(new
                        {
                            Name = x.AssociatedDataProcurementTimeframe.Description,
                            x.AssociatedDataProcurementTimeframe.SortOrder
                        })
                    }),

                BiobankServices = capability.Organisation.OrganisationServiceOfferings
                    .Select(x => new BiobankServiceDocument
                    {
                        Name = x.ServiceOffering.Name
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
                donorCount.Description,
                donorCount.SortOrder);
        }
    }
}
