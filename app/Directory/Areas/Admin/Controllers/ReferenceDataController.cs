using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Areas.Admin.Models.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
public class ReferenceDataController : Controller
{

  private readonly IBiobankReadService _biobankReadService;
  private readonly IConfigService _configService;
  private readonly IReferenceDataCrudService<AccessCondition> _accessConditionService;
  private readonly IReferenceDataCrudService<AgeRange> _ageRangeService;
  private readonly IReferenceDataCrudService<AnnualStatistic> _annualStatisticsService;
  private readonly IReferenceDataCrudService<AnnualStatisticGroup> _annualStatisticGroupService;
  private readonly IReferenceDataCrudService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeframeService;
  private readonly IReferenceDataCrudService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
  private readonly IReferenceDataCrudService<AssociatedDataType> _associatedDataTypeService;
  private readonly IReferenceDataCrudService<CollectionPercentage> _collectionPercentageService;
  private readonly IReferenceDataCrudService<CollectionStatus> _collectionStatusService;
  private readonly IReferenceDataCrudService<CollectionType> _collectionTypeService;
  private readonly IReferenceDataCrudService<Country> _countryService;
  private readonly IReferenceDataCrudService<County> _countyService;
  private readonly IReferenceDataCrudService<ConsentRestriction> _consentRestrictionService;
  private readonly IReferenceDataCrudService<DonorCount> _donorCountService;
  private readonly IMaterialTypeService _materialTypeService;
  private readonly IReferenceDataCrudService<MaterialTypeGroup> _materialTypeGroupService;
  private readonly IReferenceDataCrudService<MacroscopicAssessment> _macroscopicAssessmentService;
  private readonly IReferenceDataCrudService<PreservationType> _preservationTypeService;
  private readonly IReferenceDataCrudService<RegistrationReason> _registrationReasonService;
  private readonly IReferenceDataCrudService<SampleCollectionMode> _sampleCollectionModeService;
  private readonly IReferenceDataCrudService<ServiceOffering> _serviceOfferingService;
  private readonly IReferenceDataCrudService<Sex> _sexService;
  private readonly IReferenceDataCrudService<SopStatus> _sopStatusService;
  private readonly IReferenceDataCrudService<StorageTemperature> _storageTemperatureService;
  private readonly IOntologyTermService _ontologyTermService;

  public ReferenceDataController(
    IBiobankReadService biobankReadService,
    IConfigService configService,
    IReferenceDataCrudService<AccessCondition> accessConditionService, 
    IReferenceDataCrudService<AgeRange> ageRangeService,
    IReferenceDataCrudService<AnnualStatistic> annualStatisticsService,
    IReferenceDataCrudService<AnnualStatisticGroup> annualStatisticGroupService,
    IReferenceDataCrudService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeframeService,
    IReferenceDataCrudService<AssociatedDataType> associatedDataTypeService,
    IReferenceDataCrudService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
    IReferenceDataCrudService<CollectionPercentage> collectionPercentageService,
    IReferenceDataCrudService<CollectionStatus> collectionStatusService,
    IReferenceDataCrudService<CollectionType> collectionTypeService, 
    IReferenceDataCrudService<Country> countryService,
    IReferenceDataCrudService<County> countyService, 
    IReferenceDataCrudService<ConsentRestriction> consentRestrictionService,
    IReferenceDataCrudService<DonorCount> donorCountService, 
    IMaterialTypeService materialTypeService,
    IReferenceDataCrudService<MaterialTypeGroup> materialTypeGroupService,
    IReferenceDataCrudService<MacroscopicAssessment> macroscopicAssessmentService,
    IReferenceDataCrudService<PreservationType> preservationTypeService,
    IReferenceDataCrudService<RegistrationReason> registrationReasonService,
    IReferenceDataCrudService<SampleCollectionMode> sampleCollectionModeService,
    IReferenceDataCrudService<ServiceOffering> serviceOfferingService, 
    IReferenceDataCrudService<Sex> sexService,
    IReferenceDataCrudService<SopStatus> sopStatusService,
    IReferenceDataCrudService<StorageTemperature> storageTemperatureService, 
    IOntologyTermService ontologyTermService
    )
  {
    _biobankReadService = biobankReadService;
    _configService = configService;
    _accessConditionService = accessConditionService;
    _ageRangeService = ageRangeService;
    _annualStatisticsService = annualStatisticsService;
    _annualStatisticGroupService = annualStatisticGroupService;
    _associatedDataProcurementTimeframeService = associatedDataProcurementTimeframeService;
    _associatedDataTypeService = associatedDataTypeService;
    _associatedDataTypeGroupService = associatedDataTypeGroupService;
    _collectionPercentageService = collectionPercentageService;
    _collectionStatusService = collectionStatusService;
    _collectionTypeService = collectionTypeService;
    _countryService = countryService;
    _countyService = countyService;
    _consentRestrictionService = consentRestrictionService;
    _donorCountService = donorCountService;
    _materialTypeService = materialTypeService;
    _materialTypeGroupService = materialTypeGroupService;
    _macroscopicAssessmentService = macroscopicAssessmentService;
    _preservationTypeService = preservationTypeService;
    _registrationReasonService = registrationReasonService;
    _sampleCollectionModeService = sampleCollectionModeService;
    _serviceOfferingService = serviceOfferingService;
    _sexService = sexService;
    _sopStatusService = sopStatusService;
    _storageTemperatureService = storageTemperatureService;
    _ontologyTermService = ontologyTermService;
  }
  public ActionResult LockedRef()
  {
    return View();
  }
  
  public ActionResult AddRefDataSuccessFeedbackAjax(RefDataFeedbackModel feedback)
  {
      this.SetTemporaryFeedbackMessage($"The {feedback.RefDataType.ToLower()} \"{feedback.Name}\" has been added successfully.", FeedbackMessageType.Success);
      return Redirect(feedback.RedirectUrl);
  }
  public ActionResult EditRefDataSuccessFeedbackAjax(RefDataFeedbackModel feedback)
  {
      this.SetTemporaryFeedbackMessage($"The {feedback.RefDataType.ToLower()} \"{feedback.Name}\" has been edited successfully.", FeedbackMessageType.Success);
      return Redirect(feedback.RedirectUrl);
  }
  public ActionResult DeleteRefDataSuccessFeedbackAjax(RefDataFeedbackModel feedback)
  {
      this.SetTemporaryFeedbackMessage($"The {feedback.RefDataType.ToLower()} \"{feedback.Name}\" has been deleted successfully.", FeedbackMessageType.Success);
      return Redirect(feedback.RedirectUrl);
  }

  #region RefData: Access Conditions
  public async Task<ActionResult> AccessConditions()
  {
      var models = (await _accessConditionService.List())
      .Select(x =>
          Task.Run(async () => new ReadAccessConditionsModel
          {
              Id = x.Id,
              Description = x.Value,
              SortOrder = x.SortOrder,
              AccessConditionCount = await _accessConditionService.GetUsageCount(x.Id),
          }
          )
          .Result
      )
      .ToList();

      return View(new AccessConditionsModel
      {
          AccessConditions = models
      });
  }
  #endregion

  #region RefData: Age Ranges
  public async Task<ActionResult> AgeRanges()
  {
      var models = (await _ageRangeService.List())
          .Select(x =>
              Task.Run(async () => new AgeRangeModel()
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  SampleSetsCount = await _ageRangeService.GetUsageCount(x.Id),
                  LowerBound = ConvertFromIsoDuration(x.LowerBound),
                  UpperBound = ConvertFromIsoDuration(x.UpperBound)
              })
              .Result
          )
          .ToList();

      return View(new AgeRangesModel
      {
          AgeRanges = models
      });

  }

  // Converting from Iso Duration to 'plain text' - e.g P6M -> 6 Months
  private static string ConvertFromIsoDuration(string bound)
  {
      var dict = new Dictionary<string, string>
      {
          { "D", "Days" },
          { "M", "Months" },
          { "Y", "Years" }
      };

      var converted = string.IsNullOrEmpty(bound) ? "" : bound.Replace("P", "").Replace(bound.Last().ToString(), " " + dict[bound.Last().ToString()]);

      return converted;
  }


  #endregion
  
  #region RefData: AssociatedDataProcurementTimeFrame
  public async Task<ActionResult> AssociatedDataProcurementTimeFrame()
  {
      return View(new Models.ReferenceData.AssociatedDataProcurementTimeFrameModel
      {
          AssociatedDataProcurementTimeFrameModels = (await _associatedDataProcurementTimeframeService.List())
              .Select(x =>
                  Task.Run(async () => new ReadAssociatedDataProcurementTimeFrameModel
                  {
                      Id = x.Id,
                      Description = x.Value,
                      DisplayName = x.DisplayValue,
                      CollectionCapabilityCount = await _associatedDataProcurementTimeframeService.GetUsageCount(x.Id),
                      SortOrder = x.SortOrder
                  })
                  .Result
              )
              .ToList()
      });
  }
  #endregion

  #region RefData: AnnualStatistics
  public async Task<ActionResult> AnnualStatistics()
  {
      var groups = (await _annualStatisticGroupService.List())
          .Select(x => new AnnualStatisticGroupModel
          {
              AnnualStatisticGroupId = x.Id,
              Name = x.Value,
          })
          .ToList();

      var models = (await _annualStatisticsService.List())
          .Select(x =>
              Task.Run(async () => new AnnualStatisticModel
              {
                  Id = x.Id,
                  Name = x.Value,
                  UsageCount = await _annualStatisticsService.GetUsageCount(x.Id),
                  AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                  AnnualStatisticGroupName = groups.Where(y => y.AnnualStatisticGroupId == x.AnnualStatisticGroupId).FirstOrDefault()?.Name,
              })
              .Result
          )
          .ToList();

      return View(new AnnualStatisticsModel
      {
          AnnualStatistics = models,
          AnnualStatisticGroups = groups
      });

  }

  #endregion

  #region RefData: Material Types
  public async Task<ActionResult> MaterialTypes()
  {
      var materialTypes = await _materialTypeService.List();

      return View(new MaterialTypesModel
      {
          MaterialTypes = materialTypes.Select(x => Task.Run(
              async () => new ReadMaterialTypeModel
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  MaterialTypeGroups = x.MaterialTypeGroups.Select(x => x.Value),
                  MaterialDetailCount = await _biobankReadService.GetMaterialTypeMaterialDetailCount(x.Id),
                  UsedByExtractionProcedures = await _biobankReadService.IsMaterialTypeAssigned(x.Id)

              }))
          .Select(x => x.Result)
          .ToList()
      });
  }
  #endregion

  #region RefData: Material Type Groups
  public async Task<ActionResult> MaterialTypeGroups()
  {
      var materialTypes = await _materialTypeGroupService.List();

      return View(materialTypes.Select(x => new MaterialTypeGroupModel
      {
          Id = x.Id,
          Description = x.Value,
          MaterialTypes = x.MaterialTypes.Select(x => x.Value),
          MaterialTypeCount = x.MaterialTypes.Count()
      }));
  }
  #endregion

  #region RefData: Disease Status
  public async Task<ActionResult> DiseaseStatuses()
  {
      var diseaseTerms = await _ontologyTermService.List(tags: new List<string>
      {
          SnomedTags.Disease
      });

      return View(diseaseTerms.Select(x =>

          Task.Run(async () => new ReadOntologyTermModel
          {
              OntologyTermId = x.Id,
              Description = x.Value,
              CollectionCapabilityCount = await _ontologyTermService.CountCollectionCapabilityUsage(x.Id),
              OtherTerms = x.OtherTerms,
              AssociatedDataTypes = x.AssociatedDataTypes.Select(y => new AssociatedDataTypeModel
                  {
                      Id = y.Id,
                      Name = y.Value
                  }).ToList()
          })
          .Result
      ));
  }

  public async Task<ActionResult> PagingatedDiseaseStatuses(int draw, int start, int length, IDictionary<string, string> search)
  {
      // Select Search By Value
      var searchValue = search.TryGetValue("value", out var s) ? s : "";
      var tags = new List<string> { SnomedTags.Disease };

      // Get Disease Statuses
      var diseaseTerms = await _ontologyTermService.ListPaginated(start, length, searchValue, tags);
      var filteredCount = await _ontologyTermService.Count(value: searchValue, tags: tags);
      var totalCount = await _ontologyTermService.Count(tags: tags);

      var data = diseaseTerms.Select(x =>
          Task.Run(async () => new ReadOntologyTermModel
          {
              OntologyTermId = x.Id,
              Description = x.Value,
              OtherTerms = x.OtherTerms,
              DisplayOnDirectory = x.DisplayOnDirectory,
              CollectionCapabilityCount = await _ontologyTermService.CountCollectionCapabilityUsage(x.Id),
              AssociatedDataTypes = x.AssociatedDataTypes==null?
                  null
                  :
                  x.AssociatedDataTypes.Select(y => new AssociatedDataTypeModel
                  {
                      Id = y.Id,
                      Name = y.Value,
                  }).ToList()
          })
          .Result
      );

      return Ok(new
      {
          draw,
          data,
          recordsTotal = totalCount,
          recordsFiltered = filteredCount
      });
  }
  #endregion

  #region RefData: Collection Percentages
  public async Task<ActionResult> CollectionPercentages()
  {
      var models = (await _collectionPercentageService.List())
          .Select(x =>
              Task.Run(async () => new CollectionPercentageModel()
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  LowerBound = x.LowerBound,
                  UpperBound = x.UpperBound,
                  SampleSetsCount = await _collectionPercentageService.GetUsageCount(x.Id)
              })
              .Result
          )
          .ToList();
      if (await _configService.GetFlagConfigValue(ConfigKey.ShowPreservationPercentage) == true)
      {
          return View(new CollectionPercentagesModel()
          {
              CollectionPercentages = models
          });
      }
      else
      {
          return RedirectToAction("LockedRef");
      }
  }

  #endregion

  #region RefData: Donor Counts

  public async Task<ActionResult> DonorCounts()
  {
      var models = (await _donorCountService.List())
          .Select(x =>
              Task.Run(async () => new DonorCountModel()
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  LowerBound = x.LowerBound,
                  UpperBound = x.UpperBound,
                  SampleSetsCount = await _donorCountService.GetUsageCount(x.Id)
              })
                  .Result
          )
          .ToList();

      return View(new DonorCountsModel()
      {
          DonorCounts = models
      });
  }
  #endregion

  #region RefData: Collection Type

  public async Task<ActionResult> CollectionType()
  {
      return View(new Models.ReferenceData.CollectionTypeModel
      {
          CollectionTypes = (await _collectionTypeService.List())
               .Select(x =>

           Task.Run(async () => new ReadCollectionTypeModel
           {
               Id = x.Id,
               Description = x.Value,
               CollectionCount = await _collectionTypeService.GetUsageCount(x.Id),
               SortOrder = x.SortOrder
           }).Result)

               .ToList()
      });
  }
  #endregion

  #region RefData: Storage Temperature

  public async Task<ActionResult> StorageTemperatures()
  {
      var models = (await _storageTemperatureService.List())
          .Select(x =>
              Task.Run(async () =>
                  new StorageTemperatureModel()
                  {
                      Id = x.Id,
                      Value = x.Value,
                      SortOrder = x.SortOrder,
                      IsInUse = await _storageTemperatureService.IsInUse(x.Id),
                      SampleSetsCount = await _storageTemperatureService.GetUsageCount(x.Id)
                  }
              ).Result
          )
          .ToList();

      return View(new StorageTemperaturesModel
      {
          StorageTemperatures = models
      });
  }
  #endregion

  #region RefData: Preservation Type

  public async Task<ActionResult> PreservationTypes()
  {
      var models = (await _preservationTypeService.List())
          .Select(x =>
              Task.Run(async () =>
                  new PreservationTypeModel()
                  {
                      Id = x.Id,
                      Value = x.Value,
                      SortOrder = x.SortOrder,
                      StorageTemperatureId = x.StorageTemperatureId,
                      StorageTemperatureName = x.StorageTemperature?.Value ?? "",
                      PreservationTypeCount = await _preservationTypeService.GetUsageCount(x.Id)
                  }
              ).Result
          )
          .ToList();

      return View(new PreservationTypesModel
      {
          PreservationTypes = models,
          StorageTemperatures = await _storageTemperatureService.List()
      });
  }

  #endregion

  #region RefData: Associated Data Types

  public async Task<ActionResult> AssociatedDataTypes()
  {
      var associatedDataTypes = await _associatedDataTypeService.List();

      var model = associatedDataTypes
          .Select(x =>
              Task.Run(async () => new AssociatedDataTypeModel
              {
                  Id = x.Id,
                  Name = x.Value,
                  Message = x.Message,
                  CollectionCapabilityCount = await _associatedDataTypeService.GetUsageCount(x.Id),
                  AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                  AssociatedDataTypeGroupName = x.AssociatedDataTypeGroup?.Value,
                  OntologyTerms = (x.OntologyTerms != null)?
                      x.OntologyTerms.Select(y=> new OntologyTermModel
                      {
                          OntologyTermId = y.Id,
                          Description = y.Value,
                          OtherTerms = y.OtherTerms,
                          DisplayOnDirectory= y.DisplayOnDirectory
                      }).ToList()
                      :
                      null
              })
              .Result
          )
          .ToList();

      var groups = (await _associatedDataTypeGroupService.List())
        .Select(x =>
          Task.Run(() => Task.FromResult(new AssociatedDataTypeGroupModel
            {
              AssociatedDataTypeGroupId = x.Id,
              Name = x.Value
            }))
            .Result
        )
        .ToList();

      return View(new AssociatedDataTypesModel
      {
          AssociatedDataTypes = model,
          AssociatedDataTypeGroups = groups
      });
  }


  #endregion

  #region RefData: Associated Data Type Groups
  public async Task<ActionResult> AssociatedDataTypeGroups()
  {
      return View(new AssociatedDataTypesGroupModel
      {
          AssociatedDataTypeGroups = (await _associatedDataTypeGroupService.List())
              .Select(x =>
                  Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                  {
                      AssociatedDataTypeGroupId = x.Id,
                      Name = x.Value,
                      AssociatedDataTypeGroupCount = await _associatedDataTypeGroupService.GetUsageCount(x.Id)
                  })
                  .Result
              )
              .ToList()
      });
  }

  #endregion

  #region RefData: Consent Restrictions
  public async Task<ActionResult> ConsentRestriction()
  {
      return View(new Models.ReferenceData.ConsentRestrictionModel
      {
          ConsentRestrictions = (await _consentRestrictionService.List())
              .Select(x =>

                  Task.Run(async () => new ReadConsentRestrictionModel
                  {
                      Id = x.Id,
                      Description = x.Value,
                      CollectionCount = await _consentRestrictionService.GetUsageCount(x.Id),
                      SortOrder = x.SortOrder
                  }).Result)

              .ToList()
      });
  }

  #endregion

  #region RefData: Collection Status
  public async Task<ActionResult> CollectionStatus()
  {
      return View(new Models.ReferenceData.CollectionStatusModel
      {
          CollectionStatuses = (await _collectionStatusService.List())
              .Select(x =>

          Task.Run(async () => new ReadCollectionStatusModel
          {
              Id = x.Id,
              Description = x.Value,
              CollectionCount = await _collectionStatusService.GetUsageCount(x.Id),
              SortOrder = x.SortOrder
          }).Result)

              .ToList()
      });
  }
  #endregion 

  #region RefData: Annual Statistic Groups
  public async Task<ActionResult> AnnualStatisticGroups()
  {
      return View(new AnnualStatisticGroupsModel
      {
          AnnualStatisticGroups = (await _annualStatisticGroupService.List())
              .Select(x =>

              Task.Run(async () => new ReadAnnualStatisticGroupModel
              {
                  AnnualStatisticGroupId = x.Id,
                  Name = x.Value,
                  AnnualStatisticGroupCount = await _annualStatisticGroupService.GetUsageCount(x.Id)
              }).Result)

              .ToList()
      });
  }

  #endregion

  #region RefData: Sample Collection Mode
  public async Task<ActionResult> SampleCollectionModes()
  {
      var models = (await _sampleCollectionModeService.List())
          .Select(x =>
              Task.Run(async () => new SampleCollectionModeModel
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  SampleSetsCount = await _sampleCollectionModeService.GetUsageCount(x.Id)
              })
              .Result
          )
          .ToList();

      return View(new SampleCollectionModesModel()
      {
          SampleCollectionModes = models
      });
  }
  #endregion

  #region RefData: Sexes
  public async Task<ActionResult> Sexes()
  {
      return View(new SexesModel
      {
          Sexes = (await _sexService.List())
              .Select(x =>

              Task.Run(async () => new ReadSexModel
              {
                  Id = x.Id,
                  Description = x.Value,
                  SexCount = await _sexService.GetUsageCount(x.Id),
                  SortOrder = x.SortOrder
              }).Result)

              .ToList()
      });
  }
  #endregion

  #region RefData: Country
  public async Task<ActionResult> Country()
  {
      return View(new Models.ReferenceData.CountryModel
      {
          Countries = (await _countryService.List())
               .Select(x =>

               Task.Run(async () => new ReadCountryModel
               {
                   Id = x.Id,
                   Name = x.Value,
                   CountyOrganisationCount = await _countryService.GetUsageCount(x.Id)
               }).Result)

               .ToList()
      });
  }
  #endregion

  #region RefData: County
  public async Task<ActionResult> County()
  {
      if (await _configService.GetFlagConfigValue("site.display.counties") == true)
      {
          var countries = await _countryService.List();

          return View(
              new CountiesModel
              {
                  Counties = countries.ToDictionary(
                      x => x.Value,
                      x => x.Counties.Select(county =>
                          Task.Run(async () =>
                              new CountyModel
                              {
                                  Id = county.Id,
                                  CountryId = x.Id,
                                  Name = county.Value,
                                  CountyUsageCount = await _countyService.GetUsageCount(county.Id)
                              }
                           )
                          .Result
                      ).DefaultIfEmpty(new CountyModel() { CountryId = x.Id })
                  )
              }
          );
      }
      else
      {
          return RedirectToAction("LockedRef");
      }
  }

  #endregion

  #region RefData: Sop Status
  public async Task<ActionResult> SopStatus()
  {
      var models = (await _sopStatusService.List())
          .Select(x =>
              Task.Run(async () => new SopStatusModel()
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  SampleSetsCount = await _sopStatusService.GetUsageCount(x.Id)
              })
              .Result
          )
          .ToList();

      return View(new SopStatusesModel()
      {
          SopStatuses = models
      });
  }

  #endregion

  #region RefData: Registration Reason
  public async Task<ActionResult> RegistrationReason()
  {
      return View(new Models.ReferenceData.RegistrationReasonModel
      {
          RegistrationReasons = (await _registrationReasonService.List())
              .Select(x =>

                  Task.Run(async () => new ReadRegistrationReasonModel
                  {
                      Id = x.Id,
                      Description = x.Value,
                      OrganisationCount = await _registrationReasonService.GetUsageCount(x.Id),
                  }).Result)

              .ToList()
      });
  }

  #endregion

  #region RefData: Macroscopic Assessment
  public async Task<ActionResult> MacroscopicAssessments()
  {
      var models = (await _macroscopicAssessmentService.List())
          .Select(x =>
              Task.Run(async () => new MacroscopicAssessmentModel()
              {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder,
                  SampleSetsCount = await _macroscopicAssessmentService.GetUsageCount(x.Id)
              })
              .Result
          )
          .ToList();

      return View(new MacroscopicAssessmentsModel()
      {
          MacroscopicAssessments = models
      });
  }
  #endregion

  #region RefData: Service Offerings

  public async Task<ActionResult> ServiceOffering()
  {
      return View(new Models.ReferenceData.ServiceOfferingModel
      {
          ServiceOfferings = (await _serviceOfferingService.List())
              .Select(x =>

          Task.Run(async () => new ReadServiceOfferingModel
          {
              Id = x.Id,
              Name = x.Value,
              OrganisationCount = await _biobankReadService.GetServiceOfferingOrganisationCount(x.Id),
              SortOrder = x.SortOrder
          }).Result)

              .ToList()
      });
  }

  #endregion

  #region RefData: Extraction Procedure
  public async Task<ActionResult> ExtractionProcedure()
  {
    return View(new ExtractionProceduresModel
      {
          ExtractionProcedures = (await _ontologyTermService.List(tags: new List<string>
          {
              SnomedTags.ExtractionProcedure
          }))
          .Select(x =>

          Task.Run(async () => new ReadExtractionProcedureModel
          {
              OntologyTermId = x.Id,
              Description = x.Value,
              MaterialDetailsCount = await _materialTypeService.GetExtractionProcedureMaterialDetailsCount(x.Id),
              OtherTerms = x.OtherTerms,
              MaterialTypeIds = x.MaterialTypes.Select(x => x.Id).ToList(),
              DisplayOnDirectory = x.DisplayOnDirectory
          })
          .Result
      ).ToList(),
          MaterialTypes = await _materialTypeService.List()
      });
  }
  #endregion

}
