using Beadwork.Web.App;
using Microsoft.AspNetCore.Mvc;

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

            return View("Index", pictures);
        }
    }
}
