using Biobanks.Submissions.Api.Services.Directory.Dto;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        void Dispose();
        Task<Analytics.Dto.OrganisationReportDto> GetBiobankReport(int Id, int year, int quarter, int period);
        Task<DirectoryAnalyticReportDTO> GetDirectoryReport(int year, int quarter, int period);
        Task<Analytics.Dto.ProfileStatusDTO> GetProfileStatus(string biobankId);
    }
}
