using System;
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
    public class GoodController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public GoodController(AppDbContext appDbContext)
        { 
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var goods = unitOfWork.Goods.GetAll().ToList();

            return View(goods);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CreateGoodView model = new CreateGoodView
            {
                Producers = unitOfWork.Producers.GetAll().ToList(),
                Storages = unitOfWork.Storages.GetAll().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGoodView model, List<string> storages)
        {
            if (ModelState.IsValid)
            {
                Good good = new Good
                {
                    Name = model.Name,
                    Specification = model.Specification,
                    PhotoUrl = model.PhotoUrl,
                    YearOfManufacture = model.YearOfManufacture,
                    WarrantyTerm = model.WarrantyTerm,
                    Producer = unitOfWork.Producers.GetAll().Where(p => p.Name == Request.Form["producerSelect"]).First(),
                    Price = model.Price,
                    Type = model.Type,
                    Count = model.Count,
                };

                if (storages.Count > 0)
                {
                    foreach (var storage in storages)
                    {
                        Storage tempStorage = unitOfWork.Storages.GetAll().Where(s => s.Street == storage).First();
                        good.Storages.Add(new GoodStorage() { Good = good, Storage = tempStorage });
                    }
                }

                await unitOfWork.Goods.Create(good);
                await unitOfWork.SaveAsync();
                HttpContext.Session.Set("goods", unitOfWork.Goods.GetAll().ToList());

                return RedirectToAction("Index", "Good");

            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Good good = await unitOfWork.Goods.Get(id);
            List<Storage> goodStorages = new List<Storage>(); 

            if (good == null)
            {
                ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "GoodIsNotFounded");

                return View("ErrorPage");
            }

            foreach (var storage in good.Storages)
            {
                if (storage.GoodId == good.Id)
                {
                    goodStorages.Add(await unitOfWork.Storages.Get(storage.StorageId));
                }
            }

            EditGoodView model = new EditGoodView
            {
                Id = good.Id,
                Name = good.Name,
                Specification = good.Specification,
                PhotoUrl = good.PhotoUrl,
                YearOfManufacture = good.YearOfManufacture,
                WarrantyTerm = good.WarrantyTerm,
                Price = Convert.ToInt32(good.Price),
                Type = good.Type,
                Count = good.Count,
                Producer = await unitOfWork.Producers.Get(good.ProducerId),
                Producers = unitOfWork.Producers.GetAll().ToList(),
                AllStorages = unitOfWork.Storages.GetAll().ToList(),
                Storages = goodStorages
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditGoodView model, List<int> storagesParameter)
        {
            if (ModelState.IsValid)
            {
                Good good = await unitOfWork.Goods.Get(model.Id);

                if (good != null)
                {
                    good.Name = model.Name;
                    good.Specification = model.Specification;
                    good.PhotoUrl = model.PhotoUrl;
                    good.YearOfManufacture = model.YearOfManufacture;
                    good.WarrantyTerm = model.WarrantyTerm;
                    good.Price = model.Price;
                    good.Type = model.Type;
                    good.Count = model.Count;
                    good.Producer = unitOfWork.Producers.GetAll()
                        .Where(p => p.Name == Request.Form["producerSelect"]).First();

                    List<Storage> goodStorages = new List<Storage>();

                    foreach (var storage in good.Storages)
                    {
                        if (storage.GoodId == good.Id)
                        {
                            goodStorages.Add(await unitOfWork.Storages.Get(storage.StorageId));
                        }
                    }

                    List<Storage> checkedStorages = new List<Storage>();
                    var storages = unitOfWork.Storages.GetAll().ToList();

                    for (var i = 0; i < storagesParameter.Count; i++)
                    {
                        var item = storages.Where(s => s.Id == storagesParameter[i])
                            .FirstOrDefault();

                        if (item != null)
                        {
                            checkedStorages.Add(item);
                        }
                    }

                    var addedStorages = checkedStorages.Except(goodStorages).ToList();
                    var removedStorage = goodStorages.Except(checkedStorages).ToList();

                    await unitOfWork.Goods.DeleteGoodFromStorage(good.Id, removedStorage);
                    await unitOfWork.Goods.AddGoodToStorage(good.Id, addedStorages);

                    unitOfWork.Goods.Update(good);
                    await unitOfWork.SaveAsync();
                    HttpContext.Session.Set("goods", unitOfWork.Goods.GetAll().ToList());

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Good good = await unitOfWork.Goods.Get(id);

            if ( good != null)
            {
                await unitOfWork.Goods.Delete(id);
                await unitOfWork.SaveAsync();

                HttpContext.Session.Set("goods", unitOfWork.Goods.GetAll().ToList());
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Find(FindGoodView model)
        {
            List<Good> goods = new List<Good>();

            if (ModelState.IsValid)
            {
                var allGoods = unitOfWork.Goods.GetAll().ToList();

                foreach (var good in allGoods)
                {
                    if (this.AddToResult(model, good))
                    {
                        goods.Add(good);
                    }
                }

                HttpContext.Session.Set("list", goods);

                return RedirectToAction("FindResult", "Good");
            }

            return View(model);
        }
        

        public IActionResult FindResult()
        {
            var goods = HttpContext.Session.Get<List<Good>>("list");

            if (goods == null)
            {
                return RedirectToAction("Find");
            }

            return View(goods);
        }

        private bool AddToResult(FindGoodView model, Good good)
        {
            bool addToResult = true;

            if (model.Name == null && model.ProducerName == null &&
            model.EndPrice - model.StartPrice == 0 && model.YearOfManufacture == 0 && model.Type == null)
            {
                addToResult = false;
            }

            if (model.Name != null && good.Name != model.Name)
            {
                addToResult = false;
            }

            if (model.YearOfManufacture != null && good.YearOfManufacture != model.YearOfManufacture)
            {
                addToResult = false;
            }

            if (model.ProducerName != null && good.Producer.Name != model.ProducerName)
            {
                addToResult = false;
            }

            if (model.EndPrice - model.StartPrice != 0 && good.Price < model.StartPrice ||
                good.Price > model.EndPrice)
            {
                addToResult = false;
            }

            if (model.Type != null && good.Type != model.Type)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
