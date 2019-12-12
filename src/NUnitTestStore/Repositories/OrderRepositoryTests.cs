using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork.Classes;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace NUnitTestStore.Repositories
{
    [TestFixture]
    class OrderRepositoryTests
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
            var orders = new List<Order> { new Order{ Id = 1, }, new Order { Id = 2 }, new Order { Id = 3 } };
            using (var context = new AppDbContext(options))
            {
                var repo = new OrderRepository(context);

                //Act
                foreach (var order in orders)
                {
                    context.Orders.Add(order);
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
            var order = new Order { Id = 1};
            using (var context = new AppDbContext(options))
            {
                var repo = new OrderRepository(context);

                //Act
                await repo.Create(order);
                var expectedResult = 1;
                //Assert
                Assert.AreEqual(expectedResult, context.Orders.Local.Count());
            }
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            var order = new Order { Id = 1};
            using (var context = new AppDbContext(options))
            {
                var repo = new OrderRepository(context);

                //Act
                var expectedResult = 0;
                context.Orders.Add(order);
                var countAfterAdding = context.Orders.Local.Count();
                await repo.Delete(order.Id);

                //Assert
                Assert.AreEqual(expectedResult, context.Orders.Local.Count());
                Assert.AreEqual(1, countAfterAdding);
            }
        }

        [Test]
        public async Task GetTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var order = new Order { Id = 1 };
                var repo = new OrderRepository(context);

                //Act
                var expectedResult = order;
                context.Orders.Add(order);
                var result = await repo.Get(order.Id);

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
                var order = new Order { Id = 1};
                var repo = new OrderRepository(context);

                //Act
                repo.Update(order);
                var expectedResult = EntityState.Modified;
                var actualResult = context.Entry(order).State;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public void SetProducts()
        {
            using (var context = new AppDbContext(options))
            {
                var order = new Order { Id = 1 };
                var repo = new OrderRepository(context);
                context.Orders.Add(order);
                var goods = new List<Good>
                {
                    new Good {Id = 1, Name = "test1"},
                    new Good {Id = 2, Name = "test2"}
                };
                //Act
                repo.SetProducts(goods, order);
                var result = order.Products;
                var expectedResult = 2;
                //Assert
                Assert.AreEqual(expectedResult, result.Count);
            }
        }
    }
}
