using DAL.Classes;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Store.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTestStore.Controllers
{
    class UserControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private UserController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new UserController(context);
        }

        [Test]
        public async Task Cancel_Order_Test()
        {
            // Arrange
            var order = new Order() { Id = 1, OrderDate = DateTime.Now };

            // Act
            context.Orders.Add(order);
            context.SaveChanges();
            await controller.CancelOrder(context.Orders.First().Id);

            // Assert
            Assert.AreEqual(1, context.Orders.Count());
            Assert.AreEqual(context.Orders.First().OrderStatus, OrderStatus.Cancelled);
        }

        [Test]
        public async Task Delete_Order_Test()
        {
            // Arrange
            var order = new Order() { Id = 1, OrderDate = DateTime.Now };

            // Act
            context.Orders.Add(order);
            context.SaveChanges();
            await controller.DeleteOrder(context.Orders.First().Id);

            // Assert
            Assert.AreEqual(0, context.Orders.Count());
        }

        [Test]
        public async Task Show_Goods_In_Specified_Order()
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

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Orders.RemoveRange(context.Orders);
            context.GoodOrder.RemoveRange(context.GoodOrder);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}
