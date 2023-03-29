using AutoMapper;
using Biobanks.Directory.Areas.Biobank.Models.Collections;

namespace Biobanks.Directory.MappingProfiles;

public class AddSampleSetProfile : Profile
{
  public AddSampleSetProfile()
  {
    CreateMap<CopySampleSetModel, AddSampleSetModel>();
  }
  
}
