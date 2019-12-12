using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Helpers;
using Store.ViewModels;

namespace Store.Controllers
{
    [Authorize(Roles = "admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public CustomerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(unitOfWork.Customers.GetAll().ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.FirstName + model.SecondName,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                Customer customer = new Customer
                {
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Phone = model.Phone,
                    Email = model.Email
                };

                if (userManager.Users.Where(u => u.Email == model.Email).Count() > 0)
                {
                    ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "EmailExistAlready");

                    return View("ErrorPage");
                }

                var addResult = await userManager.CreateAsync(user, model.Password);

                if (addResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                    await unitOfWork.Customers.Create(customer);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Customer customer = await unitOfWork.Customers.Get(id);
            ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);

            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();

            if (customer == null)
            {
                return NotFound();
            }

            EditCustomerView model = new EditCustomerView
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                SecondName = customer.SecondName,
                Phone = customer.Phone,
                Email = customer.Email,
                AllRoles = allRoles,
                UserRoles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCustomerView model, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                Customer customer = await unitOfWork.Customers.Get(model.Id);
                ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);
                if (user != null && customer != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.UpdateTime = DateTime.Now;

                    var userRoles = await userManager.GetRolesAsync(user);
                    var allRoles = roleManager.Roles.ToList();

                    var addedRoles = roles.Except(userRoles);
                    var removedRoles = userRoles.Except(roles);

                    await userManager.AddToRolesAsync(user, addedRoles);
                    await userManager.RemoveFromRolesAsync(user, removedRoles);

                    customer.FirstName = model.FirstName;
                    customer.SecondName = model.SecondName;
                    customer.Phone = model.Phone;
                    customer.Email = model.Email;

                    unitOfWork.Customers.Update(customer);
                    await unitOfWork.SaveAsync();

                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Customer customer = await unitOfWork.Customers.Get(id);
            ApplicationUser user = await userManager.FindByEmailAsync(customer.Email);

            if (user != null && customer != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                await unitOfWork.Customers.Delete(id);
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
        public IActionResult Find(FindCustomerView model)
        {
            List<Customer> customers = new List<Customer>();

            if (ModelState.IsValid)
            {
                var allCustomers = unitOfWork.Customers.GetAll().ToList();

                foreach (var customer in allCustomers)
                {
                    if (this.AddToList(model, customer))
                    {
                        customers.Add(customer);
                    }
                }

                HttpContext.Session.Set("list", customers);

                return RedirectToAction("FindResult", "Customer");
            }

            return View(model);
        }

        public IActionResult FindResult()
        {
            var customers = HttpContext.Session.Get<List<Customer>>("list");

            if (customers == null)
            {
                return RedirectToAction("Find");
            }

            return View(customers);
        }

        [HttpGet]
        public async Task<ActionResult> ShowOrders(int id)
        {
            Customer customer = await unitOfWork.Customers.Get(id);
            List<Order> customerOrders = new List<Order>();

            customerOrders.AddRange(unitOfWork.Orders.GetAll().Where(o => o.CustomerId == customer.Id));

            return View(customerOrders);
        }

        private bool AddToList(FindCustomerView model, Customer customer)
        {
            bool addToResult = true;

            if (model.FirstName == null && model.SecondName == null &&
            model.Phone == null && model.Email == null)
            {
                addToResult = false;
            }

            if (model.FirstName != null && customer.FirstName != model.FirstName)
            {
                addToResult = false;
            }

            if (model.SecondName != null && customer.SecondName != model.SecondName)
            {
                addToResult = false;
            }

            if (model.Phone != null && customer.Phone != model.Phone)
            {
                addToResult = false;
            }

            if (model.Email != null && customer.Email != model.Email)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}

