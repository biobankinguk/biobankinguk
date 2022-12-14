using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Biobank.Models;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Models.Biobank;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;
public class CollectionsController : Controller
{
  [HttpGet]
  [Authorize( CustomClaimType.Biobank)]
  public async Task<ActionResult> Collections(int biobankId)
  {
    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

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
        MaterialTypes = string.Join(", ", _biobankReadService.ExtractDistinctMaterialTypes(x).Select(y => y)),
        NumberOfSampleSets = x.SampleSets.Count
      })
    };

    return View(model);
  }

  [HttpGet]
  public async Task<ViewResult> AddCollection()
  {
    return View((AddCollectionModel)(await PopulateAbstractCRUDCollectionModel(model: (new AddCollectionModel { FromApi = false }), excludeLinkedData: true)));
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
  public async Task<ActionResult> AddCollection(AddCollectionModel model, int biobankId)
  {

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

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
        id = collection.CollectionId
      });
    }
    else
    {
      //Populate Groups
      model.Groups = null;
      await PopulateAbstractCRUDCollectionModel(model);
    }

    return View((AddCollectionModel)(await PopulateAbstractCRUDCollectionModel(model)));
  }

  [HttpGet]
  // TODO: [AuthoriseToAdministerCollection]
  public async Task<ActionResult> CopyCollection(int id, int biobankId)
  {

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");


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
      await _biobankWriteService.AddSampleSetAsync(newSampleSet);
    }


    this.SetTemporaryFeedbackMessage("This is your copied collection. It has been saved and you are now free to edit it.", FeedbackMessageType.Success);

    return RedirectToAction("Collection", new
    {
      id = newCollection.CollectionId
    });
  }

  [HttpGet]
  //TODO: [AuthoriseToAdministerCollection]
  public async Task<ViewResult> EditCollection(int id)
  {
    var collection = await _collectionService.Get(id);
    var consentRestrictions = await _consentRestrictionService.List();

    var groups = await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());

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

    var assdatlist = model.ListAssociatedDataModels();

    foreach (var associatedDataModel in assdatlist)
    {
      var associatedData = collection.AssociatedData.FirstOrDefault(x => x.AssociatedDataTypeId == associatedDataModel.DataTypeId);

      if (associatedData != null)
      {
        associatedDataModel.Active = true;
        associatedDataModel.ProvisionTimeId = associatedData.AssociatedDataProcurementTimeframeId;
      }
    }

    return View((EditCollectionModel)(await PopulateAbstractCRUDCollectionModel(model, collection.ConsentRestrictions)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  //TODO:[AuthoriseToAdministerCollection]
  public async Task<ActionResult> EditCollection(EditCollectionModel model)
  {
    var biobankId = SessionHelper.GetBiobankId(Session);

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

    //Retrieve collection
    var collection = await _collectionService.Get(model.Id);

    if (collection.FromApi)
    {
      // Update description
      collection.Description = model.Description;

      await _collectionService.Update(collection);

      this.SetTemporaryFeedbackMessage("Collection updated!", FeedbackMessageType.Success);

      return RedirectToAction("Collection", new { id = model.Id });
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

      return RedirectToAction("Collection", new { id = model.Id });
    }
    else
    {
      //Populate Groups
      model.Groups = null;
      await PopulateAbstractCRUDCollectionModel(model);
    }

    return View((EditCollectionModel)(await PopulateAbstractCRUDCollectionModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  //TODO:[AuthoriseToAdministerCollection]
  public async Task<RedirectToRouteResult> DeleteCollection(int id)
  {
    if (!await _collectionService.IsFromApi(id) && await _collectionService.Delete(id))
    {
      SetTemporaryFeedbackMessage("Collection deleted!", FeedbackMessageType.Success);
      return RedirectToAction("Collections");
    }
    else
    {
      SetTemporaryFeedbackMessage(
          "The system was unable to delete this collection. Please make sure it doesn't contain any Sample Sets before trying again.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Collection", new { id });
    }
  }

  [HttpGet]
  //TODO: [AuthoriseToAdministerCollection]
  public async Task<ViewResult> Collection(int id)
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
  //TODO: [AuthoriseToAdministerCollection]
  public async Task<ViewResult> AddSampleSet(int id)
  {
    ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(id);
    var model = new AddSampleSetModel
    {
      CollectionId = id
    };
    return View((AddSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  //TODO: [AuthoriseToAdministerCollection]
  public async Task<ActionResult> AddSampleSet(int id, AddSampleSetModel model)
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
      await _biobankWriteService.AddSampleSetAsync(sampleSet);

      this.SetTemporaryFeedbackMessage("Sample Set added!", FeedbackMessageType.Success);

      return RedirectToAction("Collection", new { id });
    }

    return View((AddSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpGet]
  //TODO: [AuthoriseToAdministerSampleSet]
  public async Task<ActionResult> CopySampleSet(int id)
  {
    var sampleSet = await _biobankReadService.GetSampleSetByIdAsync(id);
    ViewData["CollectionApiStatus"] = await _collectionService.IsFromApi(sampleSet.CollectionId);
    SiteMaps.Current.CurrentNode.ParentNode.RouteValues["id"] = sampleSet.CollectionId;

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

    return View((CopySampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  //TODO: [AuthoriseToAdministerSampleSet]
  public async Task<ActionResult> CopySampleSet(int id, CopySampleSetModel model)
  {
    if (await _collectionService.IsFromApi(model.CollectionId))
    {
      return RedirectToAction("SampleSet", new { id = model.OriginalId });
    }
    var addModel = _mapper.Map<AddSampleSetModel>(model);
    return await AddSampleSet(model.CollectionId, addModel);
  }

  [HttpGet]
  //TODO: [AuthoriseToAdministerSampleSet]
  public async Task<ViewResult> EditSampleSet(int id)
  {
    var sampleSet = await _biobankReadService.GetSampleSetByIdAsync(id);

    SiteMaps.Current.CurrentNode.ParentNode.ParentNode.RouteValues["id"] = sampleSet.CollectionId;

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

    return View((EditSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [AuthoriseToAdministerSampleSet]
  public async Task<ActionResult> EditSampleSet(int id, EditSampleSetModel model)
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
      await _biobankWriteService.UpdateSampleSetAsync(sampleSet);

      SetTemporaryFeedbackMessage("Sample Set updated!", FeedbackMessageType.Success);

      return RedirectToAction("SampleSet", new { id = model.Id });
    }

    SiteMaps.Current.CurrentNode.ParentNode.ParentNode.RouteValues["id"] = model.CollectionId;

    return View((EditSampleSetModel)(await PopulateAbstractCRUDSampleSetModel(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [AuthoriseToAdministerSampleSet]
  public async Task<RedirectToRouteResult> DeleteSampleSet(int id, int collectionId)
  {
    if (!await _collectionService.IsFromApi(collectionId))
    {
      await _biobankWriteService.DeleteSampleSetAsync(id);
      SetTemporaryFeedbackMessage("Sample Set deleted!", FeedbackMessageType.Success);
    }
    return RedirectToAction("Collection", new { id = collectionId });
  }

  [HttpGet]
  //TPDP: [AuthoriseToAdministerSampleSet]
  public async Task<ViewResult> SampleSet(int id)
  {
    var sampleSet = await _biobankReadService.GetSampleSetByIdAsync(id);
    var assessments = await _macroscopicAssessmentService.List();

    SiteMaps.Current.CurrentNode.ParentNode.RouteValues["id"] = sampleSet.CollectionId;

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
