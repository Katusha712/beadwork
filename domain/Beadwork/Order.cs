using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork
{
    public class Order
    {
        public int Id { get; }

        private List<OrderItem> items;
        public IReadOnlyCollection<OrderItem> Items
        {
            get { return items; }
        }

        public int TotalCount
        {
            get { return items.Sum(item => item.Count); }
        }

        public decimal TotalPrice
        {
            get { return items.Sum(item => item.Price * item.Count); }
        }


        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;

            this.items = new List<OrderItem>(items);
        }

        public void AddItem(Picture picture, int count)
        {
            if (picture == null)
                throw new ArgumentNullException(nameof(picture));

            var item = items.SingleOrDefault(x => x.PictureId == picture.Id);

            if(item == null)
            {
                items.Add(new OrderItem(picture.Id, count, picture.Price));
            }
            else
            {
                items.Remove(item);
                items.Add(new OrderItem(picture.Id, item.Count + count, picture.Price));
            }
        }
    }
}
