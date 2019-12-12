using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Classes.UnitOfWork.Interfaces;
using DAL.Models;

namespace DAL.Classes.UnitOfWork.Classes
{
    public class ProducerRepository : IRepository<Producer>
    {
        private readonly AppDbContext applicationContext;

        public ProducerRepository(AppDbContext appDbContext)
        {
            applicationContext = appDbContext;
        }

        public async Task Create(Producer item)
        {
            await this.applicationContext.Producers.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Producer producer = await applicationContext.Producers.FindAsync(id);

            if (producer != null)
            {
                applicationContext.Producers.Remove(producer);
            }
        }

        public async Task<Producer> Get(int id)
        {
            Producer producer = await applicationContext.Producers.FindAsync(id);
            producer.Products = applicationContext.Goods.Where(g => g.ProducerId == producer.Id).ToList();

            return producer;
        }

        public IEnumerable<Producer> GetAll()
        {
            return applicationContext.Producers;   
        }

        public void Update(Producer item)
        {
            applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
