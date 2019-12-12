using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Classes.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Helpers;
using Store.Helpers.Sender;
using Store.ViewModels;

namespace Store.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UnitOfWork unitOfWork;
        private readonly ErrorMessage errorMessage;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWork = new UnitOfWork(appDbContext);
            this.errorMessage = new ErrorMessage();
        }

        /// <summary>
        /// Get method to show registration form.
        /// </summary>
        /// <returns>Register view that contains registration form.</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Post method that will be runned after pushing on the button in registration form. 
        /// </summary>
        /// <param name="model">Model to unit all form fields in one entity.</param>
        /// <returns>If operation is success it redirects to main page, otherwise it will return the same page.</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (ModelState.IsValid)
            {
                foreach (var number in model.Phone)
                {
                    if (!char.IsDigit(number))
                    {
                        ViewBag.Message = "Phone number is invalid!";
                        return View("ErrorPhonePage");
                    }
                }

                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
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
                    await signInManager.SignInAsync(user, false);
                    await unitOfWork.Customers.Create(customer);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Home");
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

        /// <summary>
        /// Get method to show login form.
        /// </summary>
        /// <returns>Login view that contains login form.</returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Method runs when user clicks on the submit button in Login view. 
        /// </summary>
        /// <param name="model">Model to unit all form fields in one entity.</param>
        /// <returns>If operation is success it redirects to main page, otherwise it will return the same page.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "UserIsNotRegistered");

                    return View("ErrorPage");
                }

                var result =
                    await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Performs log out form site.
        /// </summary>
        /// <returns>Redirects to main page.</returns>
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CustomerSettingPage()
        {
            string name = User.Identity.Name;
            Customer customer = unitOfWork.Customers.GetAll().Where(c => c.Email == name)
                .FirstOrDefault();

            if (customer != null)
            {
                EditCustomerView model = new EditCustomerView
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    SecondName = customer.SecondName,
                    Phone = customer.Phone,
                    Email = customer.Email,
                };


                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CustomerSettingPage(EditCustomerView model)
        {
            Customer customer = await unitOfWork.Customers.Get(model.Id);

            if (customer != null)
            {
                foreach (var number in model.Phone)
                {
                    if (!char.IsDigit(number))
                    {
                        ViewBag.Message = "Phone number is invalid!";
                        return View("EditPhoneError");
                    }
                }
                
                customer.FirstName = model.FirstName;
                customer.SecondName = model.SecondName;
                customer.Phone = model.Phone;

                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
                user.UserName = model.Email;

                unitOfWork.Customers.Update(customer);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ChangePasswordView model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "UserIsNotFounded");

                    return View("ErrorPage");
                }

                ViewBag.UserIsNull = false;

                var customer = unitOfWork.Customers.GetAll().Where(c => c.Email == model.Email).First();
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ChangePassword", "Account", new { userId = user.Id, code }, 
                    protocol: HttpContext.Request.Scheme);

                EmailSender emailSender = new EmailSender();

                await emailSender.PasswordChangeCodeSend(customer, callbackUrl);

                return View("ForgotPasswordInfo");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ViewBag.Message = errorMessage.ReturnErrorMessage("ErrorMessages", "UserIsNotFounded");

                return View("ErrorPage");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
