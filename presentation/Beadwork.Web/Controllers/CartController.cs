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
        private readonly IOrderRepository orderRepository;
        public CartController(IPictureRepository pictureRepository,
                              IOrderRepository orderRepository)
        {
            this.pictureRepository = pictureRepository;
            this.orderRepository = orderRepository;
        }
        public IActionResult Add(int id)
        {
            Order order;
            Cart cart;

            if (HttpContext.Session.TryGetCart(out cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }

            var picture = pictureRepository.GetById(id);
            order.AddItem(picture, 1);
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Picture", new { id });
        }
    }
}
