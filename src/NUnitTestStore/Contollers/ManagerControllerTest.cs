using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using Store.Controllers;
using Store.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnitTestStore.Controllers
{
    [TestFixture]
    public class MangerControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private ManagerController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new ManagerController(context);
        }

        [Test]
        public void Index_Valid_Data()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1 },
                new Order { Id = 2 },
                new Order { Id = 3 },
            };

            foreach (var order in orders)
            {
                context.Orders.Add(order);
            }
            context.SaveChanges();

            // Act
            var actualResult = (controller.Index() as ViewResult).Model;

            // Assert
            Assert.IsAssignableFrom<List<Order>>(actualResult);
        }

        [Test]
        public async Task Show_All_Goods_Of_Order()
        {
            // Arrange
            var good = new Good() { Id = 1, Name = "Iphone 7" };
            var order = new Order() { Id = 1, OrderDate = DateTime.Now };

            // Act
            context.GoodOrder.Add(new GoodOrder { Good = good, Order = order });
            context.SaveChanges();
            var result = await controller.ShowGoods(order.Id) as ViewResult;
            var goods = (List<Good>)result.ViewData.Model;

            // Assert
            Assert.AreEqual(1, goods.Count);
            Assert.AreEqual(good.Name, goods.First().Name);
        }

        [Test]
        public async Task Show_Customer_Of_Order()
        {
            // Arrange
            var customer = new Customer() { Id = 1, FirstName = "Petro" };
            var order = new Order() { Id = 1, OrderDate = DateTime.Now, Customer = customer };

            // Act
            context.Orders.Add(order);
            context.SaveChanges();
            var result = await controller.ShowCustomer(context.Orders.First().Id) as ViewResult;
            var customerResult = (Customer)result.ViewData.Model;

            // Assert
            Assert.IsNotNull(customerResult);
            Assert.AreEqual(customerResult.FirstName, customer.FirstName);
        }

        [Test]
        public async Task Edit_End_Point_Of_Order()
        {
            // Arrange
            string oldEndPointCity = "Lviv";
            var order = new Order() { Id = 1, EndPointCity = oldEndPointCity };
            var model = new EditOrderView() { EndPointCity = "Kiev", EndPointStreet = "Upa, 45" };

            // Act
            context.Orders.Add(order);
            context.SaveChanges();
            model.Id = context.Orders.First().Id;
            await controller.Edit(model);

            // Assert
            Assert.AreEqual(1, context.Orders.Count());
            Assert.AreEqual(model.EndPointCity, context.Orders.Single().EndPointCity);
            Assert.AreNotEqual(model.EndPointCity, oldEndPointCity);
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.GoodOrder.RemoveRange(context.GoodOrder);
            context.Orders.RemoveRange(context.Orders);
            context.GoodOrder.RemoveRange(context.GoodOrder);
            context.Goods.RemoveRange(context.Goods);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}