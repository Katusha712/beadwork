using Beadwork.Contractors;
using Beadwork.Web.Contractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork.Privat
{
    public class PrivatPaymentService : IPaymentService, IWebContractorService
    {
        public string UniqueCode => "Privat";

        public string Title => "Оплата банківською картою";

        public string GetUri => "/Privat/";

        public Form CreateForm(Order order)
        {
            return new Form(UniqueCode, order.Id, 1, false, new Field[0]);
        }

        public OrderPayment GetPayment(Form form)
        {
            return new OrderPayment(UniqueCode, "Оплата картою", new Dictionary<string, string>());
        }

        public Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            return new Form(UniqueCode, orderId, 2, true, new Field[0]);
        }
    }
}