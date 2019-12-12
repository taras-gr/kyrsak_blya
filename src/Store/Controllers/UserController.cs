using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Store.Controllers
{
    [Authorize(Roles = "customer")]
    public class UserController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public UserController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        public async Task<IActionResult> Index()
        {
            int customerId = unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name).First().Id;
            Customer customer = await unitOfWork.Customers.Get(customerId);
            List<Order> customerOrders = new List<Order>();

            customerOrders.AddRange(unitOfWork.Orders.GetAll().Where(o => o.CustomerId == customer.Id));

            return View(customerOrders);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);

            if (order != null)
            {
                order.OrderStatus = OrderStatus.Cancelled;
                unitOfWork.Orders.Update(order);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);

            if (order != null)
            {
                await unitOfWork.Orders.Delete(id);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ShowGoods(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);
            List<Good> goods = new List<Good>();

            foreach (var item in order.Products)
            {
                goods.Add(await unitOfWork.Goods.Get(item.GoodId));
            }

            ViewBag.OrderId = order.Id;

            return View(goods);
        }
    }
}
