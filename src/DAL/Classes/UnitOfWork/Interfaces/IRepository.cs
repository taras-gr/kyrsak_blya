using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Classes.UnitOfWork.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        Task<T> Get(int id);

        Task Create(T item);

        void Update(T item);

        Task Delete(int id);
    }
}
