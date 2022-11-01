using System;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Controllers;

public class StatusCodeController : Controller
{
    public StatusCodeController()
    {
    }
    
    public IActionResult StatusCode(int? code = null)
    {
        ViewData["Code"] = $"Error occurred. The ErrorCode is: {code}";
        ViewData["Greeting"] = "Hello";
        return View();
    }
}