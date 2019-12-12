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
    class ReviewControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private ReviewController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new ReviewController(context);
        }

        [Test]
        public void Index_Valid_Data()
        {
            //Arrange
            var reviews = new List<GoodReview>
            {
                new GoodReview {Id = 1, IsVisibleForAll = true },
                new GoodReview {Id = 2, IsVisibleForAll = false },
                new GoodReview {Id = 3, IsVisibleForAll = true }
            };

            foreach (var review in reviews)
            {
                context.Add(review);
            }
            context.SaveChanges();

            //Act
            var result = (controller.Index() as ViewResult).Model;

            //Assert
            Assert.IsAssignableFrom<List<GoodReview>>(result);
            Assert.AreEqual(1, ((IEnumerable<GoodReview>)result).Count());
        }

        [Test]
        public async Task Delete_Review_From_Database()
        {
            // Arrange
            var review = new GoodReview() { Id = 1, Message = "Test" };

            // Act
            context.Reviews.Add(review);
            context.SaveChanges();
            await controller.Delete(context.Reviews.First().Id);

            // Assert
            Assert.AreEqual(0, context.Reviews.Count());
        }

        [Test]
        public async Task Confirm_Review_Test()
        {
            // Arrange
            var review = new GoodReview() { Id = 1, Message = "Test", IsVisibleForAll = false };

            // Act
            context.Reviews.Add(review);
            context.SaveChanges();
            await controller.Confirm(context.Reviews.First().Id);

            // Assert
            Assert.AreEqual(1, context.Reviews.Count());
            Assert.IsTrue(context.Reviews.First().IsVisibleForAll);
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Reviews.RemoveRange(context.Reviews);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}
