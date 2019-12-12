using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using Store.Controllers;
using Store.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork;
using Moq;

namespace NUnitTestStore.Controllers
{
    [TestFixture]
    class GoodControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private GoodController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new GoodController(context);
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Goods.RemoveRange(context.Goods);
            context.GoodOrder.RemoveRange(context.GoodOrder);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }

        [Test]
        public void Index_Valid_Data()
        {
            //Arrange
            var goods = new List<Good>
            {
                new Good {Id = 1, ProducerId = 1, Count = 21},
                new Good {Id = 2, ProducerId = 1, Count = 21},
                new Good {Id = 3, ProducerId = 1, Count = 21}
            };

            var producer = new Producer {Id = 1};

            foreach (var good in goods)
            {
                context.Add(good);
            }

            context.Add(producer);
            context.SaveChanges();

            //Act
            var actualResult = (controller.Index() as ViewResult).Model;

            //Assert
            Assert.IsAssignableFrom<List<Good>>(actualResult);
            Assert.AreEqual(goods.Count, (actualResult as List<Good>).Count);
        }

        [Test]
        public void Create_Takes_Valid_Data()
        {
            //Arrange
            var goods = new List<Good>
            {
                new Good {Id = 1, ProducerId = 1},
                new Good {Id = 2, ProducerId = 1},
                new Good {Id = 3, ProducerId = 1}
            };

            var producer = new Producer {Id = 1};

            foreach (var good in goods)
            {
                context.Add(good);
            }

            context.Add(producer);
            context.SaveChanges();

            //Act
            var actualResult = (controller.Create() as ViewResult).Model;

            //Assert
            Assert.IsAssignableFrom<CreateGoodView>(actualResult);
        }
    }
}
    
