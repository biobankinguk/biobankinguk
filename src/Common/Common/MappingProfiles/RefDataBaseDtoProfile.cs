using AutoMapper;
using Common.Data.ReferenceData;
using Common.DTO;
using System.Linq;

namespace Common.MappingProfiles
{
    public class RefDataBaseDtoProfile : Profile
    {
        public RefDataBaseDtoProfile()
        {
            CreateMap<SortedRefDataBaseDto, AccessCondition>();
            CreateMap<SortedRefDataBaseDto, AgeRange>();
            CreateMap<RefDataBaseDto, AnnualStatistic>();
            CreateMap<SortedRefDataBaseDto, AssociatedDataProcurementTimeframe>();
            CreateMap<RefDataBaseDto, AssociatedDataType>();
            CreateMap<SortedRefDataBaseDto, CollectionPercentage>();
            CreateMap<SortedRefDataBaseDto, CollectionPoint>();
            CreateMap<SortedRefDataBaseDto, CollectionStatus>();
            CreateMap<SortedRefDataBaseDto, CollectionType>();
            CreateMap<SortedRefDataBaseDto, ConsentRestriction>();
            CreateMap<RefDataBaseDto, Country>();
            CreateMap<SortedRefDataBaseDto, DonorCount>();
            CreateMap<RefDataBaseDto, Funder>();
            CreateMap<SortedRefDataBaseDto, HtaStatus>();
            CreateMap<RefDataBaseDto, MacroscopicAssessment>();
            CreateMap<SortedRefDataBaseDto, ServiceOffering>();
            CreateMap<SortedRefDataBaseDto, Sex>();
            CreateMap<SortedRefDataBaseDto, SopStatus>();
            CreateMap<SortedRefDataBaseDto, StorageTemperature>();
            CreateMap<SortedRefDataBaseDto, AnnualStatisticGroup>();
            CreateMap<AnnualStatisticInboundDto, AnnualStatistic>();
            CreateMap<AnnualStatistic, AnnualStatisticOutboundDto>().
                ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.AnnualStatisticGroup.Value)).
                ForMember(dest => dest.AnnualStatisticGroupId, opt => opt.MapFrom(src => src.AnnualStatisticGroup.Id));
            CreateMap<MaterialType, MaterialTypeOutboundDto>().ForMember(dest => dest.MaterialTypeGroups, opt => opt.MapFrom(src => src.MaterialTypeGroupMaterialTypes.
                        Select(x => x.MaterialTypeGroup).Select(y => new MaterialTypeGroupChildDto { GroupId = y.Id, GroupName = y.Value })));
            CreateMap<MaterialTypeInboundDto, MaterialType>();
            CreateMap<MaterialTypeGroup, MaterialTypeGroupOutboundDto>().ForMember(dest => dest.MaterialTypes, opt => opt.MapFrom(src => src.MaterialTypeGroupMaterialTypes.
                        Select(x => x.MaterialType).Select(y => new MaterialTypeChildDto { MaterialTypeId = y.Id, MaterialTypeName = y.Value })));
            CreateMap<MaterialTypeGroupInboundDto, MaterialTypeGroup>();
            CreateMap<County, CountyOutboundDto>().ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country.Id)).ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Value));
            CreateMap<CountyInboundDto, County>();
            CreateMap<DonorCount, DonorCountInboundDto>();
            CreateMap<DonorCountInboundDto, DonorCount>();
        }
    }
}
