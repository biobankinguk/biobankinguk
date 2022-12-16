using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Capabilities;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;

[Area("Biobank")]
public class CapabilitiesController : Controller
{
  [HttpGet]
  public async Task<ActionResult> Capabilities(int biobankId)
  {

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

    // Call service to get capabilities for logged in BioBank.
    var capabilities = (await _biobankReadService.ListCapabilitiesAsync(biobankId)).ToList();

    // Build ViewModel.
    var model = new BiobankCapabilitiesModel
    {
      BiobankCapabilityModels = capabilities.Select(x => new BiobankCapabilityModel
      {
        Id = x.DiagnosisCapabilityId,
        OntologyTerm = x.OntologyTerm.Value,
        Protocol = x.SampleCollectionMode.Value
      })
    };

    return View(model);
  }

  [HttpGet]
  public async Task<ViewResult> AddCapability()
  {
    return View((AddCapabilityModel)(await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel())));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<ActionResult> AddCapability(AddCapabilityModel model, int biobankId)
  {

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

    if (await model.IsValid(ModelState, _ontologyTermService))
    {
      var associatedData = model.ListAssociatedDataModels()
          .Where(x => x.Active)
          .Select(y => new CapabilityAssociatedData
          {
            AssociatedDataTypeId = y.DataTypeId,
            AssociatedDataProcurementTimeframeId = y.ProvisionTimeId
          });

      await _biobankWriteService.AddCapabilityAsync(new CapabilityDTO
      {
        OrganisationId = biobankId,
        OntologyTerm = model.Diagnosis,
        BespokeConsentForm = model.BespokeConsentForm,
        BespokeSOP = model.BespokeSOP,
        AnnualDonorExpectation = model.AnnualDonorExpectation.Value
      },
      associatedData);

      this.SetTemporaryFeedbackMessage("Capability added!", FeedbackMessageType.Success);

      return RedirectToAction("Capabilities");
    }

    return View((AddCapabilityModel)(await PopulateAbstractCRUDAssociatedData(model)));
  }

  [HttpGet]
  //TODO:[AuthoriseToAdministerCapability]
  public async Task<ViewResult> EditCapability(int id)
  {
    var capability = await _biobankReadService.GetCapabilityByIdAsync(id);
    var groups = await PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());

    var model = new EditCapabilityModel
    {
      Id = id,
      Diagnosis = capability.OntologyTerm.Value,
      AnnualDonorExpectation = capability.AnnualDonorExpectation,
      Groups = groups.Groups
    };

    switch (capability.SampleCollectionModeId)
    {
      case 1:
        model.BespokeConsentForm = true;
        model.BespokeSOP = false;
        break;
      case 2:
        model.BespokeConsentForm = false;
        model.BespokeSOP = true;
        break;
      case 3:
        model.BespokeConsentForm = true;
        model.BespokeSOP = true;
        break;
      default:
        model.BespokeConsentForm = false;
        model.BespokeSOP = false;
        break;
    }

    foreach (var associatedDataModel in model.ListAssociatedDataModels())
    {
      var associatedData = capability.AssociatedData.FirstOrDefault(x => x.AssociatedDataTypeId == associatedDataModel.DataTypeId);

      if (associatedData != null)
      {
        associatedDataModel.Active = true;
        associatedDataModel.ProvisionTimeId = associatedData.AssociatedDataProcurementTimeframeId;
      }
    }

    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  // TODO: [AuthoriseToAdministerCapability]
  public async Task<ActionResult> EditCapability(EditCapabilityModel model)
  {
    if (await model.IsValid(ModelState, _ontologyTermService))
    {
      var associatedData = model.ListAssociatedDataModels()
          .Where(x => x.Active)
          .Select(y => new CapabilityAssociatedData
          {
            DiagnosisCapabilityId = model.Id,
            AssociatedDataTypeId = y.DataTypeId,
            AssociatedDataProcurementTimeframeId = y.ProvisionTimeId
          }).ToList();

      await _biobankWriteService.UpdateCapabilityAsync(new CapabilityDTO
      {
        Id = model.Id,
        OntologyTerm = model.Diagnosis,
        BespokeConsentForm = model.BespokeConsentForm,
        BespokeSOP = model.BespokeSOP,
        AnnualDonorExpectation = model.AnnualDonorExpectation.Value
      },
      associatedData);

      this.SetTemporaryFeedbackMessage("Capability updated!", FeedbackMessageType.Success);

      return RedirectToAction("Capability", new { id = model.Id });
    }

    return View((EditCapabilityModel)(await PopulateAbstractCRUDAssociatedData(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  //TODO: [AuthoriseToAdministerCapability]
  public async Task<IActionResult> DeleteCapability(int id)
  {
    await _biobankWriteService.DeleteCapabilityAsync(id);

    this.SetTemporaryFeedbackMessage("Capability deleted!", FeedbackMessageType.Success);

    return RedirectToAction("Capabilities");
  }

  [HttpGet]
  //TODO: [AuthoriseToAdministerCapability]
  public async Task<ViewResult> Capability(int id)
  {
    var capability = await _biobankReadService.GetCapabilityByIdAsync(id);

    var model = new CapabilityModel
    {
      Id = capability.DiagnosisCapabilityId,
      OntologyTerm = capability.OntologyTerm.Value,
      Protocols = capability.SampleCollectionMode.Value,
      AnnualDonorExpectation = capability.AnnualDonorExpectation,
      AssociatedData = capability.AssociatedData.Select(x => new AssociatedDataSummaryModel
      {
        Description = x.AssociatedDataType.Value,
        ProvisionTime = x.AssociatedDataProcurementTimeframe.Value,
        ProvisionTimeSortValue = x.AssociatedDataProcurementTimeframe.SortOrder
      })
    };

    return View(model);
  }

}
