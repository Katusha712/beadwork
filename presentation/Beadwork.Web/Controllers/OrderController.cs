using Beadwork.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beadwork.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IPictureRepository pictureRepository;
        private readonly IOrderRepository orderRepository;
        public OrderController(IPictureRepository pictureRepository,
                              IOrderRepository orderRepository)
        {
            this.pictureRepository = pictureRepository;
            this.orderRepository = orderRepository;
        }

        private OrderModel Map(Order order)
        {
            var pictureIds = order.Items.Select(item => item.PictureId);
            var pictures = pictureRepository.GetAllByIds(pictureIds);
            var itemModels = from item in order.Items
                             join picture in pictures on item.PictureId equals picture.Id
                             select new OrderItemModel
                             {
                                 PictureId = picture.Id,
                                 Title = picture.Title,
                                 Author = picture.Author,
                                 Price = item.Price,
                                 Count = item.Count,
                             };
            return new OrderModel
            {
                Id = order.Id,
                Items = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
            };
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                var order = orderRepository.GetById(cart.OrderId);
                OrderModel model = Map(order);

                return View(model);
            }

            return View("Empty");

        }
        public IActionResult AddItem(int id)
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
