using System.Collections.Generic;
using System.Linq;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Search.Dto.Documents;
using Newtonsoft.Json;

namespace Biobanks.Services.Extensions
{
    public static class SampleSetExtensions
    {
        public static CollectionDocument ToCollectionSearchDocument(this CollectionSampleSet sampleSet)
        {
            return new CollectionDocument
            {
                Id = sampleSet.SampleSetId,
                SnomedTerm = sampleSet.Collection.SnomedTerm.Description,
                BiobankId = sampleSet.Collection.OrganisationId,
                BiobankExternalId = sampleSet.Collection.Organisation.OrganisationExternalId,
                Biobank = sampleSet.Collection.Organisation.Name,

                Networks = sampleSet.Collection.Organisation.OrganisationNetworks
                    .Select(x => new NetworkDocument {Name = x.Network.Name }).ToList(),

                CollectionId = sampleSet.CollectionId,
                CollectionTitle = sampleSet.Collection.Title,
                StartYear = sampleSet.Collection.StartDate.Year.ToString(),
                CollectionPoint = sampleSet.Collection.CollectionPoint.Description,
                CollectionStatus = sampleSet.Collection.CollectionStatus.Description,
                ConsentRestrictions = BuildConsentRestrictions(sampleSet.Collection.ConsentRestrictions.ToList()),
                HTA = sampleSet.Collection.HtaStatus?.Description ?? "not provided",
                AccessCondition = sampleSet.Collection.AccessCondition.Description,
                AccessConditionMetadata = JsonConvert.SerializeObject(new
                {
                    Name = sampleSet.Collection.AccessCondition.Description,
                    sampleSet.Collection.AccessCondition.SortOrder
                }),
                CollectionType = sampleSet.Collection.CollectionType?.Description,

                AssociatedData = sampleSet.Collection.AssociatedData
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

                Sex = sampleSet.Sex.Value,
                SexMetadata = JsonConvert.SerializeObject(new
                {
                    Name = sampleSet.Sex.Value,
                    sampleSet.Sex.SortOrder
                }),
                AgeRange = sampleSet.AgeRange.Description,
                AgeRangeMetadata = JsonConvert.SerializeObject(new
                {
                    Name = sampleSet.AgeRange.Description,
                    sampleSet.AgeRange.SortOrder
                }),
                DonorCount = sampleSet.DonorCount.Description,
                DonorCountMetadata = JsonConvert.SerializeObject(new
                {
                    Name = sampleSet.DonorCount.Description,
                    sampleSet.DonorCount.SortOrder
                }),

                MaterialPreservationDetails = sampleSet.MaterialDetails
                    .Select(x => new MaterialPreservationDetailDocument
                    {
                        MaterialType = x.MaterialType.Value,
                        StorageTemperature = x.StorageTemperature.Value,
                        StorageTemperatureMetadata = JsonConvert.SerializeObject(new
                        {
                            Name = x.StorageTemperature.Value,
                            x.StorageTemperature.SortOrder
                        }),
                        MacroscopicAssessment = x.MacroscopicAssessment.Description,
                        PercentageOfSampleSet = x.CollectionPercentage?.Description
                    }),

                BiobankServices = sampleSet.Collection.Organisation.OrganisationServiceOfferings
                    .Select(x => new BiobankServiceDocument
                    {
                        Name = x.ServiceOffering.Name
                    }),

                SampleSetSummary = BuildSampleSetSummary(
                    sampleSet.DonorCount.Description,
                    sampleSet.AgeRange.Description,
                    sampleSet.Sex.Value,
                    sampleSet.MaterialDetails),
                Country = sampleSet.Collection.Organisation.Country.Name,
                County = sampleSet.Collection.Organisation.County?.Name ?? "Not Provided"
            };
        }

        public static string BuildSampleSetSummary(
            string donorCount, 
            string ageRange, 
            string sex,
            ICollection<MaterialDetail> materialDetails)
        {
            var result = string.Concat(donorCount, ", ", ageRange, ", ", sex);

            if (materialDetails != null && materialDetails.Any())
            {
                result = string.Concat(result, ", ",
                    string.Join(" / ", materialDetails.Select(x => x.MaterialType.Value)));
            }

            return result;
        }

        public static IEnumerable<ConsentRestrictionDocument> BuildConsentRestrictions(IList<ConsentRestriction> consentRestrictions)
        {
            return consentRestrictions.Any()
                ? consentRestrictions.Select(cr => new ConsentRestrictionDocument {Description = cr.Description})
                : new List<ConsentRestrictionDocument> { new ConsentRestrictionDocument {Description = "No restrictions"} };
        }
    }
}
