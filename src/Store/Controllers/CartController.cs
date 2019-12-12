using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Helpers;
using Store.ViewModels;

namespace Store.Controllers
{
    public class CartController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public CartController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
            errorMessage = new ErrorMessage();
        }

        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<IActionResult> ShowCart()
        {
            int customerId = unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name)
                .FirstOrDefault().Id;
            Customer customer = await unitOfWork.Customers.Get(customerId);
            var goodCarts = customer.Cart.Goods;
            var goods = new List<OrderPart>();

            foreach (var good in customer.Cart.Goods)
            {
                good.Good = await unitOfWork.Goods.Get(good.GoodId);
                goods.Add(new OrderPart { Good = good.Good, Count = 1 });
            }

            ViewBag.CommonPrice = Convert.ToInt32(goods.Sum(g => g.Good.Price));
            HttpContext.Session.Set("goodsConfirm", goods);

            return View(goods);
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            Good good = await unitOfWork.Goods.Get(id);
            int customerId = unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name).First().Id;
            Customer customer = await unitOfWork.Customers.Get(customerId);

            if (good != null && customer != null)
            {
                unitOfWork.Customers.AddToCart(good, customer);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("ShowGood", "GoodPage", new { goodId = id });
        }

        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            Good good = await unitOfWork.Goods.Get(id);
            int customerId = unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name).First().Id;
            Customer customer = await unitOfWork.Customers.Get(customerId);

            if (good != null && customer != null)
            {
                unitOfWork.Customers.RemoveFromCart(good, customer);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("ShowCart", "Cart");
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> ConfirmGoods(List<string> goodCount)
        {
            int customerId = unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name).First().Id;
            Customer customer = await unitOfWork.Customers.Get(customerId);
            var goods = HttpContext.Session.Get<List<OrderPart>>("goodsConfirm");
            var modelGoods = new List<OrderPart>();

            if (goods == null)
            {
                return RedirectToAction("ShowCart", "Cart");
            }

            for (int i = 0; i < goodCount.Count; i++)
            {
                if (goodCount[i] == "0")
                {
                    continue;
                }

                modelGoods.Add(new OrderPart
                {
                    Good = goods[i].Good,
                    Count = Convert.ToInt32(goodCount[i]),
                    GoodId = goods[i].Good.Id
                });
            }

            if (modelGoods.Count != 0)
            {
                ConfirmOrderView model = new ConfirmOrderView
                {
                    Country = new Country(),
                    Customer = customer,
                    Goods = modelGoods,
                    Storages = await this.GetStorages(modelGoods),
                    Count = Convert.ToInt32(Request.Form["goodCommonCount"]),
                    CommonPrice = Convert.ToInt32(Request.Form["commonPrice"])
                };

                HttpContext.Session.Set("orderConfirm", model);

                return View("ConfirmOrder", model);
            }
            else
            {
                ViewBag.Message = "You cannot buy nothing!";
                return View("ErrorPage");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(ConfirmOrderView formModel)
        {
            ConfirmOrderView model = HttpContext.Session.Get<ConfirmOrderView>("orderConfirm");

            if (model == null)
            {
                return RedirectToAction("ShowCart", "Cart");
            }

            var date = DateTime.Now;

            Order order = new Order
            {
                OrderDate = date,
                CustomerId = model.Customer.Id,
                OrderStatus = OrderStatus.Ordered,
                EndPointCity = Request.Form["EndPointCity"],
                EndPointStreet = formModel.EndPointStreet,
                CommonPrice = Convert.ToInt32(Request.Form["commonPrice"])
            };

            unitOfWork.Orders.SetProducts(await GetGoodsAsync(model.Goods), order);
            await unitOfWork.Orders.Create(order);
            
            foreach (var good in await GetGoodsAsync(model.Goods))
            {
                await this.RemoveFromCart(good.Id);
            }
            await unitOfWork.SaveAsync();

            return RedirectToAction("ShowCart");
        }

        private async Task<List<Storage>> GetStorages(List<OrderPart> orderParts)
        {
            List<Storage> storages = await unitOfWork.Goods.GetGoodStorages(orderParts[0].GoodId);

            if (storages.Count > 1)
            {
                foreach (var part in orderParts)
                {
                    if (storages.Intersect(await unitOfWork.Goods.GetGoodStorages(part.GoodId)).Count() == 0)
                    {
                        return new List<Storage>();
                    }
                    else
                    {
                        storages = storages.Intersect(await unitOfWork.Goods.GetGoodStorages(part.GoodId))
                            .ToList();
                    }
                }
            }

            return storages.Distinct().ToList();
        }

        private async Task<List<Good>> GetGoodsAsync(List<OrderPart> orderParts)
        {
            List<Good> goods = new List<Good>();

            foreach (var part in orderParts)
            {
                var good = await unitOfWork.Goods.Get(part.Good.Id);
                goods.Add(good);
            }

            return goods;
        }
    }
}
