using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Areas.Admin.Models.Funders;
using Microsoft.AspNetCore.Authorization;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]

public class FundersController : Controller
{
  private readonly IReferenceDataCrudService<Funder> _funderService;

  public FundersController(IReferenceDataCrudService<Funder> funderService)
  {
    _funderService = funderService;
  }

  public async Task<ActionResult> Funders()
  {
    return View(
        (await _funderService.List())
            .Select(x =>
                new FunderModel
                {
                  FunderId = x.Id,
                  Name = x.Value
                })
            .ToList()
        );
  }

  [HttpGet]
  public async Task<ActionResult> DeleteFunder(int id)
      => View(await _funderService.Get(id));

  [HttpPost]
  public async Task<ActionResult> DeleteFunder(FunderModel model)
  {
    try
    {
      await _funderService.Delete(model.FunderId);

      this.SetTemporaryFeedbackMessage($"{model.Name} and its associated data has been deleted.", FeedbackMessageType.Success);
    }
    catch
    {

      this.SetTemporaryFeedbackMessage("The selected funder could not be deleted.", FeedbackMessageType.Danger);
    }

    return RedirectToAction("Funders");
  }

  [HttpPost]
  public async Task<ActionResult> EditFunderAjax(FunderModel model)
  {
    //If this description is valid, it already exists
    if (await _funderService.Exists(model.Name))
    {
      ModelState.AddModelError("Name", "That funder name is already in use. Funder names must be unique.");
    }

    if (!ModelState.IsValid)
    {
      return Ok(ModelState);
    }

    await _funderService.Update(new Funder
    {
      Id = model.FunderId,
      Value = model.Name
    });

    //Everything went A-OK!
    return Ok(new
    {
      success = true,
      name = model.Name
    });
  }

  public ActionResult EditFunderSuccess(string name)
  {
    //This action solely exists so we can set a feedback message

    this.SetTemporaryFeedbackMessage($"The funder \"{name}\" has been edited successfully.",
        FeedbackMessageType.Success);

    return RedirectToAction("Funders");
  }

  [HttpPost]
  public async Task<ActionResult> AddFunderAjax(FunderModel model)
  {
    //If this description is valid, it already exists
    if (await _funderService.Exists(model.Name))
    {
      ModelState.AddModelError("Name", "That funder name is already in use. Funder names must be unique.");
    }

    if (!ModelState.IsValid)
    {
      return Ok(ModelState);
    }

    await _funderService.Add(new Funder
    {
      Value = model.Name,
    });

    //Everything went A-OK!
    return Ok(new
    {
      success = true,
      name = model.Name
    });
  }

  public ActionResult AddFunderSuccess(string name)
  {
    //This action solely exists so we can set a feedback message

    this.SetTemporaryFeedbackMessage($"The funder \"{name}\" has been added successfully.",
        FeedbackMessageType.Success);

    return RedirectToAction("Funders");
  }

}
