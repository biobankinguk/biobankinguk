using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Areas.Biobank.Models.Capabilities;
using Biobanks.Directory.Areas.Biobank.Models.Collections;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;

namespace Biobanks.Directory.Services.Directory;

public class AbstractCrudService : IAbstractCrudService
{
  private readonly IReferenceDataCrudService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeframeService;
  private readonly IReferenceDataCrudService<AssociatedDataType> _associatedDataTypeService;
  private readonly IOntologyTermService _ontologyTermService;
  private readonly IReferenceDataCrudService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
  private readonly IReferenceDataCrudService<AccessCondition> _accessConditionService;
  private readonly IReferenceDataCrudService<CollectionType> _collectionTypeService;
  private readonly IReferenceDataCrudService<CollectionStatus> _collectionStatusService;
  private readonly IReferenceDataCrudService<ConsentRestriction> _consentRestrictionService;
  private readonly IReferenceDataCrudService<Sex> _sexService;
  private readonly IReferenceDataCrudService<AgeRange> _ageRangeService;
  private readonly IReferenceDataCrudService<DonorCount> _donorCountService;
  private readonly IMaterialTypeService _materialTypeService;
  private readonly IReferenceDataCrudService<PreservationType> _preservationTypeService;
  private readonly IReferenceDataCrudService<StorageTemperature> _storageTemperatureService;
  private readonly IReferenceDataCrudService<CollectionPercentage> _collectionPercentageService;
  private readonly IReferenceDataCrudService<MacroscopicAssessment> _macroscopicAssessmentService;

  public AbstractCrudService(
    IReferenceDataCrudService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeframeService,
    IReferenceDataCrudService<AssociatedDataType> associatedDataTypeService,
    IOntologyTermService ontologyTermService,
    IReferenceDataCrudService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
    IReferenceDataCrudService<AccessCondition> accessConditionService,
    IReferenceDataCrudService<CollectionType> collectionTypeService,
    IReferenceDataCrudService<CollectionStatus> collectionStatusService,
    IReferenceDataCrudService<ConsentRestriction> consentRestrictionService,
    IReferenceDataCrudService<Sex> sexService,
    IReferenceDataCrudService<AgeRange> ageRangeService,
    IReferenceDataCrudService<DonorCount> donorCountService,
    IMaterialTypeService materialTypeService,
    IReferenceDataCrudService<PreservationType> preservationTypeService,
    IReferenceDataCrudService<StorageTemperature> storageTemperatureService,
    IReferenceDataCrudService<CollectionPercentage> collectionPercentageService,
    IReferenceDataCrudService<MacroscopicAssessment> macroscopicAssessmentService

    )
  {
    _associatedDataProcurementTimeframeService = associatedDataProcurementTimeframeService;
    _associatedDataTypeService = associatedDataTypeService;
    _ontologyTermService = ontologyTermService;
    _associatedDataTypeGroupService = associatedDataTypeGroupService;
    _accessConditionService = accessConditionService;
    _collectionTypeService = collectionTypeService;
    _collectionStatusService = collectionStatusService;
    _consentRestrictionService = consentRestrictionService;
    _sexService = sexService;
    _ageRangeService = ageRangeService;
    _donorCountService = donorCountService;
    _materialTypeService = materialTypeService;
    _preservationTypeService = preservationTypeService;
    _storageTemperatureService = storageTemperatureService;
    _collectionPercentageService = collectionPercentageService;
    _macroscopicAssessmentService = macroscopicAssessmentService;
  }

  public async Task<AbstractCRUDCapabilityModel> PopulateAbstractCRUDAssociatedData(AbstractCRUDCapabilityModel model, bool excludeLinkedData = false)
  {
    var timeFrames = (await _associatedDataProcurementTimeframeService.List())
                    .Select(x => new AssociatedDataTimeFrameModel
                    {
                      ProvisionTimeId = x.Id,
                      ProvisionTimeDescription = x.Value,
                      ProvisionTimeValue = x.DisplayValue
                    });
    var typeList = await _associatedDataTypeService.List();
    if (excludeLinkedData)
    {
      typeList = typeList.Where(x => x.OntologyTerms == null || x.OntologyTerms.Count == 0).ToList();
    }

    else
    {
      var ontologyTerm = await _ontologyTermService.Get(value: model.Diagnosis);
      if (ontologyTerm != null)
      {
        typeList = typeList.Where(x => x.OntologyTerms == null || x.OntologyTerms.Count == 0 || (x.OntologyTerms.Find(y => y.Id == ontologyTerm.Id) != null)).ToList();
      }
    }

    var types = typeList
             .Select(x => new AssociatedDataModel
             {
               DataTypeId = x.Id,
               DataTypeDescription = x.Value,
               DataGroupId = x.AssociatedDataTypeGroupId,
               Message = x.Message,
               TimeFrames = timeFrames,
               isLinked = (x.OntologyTerms != null && x.OntologyTerms.Count > 0)
             });

    model.Groups = new List<AssociatedDataGroupModel>();
    var groups = await _associatedDataTypeGroupService.List();
    foreach (var g in groups)
    {
      var groupModel = new AssociatedDataGroupModel();
      groupModel.GroupId = g.Id;
      groupModel.Name = g.Value;
      groupModel.Types = types.Where(y => y.DataGroupId == g.Id).ToList();
      model.Groups.Add(groupModel);
    }

    //Check if types are valid
    foreach (var type in types)
    {
      type.Active = model.AssociatedDataModelsValid();
    }

    return model;
  }

  public async Task<AbstractCRUDCollectionModel> PopulateAbstractCRUDCollectionModel(AbstractCRUDCollectionModel model, IEnumerable<ConsentRestriction> consentRestrictions, bool excludeLinkedData)
  {


    model.AccessConditions = (await _accessConditionService.List())
        .Select(x => new ReferenceDataModel
        {
          Id = x.Id,
          Description = x.Value,
          SortOrder = x.SortOrder
        })
        .OrderBy(x => x.SortOrder);

    model.CollectionTypes = (await _collectionTypeService.List())
        .Select(x => new ReferenceDataModel
        {
          Id = x.Id,
          Description = x.Value,
          SortOrder = x.SortOrder
        })
        .OrderBy(x => x.SortOrder);

    model.CollectionStatuses = (await _collectionStatusService.List())
        .Select(x => new ReferenceDataModel
        {
          Id = x.Id,
          Description = x.Value,
          SortOrder = x.SortOrder
        })
        .OrderBy(x => x.SortOrder);

    model.ConsentRestrictions = (await _consentRestrictionService.List())
        .OrderBy(x => x.SortOrder)
        .Select(x => new Areas.Biobank.Models.Collections.ConsentRestrictionModel
        {
          ConsentRestrictionId = x.Id,
          Description = x.Value,
          Active = consentRestrictions != null && consentRestrictions.Any(y => y.Id == x.Id)
        });

    //if not null keeps previous groups values
    if (model.Groups == null)
    {
      var groups = await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel(), excludeLinkedData);
      model.Groups = groups.Groups;
    }


    return model;

  }
  public async Task<AbstractCRUDSampleSetModel> PopulateAbstractCRUDSampleSetModel(AbstractCRUDSampleSetModel model)
  {

    model.Sexes = (await _sexService.List())
        .Select(
            x => new ReferenceDataModel
            {
              Id = x.Id,
              Description = x.Value,
              SortOrder = x.SortOrder
            })
        .OrderBy(x => x.SortOrder);

    model.AgeRanges = (await _ageRangeService.List())
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    model.DonorCounts = (await _donorCountService.List())
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    model.MaterialTypes = (await _materialTypeService.List())
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    model.ExtractionProcedures = (await _ontologyTermService.List(tags: new List<string>
                {
                    SnomedTags.ExtractionProcedure
                }, onlyDisplayable: true))
        .Select(
            x =>
                new OntologyTermModel
                {
                  OntologyTermId = x.Id,
                  Description = x.Value,
                })
        .OrderBy(x => x.Description);

    model.PreservationTypes = (await _preservationTypeService.List())
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    model.StorageTemperatures = (await _storageTemperatureService.List())
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    model.Percentages = (await _collectionPercentageService.List())
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    var assessments = await _macroscopicAssessmentService.List();

    model.MacroscopicAssessments = assessments
        .Select(
            x =>
                new ReferenceDataModel
                {
                  Id = x.Id,
                  Description = x.Value,
                  SortOrder = x.SortOrder
                })
        .OrderBy(x => x.SortOrder);

    model.ShowMacroscopicAssessment = (assessments.Count() > 1);

    if (model.DonorCountId > 0)
    {
      var donorCountList = model.DonorCounts.ToList();
      model.DonorCountSliderPosition = donorCountList.IndexOf(donorCountList.First(x => x.Id == model.DonorCountId));
    }

    return model;
  }
}
