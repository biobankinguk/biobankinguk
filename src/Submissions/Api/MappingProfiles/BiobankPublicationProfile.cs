using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class BiobankPublicationProfile : Profile
{
  public BiobankPublicationProfile()
  {
    CreateMap<Publication, BiobankPublicationModel>();
  }
}
