using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using Store.Controllers;
using Store.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnitTestStore.Controllers
{
    [TestFixture]
    public class StorageControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private StorageController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);
            controller = new StorageController(context);
        }

        [Test]
        public void Index_Change_Data()
        {
            // Arrange
            var storages = new List<Storage>
            {
                new Storage { City = "Lviv", },
                new Storage { City = "Kiev", },
                new Storage { City = "Kharkiv" },
            };

            foreach (var storage in storages)
            {
                context.Storages.Add(storage);
            }
            context.SaveChanges();

            // Act
            controller.Index();

            // Assert
            Assert.AreEqual(storages.Count, context.Storages.Count());
            Assert.AreEqual(storages[0].Id, context.Storages.First().Id);
        }

        [Test]
        public async Task Add_Storage_To_DataBase()
        {
            // Arrange
            var storage = new Storage() { Id = 1, City = "Lviv", Street = "Rubchaka, 56" };
            var model = new CreateStorageView() { City = storage.City, Street = storage.Street };

            // Act
            await controller.Create(model);

            // Assert
            Assert.AreEqual(1, context.Storages.Count());
            Assert.AreEqual(model.City, context.Storages.Single().City);
        }

        [Test]
        public async Task Delete_Storage_From_Database()
        {
            // Arrange
            var storage = new Storage() { Id = 1, City = "Lviv", Street = "Rubchaka, 56" };
            var model = new CreateStorageView() { City = storage.City, Street = storage.Street };

            // Act
            await controller.Create(model);
            await controller.Delete(context.Storages.First().Id);

            // Assert
            Assert.AreEqual(0, context.Storages.Count());
        }

        [Test]
        public async Task Edit_Some_Storage()
        {
            // Arrange
            var storage = new Storage() { Id = 1, City = "Lviv", Street = "Rubchaka, 56" };
            var model = new CreateStorageView() { City = storage.City, Street = storage.Street };
            var editModel = new EditSorageView() { City = "Kiev", Street = storage.Street };

            // Act
            await controller.Create(model);
            editModel.Id = context.Storages.First().Id;
            await controller.Edit(editModel);

            // Assert
            Assert.AreEqual(1, context.Storages.Count());
            Assert.AreEqual(model.Street, context.Storages.Single().Street);
            Assert.AreNotEqual(editModel.City, model.City);
            Assert.AreEqual(editModel.City, context.Storages.Single().City);
        }

        [Test]
        public async Task Show_Goods_In_Specified_Storage()
        {
            // Arrange
            var good = new Good() { Id = 1, Name = "Iphone 7" };
            var storage = new Storage() { Id = 1, City = "Lviv", Street = "Rubchaka, 56" };

            // Act
            context.GoodStorage.Add(new GoodStorage { Good = good, Storage = storage });
            context.SaveChanges();
            var result = await controller.ShowGoods(storage.Id) as ViewResult;
            var goods = (List<Good>)result.ViewData.Model;

            // Assert
            Assert.AreEqual(1, goods.Count);
            Assert.AreEqual(good.Name, goods.First().Name);
        }

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Storages.RemoveRange(context.Storages);
            context.Goods.RemoveRange(context.Goods);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}