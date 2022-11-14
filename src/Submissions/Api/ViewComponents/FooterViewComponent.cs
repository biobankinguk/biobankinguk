using Biobanks.Submissions.Api.Models.Footer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace Biobanks.Submissions.Api.ViewComponents;

public class FooterViewComponent : ViewComponent
{
  private IWebHostEnvironment _hostEnvironment;

  public FooterViewComponent(IWebHostEnvironment hostEnvironment)
  {
    _hostEnvironment = hostEnvironment;
  }

  public IViewComponentResult Invoke()
  {
    var _navPath = Path.Combine(_hostEnvironment.WebRootPath, @"~/App_Config/footer.json");
    var json = File.ReadAllText(_navPath);
    var model = JsonConvert.DeserializeObject<FooterModel>(json);

    return View(model);
  }
}
