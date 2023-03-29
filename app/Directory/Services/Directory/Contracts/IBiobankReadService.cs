using System;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IBiobankReadService
    {
        bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId);
        bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId);
        bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId);


        Task<bool> IsMaterialTypeAssigned(int id);

        [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding service."
        , false)]
        Task<int> GetExtractionProcedureMaterialDetailsCount(string id);
        Task<int> GetMaterialTypeMaterialDetailCount(int id);

        Task<int> GetServiceOfferingOrganisationCount(int id);
    }
}
