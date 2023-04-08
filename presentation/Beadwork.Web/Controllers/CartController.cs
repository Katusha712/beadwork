using Beadwork.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beadwork.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IPictureRepository pictureRepository;
        public CartController(IPictureRepository pictureRepository)
        {
            this.pictureRepository = pictureRepository;
        }
        public IActionResult Add(int id)
        {
            var picture = pictureRepository.GetById(id);
            Cart cart;
            if (!HttpContext.Session.TryGetCart(out cart))
                cart = new Cart();

            if (cart.Items.ContainsKey(id))
                cart.Items[id]++;
            else
                cart.Items[id] = 1;

            cart.Amount += picture.Price;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Picture", new { id });
        }
    }
}
