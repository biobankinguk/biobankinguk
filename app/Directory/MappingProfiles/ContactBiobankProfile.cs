using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Models.Home;

namespace Biobanks.Directory.MappingProfiles;

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
