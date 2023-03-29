using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Capabilities;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.Models.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;

[Area("Biobank")]
[Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
[SuspendedWarning]
public class CapabilitiesController : Controller
{
  private readonly IAbstractCrudService _abstractCrudService;
  private readonly IOntologyTermService _ontologyTermService;
  private readonly ICapabilityService _capabilityService;

  public CapabilitiesController(IAbstractCrudService abstractCrudService, 
    IOntologyTermService ontologyTermService, 
    ICapabilityService capabilityService)
  {
    _abstractCrudService = abstractCrudService;
    _ontologyTermService = ontologyTermService;
    _capabilityService = capabilityService;
  }
  
  [HttpGet]
  [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
  public async Task<ActionResult> Index(int biobankId)
  {

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

    // Call service to get capabilities for logged in BioBank.
    var capabilities = (await _capabilityService.ListCapabilitiesAsync(biobankId)).ToList();

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
  [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
  public async Task<ViewResult> AddCapability(int biobankId)
  {
    return View((AddCapabilityModel)(await _abstractCrudService.PopulateAbstractCRUDAssociatedData(new AddCapabilityModel())));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
  public async Task<ActionResult> AddCapability(int biobankId, AddCapabilityModel model)
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

      await _capabilityService.AddCapabilityAsync(new CapabilityDTO
      {
        OrganisationId = biobankId,
        OntologyTerm = model.Diagnosis,
        BespokeConsentForm = model.BespokeConsentForm,
        BespokeSOP = model.BespokeSOP,
        AnnualDonorExpectation = model.AnnualDonorExpectation.Value
      },
      associatedData);

      this.SetTemporaryFeedbackMessage("Capability added!", FeedbackMessageType.Success);

      return RedirectToAction("Index", new { biobankId = biobankId });
    }

    return View((AddCapabilityModel)(await _abstractCrudService.PopulateAbstractCRUDAssociatedData(model)));
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerCapability))]
  public async Task<ViewResult> EditCapability(int biobankId, int id)
  {
    var capability = await _capabilityService.GetCapabilityByIdAsync(id);
    var groups = await _abstractCrudService.PopulateAbstractCRUDAssociatedData(new AddCapabilityModel());

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
  [Authorize(nameof(AuthPolicies.CanAdministerCapability))]
  public async Task<ActionResult> EditCapability(int biobankId, EditCapabilityModel model)
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

      await _capabilityService.UpdateCapabilityAsync(new CapabilityDTO
      {
        Id = model.Id,
        OntologyTerm = model.Diagnosis,
        BespokeConsentForm = model.BespokeConsentForm,
        BespokeSOP = model.BespokeSOP,
        AnnualDonorExpectation = model.AnnualDonorExpectation.Value
      },
      associatedData);

      this.SetTemporaryFeedbackMessage("Capability updated!", FeedbackMessageType.Success);

      return RedirectToAction("Capability", new { biobankId = biobankId, id = model.Id });
    }

    return View((EditCapabilityModel)(await _abstractCrudService.PopulateAbstractCRUDAssociatedData(model)));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.CanAdministerCapability))]
  public async Task<IActionResult> DeleteCapability(int biobankId, int id)
  {
    await _capabilityService.DeleteCapabilityAsync(id);

    this.SetTemporaryFeedbackMessage("Capability deleted!", FeedbackMessageType.Success);

    return RedirectToAction("Index", "Capabilities", new { biobankId = biobankId });
  }

  [HttpGet]
  [Authorize(nameof(AuthPolicies.CanAdministerCapability))]
  public async Task<ViewResult> Capability(int biobankId, int id)
  {
    var capability = await _capabilityService.GetCapabilityByIdAsync(id);

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
