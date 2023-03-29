using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Admin.Models.Biobanks;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class BiobankProfile : Profile
{
  public BiobankProfile()
  {
    CreateMap<Organisation, BiobankModel>()
      .ForMember(dest => dest.BiobankId, opts => opts.MapFrom(src =>   src.OrganisationId))
      .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId));
  }
  
}
