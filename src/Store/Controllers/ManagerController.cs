using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Helpers.Sender;
using Store.ViewModels;

namespace Store.Controllers
{
    [Authorize(Roles = "manager")]
    public class ManagerController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public ManagerController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(unitOfWork.Orders.GetAll().ToList());
        }

        [HttpGet]
        public IActionResult Find()
        {
            FindOrderView model = new FindOrderView
            {
                OrderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList(),
                SelectedStatuses = new List<OrderStatus>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Find(FindOrderView model, List<OrderStatus> statuses)
        {
            List<Order> orders = new List<Order>();

            if (ModelState.IsValid)
            {
                var allOrders = unitOfWork.Orders.GetAll().ToList();
                foreach (var order in allOrders)
                {
                    if (this.AddToResult(model, order, statuses))
                    {
                        orders.Add(order);
                    }
                }

                HttpContext.Session.Set("list", orders);

                return RedirectToAction("FindResult", "Manager");
            }
            return View(model);
        }

        public IActionResult FindResult()
        {
            var orders = HttpContext.Session.Get<List<Order>>("list");

            if (orders == null)
            {
                return RedirectToAction("Find");
            }

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> ShowGoods(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);
            List<Good> goods = new List<Good>();

            if (order != null)
            {
                foreach (var item in order.Products)
                {
                    goods.Add(await unitOfWork.Goods.Get(item.GoodId));
                }

                ViewBag.OrderId = order.Id;

                return View(goods);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ShowCustomer(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);

            if (order != null)
            {
                ViewBag.OrderId = order.Id;

                return View(order.Customer);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);

            ChangeOrderStatusView model = new ChangeOrderStatusView
            {
                Id = order.Id,
                CurrentStatus = order.OrderStatus,
                OrderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeOrderStatusView model)
        {
            Order order = await unitOfWork.Orders.Get(model.Id);

            if (order != null)
            {
                bool sendEmail = model.SendEmail;
                Enum.TryParse(Request.Form["orderStatus"], out OrderStatus temp);

                if (order.OrderStatus == temp)
                {
                    sendEmail = false;
                }

                order.OrderStatus = temp;
                unitOfWork.Orders.Update(order);
                await unitOfWork.SaveAsync();

                if (sendEmail)
                {
                    EmailSender emailSender = new EmailSender();
                    await emailSender.StatusChangedSend(order);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Order order = await unitOfWork.Orders.Get(id);

            EditOrderView model = new EditOrderView
            {
                Id = order.Id,
                CustomerName = order.Customer.FirstName,
                CustomerSurname = order.Customer.SecondName,
                CustomerEmail = order.Customer.Email,
                OrderDate = order.OrderDate,
                EndPointCity = order.EndPointCity,
                EndPointStreet = order.EndPointStreet,
                Status = order.OrderStatus
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOrderView model)
        {
            Order order = await unitOfWork.Orders.Get(model.Id);

            if (order != null)
            {
                string oldEndPoint = order.EndPointCity + ", " + order.EndPointStreet;

                order.EndPointCity = model.EndPointCity;
                order.EndPointStreet = model.EndPointStreet;

                unitOfWork.Orders.Update(order);
                await unitOfWork.SaveAsync();
                
                if (model.SendEmail)
                {
                    EmailSender emailSender = new EmailSender();
                    await emailSender.EndPointChangeSend(order, oldEndPoint);
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        private bool AddToResult(FindOrderView model, Order order, List<OrderStatus> statuses)
        {
            bool addToResult = true;

            if (model.Id == null && model.CustomerName == null && model.CustomerSurname == null &&
                        model.CustomerEmail == null && model.EndPointCity == null &&
                        model.OrderDate == null && statuses.Count == 0)
            {
                addToResult = false;
            }

            if (model.Id != null && order.Id != model.Id)
            {
                addToResult = false;
            }

            if (model.CustomerName != null && order.Customer.FirstName != model.CustomerName)
            {
                addToResult = false;
            }

            if (model.CustomerSurname != null && order.Customer.SecondName != model.CustomerSurname)
            {
                addToResult = false;
            }

            if (model.EndPointCity != null && order.EndPointCity != model.EndPointCity)
            {
                addToResult = false;
            }

            if (model.CustomerEmail != null && order.Customer.Email != model.CustomerEmail)
            {
                addToResult = false;
            }

            if (model.OrderDate != null && ((DateTime)model.OrderDate).Date != order.OrderDate.Date)
            {
                addToResult = false;
            }

            if (statuses.Count > 0 && !statuses.Contains(order.OrderStatus))
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
