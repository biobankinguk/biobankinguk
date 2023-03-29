using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Areas.Admin.Models.Biobanks;

namespace Biobanks.Directory.MappingProfiles;

public class BiobankProfile : Profile
{
  public BiobankProfile()
  {
    CreateMap<Organisation, BiobankModel>()
      .ForMember(dest => dest.BiobankId, opts => opts.MapFrom(src =>   src.OrganisationId))
      .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId));
  }
  
}
