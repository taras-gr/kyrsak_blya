using System;
using System.Collections.Generic;
using DAL.Classes;

namespace Store.ViewModels
{
    public class FindOrderView
    {
        public int? Id { get; set; }

        public string CustomerName { get; set; }

        public string CustomerSurname { get; set; }

        public string CustomerEmail { get; set; }

        public DateTime? OrderDate { get; set; }

        public string EndPointCity { get; set; }

        public List<OrderStatus> OrderStatuses { get; set; }

        public List<OrderStatus> SelectedStatuses { get; set; }
    }
}
