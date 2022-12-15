using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Biobank.Models;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory;

public class AbstractCrudService : IAbstractCrudService
{
  private readonly IReferenceDataService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeframeService;
  private readonly IReferenceDataService<AssociatedDataType> _associatedDataTypeService;
  private readonly IOntologyTermService _ontologyTermService;
  private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
  private readonly IReferenceDataService<AccessCondition> _accessConditionService;
  private readonly IReferenceDataService<CollectionType> _collectionTypeService;
  private readonly IReferenceDataService<CollectionStatus> _collectionStatusService;
  private readonly IReferenceDataService<ConsentRestriction> _consentRestrictionService;

  public AbstractCrudService(
    IReferenceDataService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeframeService,
    IReferenceDataService<AssociatedDataType> assocaitedDataTypeService,
    IOntologyTermService ontologyTermService,
    IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
    IReferenceDataService<AccessCondition> accessConditionService,
    IReferenceDataService<CollectionType> collectionTypeService,
    IReferenceDataService<CollectionStatus> collectionStatusService,
    IReferenceDataService<ConsentRestriction> consentRestrictionService
    )
  {
    _associatedDataProcurementTimeframeService = associatedDataProcurementTimeframeService;
    _associatedDataTypeService = assocaitedDataTypeService;
    _ontologyTermService = ontologyTermService;
    _associatedDataTypeGroupService = associatedDataTypeGroupService;
    _accessConditionService = accessConditionService;
    _collectionTypeService = collectionTypeService;
    _collectionStatusService = collectionStatusService;
    _consentRestrictionService = consentRestrictionService;
  }
  public async Task<AbstractCRUDCapabilityModel> PopulateAbstractCRUDAssociatedData(
   AbstractCRUDCapabilityModel model, bool excludeLinkedData = false)
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

  private async Task<AbstractCRUDCollectionModel> PopulateAbstractCRUDCollectionModel(
         AbstractCRUDCollectionModel model,
         IEnumerable<ConsentRestriction> consentRestrictions = null, bool excludeLinkedData = false)
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
        .Select(x => new Areas.Biobank.Models.ConsentRestrictionModel
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
}
