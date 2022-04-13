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
using System.Net.Mail;
using System.Net;

namespace GlobalATM.Controllers
{

    public class TransactionController : Controller
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

        public TransactionController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet("Withdraw")]
        public IActionResult Withdraw()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("LogIn", "Home");
            }

            List<Transaction> allTransactions = db.Transactions
                .Where(t => t.UserId == (int)UUID)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            ViewBag.allTransactions = allTransactions;

            Account userAccount = db.Accounts
                                    .Include("Transactions")
                                    .FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
            ViewBag.UserAccount = userAccount;

            // var userAccount = db.Accounts
            //                         .FirstOrDefault(a => a.UserId == (int)UUID);
            
            // List<Transaction> allTransactions = db.Transactions
            //     .Where(t => t.UserId == (int)UUID)
            //     .ToList();

            // var Balance = userAccount.GetSum(allTransactions);

            // ViewBag.Balance = Balance;


            return View("Withdraw");
        }

        [HttpGet("Deposit")]
        public IActionResult Deposit()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("LogIn", "Home");
            }

            List<Transaction> allTransactions = db.Transactions
                .Where(t => t.UserId == (int)UUID)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            ViewBag.allTransactions = allTransactions;

            Account userAccount = db.Accounts
                                    .Include("Transactions")
                                    .FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
                                    
            ViewBag.UserAccount = userAccount;

                // var userAccount = db.Accounts
                //                     .FirstOrDefault(a => a.UserId == (int)UUID);
            
            // List<Transaction> allTransactions = db.Transactions
            //     .Where(t => t.UserId == (int)UUID)
            //     .ToList();

            // var Balance = userAccount.GetSum(allTransactions);

            // ViewBag.Balance = Balance;

            return View("Deposit");
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(double amount)
        {
            if (isLoggedIn)
            {
                Account user = db.Accounts
                                        .Include("Transactions")
                                        .FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
                ViewBag.UserAccount = user;

                User currentUser = db.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
                Account userAccount = db.Accounts.Include("Transactions").FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
                Transaction newTrans = null;
                if (amount > 0)
                {
                    newTrans = new Transaction
                    {
                        Amount = amount,
                        CreatedAt = DateTime.Now,
                        UserId = currentUser.UserId
                    };
                    userAccount.Transactions.Add(newTrans);
                    db.Add(newTrans);
                    db.SaveChanges();

                    String body = "<p> Hello, {0}</p> <p>Here is your receipt for your recent transacion with CSharp Global Banking System:</p><p>You made a deposit of ${1} on {2} to your account. Your new balance is ${3}.</p> <p>Thank you for banking with CSharp Global Banking Sytem.</p>";
                    MailMessage message = new MailMessage();
                    message.To.Add(new MailAddress(currentUser.Email)); 
                    message.From = new MailAddress("CSharpGlobalBank@gmail.com");
                    message.Subject = "Do Not Reply - CSharp Online Banking System - Your automated transaction receipt";
                    message.Body = string.Format(body, currentUser.FirstName, newTrans.Amount, newTrans.CreatedAt, userAccount.Balance);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "CSharpGlobalBank@gmail.com",
                            Password = System.IO.File.ReadAllText("EmailSenderPW.txt"),
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                        TempData["Success"] = "Your deposit has been processed. Please check your email for receipt.";
                        return RedirectToAction("Deposit");
                    }
                }
                else
                {
                    ModelState.AddModelError("Amount", "Invalid Input");
                    return View("Deposit");
                }
            }
            return RedirectToAction("LogIn", "Home");
        }

        [HttpPost("Subtract")]
        public async Task<ActionResult> Subtract(double amount)
        {
            if (isLoggedIn)
            {
                Account user = db.Accounts
                                        .Include("Transactions")
                                        .FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
                ViewBag.UserAccount = user;

                User currentUser = db.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
                Account userAccount = db.Accounts.Include("Transactions").FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
                Transaction newTrans = null;
                if (amount > 0)
                {
                    amount = amount * -1;
                }
                else
                {
                    ModelState.AddModelError("Amount", "Invalid Input");
                    return View("Withdraw");
                }
                if (amount + userAccount.Balance < 0)
                {
                    ModelState.AddModelError("Amount", "Insufficient funds");
                    return View("Withdraw");
                }
                else
                {
                    newTrans = new Transaction
                    {
                        Amount = amount,
                        CreatedAt = DateTime.Now,
                        UserId = currentUser.UserId
                    };
                    userAccount.Transactions.Add(newTrans);
                    db.Add(newTrans);
                    db.SaveChanges();

                    String body = "<p> Hello, {0}</p> <p>Here is your receipt for your recent transacion with CSharp Global Banking System:</p><p>You made a withdrawl of ${1} on {2} to your account. Your new balance is ${3}.</p> <p>Thank you for banking with CSharp Global Banking Sytem.</p>";
                    MailMessage message = new MailMessage();
                    message.To.Add(new MailAddress(currentUser.Email)); 
                    message.From = new MailAddress("CSharpGlobalBank@gmail.com");
                    message.Subject = "Do Not Reply - CSharp Online Banking System - Your automated transaction receipt";
                    message.Body = string.Format(body, currentUser.FirstName, newTrans.Amount, newTrans.CreatedAt, userAccount.Balance);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "CSharpGlobalBank@gmail.com",
                            Password = System.IO.File.ReadAllText("EmailSenderPW.txt"),
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                        TempData["Success"] = "Your withdrawal has been processed. Please collect your money and check your email for receipt.";
                    }
                    return Redirect("Withdraw");
                }
            }

            return RedirectToAction("LogIn", "Home");
        }

        [HttpGet("/transactions/currency-converter")]
        public IActionResult CurrencyConverter()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("LogIn", "Home");
            }

            return View("CurrencyConverter");
        }
    }
}
