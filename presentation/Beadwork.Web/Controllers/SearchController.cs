using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beadwork.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly PictureService pictureService;
        public SearchController(PictureService pictureService)
        {
            this.pictureService = pictureService;
        }
        public IActionResult Index(string query)
        {
            var pictures = pictureService.GetAllByQuery(query);

            return View(pictures);
        }
    }
}
