namespace DAL.Models
{
    public class GoodOrder
    {
        public int GoodId { get; set; }
        public Good Good { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
