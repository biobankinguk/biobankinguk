using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Models.Search;
using Biobanks.Directory.Search.Dto.Results;

namespace Biobanks.Directory.MappingProfiles;

public class DetailedCapabilitySearchProfile : Profile
{
  public DetailedCapabilitySearchProfile()
  {
    CreateMap<BiobankCapabilityResult, DetailedCapabilitySearchModel>();
    
    CreateMap<Organisation, DetailedCapabilitySearchModel>()
      .ForMember(dest => dest.BiobankId, opts => opts.MapFrom(src => src.OrganisationId))
      .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId))
      .ForMember(dest => dest.BiobankName, opts => opts.MapFrom(src => src.Name))
      .ForMember(dest => dest.LogoName, opts => opts.MapFrom(src => src.Logo));
  }
}

public class DetailedCapabilitySearchCapabilityProfile : Profile
{
  public DetailedCapabilitySearchCapabilityProfile()
  {
    CreateMap<CapabilitySummary, DetailedCapabilitySearchCapabilityModel>()
      .ForMember(dest => dest.AssociatedData,
        opts => opts.MapFrom(src => src.AssociatedData.Select(
          x => new KeyValuePair<string, string>(x.DataType, x.ProcurementTimeframe))));
    
    CreateMap<DiagnosisCapability, DetailedCapabilitySearchCapabilityModel>()
      .ForMember(dest => dest.Protocols, opts => opts.MapFrom(src => src.SampleCollectionMode.Value))
      .ForMember(dest => dest.Disease, opts => opts.MapFrom(src => src.OntologyTerm.Value))
      .ForMember(dest => dest.AssociatedData,
        opts => opts.MapFrom(src => src.AssociatedData.Select(
          x => new KeyValuePair<string, string>(x.AssociatedDataType.Value,
            x.AssociatedDataProcurementTimeframe.Value))));
  }
}

