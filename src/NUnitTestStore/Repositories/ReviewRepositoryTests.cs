using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork.Classes;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace NUnitTestStore.Repositories
{
    [TestFixture]
    class ReviewRepositoryTests
    {
        DbContextOptions<AppDbContext> options;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "InMemoryDatabase").Options;
        }

        [Test]
        public async Task CreateTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var review = new GoodReview { Id = 1 };
                var repo = new ReviewRepository(context);

                //Act
                await repo.Create(review);
                var expectedResult = 1;
                var actualResult = context.Reviews.Local.Count;
                
                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var review = new GoodReview { Id = 1 };
                var repo = new ReviewRepository(context);

                //Act
                context.Add(review);
                await repo.Delete(review.Id);
                var actualResult = context.Reviews.Local.Count;
                var expectedResult = 0;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public async Task GetTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var review = new GoodReview { Id = 1 , CustomerId = 1};
                var customer = new Customer { Id = 1, FirstName = "Test" };
                var repo = new ReviewRepository(context);

                //Act
                context.Customers.Add(customer);
                context.Reviews.Add(review);
                context.SaveChanges();
                var actualResult = await repo.Get(review.Id);
                var expectedResult = review;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public void GetAllTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var reviews = new List<GoodReview> { new GoodReview { Id = 1, GoodId = 1 },
                    new GoodReview { Id = 2, GoodId = 1 }, new GoodReview { Id = 3, GoodId = 1 } };
                var repo = new ReviewRepository(context);

                //Act
                foreach (var review in reviews)
                {
                    context.Reviews.Add(review);
                }
                var actualResult = repo.GetAll();
                var expectedResult = context.Reviews;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public void UpdateTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var review = new GoodReview { Id = 1, CustomerId = 1 };
                var repo = new ReviewRepository(context);

                //Act
                repo.Update(review);
                var actualResult = context.Entry(review).State;
                var expectedResult = EntityState.Modified;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }
    }
}
