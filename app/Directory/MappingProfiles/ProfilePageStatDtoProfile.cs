using AutoMapper;
using Biobanks.Analytics.Dto;
using Biobanks.Submissions.Api.Areas.Admin.Models.Analytics;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class ProfilePageStatDtoProfile : Profile
{
  public ProfilePageStatDtoProfile()
  {
    CreateMap<ProfilePageStatDto, ProfilePageStat>();
  }
}
