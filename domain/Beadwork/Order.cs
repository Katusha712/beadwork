using System;
using System.Collections.Generic;
using System.Linq;

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

        public int TotalCount => items.Sum(item => item.Count);

        public decimal TotalPrice => items.Sum(item => item.Price * item.Count);


        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;

            this.items = new List<OrderItem>(items);
        }

        public OrderItem GetItem(int pictureId)
        {
            int index = items.FindIndex(item => item.PictureId == pictureId);

            if (index == -1)
                ThrowPictureException("Picture not found.", pictureId);

            return items[index];
        }

        public void AddOrUpdateItem(Picture picture, int count)
        {
            if (picture == null)
                throw new ArgumentNullException(nameof(picture));

            int index = items.FindIndex(item => item.PictureId == picture.Id);
            if(index == -1)
                items.Add(new OrderItem(picture.Id, count, picture.Price));
            else
                items[index].Count += count;
        }

        public void RemoveItem(int pictureId)
        {
            int index = items.FindIndex(item => item.PictureId==pictureId);

            if(index ==-1)
                ThrowPictureException("Order does not contain specified item.", pictureId);

            items.RemoveAt(index);
        }

        private void  ThrowPictureException(string message, int pictureId)
        {
            var exception = new InvalidOperationException(message);
            exception.Data["PictureId"] = pictureId;

            throw exception;
        }
    }
}
