using System;
using System.Collections;
using System.Collections.Generic;

namespace Beadwork
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly List<OrderItem> items;
        public OrderItemCollection(IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            this.items = new List<OrderItem>(items);
        }

        public int Count => items.Count;
        public OrderItem Get(int pictureId)
        {
            if (TryGet(pictureId, out OrderItem orderItem))
                return orderItem;

            throw new InvalidOperationException("Picture not found.");
        }

        public IEnumerator<OrderItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (items as IEnumerable).GetEnumerator();
        }

        public bool TryGet(int pictureId, out OrderItem orderItem)
        {
            var index = items.FindIndex(item => item.PictureId == pictureId);

            if (index == -1)
            {
                orderItem = null;
                return false;
            }

            orderItem = items[index];
            return true;
        }

        public OrderItem Add(int pictureId, decimal price, int count)
        {
            if (TryGet(pictureId, out OrderItem orderItem))
                throw new InvalidOperationException("Picture already in cart.");

            orderItem = new OrderItem(pictureId, price, count);
            items.Add(orderItem);

            return orderItem;
        }

        public void Remove(int pictureId)
        {
            items.Remove(Get(pictureId));
        }
    }
}
