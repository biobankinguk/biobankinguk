using System.Threading.Tasks;
using Biobanks.Analytics.Dto;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        Task<OrganisationReportDto> GetBiobankReport(int Id, int year, int quarter, int period);
        Task<ProfileStatusDTO> GetProfileStatus(string biobankId);
    }
}
