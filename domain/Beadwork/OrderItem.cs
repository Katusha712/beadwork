using System;

namespace Beadwork
{
   public class OrderItem
    {
        public int PictureId { get; }
        private int count;
        public int Count 
        {
            get { return count; }
            set 
            {
                ThrowIfInvalidCount(value);
                count = value;
            }
        }
        public decimal Price { get; }
        public OrderItem(int pictureId, int count, decimal price)
        {
            ThrowIfInvalidCount(count);

            PictureId = pictureId;
            Count = count;
            Price = price;
        }

        private static void ThrowIfInvalidCount(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater than zero.");
        }
    }
}
