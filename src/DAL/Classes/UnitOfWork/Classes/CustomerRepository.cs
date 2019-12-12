using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Classes.UnitOfWork.Classes
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly AppDbContext applicationContext;

        public CustomerRepository(AppDbContext appDbContext)
        {
            this.applicationContext = appDbContext;
        }

        public async Task Create(Customer item)
        {
            Cart cart = new Cart
            {
                CustomerId = item.Id
            };

            await this.applicationContext.Carts.AddAsync(cart);
            item.Cart = cart;
            await this.applicationContext.Customers.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Customer customer = await applicationContext.Customers.FindAsync(id);

            if (customer != null)
            {
                Cart cart = applicationContext.Carts.First(c => c.CustomerId == customer.Id);
                applicationContext.Carts.Remove(cart);
                applicationContext.Customers.Remove(customer);
            }
        }

        public async Task<Customer> Get(int id)
        {
            Customer customer = await applicationContext.Customers.FindAsync(id);
            customer.Cart = applicationContext.Carts.Where(c => c.CustomerId == customer.Id).FirstOrDefault();
            customer.Cart.Goods = applicationContext.GoodCart.Where(c => c.CartId == customer.Cart.Id).ToList();

            if (customer.Cart == null)
            {
                Cart cart = new Cart
                {
                    CustomerId = customer.Id
                };

                cart.Goods = applicationContext.GoodCart.Where(c => c.CartId == cart.Id).ToList();
                customer.Cart = cart;

                await this.applicationContext.Carts.AddAsync(cart);
                await this.applicationContext.SaveChangesAsync();
            }

            return customer;
        }

        public IEnumerable<Customer> GetAll()
        {
            IEnumerable<Customer> customers = applicationContext.Customers;

            return customers;
        }

        public void Update(Customer item)
        {
            applicationContext.Entry(item).State = EntityState.Modified;
        }

        public void AddToCart(Good good, Customer customer)
        {
            List<GoodCart> cartGoods = customer.Cart.Goods.ToList();
            bool addToCart = true;

            foreach (var cartGood in cartGoods)
            {
                if (cartGood.GoodId == good.Id)
                {
                    addToCart = false;
                }
            }

            if (addToCart)
            {
                customer.Cart.Goods.Add(new GoodCart { Good = good, Cart = customer.Cart });
            }
        }

        public void RemoveFromCart(Good good, Customer customer)
        {
            List<GoodCart> cartGoods = customer.Cart.Goods.ToList();
            bool removeFromCart = false;

            foreach (var cartGood in cartGoods)
            {
                if (cartGood.GoodId == good.Id)
                {
                    removeFromCart = true;
                }
            }

            if (removeFromCart)
            {
                var goodCart = customer.Cart.Goods
                    .Where(g => g.GoodId == good.Id && g.Cart.Id == customer.Cart.Id).FirstOrDefault();

                if(goodCart != null)
                {
                    customer.Cart.Goods.Remove(goodCart);
                }
            }
        }
    }
}
