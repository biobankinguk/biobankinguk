using AutoMapper;
using Biobanks.Analytics.Dto;
using Biobanks.Submissions.Api.Areas.Admin.Models.Analytics;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class DirectoryReportDtoProfile : Profile
{
  public DirectoryReportDtoProfile()
  {
    CreateMap<DirectoryReportDto, DirectoryAnalyticReport>();
  }
}
