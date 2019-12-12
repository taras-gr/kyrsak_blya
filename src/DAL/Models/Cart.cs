using System.Collections.Generic;

namespace DAL.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public ICollection<GoodCart> Goods { get; set; }
    }
}
