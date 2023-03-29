using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Areas.Biobank.Models.Profile;

namespace Biobanks.Directory.MappingProfiles;

public class BiobankPublicationProfile : Profile
{
  public BiobankPublicationProfile()
  {
    CreateMap<Publication, BiobankPublicationModel>();
  }
}
