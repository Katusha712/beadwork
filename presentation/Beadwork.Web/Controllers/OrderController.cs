using Beadwork.Contractors;
using Beadwork.Messages;
using Beadwork.Web.Contractors;
using Beadwork.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Beadwork.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IPictureRepository pictureRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;
        private readonly INotificationService notificationService;
        private readonly IEnumerable<IWebContractorService> webContractorServices;

        public OrderController(IPictureRepository pictureRepository,
                              IOrderRepository orderRepository,
                              IEnumerable<IDeliveryService> deliveryServices,
                              IEnumerable<IPaymentService> paymentServices,
                              IEnumerable<IWebContractorService> webContractorServices,
                              INotificationService notificationService)
        {
            this.pictureRepository = pictureRepository;
            this.orderRepository = orderRepository;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
            this.webContractorServices = webContractorServices;
            this.notificationService = notificationService;
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

        [HttpGet]
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
        private (Order order, Cart cart) GetOrCreateOrderAndCart()
        {
            Order order;
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }

            return (order, cart);
        }

        [HttpPost]
        public IActionResult UpdateItem(int pictureId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.GetItem(pictureId).Count = count ;

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);
        }

        [HttpPost]
        public IActionResult AddItem(int pictureId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            var picture = pictureRepository.GetById(pictureId);

            order.AddOrUpdateItem(picture, count);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Picture", new { id = pictureId });

        }

        [HttpPost]
        public IActionResult RemoveItem(int pictureId)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.RemoveItem(pictureId);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string cellPhone)
        {
            var order = orderRepository.GetById(id);
            var model = Map(order);

            if (!IsValidCellPhone(cellPhone))
            {
                model.Errors["cellPhone"] = "Номер телефону не дійсний.";
                return View("Index", model);
            }

            int code = 1111; //random.Next(1000,10000)
            HttpContext.Session.SetInt32(cellPhone, code);
            notificationService.SendConfirmationCode(cellPhone, code);

            return View("Confirmation", new ConfirmationModel
            {
                CellPhone = cellPhone,
                OrderId = id,
            }); 

        }
        private bool IsValidCellPhone(string cellPhone)
        {
            if (cellPhone == null)
                return false;

            cellPhone = cellPhone.Replace(" ", "")
                                 .Replace("-", "");

            return Regex.IsMatch(cellPhone, @"^\+?\d{12}$");
        }

        [HttpPost]
        public IActionResult Confirmate(int id, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone);
            if(storedCode == null)
            {
                return View("Confirmation", new ConfirmationModel
                {
                    CellPhone = cellPhone,
                    OrderId = id,
                    Errors = new Dictionary<string, string>
                {
                    { "code", "Порожнє поле для вводу коду, повторіть відправку." }
                },
                }); ;
            }

            code = 1111; //random.Next(1000,10000) ????????

            if (storedCode != code)
            {
                return View("Confirmation", new ConfirmationModel
                {
                    CellPhone = cellPhone,
                    OrderId = id,
                    Errors = new Dictionary<string, string>
                {
                    { "code", "Код невірний." }
                },
                }); ;

            }
            var order = orderRepository.GetById(id);
            order.CellPhone = cellPhone;
            orderRepository.Update(order);

            HttpContext.Session.Remove(cellPhone);

            var model = new DeliveryModel
            {
                OrderId = id,
                Methods = deliveryServices.ToDictionary(service => service.UniqueCode,
                                                        service => service.Title),
            };
                
            return View("DelivetyMethod", model);
        }

        [HttpPost]
        public IActionResult StartDelivery(int id, string uniqueCode)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepository.GetById(id);

            var form = deliveryService.CreateForm(order);

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult NextDelivery(int id, string uniqueCode, int step, Dictionary<string,string> values)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);

            var form = deliveryService.MoveNextForm(id, step, values);

            if (form.IsFinal)
            {
                var order = orderRepository.GetById(id);
                order.Delivery = deliveryService.GetDelivery(form);
                orderRepository.Update(order);


                var model = new DeliveryModel
                {
                    OrderId = id,
                    Methods = paymentServices.ToDictionary(service => service.UniqueCode,
                                                            service => service.Title),
                };

                return View("PaymentMethod", model);
            }

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult StartPayment(int id, string uniqueCode)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepository.GetById(id);

            var form = paymentService.CreateForm(order);

            var webContractorService = webContractorServices.SingleOrDefault(service => service.UniqueCode == uniqueCode);

            if (webContractorService != null)
                return Redirect(webContractorService.GetUri);

            return View("PaymentStep", form);
        }

        [HttpPost]
        public IActionResult NextPayment(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);

            var form = paymentService.MoveNextForm(id, step, values);

            if (form.IsFinal)
            {
                var order = orderRepository.GetById(id);
                order.Payment = paymentService.GetPayment(form);
                orderRepository.Update(order);

                return View("Finish");
            }

            return View("PaymentStep", form);
        }

        public IActionResult Finish()
        {
            HttpContext.Session.RemoveCart();

            return View();
        }

    }
}
