using Beadwork.Contractors;
using Beadwork.Web.Contractors;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork.Privat
{
    public class PrivatPaymentService : IPaymentService, IWebContractorService
    {
        private readonly IHttpContextAccessor httpContexAccessor;
        public PrivatPaymentService(IHttpContextAccessor httpContexAccessor)
        {
            this.httpContexAccessor = httpContexAccessor;
        }
        private HttpRequest Request => httpContexAccessor.HttpContext.Request;
        public string Name => "Privat";

        public string Title => "Оплата банківською картою";

        public Form FirstForm(Order order)
        {
            return Form.CreateFirst(Name)
                       .AddParameter("orderId", order.Id.ToString());
        }

        public OrderPayment GetPayment(Form form)
        {
            if (form.ServiceName != Name || !form.IsFinal)
                throw new InvalidOperationException("Invalid payment form.");

            return new OrderPayment(Name, "Оплата картою",form.Parameters);
        }

        public Form NextForm( int step, IReadOnlyDictionary<string, string> values)
        {
            if (step != 1)
                throw new InvalidOperationException("Invalid Privat payment step.");

            return Form.CreateLast(Name, step + 1, values);
        }
        public Uri StartSession(IReadOnlyDictionary<string,string>parameters,Uri returnUri)
        {
            var queryString = QueryString.Create(parameters);
            queryString += QueryString.Create("returnUri", returnUri.ToString());

            var builder = new UriBuilder(Request.Scheme, Request.Host.Host)
            {
                Path = "Privat/",
                Query = queryString.ToString(),
            };

            if (Request.Host.Port != null)
                builder.Port = Request.Host.Port.Value;

            return builder.Uri;
        }
    }
}