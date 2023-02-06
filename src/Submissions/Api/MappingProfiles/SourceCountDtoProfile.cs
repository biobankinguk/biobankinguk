using AutoMapper;
using Biobanks.Analytics.Dto;
using Biobanks.Submissions.Api.Areas.Admin.Models.Analytics;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class SourceCountDtoProfile : Profile
{
  public SourceCountDtoProfile()
  {
    CreateMap<SourceCountDto, SourceCount>();
  }
}
