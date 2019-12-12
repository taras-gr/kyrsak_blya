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
    public class ProducerController : Controller
    {
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public ProducerController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(unitOfWork.Producers.GetAll().ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult> Create(CreateProducerView model)
        {
            if (ModelState.IsValid)
            {
                Producer producer = new Producer
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Email = model.Email,
                    WebSite = model.WebSite
                };

                await unitOfWork.Producers.Create(producer);
                await unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Producer");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Producer producer = await unitOfWork.Producers.Get(id);
           
            if (producer == null)
            {
                ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "ProducerIsNotFounded");

                return View("ErrorPage");
            }

            EditProducerView model = new EditProducerView
            {
                Id = producer.Id,
                Name = producer.Name,
                Phone = producer.Phone,
                Email = producer.Email,
                WebSite = producer.WebSite
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProducerView model)
        {
            if (ModelState.IsValid)
            {
                Producer producer = await unitOfWork.Producers.Get(model.Id);
                
                if (producer != null)
                {
                    producer.Name = model.Name;
                    producer.Phone = model.Phone;
                    producer.Email = model.Email;
                    producer.WebSite = model.WebSite;

                    unitOfWork.Producers.Update(producer);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Producer producer = await unitOfWork.Producers.Get(id);

            if (producer != null)
            {
                await unitOfWork.Producers.Delete(id);
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
        public IActionResult Find(FindProducerView model)
        {
            List<Producer> producers = new List<Producer>();

            if (ModelState.IsValid)
            {
                var allProducers = unitOfWork.Producers.GetAll().ToList();

                foreach (var producer in allProducers)
                {
                    if (this.AddToResult(model, producer))
                    {
                        producers.Add(producer);
                    }
                }

                HttpContext.Session.Set("list", producers);

                return RedirectToAction("FindResult", "Producer");
            }

            return View(model);
        }

        public IActionResult FindResult()
        {
            var producers = HttpContext.Session.Get<List<Producer>>("list");

            if (producers == null)
            {
                return RedirectToAction("Find");
            }

            return View(producers);
        }

        [HttpGet]
        public async Task<ActionResult> ShowGoods(int id)
        {
            Producer producer = await unitOfWork.Producers.Get(id);

            ViewBag.ProducerName = producer.Name;

            return View(producer.Products);
        }

        private bool AddToResult(FindProducerView model, Producer producer)
        {
            bool addToResult = true;

            if (model.Name == null && model.Phone == null && model.Email == null && model.WebSite == null)
            {
                addToResult = false;
            }

            if (model.Name != null && producer.Name != model.Name)
            {
                addToResult = false;
            }

            if (model.Phone != null && producer.Phone != model.Phone)
            {
                addToResult = false;
            }

            if (model.Email != null && producer.Email != model.Email)
            {
                addToResult = false;
            }

            if (model.WebSite != null && producer.WebSite != model.WebSite)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}