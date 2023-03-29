using AutoMapper;
using Biobanks.Submissions.Api.Areas.Admin.Models.Requests;
using Biobanks.Submissions.Api.Services.Directory.Dto;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class BiobankActivityProfile : Profile
{
  public BiobankActivityProfile()
  {
    CreateMap<BiobankActivityDTO, BiobankActivityModel>();
  }
}
