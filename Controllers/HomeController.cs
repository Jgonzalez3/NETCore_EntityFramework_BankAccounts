using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankAccountsContext _context;
        public HomeController(BankAccountsContext context){
            _context = context;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View("Register");
        }
        [HttpGet]
        [Route("/login")]
        public IActionResult Login(){
            HttpContext.Session.Clear();
            return View("Login");
        }
        [HttpPost]
        [Route("/register")]
        public IActionResult Register(RegisterViewModel model, User NewUser){
            if(ModelState.IsValid){
                List<User> AllUsers = _context.Users.Include(User=>User.transactions).Where(User=>User.email == model.email).ToList();
                if(AllUsers.Count > 0){
                    TempData["emailused"] = "Email already in use. Please use another.";
                    return View("Register");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                NewUser.password = Hasher.HashPassword(NewUser, NewUser.password);
                _context.Users.Add(NewUser);
                _context.SaveChanges();
                User Reg = _context.Users.SingleOrDefault(user => user.email == NewUser.email);
                HttpContext.Session.SetInt32("userid", (int)Reg.UserId);
                return RedirectToAction("DisplayAccount", "Account", new {AccountId=(int)Reg.UserId});
            }
            return View(model);
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult LoginAttempt(string email, string password){
            User Login = _context.Users.SingleOrDefault(user => user.email == email);
            if( Login == null){
                TempData["Invalidemail"] = "Email not Registered. Have you Registered?";
                return View("Login");
            }
            if(Login != null && password !=null){
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(Login, Login.password, password)){
                    HttpContext.Session.SetInt32("userid", (int)Login.UserId);
                    return RedirectToAction("DisplayAccount", "Account", new {AccountId=(int)Login.UserId});
                }
            }
            TempData["InvalidPW"] = "Invalid Password";
            return View("Login");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
