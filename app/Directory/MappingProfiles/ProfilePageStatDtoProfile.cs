using AutoMapper;
using Biobanks.Analytics.Dto;
using Biobanks.Directory.Areas.Admin.Models.Analytics;

namespace Biobanks.Directory.MappingProfiles;

public class ProfilePageStatDtoProfile : Profile
{
  public ProfilePageStatDtoProfile()
  {
    CreateMap<ProfilePageStatDto, ProfilePageStat>();
  }
}
