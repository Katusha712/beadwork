﻿using Beadwork.Contractors;
using Beadwork.Web.App;
using Beadwork.Web.Contractors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Beadwork.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService orderService;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;
        private readonly IEnumerable<IWebContractorService> webContractorServices;

        public OrderController(OrderService orderService,
                              IEnumerable<IDeliveryService> deliveryServices,
                              IEnumerable<IPaymentService> paymentServices,
                              IEnumerable<IWebContractorService> webContractorServices)
        {
            this.orderService = orderService;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
            this.webContractorServices = webContractorServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (orderService.TryGetModel(out OrderModel model))
                return View(model);

            return View("Empty");
        }

        [HttpPost]
        public IActionResult UpdateItem(int pictureId, int count)
        {
            var model = orderService.UpdatePicture(pictureId, count);

            return RedirectToAction("Index", model);
        }

        [HttpPost]
        public IActionResult AddItem(int pictureId, int count = 1)
        {
            orderService.AddPicture(pictureId, count);

            return RedirectToAction("Index", "Picture", new { id = pictureId });

        }

        [HttpPost]
        public IActionResult RemoveItem(int pictureId)
        {
            var model = orderService.RemovePicture(pictureId);

            return RedirectToAction("Index", model);
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string cellPhone)
        {
            var model = orderService.SendConfirmation(cellPhone);

            return View("Confirmation", model);

        }

        [HttpPost]
        public IActionResult ConfirmCellPhone(string cellPhone, int confirmationCode)
        {
            var model = orderService.ConfirmCellPhone(cellPhone, confirmationCode);

            if (model.Errors.Count > 0)
                return View("Confirmation", model);

            var deliveryMethods = deliveryServices.ToDictionary(service => service.Name,
                                                              service => service.Title);
                
            return View("DelivetyMethod", deliveryMethods);
        }

        [HttpPost]
        public IActionResult StartDelivery(string serviceName)
        {
            var deliveryService = deliveryServices.SingleOrDefault(service => service.Name == serviceName);
            var order = orderService.GetOrder();
            var form = deliveryService.FirstForm(order);

            var webContractorService = webContractorServices.SingleOrDefault(service => service.Name == serviceName);
            if (webContractorService == null)
                return View("DeliveryStep", form);

            var returnUri = GetReturnUri(nameof(NextDelivery));
            var redirectUri = webContractorService.StartSession(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }
        private Uri GetReturnUri(string action)
        {
            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = Url.Action(action),
                Query = null,
            };

            if (Request.Host.Port != null)
                builder.Port = Request.Host.Port.Value;

            return builder.Uri;
        }
        [HttpPost]
        public IActionResult NextDelivery(string serviceName, int step, Dictionary<string,string> values)
        {
            var deliveryService = deliveryServices.Single(service => service.Name == serviceName);

            var form = deliveryService.NextForm(step, values);
            if(!form.IsFinal)
                return View("DeliveryStep", form);

            var delivery = deliveryService.GetDelivery(form);
            orderService.SetDelivery(delivery);

            var paymentMethods = paymentServices.ToDictionary(service => service.Name,
                                                              service => service.Title);

            return View("PaymentMethod", paymentMethods);
        }

        [HttpPost]
        public IActionResult StartPayment( string serviceName)
        {
            var paymentService = paymentServices.Single(service => service.Name == serviceName);
            var order = orderService.GetOrder();
            var form = paymentService.FirstForm(order);

            var webContractorService = webContractorServices.SingleOrDefault(service => service.Name == serviceName);

            if (webContractorService == null)
                return View("PaymentStep", form);

            var returnUri = GetReturnUri(nameof(NextPayment));
            var redirectUri = webContractorService.StartSession(form.Parameters, returnUri);

            return Redirect(redirectUri.ToString());
        }

        [HttpPost]
        public IActionResult NextPayment(string serviceName, int step, Dictionary<string, string> values)
        {
            var paymentService = paymentServices.Single(service => service.Name == serviceName);

            var form = paymentService.NextForm(step, values);
            if (!form.IsFinal)
                return View("PaymentStep", form);

            var payment = paymentService.GetPayment(form);
            var model = orderService.SetPayment(payment);

            return View("Finish", model);
        }

    }
}
