using System.Collections.Generic;
using DAL.Classes;

namespace Store.ViewModels
{
    public class ChangeOrderStatusView
    {
        public int Id { get; set; }

        public OrderStatus CurrentStatus { get; set; }

        public List<OrderStatus> OrderStatuses { get; set; }

        public bool SendEmail { get; set; }
    }
}
