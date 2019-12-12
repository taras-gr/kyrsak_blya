using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Store.Controllers
{
    public class GoodPageController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public GoodPageController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        public async Task<IActionResult> ShowGood(int goodId)
        {
            Good good = await unitOfWork.Goods.Get(goodId);

            return View(good);
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> LeaveReview(int id, string reviewArea)
        {
            Customer customer = unitOfWork.Customers.GetAll().Where(c => c.Email == User.Identity.Name)
                .FirstOrDefault();

            if (customer != null)
            {
                GoodReview review = new GoodReview
                {
                    Good = await unitOfWork.Goods.Get(id),
                    Customer = customer,
                    Date = DateTime.Now,
                    Message = reviewArea,
                    StarCount = Convert.ToInt32(Request.Form["mark"])
                };

                Good good = await unitOfWork.Goods.Get(id);

                await unitOfWork.Goods.AddReview(review, good);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("ShowGood", new { goodId = id });
        }

        public async Task<IActionResult> DeleteReview(int id)
        {
            GoodReview review = unitOfWork.Reviews.GetAll().Where(r => r.Id == id).First();
            int goodId = review.GoodId;

            await unitOfWork.Reviews.Delete(review.Id);
            await unitOfWork.SaveAsync();

            return RedirectToAction("ShowGood", new { goodId });
        }

        public async Task<IActionResult> EditReview(int goodId)
        {
            Good good = await unitOfWork.Goods.Get(goodId);
            GoodReview review = unitOfWork.Goods.GetReviews(good.Id)
                .Where(r => r.Id == Convert.ToInt32(Request.Form["reviewId"])).First();
            IUpdater<GoodReview> reviewUpdater = new Updater<GoodReview>(unitOfWork.GetContext());

            review.Message = Request.Form["newMessage"];
            review.StarCount = this.CheckNewStarCount(Request.Form["newStarCount"]);
            review.Date = DateTime.Now;
            reviewUpdater.Update(review);
            await unitOfWork.SaveAsync();

            return RedirectToAction("ShowGood", new { goodId });
        }

        private int CheckNewStarCount(string starCount)
        {
            int result = Convert.ToInt32(starCount);

            if (result > 5)
            {
                result = 5;
            } 

            if (result < 0)
            {
                result = 0;
            }

            return result;
        }
    }
}
