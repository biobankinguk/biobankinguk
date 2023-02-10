using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class PublicationProfile : Profile
{
  public PublicationProfile()
  {
    CreateMap<PublicationSearchModel, Publication>();
  }
  
}
