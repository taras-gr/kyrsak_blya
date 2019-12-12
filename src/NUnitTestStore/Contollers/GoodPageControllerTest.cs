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
    [TestFixture]
    class GoodPageControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private GoodPageController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new GoodPageController(context);
        }

        [Test]
        public async Task Show_Good_Test()
        {
            // Arrange
            var good = new Good() { Id = 1, Name = "Samsung S9" };

            // Act
            context.Goods.Add(good);
            context.SaveChanges();
            var result = await controller.ShowGood(context.Goods.First().Id) as ViewResult;
            var goodResult = (Good)result.ViewData.Model;

            // Assert
            Assert.IsNotNull(goodResult);
            Assert.AreEqual(good.Name, goodResult.Name);
        }

        [Test]
        public async Task Delete_Review_Test()
        {
            // Arrange
            var review = new GoodReview() { Message = "Test" };

            // Act
            context.Reviews.Add(review);
            context.SaveChanges();
            await controller.DeleteReview(context.Reviews.First().Id);

            // Assert
            Assert.AreEqual(0, context.Reviews.Count());
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Goods.RemoveRange(context.Goods);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}
