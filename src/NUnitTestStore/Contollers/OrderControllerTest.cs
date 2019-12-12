using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using Store.Controllers;

namespace NUnitTestStore.Contollers
{
    class OrderControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private OrderController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new OrderController(context);
        }

        [Test]
        public void Index_Valid_Data()
        {
            //Arrange
            var orders = new List<Order>
            {
                new Order {Id = 1},
                new Order {Id = 2},
                new Order {Id = 3}
            };

            foreach (var order in orders)
            {
                context.Add(order);
            }
            context.SaveChanges();

            //Act
            var actualResult = (controller.Index() as ViewResult).Model;

            //Assert
            Assert.IsAssignableFrom<List<Order>>(actualResult);
            Assert.AreEqual(orders, actualResult);
        }

        [Test]
        public async Task Show_Goods_Valid_Data()
        {
            //Arrange
            var goods = new List<Good>
            {
                new Good {Id = 1, ProducerId = 1, Count = 21},
                new Good {Id = 2, ProducerId = 1, Count = 21},
                new Good {Id = 3, ProducerId = 1, Count = 21}
            };
            var goodOrders = new List<GoodOrder>
            {
                new GoodOrder {GoodId = 1},
                new GoodOrder {GoodId = 2},
                new GoodOrder {GoodId = 3}
            };
            var order = new Order { Id = 1, Products = goodOrders};
            foreach (var good in goods)
            {
                context.Add(good);
            }
            context.Add(order);
            context.SaveChanges();

            //Act
            var actualResult = (await controller.ShowGoods(order.Id) as ViewResult).Model;
            var expectedResult = goodOrders.Count;
            //Assert
            Assert.AreEqual(expectedResult, (actualResult as List<Good>).Count);
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Orders.RemoveRange(context.Orders);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}
