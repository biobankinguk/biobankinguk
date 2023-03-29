using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Dto;

namespace Biobanks.Directory.MappingProfiles;

public class NetworkDtoProfile : Profile
{
  public NetworkDtoProfile()
  {
    CreateMap<Network, NetworkDTO>();
  }
}
