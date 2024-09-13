using LeadTask2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq; // Ensure you have this for LINQ queries
using Microsoft.AspNetCore.Http; // For Session handling

namespace LeadTask2.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login Logic
        [HttpPost]
        public IActionResult Login(User user)
        {
            // Check if the User model is null or missing required fields
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Message = "Please enter both Username and Password.";
                return View();
            }

            // Query the database to check if the user exists

            var usr = _context.Users
                .FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (usr != null)
            {
                // Store the username in the session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("Id",user.ID);

                // Redirect to Lead Controller's Index action
                return RedirectToAction("Index", "Lead");
            }
            else
            {
                // If login fails, show an error message
                ViewBag.Message = "Invalid Credentials. Please try again.";
                return View();
            }
        }

        // Logout Logic
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
