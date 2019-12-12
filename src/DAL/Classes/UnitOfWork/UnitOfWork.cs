using System;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork.Classes;
using DAL.Models;

namespace DAL.Classes.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext applicationContext;

        private CustomerRepository customerRepository;
        private GoodRepository goodRepository;
        private StorageRepository storageRepository;
        private OrderRepository _orderRepository;
        private ProducerRepository producerRepository;
        private ReviewRepository reviewRepository;

        private bool disposed = false;

        public UnitOfWork(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public CustomerRepository Customers
        {
            get
            {
                if (customerRepository == null)
                {
                    customerRepository = new CustomerRepository(this.applicationContext);
                }

                return customerRepository;
            }
        }

        public ProducerRepository Producers
        {
            get
            {
                if(producerRepository == null)
                {
                    producerRepository = new ProducerRepository(applicationContext);
                }
                return producerRepository;
            }
        }

        public GoodRepository Goods
        {
            get
            {
                if (goodRepository == null)
                {
                    goodRepository = new GoodRepository(this.applicationContext);
                }

                return goodRepository;
            }
        }

        public StorageRepository Storages
        {
            get
            {
                if (storageRepository == null)
                {
                    storageRepository = new StorageRepository(this.applicationContext);
                }

                return storageRepository;
            }
        }

        public OrderRepository Orders
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(this.applicationContext);
                }

                return _orderRepository;
            }
        }

        public ReviewRepository Reviews
        {
            get
            {
                if (reviewRepository == null)
                {
                    reviewRepository = new ReviewRepository(applicationContext);
                }
                return reviewRepository;
            }
        }

        public async Task SaveAsync()
        {
            await applicationContext.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    applicationContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AppDbContext GetContext()
        {
            return this.applicationContext;
        }
    }
}
