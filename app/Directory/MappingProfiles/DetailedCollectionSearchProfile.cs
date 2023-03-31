using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Biobanks.Directory.Models.Search;
using Biobanks.Directory.Search.Dto.Results;

namespace Biobanks.Directory.MappingProfiles;

public class DetailedCollectionSearchProfile : Profile
{
  public DetailedCollectionSearchProfile()
  {
    CreateMap<BiobankCollectionResult, DetailedCollectionSearchModel>();
  }
  
}

public class DetailedCollectionSearchCollectionProfile : Profile
{
  public DetailedCollectionSearchCollectionProfile()
  {
    CreateMap<CollectionSummary, DetailedCollectionSearchCollectionModel>()
      .ForMember(dest => dest.AssociatedData,
        opts => opts.MapFrom(src => src.AssociatedData.Select(
          x => new KeyValuePair<string, string>(x.DataType, x.ProcurementTimeframe))));
  }
}

public class DetailedCollectionSearchSampleSetProfile : Profile
{
  public DetailedCollectionSearchSampleSetProfile()
  {
    CreateMap<SampleSetSummary, DetailedCollectionSearchSampleSetModel>();
  }
}

public class DetailedCollectionSearchMPDProfile : Profile
{
  public DetailedCollectionSearchMPDProfile()
  {
    CreateMap<MaterialPreservationDetailSummary, DetailedCollectionSearchMPDModel>();
  }
}
