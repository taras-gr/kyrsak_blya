using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Controllers;
using Store.ViewModels;

namespace NUnitTestStore.Contollers
{
    [TestFixture]
    public class ProducerControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private ProducerController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new ProducerController(context);
        }

        [Test]
        public void Index_Valid_Data()
        {
            //Arrange
            var producer = new Producer { Id = 1 };

            context.Add(producer);
            context.SaveChanges();

            //Act
            var actualResult = (controller.Index() as ViewResult).Model;

            //Assert
            Assert.IsAssignableFrom<List<Producer>>(actualResult);

        }

        [Test]
        public async Task Create_Valid_Data()
        {
            //Arrange

            var producesView = new CreateProducerView
            {
                Name = "Ilon",
                Phone = "+380343434394",
                Email = "lol@mail.com",
                WebSite = "https://lol.com"
            };



            //Act
            var actualResult = (await controller.Create(producesView) as RedirectToActionResult).ActionName;
            var expectedResult = "Index";

            //Assert
            Assert.AreEqual(actualResult, expectedResult);

        }

        [Test]
        public async Task Edit_Valid_Data()
        {
            //Arrange
            var producer = new Producer { Id = 1 };

            context.Add(producer);
            context.SaveChanges();

            var producesView = new EditProducerView
            {
                Id = 1,
                Name = "Ilon",
                Phone = "+380343434394",
                Email = "lol@mail.com",
                WebSite = "https://lol.com"
            };



            //Act
            var actualResult = (await controller.Edit(producesView) as RedirectToActionResult).ActionName;
            var expectedResult = "Index";

            //Assert
            Assert.AreEqual(actualResult, expectedResult);

        }

        [Test]
        public async Task Delete_Valid_Data()
        {
            //Arrange

            var producer = new Producer
            {
                Id = 1,
                Name = "Ilon",
                Phone = "+380343434394",
                Email = "lol@mail.com",
                WebSite = "https://lol.com"
            };

            context.Add(producer);
            context.SaveChanges();


            //Act
            var actualResult = (await controller.Delete(producer.Id) as RedirectToActionResult).ActionName;
            var expectedResult = "Index";

            //Assert
            Assert.AreEqual(actualResult, expectedResult);

        }

        [Test]
        public async Task Show_Goods_Valid_Data()
        {
            //Arrange
            var producer = new Producer { Id = 1 };
            producer.Products = new List<Good>();
            producer.Products.Add(new Good {Id = 1});
            context.Add(producer);

            context.SaveChanges();

            //Act
            var actualResult = (await controller.ShowGoods(producer.Id) as ViewResult).Model;

            //Assert
            Assert.IsAssignableFrom<List<Good>>(actualResult);
        }


        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.GoodOrder.RemoveRange(context.GoodOrder);
            context.Orders.RemoveRange(context.Orders);
            context.Goods.RemoveRange(context.Goods);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}
