using AutoMapper;
using Biobanks.Entities.Data;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class NetworkRegisterRequestProfile : Profile
{
  public NetworkRegisterRequestProfile()
  {
    CreateMap<NetworkRegisterRequest, NetworkRegisterRequest>();
  }
  
}
