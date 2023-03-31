using AutoMapper;
using Biobanks.Analytics.Dto;
using Biobanks.Directory.Areas.Admin.Models.Analytics;

namespace Biobanks.Directory.MappingProfiles;

public class EventStatDtoProfile : Profile
{
  public EventStatDtoProfile()
  {
    CreateMap<EventStatDto, EventStat>();
  }
}
