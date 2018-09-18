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
            return View();
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
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
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        [HttpGet("Gallery")]
        public IActionResult Gallery()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        [HttpGet("Contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        [HttpGet("Map")]
        public IActionResult Map()
        {
            return View();
        }
        [HttpGet("Reviews")]
        public IActionResult Reviews()
        {
            List<Review> reviews = _bContext.reviews.Include(u => u.User).ToList();
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
