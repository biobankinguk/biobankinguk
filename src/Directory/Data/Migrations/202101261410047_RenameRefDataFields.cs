namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRefDataFields : DbMigration
    {
        public override void Up()
        {
            // AccessCondition
            DropForeignKey("dbo.Collections", "AccessConditionId", "dbo.AccessConditions");
            DropPrimaryKey("dbo.AccessConditions");
            RenameColumn("dbo.AccessConditions", "AccessConditionId", "Id");
            RenameColumn("dbo.AccessConditions", "Description", "Value");
            AddPrimaryKey("dbo.AccessConditions", "Id");
            AddForeignKey("dbo.Collections", "AccessConditionId", "dbo.AccessConditions", "Id", cascadeDelete: true);

            // AgeRange
            DropForeignKey("dbo.CollectionSampleSets", "AgeRangeId", "dbo.AgeRanges");
            DropPrimaryKey("dbo.AgeRanges");
            RenameColumn("dbo.AgeRanges", "AgeRangeId", "Id");
            RenameColumn("dbo.AgeRanges", "Description", "Value");
            AddPrimaryKey("dbo.AgeRanges", "Id");
            AddForeignKey("dbo.CollectionSampleSets", "AgeRangeId", "dbo.AgeRanges", "Id", cascadeDelete: true);

            // AnnualStatistic
            DropForeignKey("dbo.OrganisationAnnualStatistics", "AnnualStatisticId", "dbo.AnnualStatistics");
            DropPrimaryKey("dbo.AnnualStatistics");
            RenameColumn("dbo.AnnualStatistics", "AnnualStatisticId", "Id");
            RenameColumn("dbo.AnnualStatistics", "Name", "Value");
            AddPrimaryKey("dbo.AnnualStatistics", "Id");
            AddForeignKey("dbo.OrganisationAnnualStatistics", "AnnualStatisticId", "dbo.AnnualStatistics", "Id", cascadeDelete: true);

            // AnnualStatisticGroup
            DropForeignKey("dbo.AnnualStatistics", "AnnualStatisticGroupId", "dbo.AnnualStatisticGroups");
            DropPrimaryKey("dbo.AnnualStatisticGroups");
            RenameColumn("dbo.AnnualStatisticGroups", "AnnualStatisticGroupId", "Id");
            RenameColumn("dbo.AnnualStatisticGroups", "Name", "Value");
            AddPrimaryKey("dbo.AnnualStatisticGroups", "Id");
            AddForeignKey("dbo.AnnualStatistics", "AnnualStatisticGroupId", "dbo.AnnualStatisticGroups", "Id", cascadeDelete: true);

            // AssociatedDataProcurementTimeframe
            DropForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes");
            DropForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes");
            DropPrimaryKey("dbo.AssociatedDataProcurementTimeframes");
            RenameColumn("dbo.AssociatedDataProcurementTimeframes", "AssociatedDataProcurementTimeframeId", "Id");
            RenameColumn("dbo.AssociatedDataProcurementTimeframes", "Description", "Value");
            AddPrimaryKey("dbo.AssociatedDataProcurementTimeframes", "Id");
            AddForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes", "Id");
            AddForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes", "Id");

            // AssociatedDataType
            DropForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes");
            DropForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes");
            DropPrimaryKey("dbo.AssociatedDataTypes");
            RenameColumn("dbo.AssociatedDataTypes", "AssociatedDataTypeId", "Id");
            RenameColumn("dbo.AssociatedDataTypes", "Description", "Value");
            AddPrimaryKey("dbo.AssociatedDataTypes", "Id");
            AddForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes", "Id", cascadeDelete: true);

            // AssociatedDataTypeGroup
            DropForeignKey("dbo.AssociatedDataTypes", "AssociatedDataTypeGroupId", "dbo.AssociatedDataTypeGroups");
            DropPrimaryKey("dbo.AssociatedDataTypeGroups");
            RenameColumn("dbo.AssociatedDataTypeGroups", "AssociatedDataTypeGroupId", "Id");
            RenameColumn("dbo.AssociatedDataTypeGroups", "Description", "Value");
            AddPrimaryKey("dbo.AssociatedDataTypeGroups", "Id");
            AddForeignKey("dbo.AssociatedDataTypes", "AssociatedDataTypeGroupId", "dbo.AssociatedDataTypeGroups", "Id", cascadeDelete: true);

            // CollectionPercentage
            DropForeignKey("dbo.MaterialDetails", "CollectionPercentageId", "dbo.CollectionPercentages");
            DropPrimaryKey("dbo.CollectionPercentages");
            RenameColumn("dbo.CollectionPercentages", "CollectionPercentageId", "Id");
            RenameColumn("dbo.CollectionPercentages", "Description", "Value");
            AddPrimaryKey("dbo.CollectionPercentages", "Id");
            AddForeignKey("dbo.MaterialDetails", "CollectionPercentageId", "dbo.CollectionPercentages", "Id");

            // CollectionPoint
            DropForeignKey("dbo.Collections", "CollectionPointId", "dbo.CollectionPoints");
            DropPrimaryKey("dbo.CollectionPoints");
            RenameColumn("dbo.CollectionPoints", "CollectionPointId", "Id");
            RenameColumn("dbo.CollectionPoints", "Description", "Value");
            AddPrimaryKey("dbo.CollectionPoints", "Id");
            AddForeignKey("dbo.Collections", "CollectionPointId", "dbo.CollectionPoints", "Id", cascadeDelete: true);

            // CollectionStatus
            DropForeignKey("dbo.Collections", "CollectionStatusId", "dbo.CollectionStatus");
            DropPrimaryKey("dbo.CollectionStatus");
            RenameColumn("dbo.CollectionStatus", "CollectionStatusId", "Id");
            RenameColumn("dbo.CollectionStatus", "Description", "Value");
            AddPrimaryKey("dbo.CollectionStatus", "Id");
            AddForeignKey("dbo.Collections", "CollectionStatusId", "dbo.CollectionStatus", "Id", cascadeDelete: true);

            // CollectType
            DropForeignKey("dbo.Collections", "CollectionTypeId", "dbo.CollectionTypes");
            DropPrimaryKey("dbo.CollectionTypes");
            RenameColumn("dbo.CollectionTypes", "CollectionTypeId", "Id");
            RenameColumn("dbo.CollectionTypes", "Description", "Value");
            AddPrimaryKey("dbo.CollectionTypes", "Id");
            AddForeignKey("dbo.Collections", "CollectionTypeId", "dbo.CollectionTypes", "Id");

            // ConsentRestriction
            DropForeignKey("dbo.ConsentRestrictionCollections", "ConsentRestriction_ConsentRestrictionId", "dbo.ConsentRestrictions");
            RenameColumn(table: "dbo.ConsentRestrictionCollections", name: "ConsentRestriction_ConsentRestrictionId", newName: "ConsentRestriction_Id");
            RenameIndex(table: "dbo.ConsentRestrictionCollections", name: "IX_ConsentRestriction_ConsentRestrictionId", newName: "IX_ConsentRestriction_Id");
            DropPrimaryKey("dbo.ConsentRestrictions");
            RenameColumn("dbo.ConsentRestrictions", "ConsentRestrictionId", "Id");
            RenameColumn("dbo.ConsentRestrictions", "Description", "Value");
            AddPrimaryKey("dbo.ConsentRestrictions", "Id");
            AddForeignKey("dbo.ConsentRestrictionCollections", "ConsentRestriction_Id", "dbo.ConsentRestrictions", "Id", cascadeDelete: true);

            // Country
            DropForeignKey("dbo.Counties", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Organisations", "CountryId", "dbo.Countries");
            DropPrimaryKey("dbo.Countries");
            RenameColumn("dbo.Countries", "CountryId", "Id");
            RenameColumn("dbo.Countries", "Name", "Value");
            AddPrimaryKey("dbo.Countries", "Id");
            AddForeignKey("dbo.Counties", "CountryId", "dbo.Countries", "Id");
            AddForeignKey("dbo.Organisations", "CountryId", "dbo.Countries", "Id");

            // County
            DropForeignKey("dbo.Organisations", "CountyId", "dbo.Counties");
            DropPrimaryKey("dbo.Counties");
            RenameColumn("dbo.Counties", "CountyId", "Id");
            RenameColumn("dbo.Counties", "Name", "Value");
            AddPrimaryKey("dbo.Counties", "Id");
            AddForeignKey("dbo.Organisations", "CountyId", "dbo.Counties", "Id");

            // DonorCount
            DropForeignKey("dbo.CollectionSampleSets", "DonorCountId", "dbo.DonorCounts");
            DropPrimaryKey("dbo.DonorCounts");
            RenameColumn("dbo.DonorCounts", "DonorCountId", "Id");
            RenameColumn("dbo.DonorCounts", "Description", "Value");
            AddPrimaryKey("dbo.DonorCounts", "Id");
            AddForeignKey("dbo.CollectionSampleSets", "DonorCountId", "dbo.DonorCounts", "Id", cascadeDelete: true);

            // HtaStatus
            DropForeignKey("dbo.Collections", "HtaStatusId", "dbo.HtaStatus");
            DropPrimaryKey("dbo.HtaStatus");
            RenameColumn("dbo.HtaStatus", "HtaStatusId", "Id");
            RenameColumn("dbo.HtaStatus", "Description", "Value");
            AddPrimaryKey("dbo.HtaStatus", "Id");
            AddForeignKey("dbo.Collections", "HtaStatusId", "dbo.HtaStatus", "Id");

            // Macroscopic Assessment
            DropForeignKey("dbo.MaterialDetails", "MacroscopicAssessmentId", "dbo.MacroscopicAssessments");
            DropPrimaryKey("dbo.MacroscopicAssessments");
            RenameColumn("dbo.MacroscopicAssessments", "MacroscopicAssessmentId", "Id");
            RenameColumn("dbo.MacroscopicAssessments", "Description", "Value");
            AddPrimaryKey("dbo.MacroscopicAssessments", "Id");
            AddForeignKey("dbo.MaterialDetails", "MacroscopicAssessmentId", "dbo.MacroscopicAssessments", "Id", cascadeDelete: true);

            // RegistrationReason
            DropForeignKey("dbo.OrganisationRegistrationReasons", "RegistrationReasonId", "dbo.RegistrationReasons");
            DropPrimaryKey("dbo.RegistrationReasons");
            RenameColumn("dbo.RegistrationReasons", "RegistrationReasonId", "Id");
            RenameColumn("dbo.RegistrationReasons", "Description", "Value");
            AddPrimaryKey("dbo.RegistrationReasons", "Id");
            AddForeignKey("dbo.OrganisationRegistrationReasons", "RegistrationReasonId", "dbo.RegistrationReasons", "Id", cascadeDelete: true);

            // SampleCollectionMode
            DropForeignKey("dbo.DiagnosisCapabilities", "SampleCollectionModeId", "dbo.SampleCollectionModes");
            DropPrimaryKey("dbo.SampleCollectionModes");
            RenameColumn("dbo.SampleCollectionModes", "SampleCollectionModeId", "Id");
            RenameColumn("dbo.SampleCollectionModes", "Description", "Value");
            AddPrimaryKey("dbo.SampleCollectionModes", "Id");
            AddForeignKey("dbo.DiagnosisCapabilities", "SampleCollectionModeId", "dbo.SampleCollectionModes", "Id", cascadeDelete: true);

            // ServiceOffering
            DropForeignKey("dbo.OrganisationServiceOfferings", "ServiceId", "dbo.ServiceOfferings");
            DropIndex("dbo.OrganisationServiceOfferings", new[] { "ServiceId" });
            DropPrimaryKey("dbo.ServiceOfferings");
            RenameColumn("dbo.ServiceOfferings", "ServiceId", "Id");
            RenameColumn("dbo.ServiceOfferings", "Name", "Value");
            AddPrimaryKey("dbo.ServiceOfferings", "Id");
            CreateIndex("dbo.OrganisationServiceOfferings", "ServiceId");
            AddForeignKey("dbo.OrganisationServiceOfferings", "ServiceId", "dbo.ServiceOfferings", "Id");

            // SnomedTerm
            DropIndex("dbo.SnomedTerms", new[] { "Description" });
            RenameColumn("dbo.SnomedTerms", "Description", "Value");
            CreateIndex("dbo.SnomedTerms", "Value", unique: true);

            // SopStatus
            DropForeignKey("dbo.Networks", "SopStatusId", "dbo.SopStatus");
            DropPrimaryKey("dbo.SopStatus");
            RenameColumn("dbo.SopStatus", "SopStatusId", "Id");
            RenameColumn("dbo.SopStatus", "Description", "Value");
            AddPrimaryKey("dbo.SopStatus", "Id");
            AddForeignKey("dbo.Networks", "SopStatusId", "dbo.SopStatus", "Id", cascadeDelete: true);
        }

        public override void Down()
        {
            // AccessCondition
            DropForeignKey("dbo.Collections", "AccessConditionId", "dbo.AccessConditions");
            DropPrimaryKey("dbo.AccessConditions");
            RenameColumn("dbo.AccessConditions", "Id", "AccessConditionId");
            RenameColumn("dbo.AccessConditions", "Value", "Description");
            AddPrimaryKey("dbo.AccessConditions", "AccessConditionId");
            AddForeignKey("dbo.Collections", "AccessConditionId", "dbo.AccessConditions", "AccessConditionId", cascadeDelete: true);

            // AgeRange
            DropForeignKey("dbo.CollectionSampleSets", "AgeRangeId", "dbo.AgeRanges");
            DropPrimaryKey("dbo.AgeRanges");
            RenameColumn("dbo.AgeRanges", "Id", "AgeRangeId");
            RenameColumn("dbo.AgeRanges", "Value", "Description");
            AddPrimaryKey("dbo.AgeRanges", "AgeRangeId");
            AddForeignKey("dbo.CollectionSampleSets", "AgeRangeId", "dbo.AgeRanges", "AgeRangeId", cascadeDelete: true);

            // AnnualStatistic
            DropForeignKey("dbo.OrganisationAnnualStatistics", "AnnualStatisticId", "dbo.AnnualStatistics");
            DropPrimaryKey("dbo.AnnualStatistics");
            RenameColumn("dbo.AnnualStatistics", "Id", "AnnualStatisticId");
            RenameColumn("dbo.AnnualStatistics", "Value", "Name");
            AddPrimaryKey("dbo.AnnualStatistics", "AnnualStatisticId");
            AddForeignKey("dbo.OrganisationAnnualStatistics", "AnnualStatisticId", "dbo.AnnualStatistics", "AnnualStatisticId", cascadeDelete: true);

            // AnnualStatisticGroup
            DropForeignKey("dbo.AnnualStatistics", "AnnualStatisticGroupId", "dbo.AnnualStatisticGroups");
            DropPrimaryKey("dbo.AnnualStatisticGroups");
            RenameColumn("dbo.AnnualStatisticGroups", "Id", "AnnualStatisticGroupId");
            RenameColumn("dbo.AnnualStatisticGroups", "Value", "Name");
            AddPrimaryKey("dbo.AnnualStatisticGroups", "AnnualStatisticGroupId");
            AddForeignKey("dbo.AnnualStatistics", "AnnualStatisticGroupId", "dbo.AnnualStatisticGroups", "AnnualStatisticGroupId", cascadeDelete: true);

            // AssociatedDataProcurementTimeframes
            DropForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes");
            DropForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes");
            DropPrimaryKey("dbo.AssociatedDataProcurementTimeframes");
            RenameColumn("dbo.AssociatedDataProcurementTimeframes", "Id", "AssociatedDataProcurementTimeframeId");
            RenameColumn("dbo.AssociatedDataProcurementTimeframes", "Value", "Description");
            AddPrimaryKey("dbo.AssociatedDataProcurementTimeframes", "AssociatedDataProcurementTimeframeId");
            AddForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes", "AssociatedDataProcurementTimeframeId");
            AddForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataProcurementTimeframeId", "dbo.AssociatedDataProcurementTimeframes", "AssociatedDataProcurementTimeframeId");

            // AssociatedDataType
            DropForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes");
            DropForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes");
            DropPrimaryKey("dbo.AssociatedDataTypes");
            RenameColumn("dbo.AssociatedDataTypes", "Id", "AssociatedDataTypeId");
            RenameColumn("dbo.AssociatedDataTypes", "Value", "Description");
            AddPrimaryKey("dbo.AssociatedDataTypes", "AssociatedDataTypeId");
            AddForeignKey("dbo.CapabilityAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes", "AssociatedDataTypeId", cascadeDelete: true);
            AddForeignKey("dbo.CollectionAssociatedDatas", "AssociatedDataTypeId", "dbo.AssociatedDataTypes", "AssociatedDataTypeId", cascadeDelete: true);

            // AssociatedDataTypeGroup
            DropForeignKey("dbo.AssociatedDataTypes", "AssociatedDataTypeGroupId", "dbo.AssociatedDataTypeGroups");
            DropPrimaryKey("dbo.AssociatedDataTypeGroups");
            RenameColumn("dbo.AssociatedDataTypeGroups", "Id", "AssociatedDataTypeGroupId");
            RenameColumn("dbo.AssociatedDataTypeGroups", "Value", "Description");
            AddPrimaryKey("dbo.AssociatedDataTypeGroups", "AssociatedDataTypeGroupId");
            AddForeignKey("dbo.AssociatedDataTypes", "AssociatedDataTypeGroupId", "dbo.AssociatedDataTypeGroups", "AssociatedDataTypeGroupId", cascadeDelete: true);

            // CollectionPercentage
            DropForeignKey("dbo.MaterialDetails", "CollectionPercentageId", "dbo.CollectionPercentages");
            DropPrimaryKey("dbo.CollectionPercentages");
            RenameColumn("dbo.CollectionPercentages", "Id", "CollectionPercentageId");
            RenameColumn("dbo.CollectionPercentages", "Value", "Description");
            AddPrimaryKey("dbo.CollectionPercentages", "CollectionPercentageId");
            AddForeignKey("dbo.MaterialDetails", "CollectionPercentageId", "dbo.CollectionPercentages", "CollectionPercentageId");

            // CollectionPoint
            DropForeignKey("dbo.Collections", "CollectionPointId", "dbo.CollectionPoints");
            DropPrimaryKey("dbo.CollectionPoints");
            RenameColumn("dbo.CollectionPoints", "Id", "CollectionPointId");
            RenameColumn("dbo.CollectionPoints", "Value", "Description");
            AddPrimaryKey("dbo.CollectionPoints", "CollectionPointId");
            AddForeignKey("dbo.Collections", "CollectionPointId", "dbo.CollectionPoints", "CollectionPointId", cascadeDelete: true);

            // CollectionStatus
            DropForeignKey("dbo.Collections", "CollectionStatusId", "dbo.CollectionStatus");
            DropPrimaryKey("dbo.CollectionStatus");
            RenameColumn("dbo.CollectionStatus", "Id", "CollectionStatusId");
            RenameColumn("dbo.CollectionStatus", "Value", "Description");
            AddPrimaryKey("dbo.CollectionStatus", "CollectionStatusId");
            AddForeignKey("dbo.Collections", "CollectionStatusId", "dbo.CollectionStatus", "CollectionStatusId", cascadeDelete: true);

            // CollectionType
            DropForeignKey("dbo.Collections", "CollectionTypeId", "dbo.CollectionTypes");
            DropPrimaryKey("dbo.CollectionTypes");
            RenameColumn("dbo.CollectionTypes", "Id", "CollectionTypeId");
            RenameColumn("dbo.CollectionTypes", "Value", "Description");
            AddPrimaryKey("dbo.CollectionTypes", "CollectionTypeId");
            AddForeignKey("dbo.Collections", "CollectionTypeId", "dbo.CollectionTypes", "CollectionTypeId");

            // ConsentRestriction
            DropForeignKey("dbo.ConsentRestrictionCollections", "ConsentRestriction_Id", "dbo.ConsentRestrictions");
            DropPrimaryKey("dbo.ConsentRestrictions");
            RenameColumn("dbo.ConsentRestrictions", "Id", "ConsentRestrictionId");
            RenameColumn("dbo.ConsentRestrictions", "Value", "Description");
            AddPrimaryKey("dbo.ConsentRestrictions", "ConsentRestrictionId");
            RenameIndex(table: "dbo.ConsentRestrictionCollections", name: "IX_ConsentRestriction_Id", newName: "IX_ConsentRestriction_ConsentRestrictionId");
            RenameColumn(table: "dbo.ConsentRestrictionCollections", name: "ConsentRestriction_Id", newName: "ConsentRestriction_ConsentRestrictionId");
            AddForeignKey("dbo.ConsentRestrictionCollections", "ConsentRestriction_ConsentRestrictionId", "dbo.ConsentRestrictions", "ConsentRestrictionId", cascadeDelete: true);

            // Country
            DropForeignKey("dbo.Counties", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Organisations", "CountryId", "dbo.Countries");
            DropPrimaryKey("dbo.Countries");
            RenameColumn("dbo.Countries", "Id", "CountryId");
            RenameColumn("dbo.Countries", "Value", "Name");
            AddPrimaryKey("dbo.Countries", "CountryId");
            AddForeignKey("dbo.Counties", "CountryId", "dbo.Countries");
            AddForeignKey("dbo.Organisations", "CountryId", "dbo.Countries");

            // County
            DropForeignKey("dbo.Organisations", "CountyId", "dbo.Counties");
            DropPrimaryKey("dbo.Counties");
            RenameColumn("dbo.Counties", "Id", "CountyId");
            RenameColumn("dbo.Counties", "Value", "Name");
            AddPrimaryKey("dbo.Counties", "CountyId");
            AddForeignKey("dbo.Organisations", "CountyId", "dbo.Counties", "CountyId");

            // DonorCount
            DropForeignKey("dbo.CollectionSampleSets", "DonorCountId", "dbo.DonorCounts");
            DropPrimaryKey("dbo.DonorCounts");
            RenameColumn("dbo.DonorCounts", "Id", "DonorCountId");
            RenameColumn("dbo.DonorCounts", "Value", "Description");
            AddPrimaryKey("dbo.DonorCounts", "DonorCountId");
            AddForeignKey("dbo.CollectionSampleSets", "DonorCountId", "dbo.DonorCounts", "DonorCountId", cascadeDelete: true);

            // HtaStatus
            DropForeignKey("dbo.Collections", "HtaStatusId", "dbo.HtaStatus");
            DropPrimaryKey("dbo.HtaStatus");
            RenameColumn("dbo.HtaStatus", "Id", "HtaStatusId");
            RenameColumn("dbo.HtaStatus", "Value", "Description");
            AddPrimaryKey("dbo.HtaStatus", "HtaStatusId");
            AddForeignKey("dbo.Collections", "HtaStatusId", "dbo.HtaStatus", "HtaStatusId");

            // Macroscopic Assessment
            DropForeignKey("dbo.MaterialDetails", "MacroscopicAssessmentId", "dbo.MacroscopicAssessments");
            DropPrimaryKey("dbo.MacroscopicAssessments");
            RenameColumn("dbo.MacroscopicAssessments", "Id", "MacroscopicAssessmentId");
            RenameColumn("dbo.MacroscopicAssessments", "Value", "Description");
            AddPrimaryKey("dbo.MacroscopicAssessments", "MacroscopicAssessmentId");
            AddForeignKey("dbo.MaterialDetails", "MacroscopicAssessmentId", "dbo.MacroscopicAssessments", "MacroscopicAssessmentId", cascadeDelete: true);

            // RegistrationReason
            DropForeignKey("dbo.OrganisationRegistrationReasons", "RegistrationReasonId", "dbo.RegistrationReasons");
            DropPrimaryKey("dbo.RegistrationReasons");
            RenameColumn("dbo.RegistrationReasons", "Id", "RegistrationReasonId");
            RenameColumn("dbo.RegistrationReasons", "Value", "Description");
            AddPrimaryKey("dbo.RegistrationReasons", "RegistrationReasonId");
            AddForeignKey("dbo.OrganisationRegistrationReasons", "RegistrationReasonId", "dbo.RegistrationReasons", "RegistrationReasonId", cascadeDelete: true);

            // SampleCollectionMode
            DropForeignKey("dbo.DiagnosisCapabilities", "SampleCollectionModeId", "dbo.SampleCollectionModes");
            DropPrimaryKey("dbo.SampleCollectionModes");
            RenameColumn("dbo.SampleCollectionModes", "Id", "SampleCollectionModeId");
            RenameColumn("dbo.SampleCollectionModes", "Value", "Description");
            AddPrimaryKey("dbo.SampleCollectionModes", "SampleCollectionModeId");
            AddForeignKey("dbo.DiagnosisCapabilities", "SampleCollectionModeId", "dbo.SampleCollectionModes", "SampleCollectionModeId", cascadeDelete: true);

            // ServiceOffering
            DropForeignKey("dbo.OrganisationServiceOfferings", "ServiceId", "dbo.ServiceOfferings");
            DropIndex("dbo.OrganisationServiceOfferings", new[] { "ServiceId" });
            DropPrimaryKey("dbo.ServiceOfferings");
            RenameColumn("dbo.ServiceOfferings", "Id", "ServiceId");
            RenameColumn("dbo.ServiceOfferings", "Value", "Name");
            AddPrimaryKey("dbo.ServiceOfferings", "ServiceId");
            CreateIndex("dbo.OrganisationServiceOfferings", "ServiceId");
            AddForeignKey("dbo.OrganisationServiceOfferings", "ServiceId", "dbo.ServiceOfferings", "ServiceId", cascadeDelete: true);

            // SnomedTerm
            DropIndex("dbo.SnomedTerms", new[] { "Value" });
            RenameColumn("dbo.SnomedTerms", "Value", "Description");
            CreateIndex("dbo.SnomedTerms", "Description", unique: true);

            // SopStatus
            DropForeignKey("dbo.Networks", "SopStatusId", "dbo.SopStatus");
            DropPrimaryKey("dbo.SopStatus");
            RenameColumn("dbo.SopStatus", "Id", "SopStatusId");
            RenameColumn("dbo.SopStatus", "Value", "Description");
            AddPrimaryKey("dbo.SopStatus", "SopStatusId");
            AddForeignKey("dbo.Networks", "SopStatusId", "dbo.SopStatus", "SopStatusId", cascadeDelete: true);
        }
    }
}
