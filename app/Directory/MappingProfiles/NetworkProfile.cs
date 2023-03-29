using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Dto;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class NetworkProfile : Profile
{
  public NetworkProfile()
  {
    CreateMap<NetworkDTO, Network>();
  }
}
