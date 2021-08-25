using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Search.Dto.Results;
using Biobanks.Services.Dto;
using Biobanks.Web.AutoMapper;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Biobank;
using Biobanks.Web.Models.Home;
using Biobanks.Web.Models.Search;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BiobankCollectionResult, DetailedCollectionSearchModel>();
                cfg.CreateMap<CollectionSummary, DetailedCollectionSearchCollectionModel>();

                cfg.CreateMap<BiobankActivityDTO, BiobankActivityModel>();

                cfg.CreateMap<BiobankCapabilityResult, DetailedCapabilitySearchModel>();
                cfg.CreateMap<CapabilitySummary, DetailedCapabilitySearchCapabilityModel>()
                    .ForMember(dest => dest.AssociatedData,
                        opts => opts.MapFrom(src => src.AssociatedData.Select(
                            x => new KeyValuePair<string, string>(x.DataType, x.ProcurementTimeframe))));

                cfg.CreateMap<CollectionSummary, DetailedCollectionSearchCollectionModel>();
                cfg.CreateMap<SampleSetSummary, DetailedCollectionSearchSampleSetModel>();
                cfg.CreateMap<MaterialPreservationDetailSummary, DetailedCollectionSearchMPDModel>();

                cfg.CreateMap<Organisation, DetailedCapabilitySearchModel>()
                    .ForMember(dest => dest.BiobankId, opts => opts.MapFrom(src => src.OrganisationId))
                    .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId))
                    .ForMember(dest => dest.BiobankName, opts => opts.MapFrom(src => src.Name))
                    .ForMember(dest => dest.LogoName, opts => opts.MapFrom(src => src.Logo));

                cfg.CreateMap<DiagnosisCapability, DetailedCapabilitySearchCapabilityModel>()
                    .ForMember(dest => dest.Protocols, opts => opts.MapFrom(src => src.SampleCollectionMode.Value))
                    .ForMember(dest => dest.Disease, opts => opts.MapFrom(src => src.OntologyTerm.Value))
                    .ForMember(dest => dest.AssociatedData,
                        opts => opts.MapFrom(src => src.AssociatedData.Select(
                            x => new KeyValuePair<string, string>(x.AssociatedDataType.Value,
                                x.AssociatedDataProcurementTimeframe.Value))));

                cfg.CreateMap<CopySampleSetModel, AddSampleSetModel>();

                cfg.CreateMap<Result, BaseSearchModel>().ConvertUsing<BaseSearchModelTypeConverter>();

                cfg.CreateMap<Organisation, ContactBiobankModel>()
                    .ForMember(dest => dest.BiobankName, opts => opts.MapFrom(src => src.Name))
                    .ForMember(dest => dest.ContactEmail, opts => opts.MapFrom(src => src.ContactEmail))
                    .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId));

                cfg.CreateMap<Organisation, BiobankModel>()
                    .ForMember(dest => dest.BiobankId, opts => opts.MapFrom(src => src.OrganisationId))
                    .ForMember(dest => dest.BiobankExternalId, opts => opts.MapFrom(src => src.OrganisationExternalId));

                cfg.CreateMap<BiobankDetailsModel, OrganisationDTO>()
                    .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.OrganisationName))
                    .ForMember(dest => dest.PostCode, opts => opts.MapFrom(src => src.Postcode))
                    .ForMember(dest => dest.OrganisationId,
                        opts => opts.MapFrom(src => src.BiobankId.GetValueOrDefault()))
                    .ForMember(dest => dest.OrganisationExternalId, opts => opts.MapFrom(src => src.BiobankExternalId))
                    .ForMember(dest => dest.OrganisationTypeId,
                        opts => opts.MapFrom(src => src.OrganisationTypeId.GetValueOrDefault()))
                    .ForMember(dest => dest.Logo, opts => opts.Ignore()); //Never map Logo from BiobankDetailsModel - we will alaways handle this manually

                cfg.CreateMap<BiobankDetailsModel, Organisation>()
                    .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.OrganisationName))
                    .ForMember(dest => dest.PostCode, opts => opts.MapFrom(src => src.Postcode))
                    .ForMember(dest => dest.OrganisationId,
                        opts => opts.MapFrom(src => src.BiobankId.GetValueOrDefault()))
                    .ForMember(dest => dest.OrganisationExternalId, opts => opts.MapFrom(src => src.BiobankExternalId))
                    .ForMember(dest => dest.OrganisationTypeId,
                        opts => opts.MapFrom(src => src.OrganisationTypeId.GetValueOrDefault()))
                    .ForMember(dest => dest.Logo, opts => opts.Ignore()); //Never map Logo from BiobankDetailsModel - we will alaways handle this manually

                cfg.CreateMap<OrganisationDTO, Organisation>();
                cfg.CreateMap<Organisation, OrganisationDTO>();

                cfg.CreateMap<NetworkDTO, Network>();
                cfg.CreateMap<Network, NetworkDTO>();

                cfg.CreateMap<OrganisationRegisterRequest, OrganisationRegisterRequest>();
                cfg.CreateMap<NetworkRegisterRequest, NetworkRegisterRequest>();

                cfg.CreateMap<PublicationSearchModel, Publication>();
                cfg.CreateMap<Publication, BiobankPublicationModel>();

                cfg.CreateMap<BiobankAnalyticReportDTO, BiobankAnalyticReport>();
                cfg.CreateMap<ProfileStatusDTO, ProfileStatus>();
                cfg.CreateMap<ProfilePageViewsDTO, ProfilePageViews>();
                cfg.CreateMap<SearchActivityDTO, SearchActivity>();
                cfg.CreateMap<ContactRequestsDTO, ContactRequests>();
                cfg.CreateMap<QuarterlyCountsDTO, QuarterlyCounts>();

                cfg.CreateMap<DirectoryAnalyticReportDTO, DirectoryAnalyticReport>();
                cfg.CreateMap<SessionStatDTO, SessionStat>();
                cfg.CreateMap<SearchCharacteristicDTO, SearchCharacteristic>();
                cfg.CreateMap<EventStatDTO, EventStat>();
                cfg.CreateMap<ProfilePageStatDTO, ProfilePageStat>(); 
                cfg.CreateMap<SourceCountDTO, SourceCount>();
            });
        }
    }
}
