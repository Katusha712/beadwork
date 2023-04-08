using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork
{
   public class OrderItem
    {
        public int PictureId { get; }
        public int Count { get; }
        public decimal Price { get; }
        public OrderItem(int pictureId, int count, decimal price)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater than zero.");

            PictureId = pictureId;
            Count = count;
            Price = price;
        }
    }
}
