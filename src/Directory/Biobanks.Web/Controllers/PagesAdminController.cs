using AutoMapper;
using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.ADAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "ADAC")]
    public class PagesAdminController : ApplicationBaseController
    {
        private readonly IContentPageService _contentPageService;

        private readonly IMapper _mapper;

        public PagesAdminController(IContentPageService contentPageService, IMapper mapper)
        {
            _contentPageService = contentPageService;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            // List all ContentPages
            var contentPage = _contentPageService.ListContentPages();

            if (contentPage != null)
            {
                // Map to ViewModel
                var model = _mapper.Map<ViewModel>(contentPage);
                // Return View
                return View(model);
            }
            else
                return HttpNotFound();
        }
    }
}