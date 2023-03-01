using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Models.Home;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class ContactBiobankProfile : Profile
{
  public ContactBiobankProfile()
  {
    CreateMap<Organisation, ContactBiobankModel>()
      .ForMember(dest => dest.BiobankName, opts => opts.MapFrom(src => src.Name))
      .ForMember(dest => dest.ContactEmail, opts => opts.MapFrom(src => src.ContactEmail))
      .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId));
  }
}
