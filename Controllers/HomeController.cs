using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using GlobalATM.Models;

namespace GlobalATM.Controllers
{

    public class HomeController : Controller
    {

        private int? UUID
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return UUID != null;
            }
        }

        private readonly ILogger<HomeController> _logger;
        private MyContext db;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            if(isLoggedIn){
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser, string AccountType, string CardNumber, string AccountNumber)
        {
            if(isLoggedIn)
            {
                return RedirectToAction("Dashboard");
            }

            Checking existinCardNumber = db.Checkings.
                                            FirstOrDefault(cn => cn.CardNumber == newUser.CardNumber);
            if (existinCardNumber != null) 
            {
                ModelState.AddModelError("AccountAccuracy", "This number is already registered");
                return View("Index");
            }

            Account existingSavingAccount = db.Accounts.
                                                FirstOrDefault(sa => sa.AccountNumber == newUser.AccountNumber);
            if (existingSavingAccount != null)
            {
                ModelState.AddModelError("AccountAccuracy", "This number is already registered");
                return View("Index");
            }

            if (ModelState.IsValid) 
            {
                if(db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View("Index");
                }

                PasswordHasher<User> Hasher =  new PasswordHasher<User>();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);

                newUser.Pin = Hasher.HashPassword(newUser, newUser.Pin);
                if (AccountType == "Checking")
                {
                    Checking account = new Checking();
                    account.UserId = (int)UUID;
                    account.CardNumber = CardNumber;
                    account.RoutingNumber = "123456789";
                    account.AccountNumber = account.RandomDigits(12);

                    newUser.Accounts.Add(account);
                }
                else if (AccountType == "Saving")
                {
                    Saving account = new Saving();
                    account.UserId = (int)UUID;
                    account.AccountNumber = AccountNumber;
                    account.RoutingNumber = "123456789";
                    account.InterestRate = .05;
                    newUser.Accounts.Add(account);
                }
                db.Add(newUser);
                db.SaveChanges();
                return RedirectToAction("LogIn");
            }
            ModelState.AddModelError("SecurityQuestions", "Security questions are required. Click the button to proceed.");
            ModelState.AddModelError("AccountAccuracy", "Please ensure input acccuracy.");

            return View("Index");
        }


        [HttpGet("/login")]
        public IActionResult LogIn()
        {
            // if(isLoggedIn){
            //     return RedirectToAction("Dashboard");
            // }
            return View("LogIn");
        }



        [HttpPost("Login")]
        public IActionResult Login(LogUser logUser)
        {
            if (ModelState.IsValid)
            {
                Account account = null;
                if (logUser.LoginAccountNum.Length == 12)
                {
                    account = db.Accounts
                                        .Include(a => a.User).
                                            FirstOrDefault(u => u.AccountNumber == logUser.LoginAccountNum);
                }
                else if (logUser.LoginAccountNum.Length == 16)
                {

                    account = db.Checkings
                                            .Include(u => u.User)
                                                .FirstOrDefault(u => u.CardNumber == logUser.LoginAccountNum);
                }

                if (account == null)
                {
                    ModelState.AddModelError("LoginAccountNum", "Invalid login attempt");
                    return View("Login");
                }

                PasswordHasher<LogUser> Hasher = new PasswordHasher<LogUser>();
                PasswordVerificationResult result = Hasher.VerifyHashedPassword(logUser, account.User.Pin, logUser.LoginPin); 

                if (result == 0)
                {
                    ModelState.AddModelError("LoginPin", "Invalid login attempt");
                    return View("Login");
                }

                HttpContext.Session.SetInt32("UserId", account.User.UserId);
                HttpContext.Session.SetString("AccountNumber",  account.AccountNumber);
                return RedirectToAction("Dashboard");
            }
            return View("Login");
        }

        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if(!isLoggedIn)
            {
                return RedirectToAction("LogIn");
            }

            var userAccount = db.Accounts
                                    .FirstOrDefault(a => a.UserId == (int)UUID);
            
            List<Transaction> allTransactions = db.Transactions
                .Where(t => t.UserId == (int)UUID)
                .ToList();

            // var Balance = userAccount.GetSum(allTransactions);
            ViewBag.Balance = userAccount.Balance;
            // ViewBag.Balance = Balance;

            User loggedUser = db.Users
                .Include(u => u.Accounts)
                .FirstOrDefault(u => u.UserId == (int)UUID);

            ViewBag.allTransactions = db.Transactions
                .Where(t => t.UserId == (int)UUID)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return View("Dashboard", loggedUser);
        }
        
        [HttpPost("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
