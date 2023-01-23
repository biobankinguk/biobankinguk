using System.Collections.Generic;
using Biobanks.Submissions.Api.Areas.Admin.Models;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Models.Submissions;
using Biobanks.Submissions.Api.Services.Directory.Constants;
using Biobanks.Submissions.Api.Utilities;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
public class ReferenceDataController : Controller
{

  private readonly IConfigService _configService;
  private readonly IReferenceDataService<AccessCondition> _accessConditionService;
  private readonly IReferenceDataService<AgeRange> _ageRangeService;
  private readonly IReferenceDataService<AnnualStatistic> _annualStatisticsService;
  private readonly IReferenceDataService<AnnualStatisticGroup> _annualStatisticGroupService;
  private readonly IReferenceDataService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeframeService;
  private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
  private readonly IReferenceDataService<AssociatedDataType> _associatedDataTypeService;
  private readonly IReferenceDataService<CollectionPercentage> _collectionPercentageService;
  private readonly IReferenceDataService<CollectionStatus> _collectionStatusService;
  private readonly IReferenceDataService<CollectionType> _collectionTypeService;
  private readonly IReferenceDataService<Country> _countryService;
  private readonly IReferenceDataService<County> _countyService;
  private readonly IReferenceDataService<ConsentRestriction> _consentRestrictionService;
  private readonly IReferenceDataService<DonorCount> _donorCountService;
  private readonly IReferenceDataService<MaterialType> _materialTypeService;
  private readonly IReferenceDataService<MaterialTypeGroup> _materialTypeGroupService;
  private readonly IReferenceDataService<MacroscopicAssessment> _macroscopicAssessmentService;
  private readonly IReferenceDataService<PreservationType> _preservationTypeService;
  private readonly IReferenceDataService<RegistrationReason> _registrationReasonService;
  private readonly IReferenceDataService<SampleCollectionMode> _sampleCollectionModeService;
  private readonly IReferenceDataService<ServiceOffering> _serviceOfferingService;
  private readonly IReferenceDataService<Sex> _sexService;
  private readonly IReferenceDataService<SopStatus> _sopStatusService;
  private readonly IReferenceDataService<StorageTemperature> _storageTemperatureService;
  private readonly IOntologyTermService _ontologyTermService;

  public ReferenceDataController(
    IConfigService configService,
    IReferenceDataService<AccessCondition> accessConditionService, 
    IReferenceDataService<AgeRange> ageRangeService,
    IReferenceDataService<AnnualStatistic> annualStatisticsService,
    IReferenceDataService<AnnualStatisticGroup> annualStatisticGroupService,
    IReferenceDataService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeframeService,
    IReferenceDataService<AssociatedDataType> associatedDataTypeService,
    IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
    IReferenceDataService<CollectionPercentage> collectionPercentageService,
    IReferenceDataService<CollectionStatus> collectionStatusService,
    IReferenceDataService<CollectionType> collectionTypeService, 
    IReferenceDataService<Country> countryService,
    IReferenceDataService<County> countyService, 
    IReferenceDataService<ConsentRestriction> consentRestrictionService,
    IReferenceDataService<DonorCount> donorCountService, 
    IReferenceDataService<MaterialType> materialTypeService,
    IReferenceDataService<MaterialTypeGroup> materialTypeGroupService,
    IReferenceDataService<MacroscopicAssessment> macroscopicAssessmentService,
    IReferenceDataService<PreservationType> preservationTypeService,
    IReferenceDataService<RegistrationReason> registrationReasonService,
    IReferenceDataService<SampleCollectionMode> sampleCollectionModeService,
    IReferenceDataService<ServiceOffering> serviceOfferingService, 
    IReferenceDataService<Sex> sexService,
    IReferenceDataService<SopStatus> sopStatusService,
    IReferenceDataService<StorageTemperature> storageTemperatureService, 
    IOntologyTermService ontologyTermService
    )
  {
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
  private string ConvertFromIsoDuration(string bound)
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

      return Json(new
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
      if (await _configService.GetFlagConfigValue("site.display.preservation.percent") == true)
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

      var groups = associatedDataTypes
          .Where(x => x.AssociatedDataTypeGroup != null)
          .GroupBy(x => x.AssociatedDataTypeGroupId)
          .Select(x => x.First())
          .Select(x => new AssociatedDataTypeGroupModel
          {
              AssociatedDataTypeGroupId = x.Id,
              Name = x.Value,
          })
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
                      )
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
      var ExtractionProcedures = (await _ontologyTermService.List(tags: new List<string>
          {
              SnomedTags.ExtractionProcedure
          }));
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
              MaterialDetailsCount = await _biobankReadService.GetExtractionProcedureMaterialDetailsCount(x.Id),
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
