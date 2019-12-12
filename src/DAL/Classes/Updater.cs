using System;
using System.Collections.Generic;
using System.Text;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Classes
{
    public class Updater<T> : IUpdater<T>
    {
        private readonly AppDbContext _dbContext;

        public Updater(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(T item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
        }
    }
}
