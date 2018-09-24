using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Breathtaking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;

namespace Breathtaking.Controllers
{
    public class HomeController : Controller
    {
        private BreathContext _bContext;
        public HomeController(BreathContext context)
        {
            _bContext = context;
        }
        private User ActiveUser 
        {
            get 
            {
                return _bContext.users.Where(u => u.user_id == HttpContext.Session.GetInt32("user_id")).FirstOrDefault();
            }
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            ViewBag.user = ActiveUser;
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            ViewBag.user = ActiveUser;
            return View();
        }

        [HttpPost("registeruser")]
        public IActionResult RegisterUser(RegisterUser newuser)
        {
            User CheckEmail = _bContext.users
                .Where(u => u.email == newuser.email)
                .SingleOrDefault();

            if(CheckEmail != null)
            {
                ViewBag.errors = "That email already exists";
                return RedirectToAction("Register");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<RegisterUser> Hasher = new PasswordHasher<RegisterUser>();
                User newUser = new User
                {
                    user_id = newuser.user_id,
                    first_name = newuser.first_name,
                    last_name = newuser.last_name,
                    email = newuser.email,
                    password = Hasher.HashPassword(newuser, newuser.password)
                  };
                _bContext.Add(newUser);
                _bContext.SaveChanges();
                ViewBag.success = "Successfully registered";
                return RedirectToAction("Login");
            }
            else
            {
                return View("Register");
            }
        }

        [HttpPost("loginuser")]
        public IActionResult LoginUser(LoginUser loginUser) 
        {
            User CheckEmail = _bContext.users
                .SingleOrDefault(u => u.email == loginUser.email);
            if(CheckEmail != null)
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(CheckEmail, CheckEmail.password, loginUser.password))
                {
                    HttpContext.Session.SetInt32("user_id", CheckEmail.user_id);
                    HttpContext.Session.SetString("first_name", CheckEmail.first_name);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.errors = "Incorrect Password";
                    return View("Register");
                }
            }
            else
            {
                ViewBag.errors = "Email not registered";
                return View("Register");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet("About")]
        public IActionResult About()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("Gallery")]
        public IActionResult Gallery()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("Contact")]
        public IActionResult Contact()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("Resources")]
        public IActionResult Resources()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("Map")]
        public IActionResult Map()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("Reviews")]
        public IActionResult Reviews()
        {
            List<Review> reviews = _bContext.reviews.Include(u => u.User).Include(l => l.Likes).ToList();
            List<Like> likes = _bContext.likes.Include(r => r.Review).ToList();
            ViewBag.likes = likes;
            ViewBag.user = ActiveUser;
            ViewBag.reviews = reviews;
            return View();
        }
        [HttpPost("AddReview")]
        public IActionResult AddReview(Review rev)
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                if(ModelState.IsValid)
                {
                    Review newReview = new Review
                    {
                        user_id = ActiveUser.user_id,
                        rating = rev.rating,
                        review = rev.review,
                        start_visit_date = rev.start_visit_date,
                        end_visit_date = rev.end_visit_date
                    };
                    _bContext.reviews.Add(newReview);
                    _bContext.SaveChanges();
                    return RedirectToAction("Reviews");
                }
                return View("Reviews");
            }
        }

        [Route("AddLike/{review_id}")]
        public IActionResult AddLike(int review_id)
        {
            Like like = new Like
            {
                review_id = review_id,
            };
            _bContext.likes.Add(like);
            _bContext.SaveChanges();
            return RedirectToAction("Reviews");
        }

        public void SendEmail(string toAddress, string fromAddress, string subject, string message)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    const string email = "stradtkt22@gmail.com";
                    const string password = "Buster183845";

                    var loginInfo = new NetworkCredential(email, password);


                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(new MailAddress(toAddress));
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;

                    try
                    {
                        using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = loginInfo;
                            smtpClient.Send(mail);
                        }

                    }

                    finally
                    {
                        //dispose the client
                        mail.Dispose();
                    }

                }
            }
                catch (SmtpFailedRecipientsException ex)
                {
                    foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                    {
                        var status = t.StatusCode;
                        if (status == SmtpStatusCode.MailboxBusy ||
                            status == SmtpStatusCode.MailboxUnavailable)
                        {
                            Content("Delivery failed - retrying in 5 seconds.");
                            System.Threading.Thread.Sleep(5000);
                            //resend
                            //smtpClient.Send(message);
                        }
                        else
                        {
                            Content("Failed to deliver message to {0}",
                                            t.FailedRecipient);
                        }
                    }
                }
                catch (SmtpException Se)
                {
                    // handle exception here
                    Content(Se.ToString());
                }
                catch (Exception ex)
                {
                    Content(ex.ToString());
                }
            }

        [HttpPost("SendContact")]
        public ActionResult SendContact(MailModels e)
        {
            if (ModelState.IsValid)
            {

                //prepare email
                var toAddress = "stradtkt22@gmail.com";
                var fromAddress = e.email.ToString();
                var subject = "Email from "+ e.name;
                var message = new StringBuilder();
                message.Append("Name: " + e.name + "\n");
                message.Append("Email: " + e.email + "\n");
                message.Append("Subject: " + e.subject + "\n\n");
                message.Append("Message: " + e.msg);

                //start email Thread
                var tEmail = new Thread(() => 
            SendEmail(toAddress, fromAddress, subject, message.ToString()));
                tEmail.Start();
                return RedirectToAction("Index");
            }
            return View("Contact");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
