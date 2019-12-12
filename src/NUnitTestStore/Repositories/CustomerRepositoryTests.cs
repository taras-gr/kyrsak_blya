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
    public class CustomerRepositoryTests
    {
        DbContextOptions<AppDbContext> options;
        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Customers.RemoveRange(context.Customers);
            context.SaveChanges();
        }

        [Test]
        public void GetAllTest()
        {
            //Arrange
            var customers = new List<Customer>
            { new Customer { Id = 1, },
                new Customer { Id = 2 },
                new Customer { Id = 3 },
            };
            using (var context = new AppDbContext(options))
            {
                var repo = new CustomerRepository(context);

                //Act
                foreach (var customer in customers)
                {
                    context.Customers.Add(customer);
                }
                var result = repo.GetAll();

                //Assert
                Assert.AreEqual(context.Customers, result);
            }
        }

        [Test]
        public async Task CreateTest()
        {
            //Arrange
            var customer = new Customer { Id = 1 };
            using (var context = new AppDbContext(options))
            {
                var repo = new CustomerRepository(context);

                //Act
                await repo.Create(customer);
                var expectedResult = 1;
                //Assert
                Assert.AreEqual(expectedResult, context.Customers.Local.Count());
            }
        }

        [Test]
        public async Task DeleteTest()
        {
            //Arrange
            var customer = new Customer { Id = 1 };
            var cart = new Cart{ CustomerId = customer.Id };
            using (var context = new AppDbContext(options))
            {
                var repo = new CustomerRepository(context);

                //Act
                var expectedResult = 0;
                context.Add(customer);
                context.Add(cart);
                context.SaveChanges();
                var countAfterAdding = context.Customers.Count();
                await repo.Delete(customer.Id);

                //Assert
                Assert.AreEqual(expectedResult, context.Customers.Local.Count());
                Assert.AreEqual(1, countAfterAdding);
            }
        }

        [Test]
        public async Task GetTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var customer = new Customer { Id = 1 };
                var repo = new CustomerRepository(context);
                //Act
                var expectedResult = customer;
                await repo.Create(customer);

                var result = repo.Get(customer.Id);

                //Assert
                Assert.AreEqual(expectedResult.Id, result.Id);
            }
        }

        [Test]
        public void UpdateTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var customer = new Customer { Id = 1 };
                var repo = new CustomerRepository(context);

                //Act
                repo.Update(customer);
                var expectedResult = EntityState.Modified;
                var actualResult = context.Entry(customer).State;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        [Test]
        public void AddToCartTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var customer = new Customer { Id = 1 };
                var repo = new CustomerRepository(context);

                customer.Cart = new Cart();
                customer.Cart.Goods = new List<GoodCart>();
                Good good = new Good {Id = 1, Name = "test"};
                //Act
                repo.AddToCart(good, customer);
                var expectedResult = 1;
                var actualResult = customer.Cart.Goods.Count;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);

            }
        }

        [Test]
        public void RemoveFromCartTest()
        {
            //Arrange
            using (var context = new AppDbContext(options))
            {
                var customer = new Customer { Id = 1 };
                var repo = new CustomerRepository(context);

                customer.Cart = new Cart();
                Good good = new Good { Id = 1, Name = "test" };
                customer.Cart.Goods = new List<GoodCart>
                {
                    new GoodCart
                        {
                            Good = good,
                            GoodId = good.Id,
                            Cart = customer.Cart,
                            CartId = customer.Cart.Id
                        }
                };

                //Act
                repo.RemoveFromCart(good, customer);
                var expectedResult = 0;
                var actualResult = customer.Cart.Goods.Count;

                //Assert
                Assert.AreEqual(expectedResult, actualResult);
            }
        }
    }
}
