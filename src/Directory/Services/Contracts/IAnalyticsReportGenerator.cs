using System;
using System.Threading.Tasks;
using Biobanks.Services.Dto;

namespace Biobanks.Services.Contracts
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public interface IAnalyticsReportGenerator
    {
        void Dispose();
        Task<BiobankAnalyticReportDTO> GetBiobankReport(int Id, int year, int quarter, int period);
        Task<DirectoryAnalyticReportDTO> GetDirectoryReport(int year, int quarter, int period);
        Task<ProfileStatusDTO> GetProfileStatus(string biobankId);
    }
}