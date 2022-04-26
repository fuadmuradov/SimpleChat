using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SimpleChat.Hubs;
using SimpleChat.Models;
using SimpleChat.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext db;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IHubContext<ChatHub> hubContext;

        public HomeController(MyDbContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHubContext<ChatHub> hubContext)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.hubContext = hubContext;
        }

        //public async Task<IActionResult> CreateUser()
        //{
        //    AppUser user1 = new AppUser() { Fullname = "Fuad Muradov", UserName = "Fuad" };
        //    AppUser user2 = new AppUser() { Fullname = "Murad Muradov", UserName = "Murad" };
        //    AppUser user3 = new AppUser() { Fullname = "Tural Adilov", UserName = "Tural" };
        //    AppUser user4 = new AppUser() { Fullname = "Orxan Qarayev", UserName = "Orxan" };

        //    var result = await  userManager.CreateAsync(user1, "User@1234");
        //    var result2 = await userManager.CreateAsync(user2, "User@1234");
        //    var result3 = await userManager.CreateAsync(user3, "User@1234");
        //    var result4 = await userManager.CreateAsync(user4, "User@1234");

        //    return Content("Created");
        //}


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = await userManager.FindByNameAsync(login.Username);
            if (user == null)
            {
                ModelState.AddModelError("","Username Or Password Incorrect");
                return View();
            }

            var result = signInManager.PasswordSignInAsync(user, login.Password, true, false).Result;
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username Or Password Incorrect");
                return View();
            }

            return RedirectToAction("Chat");


        }

        public async  Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Chat()
        {
            List<AppUser> users = userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ShowToaster(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user == null) NotFound();
            await hubContext.Clients.Client(user.ConnectionID).SendAsync("ShowToaster");

            return RedirectToAction("Chat");


        }

    }
}
