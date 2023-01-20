using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
public class NetworkController : Controller
{
  private readonly INetworkService _networkService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenLoggingService _tokenLog;
  private readonly IEmailService _emailService;

  public NetworkController(
    INetworkService networkService,
    UserManager<ApplicationUser> userManager,
     ITokenLoggingService tokenLog,
     IEmailService emailService)
  {
    _networkService = networkService;
    _userManager = userManager;
    _tokenLog = tokenLog;
    _emailService = emailService;
  }


}
