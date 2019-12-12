using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Helpers;
using Store.ViewModels;

namespace Store.Controllers
{
    [Authorize(Roles = "admin")]
    public class StorageController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public StorageController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(unitOfWork.Storages.GetAll().ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStorageView model)
        {
            if (ModelState.IsValid)
            {
                Storage storage = new Storage
                {
                    City = model.City,
                    Street = model.Street,
                };

                await unitOfWork.Storages.Create(storage);
                await unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Storage");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Storage storage = await unitOfWork.Storages.Get(id);

            if (storage == null)
            {
                ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "StorageIsNotFounded");

                return View("ErrorPage");
            }

            EditSorageView model = new EditSorageView
            {
                Id = storage.Id,
                City = storage.City,
                Street = storage.Street,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditSorageView model)
        {
            if (ModelState.IsValid)
            {
                Storage storage = await unitOfWork.Storages.Get(model.Id);

                if (storage != null)
                {
                    storage.City = model.City;
                    storage.Street = model.Street;

                    unitOfWork.Storages.Update(storage);
                    await unitOfWork.SaveAsync();
                }
            }

            return RedirectToAction("Index", "Storage");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Storage storage = await unitOfWork.Storages.Get(id);

            if (storage != null)
            {
                await unitOfWork.Storages.Delete(id);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]

        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Find(FindStorageView model)
        {
            List<Storage> storages = new List<Storage>();

            if (ModelState.IsValid)
            {
                var allStorages = unitOfWork.Storages.GetAll().ToList();

                foreach (var storage in allStorages)
                {
                    if (this.AddToResult(model, storage))
                    {
                        storages.Add(storage);
                    }
                }

                HttpContext.Session.Set("list", storages);

                return RedirectToAction("FindResult", "Storage");
            }

            return View(model);
        }

        public IActionResult FindResult()
        {
            var storages = HttpContext.Session.Get<List<Storage>>("list");

            if (storages == null)
            {
                return RedirectToAction("Find");
            }

            return View(storages);
        }

        public async Task<ActionResult> ShowGoods(int id)
        {
            Storage storage = await unitOfWork.Storages.Get(id);
            List<Good> storageGoods = new List<Good>();

            foreach (var good in storage.Products)
            {
                storageGoods.Add(await unitOfWork.Goods.Get(good.GoodId));
            }

            ViewBag.NameOfStorage = string.Join(", ", storage.City, storage.Street);

            return View(storageGoods);
        }

        private bool AddToResult(FindStorageView model, Storage storage)
        {
            bool addToResult = true;

            if (model.City == null && model.Street == null)
            {
                addToResult = false;
            }

            if (model.City != null && storage.City != model.City)
            {
                addToResult = false;
            }

            if (model.Street != null && storage.Street != model.Street)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
