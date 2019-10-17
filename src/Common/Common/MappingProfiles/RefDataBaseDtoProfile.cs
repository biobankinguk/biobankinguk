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
            CreateMap<RefDataBaseDto, County>();//TODO expand this out when the County DTO gets added
            CreateMap<SortedRefDataBaseDto, DonorCount>();
            CreateMap<RefDataBaseDto, Funder>();
            CreateMap<SortedRefDataBaseDto, HtaStatus>();
            CreateMap<RefDataBaseDto, MacroscopicAssessment>();
            CreateMap<SortedRefDataBaseDto, MaterialType>();//TODO expand this out with new DTO structure
            CreateMap<RefDataBaseDto, MaterialTypeGroup>();
            // CreateMap<OntologyTermDto, OntologyTerm>(); //TODO expand this out with an Ontology Term DTO, but also note this will be ultimately redundant with the move to OMOP
            CreateMap<SortedRefDataBaseDto, ServiceOffering>();
            CreateMap<SortedRefDataBaseDto, Sex>();
            CreateMap<SortedRefDataBaseDto, SopStatus>();
            CreateMap<SortedRefDataBaseDto, StorageTemperature>();
            CreateMap<SortedRefDataBaseDto, AnnualStatisticGroup>();
            CreateMap<AnnualStatisticDto, AnnualStatistic>();
            CreateMap<AnnualStatistic, AnnualStatisticDto>().
                ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.AnnualStatisticGroup.Value)).
                ForMember(dest => dest.AnnualStatisticGroupId, opt => opt.MapFrom(src => src.AnnualStatisticGroup.Id));
            CreateMap<MaterialType, MaterialTypeDto>().ForMember(dest => dest.MaterialTypeGroups, opt => opt.MapFrom(src => src.MaterialTypeGroupMaterialTypes.
                        Select(x => x.MaterialTypeGroup).Select(y => new MaterialTypeGroupChildDto { GroupId = y.Id, GroupName = y.Value })));
            CreateMap<MaterialTypeDto, MaterialType>();
        }
    }
}
