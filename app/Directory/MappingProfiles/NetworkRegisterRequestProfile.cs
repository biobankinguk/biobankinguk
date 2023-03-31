using AutoMapper;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.MappingProfiles;

public class NetworkRegisterRequestProfile : Profile
{
  public NetworkRegisterRequestProfile()
  {
    CreateMap<NetworkRegisterRequest, NetworkRegisterRequest>();
  }
  
}
