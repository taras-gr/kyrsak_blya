using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Classes.UnitOfWork.Interfaces;
using DAL.Models;

namespace DAL.Classes.UnitOfWork.Classes
{
    public class ReviewRepository : IRepository<GoodReview>
    {
        private readonly AppDbContext appContext;

        public ReviewRepository(AppDbContext appDbContext)
        {
            this.appContext = appDbContext;
        }

        public async Task Create(GoodReview item)
        {
            await this.appContext.Reviews.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            GoodReview review = await appContext.Reviews.FindAsync(id);

            if (review != null)
            {
                appContext.Reviews.Remove(review);
            }
        }

        public async Task<GoodReview> Get(int id)
        {
            GoodReview review = await appContext.Reviews.FindAsync(id);
            review.Customer = appContext.Customers.FirstOrDefault(c => c.Id == review.CustomerId);

            return review;
        }

        public IEnumerable<GoodReview> GetAll()
        {
            IEnumerable<GoodReview> reviews = appContext.Reviews;

            foreach (var review in reviews)
            {
                review.Customer = appContext.Customers.Where(c => c.Id == review.CustomerId).FirstOrDefault();
                review.Good = appContext.Goods.Where(g => g.Id == review.GoodId).FirstOrDefault();
            }

            return appContext.Reviews;
        }

        public void Update(GoodReview item)
        {
            appContext.Entry(item).State = EntityState.Modified;
        }
    }
}
