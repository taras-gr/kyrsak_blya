namespace DAL.Models
{
    public class GoodStorage
    {
        public int GoodId { get; set; }
        public Good Good { get; set; }

        public int StorageId { get; set; }
        public Storage Storage { get; set; }
    }
}
