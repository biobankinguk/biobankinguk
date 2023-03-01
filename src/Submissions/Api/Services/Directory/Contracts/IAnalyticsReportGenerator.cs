using Biobanks.Analytics.Dto;
using Biobanks.Submissions.Api.Services.Directory.Dto;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        void Dispose();
        Task<OrganisationReportDto> GetBiobankReport(int Id, int year, int quarter, int period);
        Task<DirectoryAnalyticReportDTO> GetDirectoryReport(int year, int quarter, int period);
        Task<ProfileStatusDTO> GetProfileStatus(string biobankId);
    }
}
