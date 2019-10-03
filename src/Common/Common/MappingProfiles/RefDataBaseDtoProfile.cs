﻿using AutoMapper;
using Common.Data.ReferenceData;
using Common.DTO;

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
            CreateMap<SortedRefDataBaseDto, MaterialType>();
            // CreateMap<OntologyTermDto, OntologyTerm>(); //TODO expand this out with an Ontology Term DTO, but also note this will be ultimately redundant with the move to OMOP
            CreateMap<SortedRefDataBaseDto, ServiceOffering>();
            CreateMap<SortedRefDataBaseDto, Sex>();
            CreateMap<SortedRefDataBaseDto, SopStatus>();
            CreateMap<SortedRefDataBaseDto, StorageTemperature>();
        }
    }
}
