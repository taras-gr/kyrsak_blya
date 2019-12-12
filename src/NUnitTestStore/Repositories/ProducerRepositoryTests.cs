using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork.Classes;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace NUnitTestStore.Repositories
{
    [TestFixture]
    public class ProducerRepositoryTests
    {
        DbContextOptions<AppDbContext> options;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
        }

        [Test]
        public void GetAllTest()
        {
            //Arrange
            var producers = new List<Producer>
                { new Producer { Id = 1, },
                    new Producer { Id = 2 },
                    new Producer { Id = 3 },
                };
            using (var context = new AppDbContext(options))
            {
                var repo = new ProducerRepository(context);

                //Act
                foreach (var producer in producers)
                {
                    context.Producers.Add(producer);
                }
                var result = repo.GetAll();

                //Assert
                Assert.AreEqual(context.Orders, result);
            }
        }

        [Test]
        public async Task CreateTest()
        {
            //Arrange
            var producer = new Producer { Id = 1 };
            using (var context = new AppDbContext(options))
            {
                var repo = new ProducerRepository(context);

                //Act
                await repo.Create(producer);
                var expectedResult = 1;
                //Assert
                Assert.AreEqual(expectedResult, context.Producers.Local.Count());
            }
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            var producer = new Producer { Id = 1 };
            using (var context = new AppDbContext(options))
            {
                var repo = new ProducerRepository(context);

                //Act
                var expectedResult = 0;
                context.Producers.Add(producer);
                var countAfterAdding = context.Producers.Local.Count();
                await repo.Delete(producer.Id);

                //Assert
                Assert.AreEqual(expectedResult, context.Producers.Local.Count());
                Assert.AreEqual(1, countAfterAdding);
            }
        }

        [Test]
        public async Task GetTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var producer = new Producer { Id = 1 };
                var repo = new ProducerRepository(context);

                //Act
                var expectedResult = producer;
                context.Producers.Add(producer);
                var result = await repo.Get(producer.Id);

                //Assert
                Assert.AreEqual(expectedResult, result);
            }
        }

        [Test]
        public void UpdateTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var producer = new Producer { Id = 1 };
                var repo = new ProducerRepository(context);

                //Act
                repo.Update(producer);
                var expectedResult = EntityState.Modified;
                var actualResult = context.Entry(producer).State;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }
    }
}
