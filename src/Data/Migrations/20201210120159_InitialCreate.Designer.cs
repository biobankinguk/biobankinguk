﻿// <auto-generated />
using System;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Biobanks.Data.Migrations
{
    [DbContext(typeof(BiobanksDbContext))]
    [Migration("20201210120159_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Biobanks.Common.Data.Entities.Error", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecordIdentifiers")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SubmissionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SubmissionId");

                    b.ToTable("Errors");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.LiveDiagnosis", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateDiagnosed")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiagnosisCodeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DiagnosisCodeOntologyVersionId")
                        .HasColumnType("int");

                    b.Property<string>("IndividualReferenceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("SubmissionTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("DiagnosisCodeId");

                    b.HasIndex("DiagnosisCodeOntologyVersionId");

                    b.HasIndex("OrganisationId", "IndividualReferenceId", "DateDiagnosed", "DiagnosisCodeId")
                        .IsUnique();

                    b.ToTable("Diagnoses");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.LiveSample", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AgeAtDonation")
                        .HasColumnType("int");

                    b.Property<string>("Barcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("date");

                    b.Property<string>("ExtractionProcedureId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ExtractionSiteId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ExtractionSiteOntologyVersionId")
                        .HasColumnType("int");

                    b.Property<string>("IndividualReferenceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("MaterialTypeId")
                        .HasColumnType("int");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<string>("SampleContentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("SampleContentMethodId")
                        .HasColumnType("int");

                    b.Property<int?>("SexId")
                        .HasColumnType("int");

                    b.Property<int?>("StorageTemperatureId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("SubmissionTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("YearOfBirth")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ExtractionProcedureId");

                    b.HasIndex("ExtractionSiteId");

                    b.HasIndex("ExtractionSiteOntologyVersionId");

                    b.HasIndex("MaterialTypeId");

                    b.HasIndex("SampleContentId");

                    b.HasIndex("SampleContentMethodId");

                    b.HasIndex("SexId");

                    b.HasIndex("StorageTemperatureId");

                    b.HasIndex("OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName")
                        .IsUnique()
                        .HasFilter("[CollectionName] IS NOT NULL");

                    b.ToTable("Samples");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.LiveTreatment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateTreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("IndividualReferenceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("SubmissionTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("TreatmentCodeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TreatmentCodeOntologyVersionId")
                        .HasColumnType("int");

                    b.Property<int?>("TreatmentLocationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TreatmentCodeId");

                    b.HasIndex("TreatmentCodeOntologyVersionId");

                    b.HasIndex("TreatmentLocationId");

                    b.HasIndex("OrganisationId", "IndividualReferenceId", "DateTreated", "TreatmentCodeId")
                        .IsUnique();

                    b.ToTable("Treatments");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.MaterialType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MaterialTypes");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.MaterialTypeGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MaterialTypeGroups");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.Ontology", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ontologies");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("OntologyId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OntologyId");

                    b.ToTable("OntologyVersions");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.SampleContentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SampleContentMethods");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.Sex", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sexes");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.SnomedTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SnomedTags");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SnomedTagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SnomedTagId");

                    b.ToTable("SnomedTerms");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.Status", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.StorageTemperature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StorageTemperatures");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.TreatmentLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TreatmentLocations");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedDiagnosis", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateDiagnosed")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiagnosisCodeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("DiagnosisCodeOntologyVersionId")
                        .HasColumnType("int");

                    b.Property<string>("IndividualReferenceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("SubmissionTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("DiagnosisCodeId");

                    b.HasIndex("DiagnosisCodeOntologyVersionId");

                    b.HasIndex("OrganisationId", "IndividualReferenceId", "DateDiagnosed", "DiagnosisCodeId")
                        .IsUnique();

                    b.ToTable("StagedDiagnoses");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedDiagnosisDelete", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("StagedDiagnosisDeletes");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedSample", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AgeAtDonation")
                        .HasColumnType("int");

                    b.Property<string>("Barcode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("date");

                    b.Property<string>("ExtractionProcedureId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ExtractionSiteId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ExtractionSiteOntologyVersionId")
                        .HasColumnType("int");

                    b.Property<string>("IndividualReferenceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("MaterialTypeId")
                        .HasColumnType("int");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<string>("SampleContentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("SampleContentMethodId")
                        .HasColumnType("int");

                    b.Property<int?>("SexId")
                        .HasColumnType("int");

                    b.Property<int?>("StorageTemperatureId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("SubmissionTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("YearOfBirth")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ExtractionProcedureId");

                    b.HasIndex("ExtractionSiteId");

                    b.HasIndex("ExtractionSiteOntologyVersionId");

                    b.HasIndex("MaterialTypeId");

                    b.HasIndex("SampleContentId");

                    b.HasIndex("SampleContentMethodId");

                    b.HasIndex("SexId");

                    b.HasIndex("StorageTemperatureId");

                    b.HasIndex("OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName")
                        .IsUnique()
                        .HasFilter("[CollectionName] IS NOT NULL");

                    b.ToTable("StagedSamples");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedSampleDelete", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("StagedSampleDeletes");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedTreatment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateTreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("IndividualReferenceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("SubmissionTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("TreatmentCodeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TreatmentCodeOntologyVersionId")
                        .HasColumnType("int");

                    b.Property<int?>("TreatmentLocationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TreatmentCodeId");

                    b.HasIndex("TreatmentCodeOntologyVersionId");

                    b.HasIndex("TreatmentLocationId");

                    b.HasIndex("OrganisationId", "IndividualReferenceId", "DateTreated", "TreatmentCodeId")
                        .IsUnique();

                    b.ToTable("StagedTreatments");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedTreatmentDelete", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("StagedTreatmentDeletes");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.Submission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BiobankId")
                        .HasColumnType("int");

                    b.Property<int>("RecordsProcessed")
                        .HasColumnType("int");

                    b.Property<DateTime>("StatusChangeTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmissionTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<int>("TotalRecords")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("MaterialTypeMaterialTypeGroup", b =>
                {
                    b.Property<int>("MaterialTypeGroupsId")
                        .HasColumnType("int");

                    b.Property<int>("MaterialTypesId")
                        .HasColumnType("int");

                    b.HasKey("MaterialTypeGroupsId", "MaterialTypesId");

                    b.HasIndex("MaterialTypesId");

                    b.ToTable("MaterialTypeMaterialTypeGroup");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.Error", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.Submission", "Submission")
                        .WithMany("Errors")
                        .HasForeignKey("SubmissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Submission");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.LiveDiagnosis", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "DiagnosisCode")
                        .WithMany()
                        .HasForeignKey("DiagnosisCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", "DiagnosisCodeOntologyVersion")
                        .WithMany()
                        .HasForeignKey("DiagnosisCodeOntologyVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiagnosisCode");

                    b.Navigation("DiagnosisCodeOntologyVersion");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.LiveSample", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "ExtractionProcedure")
                        .WithMany()
                        .HasForeignKey("ExtractionProcedureId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "ExtractionSite")
                        .WithMany()
                        .HasForeignKey("ExtractionSiteId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", "ExtractionSiteOntologyVersion")
                        .WithMany()
                        .HasForeignKey("ExtractionSiteOntologyVersionId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.MaterialType", "MaterialType")
                        .WithMany()
                        .HasForeignKey("MaterialTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "SampleContent")
                        .WithMany()
                        .HasForeignKey("SampleContentId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SampleContentMethod", "SampleContentMethod")
                        .WithMany()
                        .HasForeignKey("SampleContentMethodId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.Sex", "Sex")
                        .WithMany()
                        .HasForeignKey("SexId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.StorageTemperature", "StorageTemperature")
                        .WithMany()
                        .HasForeignKey("StorageTemperatureId");

                    b.Navigation("ExtractionProcedure");

                    b.Navigation("ExtractionSite");

                    b.Navigation("ExtractionSiteOntologyVersion");

                    b.Navigation("MaterialType");

                    b.Navigation("SampleContent");

                    b.Navigation("SampleContentMethod");

                    b.Navigation("Sex");

                    b.Navigation("StorageTemperature");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.LiveTreatment", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "TreatmentCode")
                        .WithMany()
                        .HasForeignKey("TreatmentCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", "TreatmentCodeOntologyVersion")
                        .WithMany()
                        .HasForeignKey("TreatmentCodeOntologyVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.TreatmentLocation", "TreatmentLocation")
                        .WithMany()
                        .HasForeignKey("TreatmentLocationId");

                    b.Navigation("TreatmentCode");

                    b.Navigation("TreatmentCodeOntologyVersion");

                    b.Navigation("TreatmentLocation");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.Ontology", "Ontology")
                        .WithMany("OntologyVersions")
                        .HasForeignKey("OntologyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ontology");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTag", "SnomedTag")
                        .WithMany("SnomedTerms")
                        .HasForeignKey("SnomedTagId");

                    b.Navigation("SnomedTag");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedDiagnosis", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "DiagnosisCode")
                        .WithMany()
                        .HasForeignKey("DiagnosisCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", "DiagnosisCodeOntologyVersion")
                        .WithMany()
                        .HasForeignKey("DiagnosisCodeOntologyVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DiagnosisCode");

                    b.Navigation("DiagnosisCodeOntologyVersion");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedSample", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "ExtractionProcedure")
                        .WithMany()
                        .HasForeignKey("ExtractionProcedureId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "ExtractionSite")
                        .WithMany()
                        .HasForeignKey("ExtractionSiteId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", "ExtractionSiteOntologyVersion")
                        .WithMany()
                        .HasForeignKey("ExtractionSiteOntologyVersionId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.MaterialType", "MaterialType")
                        .WithMany()
                        .HasForeignKey("MaterialTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "SampleContent")
                        .WithMany()
                        .HasForeignKey("SampleContentId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SampleContentMethod", "SampleContentMethod")
                        .WithMany()
                        .HasForeignKey("SampleContentMethodId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.Sex", "Sex")
                        .WithMany()
                        .HasForeignKey("SexId");

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.StorageTemperature", "StorageTemperature")
                        .WithMany()
                        .HasForeignKey("StorageTemperatureId");

                    b.Navigation("ExtractionProcedure");

                    b.Navigation("ExtractionSite");

                    b.Navigation("ExtractionSiteOntologyVersion");

                    b.Navigation("MaterialType");

                    b.Navigation("SampleContent");

                    b.Navigation("SampleContentMethod");

                    b.Navigation("Sex");

                    b.Navigation("StorageTemperature");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.StagedTreatment", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.SnomedTerm", "TreatmentCode")
                        .WithMany()
                        .HasForeignKey("TreatmentCodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.OntologyVersion", "TreatmentCodeOntologyVersion")
                        .WithMany()
                        .HasForeignKey("TreatmentCodeOntologyVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.TreatmentLocation", "TreatmentLocation")
                        .WithMany()
                        .HasForeignKey("TreatmentLocationId");

                    b.Navigation("TreatmentCode");

                    b.Navigation("TreatmentCodeOntologyVersion");

                    b.Navigation("TreatmentLocation");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.Submission", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Status");
                });

            modelBuilder.Entity("MaterialTypeMaterialTypeGroup", b =>
                {
                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.MaterialTypeGroup", null)
                        .WithMany()
                        .HasForeignKey("MaterialTypeGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Biobanks.Common.Data.Entities.ReferenceData.MaterialType", null)
                        .WithMany()
                        .HasForeignKey("MaterialTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.Ontology", b =>
                {
                    b.Navigation("OntologyVersions");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.ReferenceData.SnomedTag", b =>
                {
                    b.Navigation("SnomedTerms");
                });

            modelBuilder.Entity("Biobanks.Common.Data.Entities.Submission", b =>
                {
                    b.Navigation("Errors");
                });
#pragma warning restore 612, 618
        }
    }
}
