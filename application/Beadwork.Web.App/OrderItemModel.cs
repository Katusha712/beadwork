using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beadwork.Web.App
{
    public class OrderItemModel
    {
        public int PictureId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; } 
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
