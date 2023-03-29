using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Dto;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class NetworkDtoProfile : Profile
{
  public NetworkDtoProfile()
  {
    CreateMap<Network, NetworkDTO>();
  }
}
