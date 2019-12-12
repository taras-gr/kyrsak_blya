using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Classes.UnitOfWork.Interfaces;
using DAL.Models;

namespace DAL.Classes.UnitOfWork.Classes
{
    public class GoodRepository : IRepository<Good>
    {
        private readonly AppDbContext applicationContext;

        public GoodRepository(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public async Task Create(Good item)
        {
            await this.applicationContext.Goods.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Good good = await applicationContext.Goods.FindAsync(id);

            if (good != null)
            {
                applicationContext.Goods.Remove(good);
            }
        }

        public async Task<Good> Get(int id)
        {
            Good good = await applicationContext.Goods.FindAsync(id);
            good.Storages = applicationContext.GoodStorage.Where(g => g.GoodId == id).ToList();
            good.Reviews = applicationContext.Reviews.Where(r => r.GoodId == id).ToList();
            good.Producer = await applicationContext.Producers
                .Where(p => p.Id == good.ProducerId).FirstOrDefaultAsync();
            
            foreach (var review in good.Reviews)
            {
                review.Customer = await applicationContext.Customers
                    .Where(c => c.Id == review.CustomerId).FirstOrDefaultAsync();
            }

            return good;
        }

        public IEnumerable<Good> GetAll()
        {
            IEnumerable<Good> goods = applicationContext.Goods;

            foreach (var good in goods)
            {
                good.Producer = applicationContext.Producers.Where(p => p.Id == good.ProducerId).First();
                good.Reviews = GetReviews(good.Id);
            }

            return goods;
        }

        public void Update(Good item)
        {
            applicationContext.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteGoodFromStorage(int goodId, List<Storage> storages)
        {
            Good good = await Get(goodId);

            for (int i = 0; i < storages.Count; i++)
            {
                var item = good.Storages.Where(g => g.StorageId == storages[i].Id && g.GoodId == goodId).First();

                if (item != null)
                {
                    applicationContext.GoodStorage.Remove(item);
                }
            }
        }

        public async Task AddGoodToStorage(int goodId, List<Storage> storages)
        {
            Good good = await Get(goodId);

            for (int i = 0; i < storages.Count; i++)
            {
                applicationContext.GoodStorage.Add(new GoodStorage { Good = good, Storage = storages[i] });
            }
        }

        public List<GoodReview> GetReviews(int id)
        {
            List<GoodReview> reviews = applicationContext.Reviews.Where(r => r.GoodId == id).ToList();

            return reviews;
        }

        public async Task AddReview(GoodReview review, Good good)
        {
            review.Good = good;
            await applicationContext.Reviews.AddAsync(review);
        }

        public async Task<List<Storage>> GetGoodStorages(int goodId)
        {
            List<Storage> storages = new List<Storage>();
            List<GoodStorage> goodStorages = applicationContext.GoodStorage.Where(g => g.GoodId == goodId)
                .ToList();

            foreach (var goodStorage in goodStorages)
            {
                storages.Add(await applicationContext.Storages.FindAsync(goodStorage.StorageId));
            }

            return storages;
        }
    }
}
