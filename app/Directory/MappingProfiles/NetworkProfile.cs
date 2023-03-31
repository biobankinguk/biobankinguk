using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Dto;

namespace Biobanks.Directory.MappingProfiles;

public class NetworkProfile : Profile
{
  public NetworkProfile()
  {
    CreateMap<NetworkDTO, Network>();
  }
}
