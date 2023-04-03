namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IBiobankAuthorizationService
    {
        bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId);
        bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId);
        bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId);
    }
}
