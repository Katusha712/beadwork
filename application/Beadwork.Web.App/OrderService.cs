using Beadwork.Messages;
using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Beadwork.Web.App
{
    public class OrderService
    {
        private readonly IPictureRepository pictureRepository;
        private readonly IOrderRepository orderRepository;
        private readonly INotificationService notificationService;
        private readonly IHttpContextAccessor httpContexAccesor;

        protected ISession Session => httpContexAccesor.HttpContext.Session;

        public OrderService(IPictureRepository pictureRepository,
                            IOrderRepository orderRepository,
                            INotificationService notificationService,
                            IHttpContextAccessor httpContexAccesor)
        {
            this.pictureRepository = pictureRepository;
            this.orderRepository = orderRepository;
            this.notificationService = notificationService;
            this.httpContexAccesor = httpContexAccesor;
        }
        public bool TryGetModel(out OrderModel model)
        {
            if(TryGetOrder(out Order order))
            {
                model = Map(order);
                return true;
            }
            model = null;
            return false;
        }

        internal bool TryGetOrder(out Order order)
        {
            if(Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
                return true;
            }

            order = null;
            return false;
        }
        internal OrderModel Map(Order order)
        {
            var pictures = GetPictures(order);
            var items = from item in order.Items
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
                Items = items.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                CellPhone = order.CellPhone,
                DeliveryDescription = order.Delivery?.Description,
                PaymentDescription = order.Payment?.Description,
            };
        }
        internal IEnumerable<Picture> GetPictures(Order order)
        {
            var pictureIds = order.Items.Select(item => item.PictureId);

            return pictureRepository.GetAllByIds(pictureIds);
        }
        public  OrderModel AddPicture(int pictureId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few pictures to add.");

            if (!TryGetOrder(out Order order))
                order = orderRepository.Create();

            AddOrUpdatePicture(order, pictureId, count);
            UpdateSession(order);

            return Map(order);
        }
        internal void AddOrUpdatePicture(Order order, int pictureId, int count)
        {
            var picture = pictureRepository.GetById(pictureId);

            if (order.Items.TryGet(pictureId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(picture.Id, picture.Price, count);
        }
        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }
        public OrderModel UpdatePicture(int pictureId, int count)
        {
            var order = GetOrder();
            order.Items.Get(pictureId).Count = count;

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
        public OrderModel RemovePicture(int pictureId)
        {
            var order = GetOrder();
            order.Items.Remove(pictureId);

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }
        public Order GetOrder()
        {
            if (TryGetOrder(out Order order))
                return order;

            throw new InvalidOperationException("Empty session.");
        }

        public OrderModel SendConfirmation(string cellPhone)
        {
            var order = GetOrder();
            var model = Map(order);

            if (TryFormatPhone(cellPhone, out string formattedPhone))
            {
                var confirmationCode = 1111; // to do: random.Next(1000,10000) = 1000, 1001;
                model.CellPhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                notificationService.SendConfirmationCode(formattedPhone, confirmationCode);
            }
            else
                model.Errors["cellPhone"] = "Номер телефону не дійсний.";

            return model;
        }

        private readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
        internal bool TryFormatPhone(string cellPhone, out string formattedPhone)
        {
            try
            {
                var phoneNumber = phoneNumberUtil.Parse(cellPhone, "uk");
                formattedPhone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                return true;
            }
            catch(NumberParseException)
            {
                formattedPhone = null;
                return false;
            }
        }

        public OrderModel ConfirmCellPhone(string cellPhone, int confirmationCode)
        {
            int? storedCode = Session.GetInt32(cellPhone);
            var model = new OrderModel();

            if(storedCode == null)
            {
                model.Errors["cellPhone"] = "Щось сталося. Спробуйте отримати код знову.";
                return model;
            }

            if (storedCode != confirmationCode)
            {
                model.Errors["cellPhone"] = "Невірний код. Перевірте і спробуйте знову.";
                return model;
            }

            var order = GetOrder();
            order.CellPhone = cellPhone;
            orderRepository.Update(order);

            Session.Remove(cellPhone);

            return Map(order);
        }
        public OrderModel SetDelivery(OrderDelivery delivery)
        {
            var order = GetOrder();
            order.Delivery = delivery;
            orderRepository.Update(order);

            return Map(order);
        }

        public OrderModel SetPayment(OrderPayment payment)
        {
            var order = GetOrder();
            order.Payment = payment;
            orderRepository.Update(order);
            Session.RemoveCart();

            return Map(order);
        }
    }
}
