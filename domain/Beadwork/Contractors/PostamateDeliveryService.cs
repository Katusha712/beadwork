using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string UniqueCode => "Postamate";
        public string Title => "Доставка у поштомати";

        public Form CreateForm(Order order)
        {
            if (order == null)
                throw new ArgumentException(nameof(order));

            return new Form(UniqueCode,order.Id, 1, false, new[]
            {
                new SelectionField("Місто", "city", "1", cities),
            });
        }

        public Form MoveNext(int orderId, int step, IReadOnlyDictionary<string, string> values)
        {
            if(step == 1)
            {
                if (values["city"] == "1")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Місто","city","1"),
                        new SelectionField("Поштамат","postamate","1", postamates["1"])
                    });
                }
                else if (values["city"] == "2")
                {
                    return new Form(UniqueCode, orderId, 2, false, new Field[]
                    {
                        new HiddenField("Місто","city","2"),
                        new SelectionField("Поштамат","postamate","4", postamates["2"])
                    });
                }
                else
                    throw new InvalidOperationException("Invalid postamate city.");
            }
            else if(step == 2)
            {
                return new Form(UniqueCode, orderId, 3, true, new Field[]
                {
                        new HiddenField("Місто","city",values["city"]),
                        new SelectionField("Поштамат","postamate", values["postamates"], postamates[""]) //?
                });
            }
            else
                throw new InvalidOperationException("Invalid postamate step.");


        }
    }
}
