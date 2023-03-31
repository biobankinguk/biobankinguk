using System.IO;
using Biobanks.Directory.Models.Footer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biobanks.Directory.ViewComponents;

public class FooterViewComponent : ViewComponent
{
  private readonly IWebHostEnvironment _env;

  public FooterViewComponent(IWebHostEnvironment env)
  {
    _env = env;
  }

  public IViewComponentResult Invoke()
  {
    var _navPath = Path.Combine(_env.ContentRootPath, "Settings/footer.json");
    var json = File.ReadAllText(_navPath);
    var model = JsonConvert.DeserializeObject<FooterModel>(json);

    return View(model);
  }
}
