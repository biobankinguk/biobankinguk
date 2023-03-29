using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Collections;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Capabilities;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Web.Navigation;
using Biobanks.Submissions.Api.Auth;
using AssociatedDataSummaryModel = Biobanks.Submissions.Api.Models.Biobank.AssociatedDataSummaryModel;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;


[Area("Biobank")]
[Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
[SuspendedWarning]
public class CollectionsController : Controller
{
  private readonly ICollectionService _collectionService;
  private readonly ISampleSetService _sampleSetService;
  private readonly IReferenceDataCrudService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeframeService;
  private readonly IOntologyTermService _ontologyTermService;
  private readonly IReferenceDataCrudService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
  private readonly IReferenceDataCrudService<AssociatedDataType> _associatedDataTypeService;
  private readonly IReferenceDataCrudService<ConsentRestriction> _consentRestrictionService;
  private readonly IMapper _mapper;
  private readonly IReferenceDataCrudService<MacroscopicAssessment> _macroscopicAssessmentService;
  private readonly IAbstractCrudService _abstractCrudService;
  private readonly IMaterialTypeService _materialTypeService;

  public CollectionsController(
      ICollectionService collectionService,
      ISampleSetService sampleSetService,
      IReferenceDataCrudService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeframeService,
      IOntologyTermService ontologyTermService,
      IReferenceDataCrudService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
      IReferenceDataCrudService<AssociatedDataType> associatedDataTypeService,
      IReferenceDataCrudService<ConsentRestriction> consentRestrictionService,
      IMapper mapper,
      IReferenceDataCrudService<MacroscopicAssessment> macroscopicAssessmentService,
      IAbstractCrudService abstractCrudService,
      IMaterialTypeService materialTypeService
  )
  {
    _collectionService = collectionService;
    _sampleSetService = sampleSetService;
    _associatedDataProcurementTimeframeService = associatedDataProcurementTimeframeService;
    _ontologyTermService = ontologyTermService;
    _associatedDataTypeGroupService = associatedDataTypeGroupService;
    _associatedDataTypeService = associatedDataTypeService;
    _consentRestrictionService = consentRestrictionService;
    _mapper = mapper;
    _macroscopicAssessmentService = macroscopicAssessmentService;
    _abstractCrudService = abstractCrudService;
    _materialTypeService = materialTypeService;
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
  public async Task<ActionResult> Index(int biobankId)
  {
    var collections = await _collectionService.List(biobankId);

    // Build ViewModel.
    var model = new BiobankCollectionsModel
    {
      BiobankCollectionModels = collections.Select(x => new BiobankCollectionModel
      {
        Id = x.CollectionId,
        OntologyTerm = x.OntologyTerm.Value,
        Title = x.Title,
        StartYear = x.StartDate.Year,
        MaterialTypes = string.Join(", ", _materialTypeService.ExtractDistinctMaterialTypes(x).Select(y => y)),
        NumberOfSampleSets = x.SampleSets.Count
      })
    };
    return View(model);
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
  public async Task<ViewResult> AddCollection(int biobankId)
  {
    return View((AddCollectionModel)(await _abstractCrudService.PopulateAbstractCRUDCollectionModel(model: (new AddCollectionModel { FromApi = false }), excludeLinkedData: true)));
  }

  [HttpGet]
  public async Task<ActionResult> GetAssociatedDataTypeViewsAjax(string id)
  {
    AddCollectionModel model = new AddCollectionModel { FromApi = false };

    var timeFrames = (await _associatedDataProcurementTimeframeService.List())
       .Select(x => new AssociatedDataTimeFrameModel
       {
         ProvisionTimeId = x.Id,
         ProvisionTimeDescription = x.Value,
         ProvisionTimeValue = x.DisplayValue
       });

    var types = (await _ontologyTermService.ListAssociatedDataTypesByOntologyTerm(id))
             .Select(x => new AssociatedDataModel
             {
               DataTypeId = x.Id,
               DataTypeDescription = x.Value,
               DataGroupId = x.AssociatedDataTypeGroupId,
               Message = x.Message,
               TimeFrames = timeFrames,
               isLinked = true
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

    return PartialView("_LinkedAssociatedData", model);

  }

  private async Task<bool> IsLinkedAssociatedDataValid(
      List<AssociatedDataModel> linkedData, string ontologyTermId)
  {
    var associatedDataList = await _associatedDataTypeService.List();
    var newAssociatedData = associatedDataList.Where(x => linkedData.Find(y => y.DataTypeId == x.Id) != null);
    // first check that all the data is present in the data list
    if (newAssociatedData.ToList().Count() != linkedData.Count())
    {
      return false;
    }
    // then check that all the linked data is linked to the ontologyTerm
    foreach (var type in newAssociatedData)
    {
      // only check linked data
      if ((type.OntologyTerms != null && type.OntologyTerms.Count() > 0) && (type.OntologyTerms.Find(x => x.Id == ontologyTermId) == null))
      {
        return false;
      }
    }

    return true;
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
  public async Task<ActionResult> AddCollection(int biobankId, AddCollectionModel model)
  {
    // check linked types are valid
    List<AssociatedDataModel> associatedDataModels = model.ListAssociatedDataModels().ToList();
    // check that any linked associated data is related to the ontology term
    bool linkedIsValid = await IsLinkedAssociatedDataValid(associatedDataModels, (await _ontologyTermService.Get(value: model.Diagnosis)).Id);

    if (await model.IsValid(ModelState, _ontologyTermService) && linkedIsValid)
    {
      var associatedData = associatedDataModels
          .Where(x => x.Active)
          .Select(y => new CollectionAssociatedData
          {
            AssociatedDataTypeId = y.DataTypeId,
            AssociatedDataProcurementTimeframeId = y.ProvisionTimeId // GroupID
          })
          .ToList();

      var consentRestrictions = model.ConsentRestrictions
          .Where(x => x.Active)
          .Select(x => new ConsentRestriction
          {
            Id = x.ConsentRestrictionId
          })
          .ToList();

      var ontologyTerm = await _ontologyTermService.Get(value: model.Diagnosis);

      // Create and Add New Collection
      var collection = await _collectionService.Add(new Collection
      {
        OrganisationId = biobankId,
        Title = model.Title,
        Description = model.Description,
        StartDate = new DateTime(year: model.StartDate.Value, month: 1, day: 1),
        AssociatedData = associatedData,
        AccessConditionId = model.AccessCondition,
        CollectionTypeId = model.CollectionType,
        CollectionStatusId = model.CollectionStatus,
        ConsentRestrictions = consentRestrictions,
        OntologyTermId = ontologyTerm.Id,
        FromApi = model.FromApi,
        Notes = model.Notes
      });

      this.SetTemporaryFeedbackMessage("Collection added!", FeedbackMessageType.Success);

      return RedirectToAction("Collection", new
      {
        biobankId = biobankId,
        id = collection.CollectionId
      });
    }
    else
    {
      //Populate Groups
      model.Groups = null;
      await _abstractCrudService.PopulateAbstractCRUDCollectionModel(model);
    }

    return View((AddCollectionModel)(await _abstractCrudService.PopulateAbstractCRUDCollectionModel(model)));
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ActionResult> CopyCollection(int biobankId, int id)
  {
    // Copy and Add New Collection  
    var newCollection = await _collectionService.Copy(id, biobankId);

    var originalCollection = await _collectionService.Get(id);

    // Copy Sample Set 
    foreach (SampleSet sampleSet in originalCollection.SampleSets)
    {
      var newSampleSet = new SampleSet
      {
        CollectionId = newCollection.CollectionId,
        SexId = sampleSet.SexId,
        AgeRangeId = sampleSet.AgeRangeId,
        DonorCountId = sampleSet.DonorCountId,
        MaterialDetails = sampleSet.MaterialDetails.Select(x =>
           new MaterialDetail
           {
             MaterialTypeId = x.MaterialTypeId,
             PreservationTypeId = x.PreservationTypeId,
             StorageTemperatureId = x.StorageTemperatureId,
             CollectionPercentageId = x.CollectionPercentageId,
             MacroscopicAssessmentId = x.MacroscopicAssessmentId,
             ExtractionProcedureId = x.ExtractionProcedureId
           }
            )
            .ToList()
      };

      // Add New SampleSet
      await _sampleSetService.AddSampleSetAsync(newSampleSet);
    }


    this.SetTemporaryFeedbackMessage("This is your copied collection. It has been saved and you are now free to edit it.", FeedbackMessageType.Success);

    return RedirectToAction("Collection", new
    {
      biobankId = biobankId,
      id = newCollection.CollectionId
    });
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ViewResult> EditCollection(int biobankId, int id)
  {
    var collection = await _collectionService.Get(id);
    var groups = await _abstractCrudService.PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());

    var model = new EditCollectionModel
    {
      Id = collection.CollectionId,
      Diagnosis = collection.OntologyTerm.Value,
      Title = collection.Title,
      Description = collection.Description,
      StartDate = collection.StartDate.Year,
      AccessCondition = collection.AccessCondition.Id,
      FromApi = collection.FromApi,
      CollectionType = collection.CollectionType?.Id,
      CollectionStatus = collection.CollectionStatus.Id,
      Groups = groups.Groups

    };

    var associatedDataModels = model.ListAssociatedDataModels();

    foreach (var associatedDataModel in associatedDataModels)
    {
      var associatedData = collection.AssociatedData.FirstOrDefault(x => x.AssociatedDataTypeId == associatedDataModel.DataTypeId);

      if (associatedData != null)
      {
        associatedDataModel.Active = true;
        associatedDataModel.ProvisionTimeId = associatedData.AssociatedDataProcurementTimeframeId;
      }
    }

    return View((EditCollectionModel)(await _abstractCrudService.PopulateAbstractCRUDCollectionModel(model, collection.ConsentRestrictions)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ActionResult> EditCollection(int biobankId, EditCollectionModel model)
  {
    //Retrieve collection
    var collection = await _collectionService.Get(model.Id);

    if (collection.FromApi)
    {
      // Update description
      collection.Description = model.Description;

      await _collectionService.Update(collection);

      this.SetTemporaryFeedbackMessage("Collection updated!", FeedbackMessageType.Success);

      return RedirectToAction("Collection", new { biobankId = biobankId, id = model.Id });
    }
    // check linked types are valid
    List<AssociatedDataModel> associatedDataModels = model.ListAssociatedDataModels().ToList();
    // check that any linked associated data is related to the ontology term
    bool linkedIsValid = await IsLinkedAssociatedDataValid(associatedDataModels, (await _ontologyTermService.Get(value: model.Diagnosis)).Id);

    if (await model.IsValid(ModelState, _ontologyTermService) && model.FromApi == false && linkedIsValid)
    {
      var associatedData = associatedDataModels
          .Where(x => x.Active)
          .Select(y => new CollectionAssociatedData
          {
            CollectionId = model.Id,
            AssociatedDataTypeId = y.DataTypeId,
            AssociatedDataProcurementTimeframeId = y.ProvisionTimeId
          }).ToList();

      var consentRestrictions = model.ConsentRestrictions
          .Where(x => x.Active)
          .Select(x => new ConsentRestriction
          {
            Id = x.ConsentRestrictionId
          })
          .ToList();

      var ontologyTerm = await _ontologyTermService.Get(value: model.Diagnosis);

      await _collectionService.Update(new Collection
      {
        AccessConditionId = model.AccessCondition,
        AssociatedData = associatedData,
        CollectionId = model.Id,
        CollectionStatusId = model.CollectionStatus,
        CollectionTypeId = model.CollectionType,
        ConsentRestrictions = consentRestrictions,
        Description = model.Description,
        FromApi = model.FromApi,
        OntologyTermId = ontologyTerm.Id,
        OrganisationId = biobankId,
        StartDate = new DateTime(year: model.StartDate.Value, month: 1, day: 1),
        Title = model.Title

      });

      this.SetTemporaryFeedbackMessage("Collection updated!", FeedbackMessageType.Success);

      return RedirectToAction("Collection", new { biobankId = biobankId, id = model.Id });
    }
    else
    {
      //Populate Groups
      model.Groups = null;
      await _abstractCrudService.PopulateAbstractCRUDCollectionModel(model);
    }

    return View((EditCollectionModel)(await _abstractCrudService.PopulateAbstractCRUDCollectionModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ActionResult> DeleteCollection(int biobankId, int id)
  {
    if (!await _collectionService.IsFromApi(id) && await _collectionService.Delete(id))
    {
      this.SetTemporaryFeedbackMessage("Collection deleted!", FeedbackMessageType.Success);
      return RedirectToAction("Index", "Collections", new { biobankId = biobankId });
    }
    else
    {
      this.SetTemporaryFeedbackMessage(
          "The system was unable to delete this collection. Please make sure it doesn't contain any Sample Sets before trying again.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Collection", new { biobankId = biobankId, id });
    }
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ViewResult> Collection(int biobankId, int id)
  {
    var collection = await _collectionService.GetWithSampleSets(id);

    var model = new CollectionModel
    {
      Id = collection.CollectionId,
      Title = collection.Title,
      Description = collection.Description,
      OntologyTerm = collection.OntologyTerm.Value,
      StartDate = collection.StartDate,
      AccessCondition = collection.AccessCondition.Value,
      CollectionType = collection.CollectionType?.Value,
      FromApi = collection.FromApi,
      Notes = collection.Notes,
      AssociatedData = collection.AssociatedData.Select(x => new AssociatedDataSummaryModel
      {
        Description = x.AssociatedDataType.Value,
        ProvisionTime = x.AssociatedDataProcurementTimeframe.Value,
        ProvisionTimeSortValue = x.AssociatedDataProcurementTimeframe.SortOrder
      }),
      SampleSets = collection.SampleSets.Select(sampleSet => new CollectionSampleSetSummaryModel
      {
        Id = sampleSet.Id,
        Sex = sampleSet.Sex.Value,
        Age = sampleSet.AgeRange.Value,
        MaterialTypes = string.Join(" / ", sampleSet.MaterialDetails.Select(x => x.MaterialType.Value).Distinct()),
        PreservationTypes = string.Join(" / ", sampleSet.MaterialDetails.Select(x => x.PreservationType?.Value).Distinct()),
        StorageTemperatures = string.Join(" / ", sampleSet.MaterialDetails.Select(x => x.StorageTemperature.Value).Distinct()),
        ExtractionProcedures = string.Join(" / ", sampleSet.MaterialDetails.Where(x => x.ExtractionProcedure?.DisplayOnDirectory == true)
                                 .Select(x => x.ExtractionProcedure?.Value).Distinct())
      })
    };

    return View(model);
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ViewResult> AddSampleSet(int biobankId, int id)
  {
    ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(id);
    var model = new AddSampleSetModel
    {
      CollectionId = id
    };
    return View((AddSampleSetModel)(await _abstractCrudService.PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.CanAdministerCollection))]
  public async Task<ActionResult> AddSampleSet(int biobankId, int id, AddSampleSetModel model)
  {
    var apiCheck = await _collectionService.IsFromApi(id);

    ViewData["CollectionApiStatus"] = apiCheck;

    if (model.IsValid(ModelState) && apiCheck == false)
    {
      var sampleSet = new SampleSet
      {
        CollectionId = id,
        SexId = model.Sex,
        AgeRangeId = model.AgeRange,
        DonorCountId = model.DonorCountId,
        MaterialDetails = model.MaterialPreservationDetails.Select(x =>
            new MaterialDetail
            {
              MaterialTypeId = x.materialType,
              PreservationTypeId = x.preservationType,
              StorageTemperatureId = x.storageTemperature,
              CollectionPercentageId = x.percentage,
              MacroscopicAssessmentId = x.macroscopicAssessment,
              ExtractionProcedureId = x.extractionProcedure
            }
          )
          .ToList()
      };

      // Add New SampleSet
      await _sampleSetService.AddSampleSetAsync(sampleSet);

      this.SetTemporaryFeedbackMessage("Sample Set added!", FeedbackMessageType.Success);

      return RedirectToAction("Collection", new { biobankId = biobankId, id });
    }

    return View((AddSampleSetModel)(await _abstractCrudService.PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerSampleSet))]
  public async Task<ActionResult> CopySampleSet(int biobankId, int id)
  {
    var sampleSet = await _sampleSetService.GetSampleSetByIdAsync(id);
    ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);

    //Build the model using all details of the existing sampleset, except id, which is stored in a separate property
    var model = new CopySampleSetModel
    {
      OriginalId = id,
      CollectionId = sampleSet.CollectionId,
      Sex = sampleSet.SexId,
      AgeRange = sampleSet.AgeRangeId,
      DonorCountId = sampleSet.DonorCountId,

      MaterialPreservationDetailsJson = JsonConvert.SerializeObject(sampleSet.MaterialDetails.Select(x => new MaterialDetailModel
      {
        materialType = x.MaterialTypeId,
        preservationType = x.PreservationTypeId,
        storageTemperature = x.StorageTemperatureId,
        percentage = x.CollectionPercentageId,
        macroscopicAssessment = x.MacroscopicAssessmentId,
        extractionProcedure = x.ExtractionProcedureId
      }))
    };

    return View((CopySampleSetModel)(await _abstractCrudService.PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [Authorize(nameof(AuthPolicies.CanAdministerSampleSet))]
  public async Task<ActionResult> CopySampleSet(int biobankId, int id, CopySampleSetModel model)
  {
    if (await _collectionService.IsFromApi(model.CollectionId))
    {
      return RedirectToAction("SampleSet", new { biobankId = biobankId, id = model.OriginalId });
    }
    var addModel = _mapper.Map<AddSampleSetModel>(model);
    return await AddSampleSet(biobankId, model.CollectionId, addModel);
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerSampleSet))]
  public async Task<ViewResult> EditSampleSet(int biobankId, int id)
  {
    var sampleSet = await _sampleSetService.GetSampleSetByIdAsync(id);
    
    ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);

    var model = new EditSampleSetModel
    {
      Id = sampleSet.Id,
      CollectionId = sampleSet.CollectionId,
      Sex = sampleSet.SexId,
      AgeRange = sampleSet.AgeRangeId,
      DonorCountId = sampleSet.DonorCountId,

      MaterialPreservationDetailsJson = JsonConvert.SerializeObject(sampleSet.MaterialDetails.Select(x => new MaterialDetailModel
      {
        id = x.Id,
        materialType = x.MaterialTypeId,
        preservationType = x.PreservationTypeId,
        storageTemperature = x.StorageTemperatureId,
        percentage = x.CollectionPercentageId,
        macroscopicAssessment = x.MacroscopicAssessmentId,
        extractionProcedure = x.ExtractionProcedureId
      }))
    };

    return View((EditSampleSetModel)(await _abstractCrudService.PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.CanAdministerSampleSet))]
  public async Task<ActionResult> EditSampleSet(int biobankId, int id, EditSampleSetModel model)
  {
    var apiCheck = await _collectionService.IsFromApi(model.CollectionId);
    ViewData["CollectionApiStatus"] = apiCheck;

    if (model.IsValid(ModelState) && !apiCheck)
    {
      var sampleSet = new SampleSet
      {
        Id = id,
        SexId = model.Sex,
        AgeRangeId = model.AgeRange,
        DonorCountId = model.DonorCountId,
        MaterialDetails = model.MaterialPreservationDetails.Select(x =>
            new MaterialDetail
            {
              Id = x.id ?? 0,
              MaterialTypeId = x.materialType,
              PreservationTypeId = x.preservationType,
              StorageTemperatureId = x.storageTemperature,
              CollectionPercentageId = x.percentage,
              MacroscopicAssessmentId = x.macroscopicAssessment,
              ExtractionProcedureId = x.extractionProcedure
            }
          )
          .ToList()
      };

      // Update SampleSet
      await _sampleSetService.UpdateSampleSetAsync(sampleSet);

      this.SetTemporaryFeedbackMessage("Sample Set updated!", FeedbackMessageType.Success);

      return RedirectToAction("SampleSet", new { biobankId = biobankId, id = model.Id });
    }

    return View((EditSampleSetModel)(await _abstractCrudService.PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.CanAdministerSampleSet))]
  public async Task<ActionResult> DeleteSampleSet(int biobankId, int id, int collectionId)
  {
    if (!await _collectionService.IsFromApi(collectionId))
    {
      await _sampleSetService.DeleteSampleSetAsync(id);
      this.SetTemporaryFeedbackMessage("Sample Set deleted!", FeedbackMessageType.Success);
    }
    return RedirectToAction("Collection", new { biobankId = biobankId, id = collectionId });
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerSampleSet))]
  public async Task<ViewResult> SampleSet(int biobankId, int id)
  {
    var sampleSet = await _sampleSetService.GetSampleSetByIdAsync(id);
    var assessments = await _macroscopicAssessmentService.List();
    
    ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);

    var model = new SampleSetModel
    {
      Id = sampleSet.Id,
      CollectionId = sampleSet.CollectionId,
      Sex = sampleSet.Sex.Value,
      AgeRange = sampleSet.AgeRange.Value,
      DonorCount = sampleSet.DonorCount.Value,
      MaterialPreservationDetails = sampleSet.MaterialDetails.Select(x => new MaterialPreservationDetailModel
      {
        CollectionPercentage = x.CollectionPercentage?.Value,
        MacroscopicAssessment = x.MacroscopicAssessment.Value,
        MaterialType = x.MaterialType.Value,
        PreservationType = x.PreservationType?.Value,
        StorageTemperature = x.StorageTemperature.Value,
        ExtractionProcedure = x.ExtractionProcedure?.DisplayOnDirectory == true ? x.ExtractionProcedure.Value : null

      }),
      ShowMacroscopicAssessment = (assessments.Count() > 1)
    };

    return View(model);
  }

}
