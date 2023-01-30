using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Areas.Admin.Models.Networks;
using Microsoft.AspNetCore.Authorization;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]

public class NetworksController : Controller
{
  private readonly INetworkService _networkService;

  public NetworksController(INetworkService networkService)
  {
    _networkService = networkService;
  }
  
  public async Task<ActionResult> Index()
  {
    var allNetworks =
      (await _networkService.List()).ToList();

    var networks = allNetworks.Select(x => new NetworkModel
    {
      NetworkId = x.NetworkId,
      Name = x.Name
    }).ToList();

    foreach (var network in networks)
    {
      //get the admins
      network.Admins =
        (await _networkService.ListAdmins(network.NetworkId)).Select(x => new RegisterEntityAdminModel
        {
          UserId = x.Id,
          UserFullName = x.Name,
          UserEmail = x.Email,
          EmailConfirmed = x.EmailConfirmed
        }).ToList();
    }

    var model = new NetworksModel
    {
      Networks = networks,
      RequestUrl = HttpContext.Request.GetEncodedUrl()
    };

    return View(model);
  }

}
