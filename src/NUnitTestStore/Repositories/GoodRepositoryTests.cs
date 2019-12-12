using DAL.Models;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork;
using DAL.Classes.UnitOfWork.Classes;
using Microsoft.EntityFrameworkCore;
using Moq;
using DAL.Classes.UnitOfWork.Interfaces;
using System.Collections.Generic;
using System;

namespace NUnitTestStore.Repositories
{
    [TestFixture]
    class GoodRepositoryTests
    {
        DbContextOptions<AppDbContext> options;
        [SetUp]
        public void Setup()
        {
            options= new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "InMemoryDatabase").Options;
        }
        
        [Test]
        public async Task CreateTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                await repo.Create(good);
                var expectedResult = 1;
                var actualResult = context.Goods.Local.Count();

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
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                var expectedResult = 0;
                context.Goods.Add(good);
                await repo.Delete(good.Id);
                var actualResult = context.Goods.Local.Count();

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
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                var expectedResult = good;
                context.Goods.Add(good);
                var result = await repo.Get(good.Id);

                //Assert
                Assert.AreEqual(expectedResult, result);
            }
        }

        [Test]
        public void GetAllTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var goods = new List<Good> { new Good { Id = 1, }, new Good { Id = 2 }, new Good { Id = 3 } };
                var repo = new GoodRepository(context);

                //Act
                foreach (var good in goods)
                {
                    context.Goods.Add(good);
                }
                var actualResult = repo.GetAll();
                var expectedResult = context.Goods;

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
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                repo.Update(good);
                var expectedResult = EntityState.Modified;
                var actualResult = context.Entry(good).State;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public async Task AddGoodToStorageTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var storages = new List<Storage> { new Storage { Id = 1, }, new Storage { Id = 2 },
                    new Storage { Id = 3 } };
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                await repo.Create(good);
                await repo.AddGoodToStorage(good.Id, storages);
                var actualResult = context.GoodStorage.Local.Count();
                var expectedResult = 3;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public async Task GetReviews()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var reviews = new List<GoodReview> { new GoodReview { Id = 1, GoodId = 1 },
                    new GoodReview { Id = 2, GoodId = 1 }, new GoodReview { Id = 3, GoodId = 1 } };
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                await repo.Create(good);
                foreach (var review in reviews)
                {
                    context.Reviews.Add(review);
                    context.SaveChanges();
                }
                var actualResult = repo.GetReviews(good.Id).Count();
                var expectedResult = 3;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public async Task AddReview()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var review = new GoodReview {  Id = 1, GoodId = 1 };
                var good = new Good { Id = 1, Name = "Test" };
                var repo = new GoodRepository(context);

                //Act
                await repo.Create(good);
                context.Reviews.Add(review);
                await repo.AddReview(review, good);
                var actualResult = context.Reviews.Find(review.Id);
                var expectedResult = review;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
                Assert.AreEqual(good.Reviews, new List<GoodReview> { review });
            }
        }

        [Test]
        public async Task GetGoodStoragesTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var good = new Good { Id = 1, Name = "Test" };
                var storages = new List<GoodStorage> { new GoodStorage { GoodId = good.Id }};
                var repo = new GoodRepository(context);

                //Act
                foreach (var storage in storages)
                {
                    context.GoodStorage.Add(storage);
                    context.SaveChanges();
                }
                var actualResult = await repo.GetGoodStorages(good.Id);
                var expectedResult = new List<Storage> { new Storage { Products = storages } };

                //Assert
                Assert.AreEqual(expectedResult.Count, actualResult.Count);
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new AppDbContext(options))
            {
                context.Reviews.RemoveRange(context.Reviews);
                context.SaveChanges();
            }
        }
    }
}
