namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessConditions",
                c => new
                    {
                        AccessConditionId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccessConditionId);
            
            CreateTable(
                "dbo.AgeRanges",
                c => new
                    {
                        AgeRangeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AgeRangeId);
            
            CreateTable(
                "dbo.AnnualStatisticGroups",
                c => new
                    {
                        AnnualStatisticGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AnnualStatisticGroupId);
            
            CreateTable(
                "dbo.AnnualStatistics",
                c => new
                    {
                        AnnualStatisticId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AnnualStatisticGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnnualStatisticId)
                .ForeignKey("dbo.AnnualStatisticGroups", t => t.AnnualStatisticGroupId, cascadeDelete: true)
                .Index(t => t.AnnualStatisticGroupId);
            
            CreateTable(
                "dbo.OrganisationAnnualStatistics",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false),
                        AnnualStatisticId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        Value = c.Int(),
                    })
                .PrimaryKey(t => new { t.OrganisationId, t.AnnualStatisticId, t.Year })
                .ForeignKey("dbo.AnnualStatistics", t => t.AnnualStatisticId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.AnnualStatisticId);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        IsSuspended = c.Boolean(nullable: false),
                        OrganisationExternalId = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ContactEmail = c.String(nullable: false),
                        ContactNumber = c.String(),
                        Logo = c.String(),
                        Url = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        PostCode = c.String(),
                        City = c.String(),
                        CountyId = c.Int(),
                        CountryId = c.Int(),
                        OrganisationTypeId = c.Int(nullable: false),
                        GoverningInstitution = c.String(),
                        GoverningDepartment = c.String(),
                        SharingOptOut = c.Boolean(nullable: false),
                        EthicsRegistration = c.String(),
                        HtaLicence = c.String(),
                        LastUpdated = c.DateTime(),
                        AnonymousIdentifier = c.Guid(),
                        OtherRegistrationReason = c.String(),
                    })
                .PrimaryKey(t => t.OrganisationId)
                .ForeignKey("dbo.Counties", t => t.CountyId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.OrganisationTypes", t => t.OrganisationTypeId, cascadeDelete: true)
                .Index(t => t.CountyId)
                .Index(t => t.CountryId)
                .Index(t => t.OrganisationTypeId);
            
            CreateTable(
                "dbo.Collections",
                c => new
                    {
                        CollectionId = c.Int(nullable: false, identity: true),
                        OrganisationId = c.Int(nullable: false),
                        DiagnosisId = c.Int(nullable: false),
                        Title = c.String(maxLength: 250),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdated = c.DateTime(nullable: false),
                        HtaStatusId = c.Int(),
                        AccessConditionId = c.Int(nullable: false),
                        CollectionTypeId = c.Int(),
                        CollectionStatusId = c.Int(nullable: false),
                        CollectionPointId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionId)
                .ForeignKey("dbo.AccessConditions", t => t.AccessConditionId, cascadeDelete: true)
                .ForeignKey("dbo.CollectionPoints", t => t.CollectionPointId, cascadeDelete: true)
                .ForeignKey("dbo.CollectionStatus", t => t.CollectionStatusId, cascadeDelete: true)
                .ForeignKey("dbo.CollectionTypes", t => t.CollectionTypeId)
                .ForeignKey("dbo.Diagnosis", t => t.DiagnosisId, cascadeDelete: true)
                .ForeignKey("dbo.HtaStatus", t => t.HtaStatusId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.DiagnosisId)
                .Index(t => t.HtaStatusId)
                .Index(t => t.AccessConditionId)
                .Index(t => t.CollectionTypeId)
                .Index(t => t.CollectionStatusId)
                .Index(t => t.CollectionPointId);
            
            CreateTable(
                "dbo.CollectionAssociatedDatas",
                c => new
                    {
                        CollectionId = c.Int(nullable: false),
                        AssociatedDataTypeId = c.Int(nullable: false),
                        AssociatedDataProcurementTimeframeId = c.Int(),
                    })
                .PrimaryKey(t => new { t.CollectionId, t.AssociatedDataTypeId })
                .ForeignKey("dbo.AssociatedDataProcurementTimeframes", t => t.AssociatedDataProcurementTimeframeId)
                .ForeignKey("dbo.AssociatedDataTypes", t => t.AssociatedDataTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .Index(t => t.CollectionId)
                .Index(t => t.AssociatedDataTypeId)
                .Index(t => t.AssociatedDataProcurementTimeframeId);
            
            CreateTable(
                "dbo.AssociatedDataProcurementTimeframes",
                c => new
                    {
                        AssociatedDataProcurementTimeframeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        DisplayValue = c.String(nullable: false, maxLength: 10),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssociatedDataProcurementTimeframeId);
            
            CreateTable(
                "dbo.AssociatedDataTypes",
                c => new
                    {
                        AssociatedDataTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Message = c.String(),
                        AssociatedDataTypeGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssociatedDataTypeId)
                .ForeignKey("dbo.AssociatedDataTypeGroups", t => t.AssociatedDataTypeGroupId, cascadeDelete: true)
                .Index(t => t.AssociatedDataTypeGroupId);
            
            CreateTable(
                "dbo.AssociatedDataTypeGroups",
                c => new
                    {
                        AssociatedDataTypeGroupId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AssociatedDataTypeGroupId);
            
            CreateTable(
                "dbo.CollectionPoints",
                c => new
                    {
                        CollectionPointId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionPointId);
            
            CreateTable(
                "dbo.CollectionStatus",
                c => new
                    {
                        CollectionStatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionStatusId);
            
            CreateTable(
                "dbo.CollectionTypes",
                c => new
                    {
                        CollectionTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CollectionTypeId);
            
            CreateTable(
                "dbo.ConsentRestrictions",
                c => new
                    {
                        ConsentRestrictionId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConsentRestrictionId);
            
            CreateTable(
                "dbo.Diagnosis",
                c => new
                    {
                        DiagnosisId = c.Int(nullable: false, identity: true),
                        OtherTerms = c.String(),
                        SnomedIdentifier = c.String(maxLength: 20),
                        Description = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.DiagnosisId)
                .Index(t => t.Description, unique: true, name: "IX_UniqueDiagnosisDescription");
            
            CreateTable(
                "dbo.HtaStatus",
                c => new
                    {
                        HtaStatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HtaStatusId);
            
            CreateTable(
                "dbo.CollectionSampleSets",
                c => new
                    {
                        SampleSetId = c.Int(nullable: false, identity: true),
                        CollectionId = c.Int(nullable: false),
                        SexId = c.Int(nullable: false),
                        AgeRangeId = c.Int(nullable: false),
                        DonorCountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SampleSetId)
                .ForeignKey("dbo.AgeRanges", t => t.AgeRangeId, cascadeDelete: true)
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .ForeignKey("dbo.DonorCounts", t => t.DonorCountId, cascadeDelete: true)
                .ForeignKey("dbo.Sexes", t => t.SexId, cascadeDelete: true)
                .Index(t => t.CollectionId)
                .Index(t => t.SexId)
                .Index(t => t.AgeRangeId)
                .Index(t => t.DonorCountId);
            
            CreateTable(
                "dbo.DonorCounts",
                c => new
                    {
                        DonorCountId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        LowerBound = c.Int(),
                        UpperBound = c.Int(),
                    })
                .PrimaryKey(t => t.DonorCountId);
            
            CreateTable(
                "dbo.MaterialDetails",
                c => new
                    {
                        SampleSetId = c.Int(nullable: false),
                        MaterialTypeId = c.Int(nullable: false),
                        PreservationTypeId = c.Int(nullable: false),
                        MacroscopicAssessmentId = c.Int(nullable: false),
                        CollectionPercentageId = c.Int(),
                    })
                .PrimaryKey(t => new { t.SampleSetId, t.MaterialTypeId, t.PreservationTypeId, t.MacroscopicAssessmentId })
                .ForeignKey("dbo.CollectionPercentages", t => t.CollectionPercentageId)
                .ForeignKey("dbo.MacroscopicAssessments", t => t.MacroscopicAssessmentId, cascadeDelete: true)
                .ForeignKey("dbo.MaterialTypes", t => t.MaterialTypeId, cascadeDelete: true)
                .ForeignKey("dbo.PreservationTypes", t => t.PreservationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.CollectionSampleSets", t => t.SampleSetId, cascadeDelete: true)
                .Index(t => t.SampleSetId)
                .Index(t => t.MaterialTypeId)
                .Index(t => t.PreservationTypeId)
                .Index(t => t.MacroscopicAssessmentId)
                .Index(t => t.CollectionPercentageId);
            
            CreateTable(
                "dbo.CollectionPercentages",
                c => new
                    {
                        CollectionPercentageId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        LowerBound = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpperBound = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CollectionPercentageId);
            
            CreateTable(
                "dbo.MacroscopicAssessments",
                c => new
                    {
                        MacroscopicAssessmentId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MacroscopicAssessmentId);
            
            CreateTable(
                "dbo.MaterialTypes",
                c => new
                    {
                        MaterialTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialTypeId);
            
            CreateTable(
                "dbo.PreservationTypes",
                c => new
                    {
                        PreservationTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PreservationTypeId);
            
            CreateTable(
                "dbo.Sexes",
                c => new
                    {
                        SexId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SexId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Counties",
                c => new
                    {
                        CountyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CountryId = c.Int(),
                    })
                .PrimaryKey(t => t.CountyId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.DiagnosisCapabilities",
                c => new
                    {
                        DiagnosisCapabilityId = c.Int(nullable: false, identity: true),
                        OrganisationId = c.Int(nullable: false),
                        DiagnosisId = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        SampleCollectionModeId = c.Int(nullable: false),
                        AnnualDonorExpectation = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiagnosisCapabilityId)
                .ForeignKey("dbo.Diagnosis", t => t.DiagnosisId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.SampleCollectionModes", t => t.SampleCollectionModeId, cascadeDelete: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.DiagnosisId)
                .Index(t => t.SampleCollectionModeId);
            
            CreateTable(
                "dbo.CapabilityAssociatedDatas",
                c => new
                    {
                        DiagnosisCapabilityId = c.Int(nullable: false),
                        AssociatedDataTypeId = c.Int(nullable: false),
                        AssociatedDataProcurementTimeframeId = c.Int(),
                    })
                .PrimaryKey(t => new { t.DiagnosisCapabilityId, t.AssociatedDataTypeId })
                .ForeignKey("dbo.AssociatedDataProcurementTimeframes", t => t.AssociatedDataProcurementTimeframeId)
                .ForeignKey("dbo.AssociatedDataTypes", t => t.AssociatedDataTypeId, cascadeDelete: true)
                .ForeignKey("dbo.DiagnosisCapabilities", t => t.DiagnosisCapabilityId, cascadeDelete: true)
                .Index(t => t.DiagnosisCapabilityId)
                .Index(t => t.AssociatedDataTypeId)
                .Index(t => t.AssociatedDataProcurementTimeframeId);
            
            CreateTable(
                "dbo.SampleCollectionModes",
                c => new
                    {
                        SampleCollectionModeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SampleCollectionModeId);
            
            CreateTable(
                "dbo.Funders",
                c => new
                    {
                        FunderId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.FunderId);
            
            CreateTable(
                "dbo.OrganisationNetworks",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false),
                        NetworkId = c.Int(nullable: false),
                        ExternalID = c.String(),
                    })
                .PrimaryKey(t => new { t.OrganisationId, t.NetworkId })
                .ForeignKey("dbo.Networks", t => t.NetworkId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.NetworkId);
            
            CreateTable(
                "dbo.Networks",
                c => new
                    {
                        NetworkId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Url = c.String(),
                        Logo = c.String(),
                        Description = c.String(nullable: false),
                        SopStatusId = c.Int(nullable: false),
                        LastUpdated = c.DateTime(),
                        ContactHandoverEnabled = c.Boolean(nullable: false),
                        HandoverBaseUrl = c.String(),
                        HandoverOrgIdsUrlParamName = c.String(),
                        MultipleHandoverOrdIdsParams = c.Boolean(nullable: false),
                        HandoverNonMembers = c.Boolean(nullable: false),
                        HandoverNonMembersUrlParamName = c.String(),
                    })
                .PrimaryKey(t => t.NetworkId)
                .ForeignKey("dbo.SopStatus", t => t.SopStatusId, cascadeDelete: true)
                .Index(t => t.SopStatusId);
            
            CreateTable(
                "dbo.SopStatus",
                c => new
                    {
                        SopStatusId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SopStatusId);
            
            CreateTable(
                "dbo.OrganisationRegistrationReasons",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false),
                        RegistrationReasonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganisationId, t.RegistrationReasonId })
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.RegistrationReasons", t => t.RegistrationReasonId, cascadeDelete: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.RegistrationReasonId);
            
            CreateTable(
                "dbo.RegistrationReasons",
                c => new
                    {
                        RegistrationReasonId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.RegistrationReasonId);
            
            CreateTable(
                "dbo.OrganisationServiceOfferings",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganisationId, t.ServiceId })
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .ForeignKey("dbo.ServiceOfferings", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.ServiceOfferings",
                c => new
                    {
                        ServiceId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceId);
            
            CreateTable(
                "dbo.OrganisationTypes",
                c => new
                    {
                        OrganisationTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrganisationTypeId);
            
            CreateTable(
                "dbo.OrganisationUsers",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false),
                        OrganisationUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.OrganisationId, t.OrganisationUserId })
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.Blobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false),
                        ContentType = c.String(nullable: false),
                        Content = c.Binary(nullable: false),
                        ContentDisposition = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Configs",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ReadOnly = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.NetworkRegisterRequests",
                c => new
                    {
                        NetworkRegisterRequestId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        UserEmail = c.String(nullable: false),
                        NetworkName = c.String(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        AcceptedDate = c.DateTime(),
                        NetworkCreatedDate = c.DateTime(),
                        DeclinedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.NetworkRegisterRequestId);
            
            CreateTable(
                "dbo.NetworkUsers",
                c => new
                    {
                        NetworkId = c.Int(nullable: false),
                        NetworkUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.NetworkId, t.NetworkUserId })
                .ForeignKey("dbo.Networks", t => t.NetworkId, cascadeDelete: true)
                .Index(t => t.NetworkId);
            
            CreateTable(
                "dbo.OrganisationRegisterRequests",
                c => new
                    {
                        OrganisationRegisterRequestId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        UserEmail = c.String(nullable: false),
                        OrganisationTypeId = c.Int(nullable: false),
                        OrganisationName = c.String(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        AcceptedDate = c.DateTime(),
                        OrganisationCreatedDate = c.DateTime(),
                        DeclinedDate = c.DateTime(),
                        OrganisationExternalId = c.String(),
                    })
                .PrimaryKey(t => t.OrganisationRegisterRequestId)
                .ForeignKey("dbo.OrganisationTypes", t => t.OrganisationTypeId, cascadeDelete: true)
                .Index(t => t.OrganisationTypeId);
            
            CreateTable(
                "dbo.Publications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PublicationId = c.String(),
                        Title = c.String(),
                        Authors = c.String(),
                        Journal = c.String(),
                        Year = c.Int(nullable: false),
                        DOI = c.String(),
                        Accepted = c.Boolean(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.TokenIssueRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        Purpose = c.String(),
                        UserId = c.String(),
                        IssueDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TokenValidationRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        Purpose = c.String(),
                        UserId = c.String(),
                        ValidationDate = c.DateTime(nullable: false),
                        ValidationSuccessful = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConsentRestrictionCollections",
                c => new
                    {
                        ConsentRestriction_ConsentRestrictionId = c.Int(nullable: false),
                        Collection_CollectionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ConsentRestriction_ConsentRestrictionId, t.Collection_CollectionId })
                .ForeignKey("dbo.ConsentRestrictions", t => t.ConsentRestriction_ConsentRestrictionId, cascadeDelete: true)
                .ForeignKey("dbo.Collections", t => t.Collection_CollectionId, cascadeDelete: true)
                .Index(t => t.ConsentRestriction_ConsentRestrictionId)
                .Index(t => t.Collection_CollectionId);
            
            CreateTable(
                "dbo.FunderOrganisations",
                c => new
                    {
                        Funder_FunderId = c.Int(nullable: false),
                        Organisation_OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Funder_FunderId, t.Organisation_OrganisationId })
                .ForeignKey("dbo.Funders", t => t.Funder_FunderId, cascadeDelete: true)
                .ForeignKey("dbo.Organisations", t => t.Organisation_OrganisationId, cascadeDelete: true)
                .Index(t => t.Funder_FunderId)
                .Index(t => t.Organisation_OrganisationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Publications", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationRegisterRequests", "OrganisationTypeId", "dbo.OrganisationTypes");
            DropForeignKey("dbo.NetworkUsers", "NetworkId", "dbo.Networks");
            DropForeignKey("dbo.OrganisationUsers", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Organisations", "OrganisationTypeId", "dbo.OrganisationTypes");
            DropForeignKey("dbo.OrganisationServiceOfferings", "ServiceId", "dbo.ServiceOfferings");
            DropForeignKey("dbo.OrganisationServiceOfferings", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationRegistrationReasons", "RegistrationReasonId", "dbo.RegistrationReasons");
            DropForeignKey("dbo.OrganisationRegistrationReasons", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.OrganisationNetworks", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Networks", "SopStatusId", "dbo.SopStatus");
            DropForeignKey("dbo.OrganisationNetworks", "NetworkId", "dbo.Networks");
            DropForeignKey("dbo.OrganisationAnnualStatistics", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.FunderOrganisations", "Organisation_OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.FunderOrganisations", "Funder_FunderId", "dbo.Funders");
            DropForeignKey("dbo.DiagnosisCapabilities", "SampleCollectionModeId", "dbo.SampleCollectionModes");
            DropForeignKey("dbo.DiagnosisCapabilities", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.DiagnosisCapabilities", "DiagnosisId", "dbo.Diagnosis");
            DropForeignKey("dbo.CapabilityAssociatedDatas", "DiagnosisCapabilityId", "dbo.DiagnosisCapabilities");
            DropForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes");
            DropForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes");
            DropForeignKey("dbo.Organisations", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Organisations", "CountyId", "dbo.Counties");
            DropForeignKey("dbo.Counties", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.CollectionSampleSets", "SexId", "dbo.Sexes");
            DropForeignKey("dbo.MaterialDetails", "SampleSetId", "dbo.CollectionSampleSets");
            DropForeignKey("dbo.MaterialDetails", "PreservationTypeId", "dbo.PreservationTypes");
            DropForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes");
            DropForeignKey("dbo.MaterialDetails", "MacroscopicAssessmentId", "dbo.MacroscopicAssessments");
            DropForeignKey("dbo.MaterialDetails", "CollectionPercentageId", "dbo.CollectionPercentages");
            DropForeignKey("dbo.CollectionSampleSets", "DonorCountId", "dbo.DonorCounts");
            DropForeignKey("dbo.CollectionSampleSets", "CollectionId", "dbo.Collections");
            DropForeignKey("dbo.CollectionSampleSets", "AgeRangeId", "dbo.AgeRanges");
            DropForeignKey("dbo.Collections", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Collections", "HtaStatusId", "dbo.HtaStatus");
            DropForeignKey("dbo.Collections", "DiagnosisId", "dbo.Diagnosis");
            DropForeignKey("dbo.ConsentRestrictionCollections", "Collection_CollectionId", "dbo.Collections");
            DropForeignKey("dbo.ConsentRestrictionCollections", "ConsentRestriction_ConsentRestrictionId", "dbo.ConsentRestrictions");
            DropForeignKey("dbo.Collections", "CollectionTypeId", "dbo.CollectionTypes");
            DropForeignKey("dbo.Collections", "CollectionStatusId", "dbo.CollectionStatus");
            DropForeignKey("dbo.Collections", "CollectionPointId", "dbo.CollectionPoints");
            DropForeignKey("dbo.CollectionAssociatedDatas", "CollectionId", "dbo.Collections");
            DropForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes");
            DropForeignKey("dbo.AssociatedDataTypes", "AssociatedDataTypeGroupId", "dbo.AssociatedDataTypeGroups");
            DropForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes");
            DropForeignKey("dbo.Collections", "AccessConditionId", "dbo.AccessConditions");
            DropForeignKey("dbo.OrganisationAnnualStatistics", "AnnualStatisticId", "dbo.AnnualStatistics");
            DropForeignKey("dbo.AnnualStatistics", "AnnualStatisticGroupId", "dbo.AnnualStatisticGroups");
            DropIndex("dbo.FunderOrganisations", new[] { "Organisation_OrganisationId" });
            DropIndex("dbo.FunderOrganisations", new[] { "Funder_FunderId" });
            DropIndex("dbo.ConsentRestrictionCollections", new[] { "Collection_CollectionId" });
            DropIndex("dbo.ConsentRestrictionCollections", new[] { "ConsentRestriction_ConsentRestrictionId" });
            DropIndex("dbo.Publications", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationRegisterRequests", new[] { "OrganisationTypeId" });
            DropIndex("dbo.NetworkUsers", new[] { "NetworkId" });
            DropIndex("dbo.OrganisationUsers", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationServiceOfferings", new[] { "ServiceId" });
            DropIndex("dbo.OrganisationServiceOfferings", new[] { "OrganisationId" });
            DropIndex("dbo.OrganisationRegistrationReasons", new[] { "RegistrationReasonId" });
            DropIndex("dbo.OrganisationRegistrationReasons", new[] { "OrganisationId" });
            DropIndex("dbo.Networks", new[] { "SopStatusId" });
            DropIndex("dbo.OrganisationNetworks", new[] { "NetworkId" });
            DropIndex("dbo.OrganisationNetworks", new[] { "OrganisationId" });
            DropIndex("dbo.CapabilityAssociatedDatas", new[] { "AssociatedDataProcurementTimeframeId" });
            DropIndex("dbo.CapabilityAssociatedDatas", new[] { "AssociatedDataTypeId" });
            DropIndex("dbo.CapabilityAssociatedDatas", new[] { "DiagnosisCapabilityId" });
            DropIndex("dbo.DiagnosisCapabilities", new[] { "SampleCollectionModeId" });
            DropIndex("dbo.DiagnosisCapabilities", new[] { "DiagnosisId" });
            DropIndex("dbo.DiagnosisCapabilities", new[] { "OrganisationId" });
            DropIndex("dbo.Counties", new[] { "CountryId" });
            DropIndex("dbo.MaterialDetails", new[] { "CollectionPercentageId" });
            DropIndex("dbo.MaterialDetails", new[] { "MacroscopicAssessmentId" });
            DropIndex("dbo.MaterialDetails", new[] { "PreservationTypeId" });
            DropIndex("dbo.MaterialDetails", new[] { "MaterialTypeId" });
            DropIndex("dbo.MaterialDetails", new[] { "SampleSetId" });
            DropIndex("dbo.CollectionSampleSets", new[] { "DonorCountId" });
            DropIndex("dbo.CollectionSampleSets", new[] { "AgeRangeId" });
            DropIndex("dbo.CollectionSampleSets", new[] { "SexId" });
            DropIndex("dbo.CollectionSampleSets", new[] { "CollectionId" });
            DropIndex("dbo.Diagnosis", "IX_UniqueDiagnosisDescription");
            DropIndex("dbo.AssociatedDataTypes", new[] { "AssociatedDataTypeGroupId" });
            DropIndex("dbo.CollectionAssociatedDatas", new[] { "AssociatedDataProcurementTimeframeId" });
            DropIndex("dbo.CollectionAssociatedDatas", new[] { "AssociatedDataTypeId" });
            DropIndex("dbo.CollectionAssociatedDatas", new[] { "CollectionId" });
            DropIndex("dbo.Collections", new[] { "CollectionPointId" });
            DropIndex("dbo.Collections", new[] { "CollectionStatusId" });
            DropIndex("dbo.Collections", new[] { "CollectionTypeId" });
            DropIndex("dbo.Collections", new[] { "AccessConditionId" });
            DropIndex("dbo.Collections", new[] { "HtaStatusId" });
            DropIndex("dbo.Collections", new[] { "DiagnosisId" });
            DropIndex("dbo.Collections", new[] { "OrganisationId" });
            DropIndex("dbo.Organisations", new[] { "OrganisationTypeId" });
            DropIndex("dbo.Organisations", new[] { "CountryId" });
            DropIndex("dbo.Organisations", new[] { "CountyId" });
            DropIndex("dbo.OrganisationAnnualStatistics", new[] { "AnnualStatisticId" });
            DropIndex("dbo.OrganisationAnnualStatistics", new[] { "OrganisationId" });
            DropIndex("dbo.AnnualStatistics", new[] { "AnnualStatisticGroupId" });
            DropTable("dbo.FunderOrganisations");
            DropTable("dbo.ConsentRestrictionCollections");
            DropTable("dbo.TokenValidationRecords");
            DropTable("dbo.TokenIssueRecords");
            DropTable("dbo.Publications");
            DropTable("dbo.OrganisationRegisterRequests");
            DropTable("dbo.NetworkUsers");
            DropTable("dbo.NetworkRegisterRequests");
            DropTable("dbo.Configs");
            DropTable("dbo.Blobs");
            DropTable("dbo.OrganisationUsers");
            DropTable("dbo.OrganisationTypes");
            DropTable("dbo.ServiceOfferings");
            DropTable("dbo.OrganisationServiceOfferings");
            DropTable("dbo.RegistrationReasons");
            DropTable("dbo.OrganisationRegistrationReasons");
            DropTable("dbo.SopStatus");
            DropTable("dbo.Networks");
            DropTable("dbo.OrganisationNetworks");
            DropTable("dbo.Funders");
            DropTable("dbo.SampleCollectionModes");
            DropTable("dbo.CapabilityAssociatedDatas");
            DropTable("dbo.DiagnosisCapabilities");
            DropTable("dbo.Counties");
            DropTable("dbo.Countries");
            DropTable("dbo.Sexes");
            DropTable("dbo.PreservationTypes");
            DropTable("dbo.MaterialTypes");
            DropTable("dbo.MacroscopicAssessments");
            DropTable("dbo.CollectionPercentages");
            DropTable("dbo.MaterialDetails");
            DropTable("dbo.DonorCounts");
            DropTable("dbo.CollectionSampleSets");
            DropTable("dbo.HtaStatus");
            DropTable("dbo.Diagnosis");
            DropTable("dbo.ConsentRestrictions");
            DropTable("dbo.CollectionTypes");
            DropTable("dbo.CollectionStatus");
            DropTable("dbo.CollectionPoints");
            DropTable("dbo.AssociatedDataTypeGroups");
            DropTable("dbo.AssociatedDataTypes");
            DropTable("dbo.AssociatedDataProcurementTimeframes");
            DropTable("dbo.CollectionAssociatedDatas");
            DropTable("dbo.Collections");
            DropTable("dbo.Organisations");
            DropTable("dbo.OrganisationAnnualStatistics");
            DropTable("dbo.AnnualStatistics");
            DropTable("dbo.AnnualStatisticGroups");
            DropTable("dbo.AgeRanges");
            DropTable("dbo.AccessConditions");
        }
    }
}
