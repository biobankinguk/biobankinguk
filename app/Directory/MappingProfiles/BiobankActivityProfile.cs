using AutoMapper;
using Biobanks.Directory.Areas.Admin.Models.Requests;
using Biobanks.Directory.Services.Directory.Dto;

namespace Biobanks.Directory.MappingProfiles;

public class BiobankActivityProfile : Profile
{
  public BiobankActivityProfile()
  {
    CreateMap<BiobankActivityDTO, BiobankActivityModel>();
  }
}
