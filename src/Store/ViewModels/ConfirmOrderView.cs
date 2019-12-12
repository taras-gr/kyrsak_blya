using System.Collections.Generic;
using DAL.Models;

namespace Store.ViewModels
{
    public class ConfirmOrderView
    {
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Country Country { get; set; }

        public List<OrderPart> Goods { get; set; }

        public int Count { get; set; }

        public List<Storage> Storages { get; set; }

        public int CommonPrice { get; set; }

        public string EndPointCity { get; set; }

        public string EndPointStreet { get; set; }
    }
}
