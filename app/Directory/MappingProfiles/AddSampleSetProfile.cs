using AutoMapper;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Collections;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class AddSampleSetProfile : Profile
{
  public AddSampleSetProfile()
  {
    CreateMap<CopySampleSetModel, AddSampleSetModel>();
  }
  
}
