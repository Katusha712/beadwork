using System;
using System.Collections.Generic;
using System.Linq;

namespace Beadwork.Contractors
{
    public class PostamateDeliveryService : IDeliveryService
    {
        private static IReadOnlyDictionary<string, string> cities = new Dictionary<string,string>()
        {
            {"1","Харків" },
            {"2","Київ" },
        };
        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>postamates = new Dictionary<string,IReadOnlyDictionary<string,string>>()
        {
            { 
                "1", 
                new Dictionary<string, string>
                {
                    {"1","№3158 - Супермаркет РОСТ, проїзд Рогатинський, 3"},
                    {"2","№5945 - WOG, вулиця Шевченка, 41"},
                    {"3","№25204 - маг. Хазар, Астрономічна вулиця, 35л"},
                }
            },
            {
                "2", 
                new Dictionary<string, string>
                {
                    {"4","№3024 - Iland, вул. Машинобудівна, 37"},
                    {"5","№5466 - маг. АТБ, проспект Перемоги, 18"},
                    {"6","№3021 - вул. Оболонська, 35"},
                }
            }
        };

        public string Name => "Postamate";
        public string Title => "Доставка у поштомат";

        public OrderDelivery GetDelivery(Form form)
        {
            if (form.ServiceName != Name || !form.IsFinal)
                throw new InvalidOperationException("Invalid form.");

            var cityId = form.Parameters["city"];
            var cityName = cities[cityId];
            var postamateId = form.Parameters["postamate"];
            var postamateName = postamates[cityId][postamateId];

            var parameters = new Dictionary<string, string>
            {
                { nameof(cityId), cityId },
                { nameof(cityName), cityName },
                { nameof(postamateId), postamateId },
                { nameof(postamateName), postamateName },
            };

            var description = $"Місто: {cityName}\nПоштамат: {postamateName}";

            return new OrderDelivery(Name, description, 150m, parameters);
        }

        public Form FirstForm(Order order)
        {
            return Form.CreateFirst(Name)
                        .AddParameter("orderId", order.Id.ToString())
                        .AddField(new SelectionField("Місто", "city", "1", cities));
           
        }

        public Form NextForm(int step, IReadOnlyDictionary<string, string> values)
        {
            if(step == 1)
            {
                if (values["city"] == "1")
                {
                    return Form.CreateNext(Name, 2, values)
                               .AddField(new SelectionField("Поштамат", "postamate", "1", postamates["1"]));
                }
                else if (values["city"] == "2")
                {
                    return Form.CreateNext(Name, 2, values)
                               .AddField(new SelectionField("Поштамат", "postamate", "4", postamates["2"]));
                }
                else
                    throw new InvalidOperationException("Invalid postamate city.");
            }
            else if(step == 2)
            {
                return Form.CreateLast(Name, 3, values);
            }
            else
                throw new InvalidOperationException("Invalid postamate step.");
        }
    }
}
