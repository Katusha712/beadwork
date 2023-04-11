using Beadwork.Web.App;
using Microsoft.AspNetCore.Mvc;

namespace Beadwork.Web.Controllers
{
    public class PictureController : Controller
    {
        private readonly PictureService pictureService;
        public PictureController(PictureService pictureService)
        {
            this.pictureService = pictureService;
        }
        public IActionResult Index(int id)
        {
            var model = pictureService.GetById(id);

            return View(model);
        }
    }
}
