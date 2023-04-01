using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beadwork.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPictureRepository pictureRepository;
        public SearchController(IPictureRepository pictureRepository)
        {
            this.pictureRepository = pictureRepository;
        }
        public IActionResult Index(string query)
        {
            var pictures = pictureRepository.GetAllByTitle(query);

            return View(pictures);
        }
    }
}
