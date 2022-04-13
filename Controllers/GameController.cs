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

    public class GameController : Controller
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

        public GameController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet("/reportlostorstolen")]
        public IActionResult ReportLostOrStolen()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("LogIn", "Home");
            }
            User loggedUser = db.Users
                .Include(u => u.Accounts)
                .FirstOrDefault(u => u.UserId == (int)UUID);

            return View("ReportLostOrStolen", loggedUser);
        }

        [HttpPost("/stolen")]
        public async Task<ActionResult> YesCardStolen()
        {
            if (isLoggedIn)
            {
                Checking userAccount = db.Checkings
                                        .FirstOrDefault(a => a.AccountNumber == HttpContext.Session.GetString("AccountNumber"));
                userAccount.IsCardStolen = true;
                db.SaveChanges();

                User currentUser = db.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

                String body = "<p> Hello, {0}</p> <p>Your ATM card was recently reported as stolen via one of our ATM Machines on {1}. If you did not do this, please change your account pin and contact customer service for additional assistance.</p> <p>Thank you for banking with CSharp Global Banking Sytem.</p>";
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(currentUser.Email));
                message.From = new MailAddress("CSharpGlobalBank@gmail.com");
                message.Subject = "Do Not Reply - CSharp Online Banking System";
                message.Body = string.Format(body, currentUser.FirstName, userAccount.UpdatedAt);
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
                    return View("Game");
                }
            }
            return RedirectToAction("LogIn", "Home");
        }

        [HttpGet("/game")]
        public IActionResult Game()
        {
            if(isLoggedIn)
            {
                User loggedUser = db.Users
                    .FirstOrDefault(u => u.UserId == (int)UUID);
                return View("Game");
            }
            return RedirectToAction("LogIn", "Home");
        }

        [HttpPost("/game/submit")]
        public IActionResult GameVerifyUser(User verifiedUser)
        {
            if(isLoggedIn)
            {
                User loggedUser = db.Users
                    .FirstOrDefault(u => u.UserId == (int)UUID);

                if(verifiedUser.FaveColor == loggedUser.FaveColor && verifiedUser.Breakfast == loggedUser.Breakfast && verifiedUser.AvgSpeedSwallow == loggedUser.AvgSpeedSwallow && verifiedUser.KPop == loggedUser.KPop && verifiedUser.DOB == loggedUser.DOB)
                {
                    return RedirectToAction("AccountRecovery");
                }
                // Lock account? 
                return RedirectToAction("LogIn", "Home");
            }
            return RedirectToAction("LogIn", "Home");
        }

        [HttpGet("/game/accountrecovery/success")]
        public IActionResult AccountRecovery()
        {
            if(isLoggedIn)
            {
                return View("AccountRecovery");
            }
            return RedirectToAction("LogIn", "Home");
        }
    }
}
