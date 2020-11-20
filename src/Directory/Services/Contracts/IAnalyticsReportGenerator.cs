﻿using Directory.Services.Dto;
using System.Threading.Tasks;

namespace Directory.Services.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        void Dispose();
        Task<BiobankAnalyticReportDTO> GetBiobankReport(int Id, int year, int quarter, int period);
        Task<DirectoryAnalyticReportDTO> GetDirectoryReport(int year, int quarter, int period);
        Task<ProfileStatusDTO> GetProfileStatus(string biobankId);
    }
}