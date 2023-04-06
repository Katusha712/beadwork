using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beadwork.Web.Controllers
{
    public class PictureController : Controller
    {
        private readonly IPictureRepository pictureRepository;
        public PictureController(IPictureRepository pictureRepository)
        {
            this.pictureRepository = pictureRepository;
        }
        public IActionResult Index(int id)
        {
            Picture picture = pictureRepository.GetById(id);

            return View(picture);
        }
    }
}
