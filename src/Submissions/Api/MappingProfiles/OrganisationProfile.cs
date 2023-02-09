using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class OrganisationProfile : Profile
{
  public OrganisationProfile()
  {
    CreateMap<BiobankDetailsModel, Organisation>()
      .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.OrganisationName))
      .ForMember(dest => dest.PostCode, opts => opts.MapFrom(src => src.Postcode))
      .ForMember(dest => dest.OrganisationId,
        opts => opts.MapFrom(src => src.BiobankId.GetValueOrDefault()))
      .ForMember(dest => dest.OrganisationExternalId, opts => opts.MapFrom(src => src.BiobankExternalId))
      .ForMember(dest => dest.OrganisationTypeId,
        opts => opts.MapFrom(src => src.OrganisationTypeId.GetValueOrDefault()))
      .ForMember(dest => dest.Logo, opts => opts.Ignore()); //Never map Logo from BiobankDetailsModel - we will alaways handle this manually

  }
}
