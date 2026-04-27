using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NutriTrackAI.Data;
using NutriTrackAI.Models;

namespace NutriTrackAI.Controllers
{
    //handles user registration, login and logout
    public class AccountController : Controller
    {
        private readonly NutriTrackContext _context;
        private readonly PasswordHasher<User> _passwordHasher;


        public AccountController(NutriTrackContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        //display reg form
        public IActionResult Register()
        {
            return View();
        }

       //handles user registration
        [HttpPost]
        public IActionResult Register(User user, string password)
        {
            //no dupe accounts
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "An account with this email already exists.";
                return View(user);
            }

            //hashes password
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            _context.SaveChanges();

            //keeps user info so they don't have to login each time they click a new link
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserName", user.Name);

            return RedirectToAction("Index", "Home");
        }
        //displays login
        public IActionResult Login()
        {
            return View();
        }


        //handles user login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            //makes sure user exists
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            //verifys hash
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            //keeps user info so they don't have to login each time they click a new link

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserName", user.Name);

            return RedirectToAction("Index", "Home");
        }

        //logs user out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}