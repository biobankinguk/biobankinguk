using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;
using Biobanks.Submissions.Api.Models.Directory;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class OrganisationDTOProfile : Profile
{
  public OrganisationDTOProfile()
  {
    // needed for creation of biobank or else it doesn't map correctly
    CreateMap<BiobankDetailsModel, OrganisationDTO>()
      .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.OrganisationName))
      .ForMember(dest => dest.PostCode, opts => opts.MapFrom(src => src.Postcode))
      .ForMember(dest => dest.OrganisationId,
        opts => opts.MapFrom(src => src.BiobankId.GetValueOrDefault()))
      .ForMember(dest => dest.OrganisationExternalId, opts => opts.MapFrom(src => src.BiobankExternalId))
      .ForMember(dest => dest.OrganisationTypeId,
        opts => opts.MapFrom(src => src.OrganisationTypeId.GetValueOrDefault()))
      .ForMember(dest => dest.Logo, opts => opts.Ignore()); //Never map Logo from BiobankDetailsModel - we will alaways handle this manually
    
    CreateMap<Organisation, OrganisationDTO>();
  }
}
