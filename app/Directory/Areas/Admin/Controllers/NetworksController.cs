using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Areas.Admin.Models.Networks;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
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
